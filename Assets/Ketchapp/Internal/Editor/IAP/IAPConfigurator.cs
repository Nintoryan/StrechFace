using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ketchapp.Editor;
using Ketchapp.Editor.Utils;
using Ketchapp.MayoSDK.Purchasing;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_PURCHASING
using UnityEngine.Purchasing;
#endif

namespace Ketchapp.Editor.Purchasing
{
#if UNITY_PURCHASING
    public class IAPConfigurator : EditorWindow
    {
        private Color _baseColor;
        private Vector2 _scroll = Vector2.zero;

        private static string IapConfigurationPath => Path.Combine("Assets", "Dependencies", "Ketchapp", "Configuration", "Resources", "IAPConfiguration.asset");

        private List<ProductDescription> _products;
        private IAPConfiguration _config;

        private bool _initialized;
        private List<ProductDescription> products;

        private IAPConfiguration _tempConfig;

        private bool _addedAndroidStoreId;

        [MenuItem("Ketchapp Mayo SDK/IAP Configuration")]
        public static void ShowWindow()
        {
            GetWindow<IAPConfigurator>("Setup In App Purchases", Type.GetType("UnityEditor.InspectorWindow,UnityEditor.dll"));

            if (!File.Exists(IapConfigurationPath))
            {
                var asset = CreateInstance<IAPConfiguration>();
                AssetDatabase.CreateAsset(asset, IapConfigurationPath);
                AssetDatabase.Refresh();
            }
        }

        private void OnGUI()
        {
            if (!_initialized)
            {
                _config = Resources.Load<IAPConfiguration>("IAPConfiguration");
                _products = _config.Products;
                if (_products == null)
                {
                    _products = new List<ProductDescription>();
                }

                products = _products;
                _initialized = true;
            }

            if (_config == null)
            {
                return;
            }

            var baseColor = GUI.color;
            var baseBGColor = GUI.backgroundColor;
            GUILayout.Box(KetchappEditorHelper.KetchappLogo, GUILayout.Height(100), GUILayout.Width(position.width));
            KetchappEditorHelper.GuiLine(GUI.skin);
            KetchappEditorHelper.Label("In App Purchase Configuration", 20);
            KetchappEditorHelper.GuiLine(GUI.skin);

            if (GUILayout.Button("Add Product"))
            {
                products.Add(new ProductDescription());
            }

            _scroll = EditorGUILayout.BeginScrollView(_scroll);
            if (products.Count > 0)
            {
                for (int i = 0; i < products.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal(GUI.skin.GetStyle("HelpBox"));
                    GUI.backgroundColor = new Color(0, 0, 0, 0);
                    GUI.color = ProductState(products[i]);
                    GUILayout.Box(KetchappEditorHelper.DollarIcon, GUILayout.Height(100), GUILayout.Width(KetchappEditorHelper.DollarIcon.width));
                    GUI.color = baseColor;
                    GUI.backgroundColor = baseBGColor;
                    EditorGUILayout.BeginVertical();
                    KetchappEditorHelper.Label(products[i].ProductName, 16);
                    products[i].ProductName = EditorGUILayout.TextField("Product Name", products[i].ProductName);
                    if (!products[i].ProductNameValid)
                    {
                        EditorGUILayout.HelpBox("Product Name is empty", MessageType.Error);
                    }

                    if (products[i].ProductNameHasWhiteSpace)
                    {
                        EditorGUILayout.HelpBox("Product Name has whitespace", MessageType.Error);
                    }

                    if (!products[i].ProductHasNoDigits)
                    {
                        EditorGUILayout.HelpBox("Product Name must have no digits", MessageType.Error);
                    }

                    products[i].Type = (ProductType)EditorGUILayout.EnumPopup("Product Type", products[i].Type);
                    products[i].ProductId = EditorGUILayout.TextField(string.IsNullOrEmpty(products[i].ProductIdAndroid) ? "Product Identifier" : "Product Identifier iOS", products[i].ProductId);
                    if (string.IsNullOrEmpty(products[i].ProductIdAndroid))
                    {
                        if (GUILayout.Button(new GUIContent()
                        {
                            text = "Add Android Specific Id",
                            image = EditorGUIUtility.IconContent("BuildSettings.Android.Small").image
                        }))
                        {
                            products[i].ProductIdAndroid = products[i].ProductId;
                        }
                    }
                    else
                    {
                        products[i].ProductIdAndroid = EditorGUILayout.TextField("Product Identifier Android", products[i].ProductIdAndroid);
                    }

                    if (!products[i].ProductIdValid)
                    {
                        EditorGUILayout.HelpBox("Product Id is empty", MessageType.Error);
                    }

                    products[i].IsNoAds = EditorGUILayout.Toggle("Is No Ads", products[i].IsNoAds);

                    /*SerializedObject serializedEvent = new SerializedObject(_config);
                    var serializedProduct = serializedEvent.FindProperty("Products");
                    Debug.Log("Array size : " + serializedProduct.arraySize);
                    Debug.Log("Serialized : " + serializedProduct.GetArrayElementAtIndex(i).FindPropertyRelative("PurchaseEvent"));
                    EditorGUILayout.PropertyField(serializedProduct.GetArrayElementAtIndex(i).FindPropertyRelative("PurchaseEvent"));
                    serializedEvent.ApplyModifiedProperties();*/

                    if (ProductExists(products[i]))
                    {
                        EditorGUILayout.HelpBox("A product already exists with this ID !", MessageType.Error);
                    }

                    EditorGUILayout.Space();
                    GUI.backgroundColor = Color.red;
                    if (GUILayout.Button("Remove"))
                    {
                        products.Remove(products[i]);
                    }

                    GUI.backgroundColor = baseBGColor;
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.EndHorizontal();
                }
            }

            EditorGUILayout.EndScrollView();

            GUI.enabled = products.All(p => p.IsValid) && ProductListIsUnique();
            GUI.backgroundColor = Color.green;
            if (GUILayout.Button("Generate IAP Configuration", GUILayout.Height(75)))
            {
                GenerateIAPConfiguration(products);
            }

            GUI.backgroundColor = baseBGColor;
            GUI.enabled = true;
        }

        private void GenerateIAPConfiguration(List<ProductDescription> products)
        {
            _config.Products = products;
            EditorUtility.SetDirty(_config);
            AssetDatabase.Refresh();
            IAPEditor.CreatePurchasingEnum(products);
            _initialized = false;
        }

        private bool ProductExists(ProductDescription product)
        {
            return products.Any(p => p.ProductId == product.ProductId && product != p);
        }

        private bool ProductListIsUnique()
        {
            return !products.Any(p => ProductExists(p));
        }

        private Color ProductState(ProductDescription product)
        {
            Color orange = new Color(1, 0.807f, 0.560f);

            if (ProductExists(product))
            {
                return orange;
            }
            else if (_products.Any(p => p.ProductId == product.ProductId))
            {
                return Color.green;
            }
            else if (product.IsValid)
            {
                return orange;
            }
            else
            {
                return Color.red;
            }
        }
    }
#endif
}
