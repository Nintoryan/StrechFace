using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ketchapp.MayoSDK.Purchasing
{
    [Serializable]
    public class IAPConfiguration : ScriptableObject
    {
        [SerializeField]
        public List<ProductDescription> Products;
    }
}
