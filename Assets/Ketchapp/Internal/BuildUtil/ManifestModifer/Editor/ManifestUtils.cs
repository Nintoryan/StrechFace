using System.IO;
using System.Text;

namespace Ketchapp.Internal.BuildUtil.Editor.ManifestModifer
{
    internal static class ManifestUtils
    {
        public static string GetManifestPath(string basePath)
        {
            var pathBuilder = new StringBuilder(basePath);
            pathBuilder.Append(Path.DirectorySeparatorChar).Append("src");
            pathBuilder.Append(Path.DirectorySeparatorChar).Append("main");
            pathBuilder.Append(Path.DirectorySeparatorChar).Append("AndroidManifest.xml");
            return pathBuilder.ToString();
        }
    }
}
