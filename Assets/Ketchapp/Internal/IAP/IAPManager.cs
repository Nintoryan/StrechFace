using System;
using System.Collections.Generic;
using System.Linq;
using Ketchapp.MayoSDK;
using Ketchapp.MayoSDK.Purchasing;
using UnityEngine;
#if UNITY_PURCHASING
using UnityEngine.Purchasing;
#endif
namespace Ketchapp.Internal.Purchasing
{
    public class IAPManager
#if UNITY_PURCHASING
: IStoreListener
#endif
    {
        public string NoAdsKey => "NoAds";
#if UNITY_PURCHASING
        private IStoreController _storeController;
        private IExtensionProvider _storeExtensionProvider;

        public List<ProductDescription> Products;

        private Action<PurchaseProductResult> _purchaseCallback;

        public bool IsInitialized
        {
            get
            {
                return _storeController != null && _storeExtensionProvider != null;
            }
        }

        public void Initialize()
        {
            if (KetchappInternal.IAPConfiguration == null)
            {
                throw new Exception("No IAP configuration found, purchasing won't initialize");
            }

            Products = KetchappInternal.IAPConfiguration.Products;

            if (Products == null)
            {
                throw new Exception("Product configuration is null");
            }

            if (Products.Count == 0)
            {
                Debug.Log("No product found");
            }

            if (!IsInitialized)
            {
                var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

                foreach (var product in Products)
                {
                    builder.AddProduct(product.ProductId, product.Type, new IDs
                {
                    { product.ProductId, MacAppStore.Name },
                    { string.IsNullOrEmpty(product.ProductIdAndroid) ? product.ProductId : product.ProductIdAndroid, GooglePlay.Name }
                });
                }

                UnityPurchasing.Initialize(this, builder);
            }
        }

        public ProductMetadata GetProductMetaData(ProductDescription product)
        {
            var metadatas = _storeController.products.all;
            var selectedProduct = metadatas.FirstOrDefault(p => p.definition.id == product.ProductId);

            return selectedProduct?.metadata;
        }

        public List<ProductMetadata> GetAllProductMetadata()
        {
            return _storeController.products.all.Select(p => p.metadata).ToList();
        }

        public void BuyProduct(ProductDescription product, Action<PurchaseProductResult> onPurchaseResult)
        {
            _purchaseCallback = onPurchaseResult;
            _storeController.InitiatePurchase(product.ProductId);
        }

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            _storeController = controller;
            _storeExtensionProvider = extensions;
        }

        [ObsoleteAttribute("This method is obsolete. Call void void RestorePurchasesWithResult(Action<bool> onRestoreFinished) instead.", false)]
        public void RestorePurchases(Action onRestoreFinished)
        {
            if (_storeExtensionProvider == null)
            {
                Debug.Log("Store Extension Provider cannot be found, IAP haven't initialized properly or you are calling restore purchase too early");
                return;
            }

            _storeExtensionProvider.GetExtension<IAppleExtensions>().RestoreTransactions((result) =>
            {
                Debug.Log(result ? "Successfully restored IAP" : "Failed to restore IAP");
                onRestoreFinished?.Invoke();
            });
        }

        public void RestorePurchasesWithResult(Action<bool> onRestoreFinished)
        {
            if (_storeExtensionProvider == null)
            {
                Debug.Log("Store Extension Provider cannot be found, IAP haven't initialized properly or you are calling restore purchase too early");
                return;
            }

            _storeExtensionProvider.GetExtension<IAppleExtensions>().RestoreTransactions((result) =>
            {
                Debug.Log(result ? "Successfully restored IAP" : "Failed to restore IAP");
                onRestoreFinished?.Invoke(result);
            });
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            Debug.Log(error.ToString());
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            _purchaseCallback?.Invoke(new PurchaseProductResult()
            {
                Result = false,
                PurchaseResult = failureReason.ToString(),
                ProductMetadata = GetProductMetaData(product.MapFromProduct())
            });

            _purchaseCallback = null;
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
        {
            var purchaseProduct = new PurchaseProductResult()
            {
                Result = true,
                PurchaseResult = "Success",
                ProductMetadata = GetProductMetaData(purchaseEvent.purchasedProduct.MapFromProduct())
            };

            var isNoAds = ProductIsNoAds(purchaseEvent.purchasedProduct.MapFromProduct());
            if (isNoAds)
            {
                PlayerPrefs.SetInt(NoAdsKey, 1);
                KetchappSDK.Advertisement.HideBanner();
            }

            if (_purchaseCallback == null && !isNoAds)
            {
                Debug.Log("Process purchase from restoration, searching for implementation of IPurchasingEvent");
                var inter = GameObject.FindObjectsOfType<MonoBehaviour>().OfType<IPurchasingEvents>();
                if (inter.Any())
                {
                    foreach (var purchaseInterface in inter)
                    {
                        purchaseInterface.OnPurchaseRestoreResult(purchaseProduct);
                    }
                }
                else
                {
                    throw new Exception("[Mayo SDK] : Restoration failed : IPurchasingEvent is not implemented, implement this interface to allow purchase restoration");
                }
            }

            if (_purchaseCallback != null)
            {
                var product = purchaseEvent.purchasedProduct;
                var price = Convert.ToInt32(product.metadata.localizedPrice * 100);
                KetchappInternal.Analytics.InAppPurchaseMade(product.definition.id, product.metadata.isoCurrencyCode, price.ToString());
            }

            _purchaseCallback?.Invoke(purchaseProduct);
            _purchaseCallback = null;
            return PurchaseProcessingResult.Complete;
        }

        private bool ProductIsNoAds(ProductDescription product)
        {
            var noads = Products.Where(p => p.IsNoAds).ToList();
            if (noads == null)
            {
                return false;
            }

            return noads.Any(n => n.ProductId == product.ProductId);
        }
    }

    public static class ProductMapper
    {
        public static ProductDescription MapFromProduct(this Product product)
        {
            return new ProductDescription()
            {
                ProductId = product.definition.id,
                ProductName = product.metadata.localizedTitle,
                Type = product.definition.type,
                Price = (float)product.metadata.localizedPrice
            };
        }
#endif
    }
}
