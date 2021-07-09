using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ketchapp.Editor.Utils;
using Ketchapp.MayoAPI.Dto;
using Ketchapp.MayoSDK.Purchasing;
using UnityEditor;
using UnityEngine;

namespace Ketchapp.Internal.Configuration
{
    internal class ConfigurationManager
    {
        public string ConfigurationObjectPath => Path.Combine("Assets", "Dependencies", "Ketchapp", "Configuration", "Resources", "KetchappSettings.asset");
        public string ConfigurationObjectName => "KetchappSettings";

        public ConfigurationManager()
        {
            var configurationFolder = Path.GetDirectoryName(ConfigurationObjectPath);
            if (!Directory.Exists(configurationFolder))
            {
                Directory.CreateDirectory(configurationFolder);
            }
        }

        public GameInfos ConfigurationObject
        {
            get
            {
                return Resources.Load<GameInfos>("KetchappSettings");
            }
        }

        public GameConfiguration PlatformConfiguration
        {
            get
            {
                if (Application.isEditor)
                {
                    if (ConfigurationObject == null)
                    {
                        return null;
                    }

                    if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
                    {
                        _platformConfiguration = ConfigurationObject.AndroidConfiguration;
                    }
                    else if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS)
                    {
                        _platformConfiguration = ConfigurationObject.IosConfiguration;
                    }
                    else
                    {
                        _platformConfiguration = null;
                    }

                    if (_platformConfiguration != null)
                    {
                        if (string.IsNullOrEmpty(_platformConfiguration.FacebookAppId))
                        {
                            _platformConfiguration = null;
                        }
                    }
                }
                else
                {
                    var platform = Application.platform;
                    if (platform == RuntimePlatform.Android)
                    {
                        _platformConfiguration = ConfigurationObject.AndroidConfiguration;
                    }
                    else
                    {
                        _platformConfiguration = ConfigurationObject.IosConfiguration;
                    }
                }

                return _platformConfiguration;
            }
        }

        private GameConfiguration _platformConfiguration;
        private GameInfos _configurationObject;

        public List<StudioDto> StudioList
        {
            get
            {
                return _studioList;
            }
            set
            {
                _studioList = value;
            }
        }

        private List<StudioDto> _studioList = new List<StudioDto>();

        public string[] UserStudios
        {
            get
            {
                var list = StudioList.Select(s => s.Name).ToList();
                return list.ToArray();
            }
        }

        public bool ConfigurationObjectExists
        {
            get
            {
                return AssetDatabase.LoadAssetAtPath<GameInfos>(ConfigurationObjectPath) != null && _platformConfiguration != null;
            }
        }

        public string[] GetGamesNameById(int id)
        {
            if (StudioList.Count > 0)
            {
                return StudioList[id].Games.Select(g => g.GameName).ToArray();
            }

            return new string[] { "None" };
        }

        public void CreateConfigurationObjectAsync(GameDetailDto game, string studioName)
        {
            if (ConfigurationObjectExists)
            {
                AssetDatabase.DeleteAsset(ConfigurationObjectPath);
            }

            var asset = ScriptableObject.CreateInstance<GameInfos>();
            asset.GameName = game.Name;
            asset.GameState = (int)game.GameState;
            asset.StudioName = studioName;
            asset.SKAdNetworksId = game.SKAdNetworksId;
            asset.IosConfiguration = game.ConfigurationIos.MapFromDto();
            asset.AndroidConfiguration = game.ConfigurationAndroid.MapFromDto();

            AssetDatabase.CreateAsset(asset, ConfigurationObjectPath);
            EditorUtility.SetDirty(asset);
        }

        public async Task PopulateStudioListAsync()
        {
            StudioList = await KetchappEditorUtils.MayoApiClient.GetStudiosAsync(CancellationToken.None);
        }
    }
}
