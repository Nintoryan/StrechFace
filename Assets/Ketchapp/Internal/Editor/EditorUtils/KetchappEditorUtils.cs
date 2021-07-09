using System.Net.Http;
using Ketchapp.Editor.Authentication;
using Ketchapp.Editor.Utils.Migrations;
using Ketchapp.Editor.Utils.Package;
using Ketchapp.Internal.Configuration;
using Ketchapp.MayoAPI.Dto;
namespace Ketchapp.Editor.Utils
{
    internal static class KetchappEditorUtils
    {
        static KetchappEditorUtils()
        {
            var client = new HttpClient();
            AuthenticationManager = new AuthenticationManager();
            MayoApiClient = new KetchappMayoApiClient(new Tiny.RestClient.TinyRestClient(client, "https://mayo.ketchappgames.com/api"));
            SdkService = new KetchappSdkService();
            Configuration = new ConfigurationManager();
            PackageManagerModifier = new PackageManagerModifier();
            UnityServicesManager = new UnityServicesManager();
            MigrationManager = new MigrationManager();
        }

        public static ConfigurationManager Configuration { get; set; }
        public static KetchappSdkService SdkService { get; set; }
        public static KetchappMayoApiClient MayoApiClient { get; set; }
        public static AuthenticationManager AuthenticationManager { get; set; }
        public static PackageManagerModifier PackageManagerModifier { get; set; }
        public static UnityServicesManager UnityServicesManager { get; set; }
        public static MigrationManager MigrationManager { get; set; }
    }
}
