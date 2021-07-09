using System;
using System.Collections.Generic;

namespace Ketchapp.Internal.Configuration
{
    [Serializable]
    public class MediationAdapter
    {
#pragma warning disable SA1401 // Fields should be private
        public List<IosPods> PodInfo;
        public List<AndroidPackages> AndroidPackagesInfos;
#pragma warning restore SA1401 // Fields should be private
    }
}