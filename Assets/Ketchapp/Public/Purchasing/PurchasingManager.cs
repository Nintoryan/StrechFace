using System;
using System.Collections.Generic;
using System.Linq;
using Ketchapp.Internal;
using Ketchapp.Internal.Purchasing;
using Ketchapp.MayoSDK.Purchasing;
#if UNITY_PURCHASING
using UnityEngine.Purchasing;
#endif

namespace Ketchapp.MayoSDK.Purchasing
{
    public class PurchasingManager
    {
        public void Initialize()
        {
#if UNITY_PURCHASING
            KetchappInternal.PurchasingManager.Initialize();
#endif
        }
#if UNITY_PURCHASING
        /// <summary>
        /// Initialize an IAP transaction to buy a product.
        /// </summary>
        /// <param name="product">The product the user will buy.</param>
        /// <param name="onPurchaseResult">Callback when the transaction is over.</param>
        public void BuyProduct(PurchasingObjects product, Action<PurchaseProductResult> onPurchaseResult)
        {
            if (KetchappInternal.PurchasingManager.IsInitialized)
            {
                ProductDescription item = GetProductDescription(product);
                if (item == null)
                {
                    throw new Exception("Product not found, check your IAPConfiguration and PurchasingObject.cs");
                }

                KetchappInternal.PurchasingManager.BuyProduct(item, onPurchaseResult);
            }
            else
            {
                onPurchaseResult.Invoke(new PurchaseProductResult() { Result = false, PurchaseResult = "Not initialized" });
            }
        }

        /// <summary>
        /// Returns the localized metadata of a product.
        /// </summary>
        /// <param name="product">The product you want to get the metada from.</param>
        /// <returns></returns>
        public ProductMetadata GetProductMetadata(PurchasingObjects product)
        {
            if (KetchappInternal.PurchasingManager.IsInitialized)
            {
                ProductDescription item = GetProductDescription(product);
                if (item != null)
                {
                    return KetchappInternal.PurchasingManager.GetProductMetaData(item);
                }

                return null;
            }

            return null;
        }

        /// <summary>
        /// Return the metadatas of all the available products.
        /// </summary>
        /// <returns></returns>
        public List<ProductMetadata> GetAllProductMetadata()
        {
            return KetchappInternal.PurchasingManager.GetAllProductMetadata();
        }

        /// <summary>
        /// Will ask a manual restore for NonConsumable/Subscription purchases. Callback are available with IPurchasingEvents.
        /// </summary>
        [ObsoleteAttribute("This method is obsolete. Call void RestorePurchasesWithResult(Action<bool> onRestoreFinished = null) instead.", false)]
        public void RestorePurchases(Action onRestoreFinished = null)
        {
            KetchappInternal.PurchasingManager.RestorePurchases(onRestoreFinished);
        }

        /// <summary>
        /// Will ask a manual restore for NonConsumable/Subscription purchases. Callback indicates if Restore was successful.
        /// </summary>
        /// <param name="onRestoreFinished">true if restore was successful.</param>
        public void RestorePurchasesWithResult(Action<bool> onRestoreFinished = null)
        {
            KetchappInternal.PurchasingManager.RestorePurchasesWithResult(onRestoreFinished);
        }

        private ProductDescription GetProductDescription(PurchasingObjects product)
        {
            return KetchappInternal.IAPConfiguration.Products.FirstOrDefault(p => p.ProductName == product.ToString());
        }
#endif
    }
}