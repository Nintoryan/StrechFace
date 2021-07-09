using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ketchapp.Editor.Utils;
using Ketchapp.Editor.Utils.Migrations;
using Ketchapp.Internal.Configuration;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Migrating old version file from 1 info per line to json.
/// </summary>
public class Migration_1_3_0_0 : KetchappMigrationBase
{
    public override System.Version MigrationSDKVersion => new System.Version(1, 3, 0, 0);

    public override void ApplyMigration()
    {
        var sdkVersions = new List<SDKVersion>();

        var path = Path.Combine(Application.dataPath, "Dependencies", "Ketchapp", "Versions");
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        foreach (var file in Directory.EnumerateFiles(path))
        {
            FileInfo info = new FileInfo(file);
            if (info.Name.Contains(".meta") || info.Name.Contains(".json"))
            {
                continue;
            }

            try
            {
                var version = File.ReadAllLines(info.FullName);
                sdkVersions.Add(new SDKVersion()
                {
                    Name = version[1],
                    Type = (SDKType)Enum.Parse(typeof(SDKType), version[2]),
                    Version = version[0],
                    PluginPaths = version.Skip(3).Take(version.Length - 3).ToArray(),
                    PackageType = Ketchapp.MayoAPI.Dto.PackageType.LegacyUnityPackage
                });

                info.Delete();
            }
            catch
            {
                continue;
            }
        }

        foreach (var sdk in sdkVersions)
        {
            KetchappEditorUtils.SdkService.WritePackageVersion(sdk);
        }

        AssetDatabase.Refresh();
    }
}
