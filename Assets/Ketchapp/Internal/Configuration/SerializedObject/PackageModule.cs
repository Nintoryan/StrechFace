using System;
namespace Ketchapp.Internal.Configuration
{
    [Serializable]
    public class PackageModule
    {
#pragma warning disable SA1401 // Fields should be private
        public string ModuleName;
        public string ModuleVersion;
#pragma warning restore SA1401 // Fields should be private
    }
}