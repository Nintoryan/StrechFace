using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Ketchapp.Internal.DebugAds
{
    public class DebugInterstitial : MonoBehaviour
    {
        private HideButton _hideButton;
        public Action<bool> OnInterstitialClosed;
        private Text _interstitialText;

        public void Initialize(float timer, InterstitialDebugType interstitialDebugType)
        {
            var canvas = GetComponent<Canvas>();
            canvas.sortingOrder = 10;
            _hideButton = GetComponentInChildren<HideButton>();
            _interstitialText = GetComponentInChildren<Text>();
            _hideButton.OnHideButtonClicked += CloseInterstitial;
            _hideButton.HideTimer = timer;

            switch (interstitialDebugType)
            {
                case InterstitialDebugType.Interstitial:
                    {
                        _interstitialText.text = $"This is an interstitial debug UI for Editor Preview Purpose\n Hide Timer : {timer}s";
                        break;
                    }

                case InterstitialDebugType.RewardedVideo:
                    {
                        _interstitialText.text = $"This is a rewarded video debug UI for Editor Preview Purpose \n Hide Timer : {timer}s";
                        break;
                    }

                default:
                    break;
            }
        }

        private void CloseInterstitial()
        {
            OnInterstitialClosed?.Invoke(true);
        }

        public enum InterstitialDebugType
        {
            Interstitial,
            RewardedVideo
        }

        public void MarkInterstitialAsFail()
        {
            OnInterstitialClosed?.Invoke(false);
        }
    }
}
