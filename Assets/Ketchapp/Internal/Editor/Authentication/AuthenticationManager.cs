using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Ketchapp.Editor.Utils;
using Ketchapp.MayoAPI.Dto;
using Tiny.RestClient;
using UnityEditor;
using UnityEngine;
namespace Ketchapp.Editor.Authentication
{
    internal class AuthenticationManager : IDisposable
    {
        public Action OnUserLogged { get; set; }
        public string UserToken
        {
            get
            {
                if (File.Exists(Path.Combine(TokenPath, TokenFileName)))
                {
                    return File.ReadAllText(Path.Combine(TokenPath, TokenFileName));
                }
                else
                {
                    return string.Empty;
                }
            }
            set
            {
                if (value != _userToken)
                {
                    _userToken = value;
                }
            }
        }

        public string TokenPath => Path.Combine(Application.dataPath, "Dependencies", "Ketchapp", "Configuration", "Auth");

        private string TokenFileName => "Token";
        private string _userToken;

        public bool IsUserAuthenticated
        {
            get
            {
                return !string.IsNullOrEmpty(UserToken);
            }
        }

        public AuthenticationManager()
        {
            CheckOnAuthDirectory();
        }

        private void CheckOnAuthDirectory()
        {
            if (!Directory.Exists(TokenPath))
            {
                Directory.CreateDirectory(TokenPath);
            }
        }

        public async Task LoginAsync(LoginDto login)
        {
            CheckOnAuthDirectory();
            var result = await KetchappEditorUtils.MayoApiClient.LoginAsync(login, CancellationToken.None);
            StoreTokenLocally(result.Token);
            OnUserLogged?.Invoke();
        }

        public void SetToken()
        {
            KetchappMayoApiClient.Token = UserToken;
        }

        private void StoreTokenLocally(string token)
        {
            File.WriteAllText(Path.Combine(TokenPath, TokenFileName), token);
            AssetDatabase.Refresh();
        }

        public void RemoveLocalToken()
        {
            FileUtil.DeleteFileOrDirectory(Path.Combine(TokenPath, TokenFileName));
        }

        public void Dispose()
        {
#if UNITY_2019_1_OR_NEWER
            EditorApplication.quitting -= RemoveLocalToken;
#else
        RemoveLocalToken();
#endif
        }
    }
}