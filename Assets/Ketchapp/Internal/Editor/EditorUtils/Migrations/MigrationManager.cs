using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace Ketchapp.Editor.Utils.Migrations
{
    public class MigrationManager
    {
        private string MigrationHistoryFolder => "Assets/Dependencies/Ketchapp/Migrations";
        private string MigrationHistoryFileName => "history.json";

        public string SavedLastMigrationVersionKey => "last_migration_version_applied";

        public MigrationManager()
        {
            if (!Directory.Exists(MigrationHistoryFolder))
            {
                Directory.CreateDirectory(MigrationHistoryFolder);
            }
        }

        public void Test()
        {
            Migrate();
        }

        public void Migrate()
        {
            var migrationList = GetOrderedMigrationList();
            var appliedMigrations = GetAppliedMigrations();

            if (!File.Exists(KetchappEditorUtils.Configuration.ConfigurationObjectPath))
            {
                Debug.Log("Fresh install, won't migrate");

                // No migration to apply on a fresh import
                migrationList.ForEach(m => AddMigrationToHistory(m.MigrationSDKVersion));

                return;
            }

            float iteration = 0;

            foreach (var migration in migrationList)
            {
                iteration++;
                if (!appliedMigrations.Contains(migration.MigrationSDKVersion))
                {
                    migration.ApplyMigration();
                    EditorUtility.DisplayProgressBar("Applying Migrations", $"Applying migration {migration.MigrationSDKVersion}", iteration / migrationList.Count);
                    AddMigrationToHistory(migration.MigrationSDKVersion);
                }
            }

            EditorUtility.ClearProgressBar();

            PlayerPrefs.SetString(SavedLastMigrationVersionKey, KetchappEditorUtils.SdkService.KetchappSDKVersion);
        }

        private void AddMigrationToHistory(System.Version version)
        {
            var appliedMigrations = GetAppliedMigrations();
            appliedMigrations.Add(version);
            var json = JsonConvert.SerializeObject(appliedMigrations);

            var historyPath = Path.Combine(MigrationHistoryFolder, MigrationHistoryFileName);
            File.WriteAllText(historyPath, json);
            AssetDatabase.Refresh();
        }

        private List<System.Version> GetAppliedMigrations()
        {
            var historyPath = Path.Combine(MigrationHistoryFolder, MigrationHistoryFileName);
            if (File.Exists(historyPath))
            {
                var json = File.ReadAllText(historyPath);

                return JsonConvert.DeserializeObject<List<System.Version>>(json);
            }
            else
            {
                return new List<System.Version>();
            }
        }

        private List<KetchappMigrationBase> GetOrderedMigrationList()
        {
            var migrationsReturned = new List<KetchappMigrationBase>();
            var assembly = Assembly.Load("Assembly-CSharp-Editor");
            var migrations = assembly.GetTypes().Where(t => t.BaseType == typeof(KetchappMigrationBase)).ToList();
            foreach (var migration in migrations)
            {
                var instance = (KetchappMigrationBase)Activator.CreateInstance(migration);
                migrationsReturned.Add(instance);
            }

            return migrationsReturned.OrderBy(m => m.MigrationSDKVersion).ToList();
        }
    }
}
