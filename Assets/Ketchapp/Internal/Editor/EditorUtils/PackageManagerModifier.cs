using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

namespace Ketchapp.Editor.Utils.Package
{
    public class PackageManagerModifier
    {
        private string PackageManagerManifestPath => Path.Combine(Application.dataPath, "..", "Packages", "manifest.json");
        public PackageManagerModifier()
        {
        }

        private Packages ReadManifestJson()
        {
            var json = File.ReadAllText(PackageManagerManifestPath);
            return JsonConvert.DeserializeObject<Packages>(json);
        }

        public void UpsertElementToManifest(string package, string version)
        {
            var packages = ReadManifestJson();
            if (ManifestElementAlreadyExists(package))
            {
                Debug.Log($"Package {package} already exists, checking version");
                if (ManifestElementIsLastVersion(package, version))
                {
                    Debug.Log($"Package {package} is already up-to-date, aborting..");
                    return;
                }

                packages.Modules[package] = version;
            }
            else
            {
                packages.Modules.Add(package, version);
            }

            File.WriteAllText(PackageManagerManifestPath, JsonConvert.SerializeObject(packages, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
            Debug.Log($"Package {package} added !");
        }

        public void RemoveElementFromManifest(string package)
        {
            var packages = ReadManifestJson();
            packages.Modules.Remove(package);
            File.WriteAllText(PackageManagerManifestPath, JsonConvert.SerializeObject(packages, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
        }

        private bool ManifestElementAlreadyExists(string package)
        {
            var packages = ReadManifestJson();
            return packages.Modules.ContainsKey(package);
        }

        private bool ManifestElementIsLastVersion(string package, string version)
        {
            var packages = ReadManifestJson();
            return packages.Modules[package] == version;
        }
    }

    public class Packages
    {
        [JsonProperty("scopedRegistries")]
        public List<ScopedRegistry> ScopedRegistries { get; set; }

        [JsonProperty("dependencies")]
        public Dictionary<string, string> Modules { get; set; }
    }

    public partial class ScopedRegistry
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("url")]
        public Uri Url { get; set; }

        [JsonProperty("scopes")]
        public List<string> Scopes { get; set; }
    }
}