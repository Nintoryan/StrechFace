using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_PURCHASING
using UnityEngine.Purchasing;
#endif

namespace Ketchapp.MayoSDK.Purchasing
{
    [Serializable]
    public class ProductDescription
    {
#pragma warning disable SA1401 // Fields should be private
        public string ProductId;
        public string ProductIdAndroid;
        public string ProductName;
        #if UNITY_PURCHASING
        public ProductType Type;
#endif
        public float Price;
        public bool IsNoAds;

       /* [SerializeField]
        public UnityEvent PurchaseEvent;*/

#pragma warning restore SA1401 // Fields should be private

        public bool IsValid
        {
            get
            {
                return ProductNameValid && ProductIdValid && ProductHasNoDigits && !ProductNameHasWhiteSpace;
            }
        }

        public bool ProductNameHasWhiteSpace
        {
            get
            {
                if (string.IsNullOrEmpty(ProductName))
                {
                    return false;
                }

                return ProductName.Any(x => char.IsWhiteSpace(x));
            }
        }

        public bool ProductNameValid
        {
            get
            {
                return !string.IsNullOrEmpty(ProductName);
            }
        }

        public bool ProductIdValid
        {
            get
            {
                return !string.IsNullOrEmpty(ProductId);
            }
        }

        public bool ProductHasNoDigits
        {
            get
            {
                if (!ProductNameValid)
                {
                    return true;
                }

                return ProductName.All(c => !char.IsDigit(c));
            }
        }
    }
}