using System;
using System.IO;
using System.Threading;
using Ketchapp.Editor.Utils;
using Ketchapp.MayoAPI.Dto;
using Tiny.RestClient;
using UnityEditor;
using UnityEngine;

namespace Ketchapp.Editor
{
    internal class AuthorizedWindow : EditorWindow
    {
        public LoginDto Login { get; set; } = new LoginDto();
        public bool IsLogging { get; set; }
        private bool _hasError;
        private string _errorMessage;
        public async virtual void OnGUI()
        {
            GUILayout.Box(KetchappEditorHelper.KetchappLogo, GUILayout.Height(100), GUILayout.Width(position.width));

            if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android && EditorUserBuildSettings.activeBuildTarget != BuildTarget.iOS && EditorUserBuildSettings.activeBuildTarget != BuildTarget.iOS)
            {
                EditorGUILayout.HelpBox("Ketchapp Mayo SDK is designed to work for iPhone or Android build target, being on any other target may cause compilation issues", MessageType.Error);
                EditorGUILayout.HelpBox("If your game is currently in softlaunch state, remember to switch to your softlaunch platform since configuration is platform dependent", MessageType.Warning);
                return;
            }

            if (!KetchappEditorUtils.AuthenticationManager.IsUserAuthenticated)
            {
                EditorGUILayout.Space(10);
                Login.Email = EditorGUILayout.TextField("Email", Login.Email);
                Login.Password = EditorGUILayout.PasswordField("Password", Login.Password);

                EditorGUILayout.Space(10);
                GUI.enabled = !IsLogging;
                if (GUILayout.Button(IsLogging ? "Logging in ..." : "Login"))
                {
                    _hasError = false;
                    IsLogging = true;
                    try
                    {
                        await KetchappEditorUtils.AuthenticationManager.LoginAsync(Login);
                    }
                    catch (Tiny.RestClient.HttpException e)
                    {
                        _hasError = true;
                        if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                        {
                            Debug.Log("not found");
                            _errorMessage = "Wrong password";
                        }
                        else
                        {
                            _errorMessage = e.Message;
                        }
                    }
                    catch (Exception e)
                    {
                        _errorMessage = $"[Mayo SDK] : Failed to login : {e.Message}";
                        _hasError = true;
                    }

                    IsLogging = false;
                    GUI.changed = true;
                }

                if (_hasError)
                {
                    EditorGUILayout.HelpBox($"Error : {_errorMessage}", MessageType.Error);
                }

                GUI.enabled = true;
            }
            else
            {
                KetchappEditorUtils.AuthenticationManager.SetToken();
            }
        }
    }
}