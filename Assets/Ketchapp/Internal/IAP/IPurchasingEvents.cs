using System;
using Ketchapp.MayoSDK.Purchasing;

namespace Ketchapp.MayoSDK.Purchasing
{
    public interface IPurchasingEvents
    {
#if UNITY_PURCHASING
        void OnPurchaseRestoreResult(PurchaseProductResult restoredProduct);
#endif
    }
}
