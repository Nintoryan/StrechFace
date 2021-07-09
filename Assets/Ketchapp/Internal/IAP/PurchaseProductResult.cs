using System;
#if UNITY_PURCHASING
using UnityEngine.Purchasing;

namespace Ketchapp.MayoSDK.Purchasing
{
    public class PurchaseProductResult
    {
        public ProductMetadata ProductMetadata { get; set; }
        public bool Result { get; set; }
        public string PurchaseResult { get; set; }
    }
}
#endif