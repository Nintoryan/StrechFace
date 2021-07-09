using System;
using System.Collections;
using System.Collections.Generic;
using Ketchapp.MayoSDK;
using UnityEngine;
#if CrossPromotion
using KetchappCrossPromotion.NativeSDK;
#endif
namespace Ketchapp.Internal.CrossPromo
{
    internal class CrossPromoManager : MonoBehaviour
    {
        public void Initialize(Action onDone)
        {
#if CrossPromotion
            KetchappPromo.Initialize(
                () =>
            {
                onDone?.Invoke();
            }, false);
#endif
        }

        public void ShowSquare(float x, float y)
        {
            if (KetchappSDK.Advertisement.HasNoAds)
            {
                Debug.Log("No Ads is enabled, won't show square");
                return;
            }
#if CrossPromotion
            StartCoroutine(KetchappPromo.ShowSquare(new Vector2(x, y)));
#endif
        }

        public void ShowSquare(Vector2 position)
        {
            if (KetchappSDK.Advertisement.HasNoAds)
            {
                Debug.Log("No Ads is enabled, won't show square");
                return;
            }
#if CrossPromotion && UNITY_IOS
            StartCoroutine(KetchappPromo.ShowSquare(new KetchappSquareTransform(position, new Vector2(350,400), 0)));
#endif
        }

        public void ShowSquare(Vector2 position, Vector3 scale)
        {
            if (KetchappSDK.Advertisement.HasNoAds)
            {
                Debug.Log("No Ads is enabled, won't show square");
                return;
            }

            scale.x = Mathf.Abs(scale.x);
            scale.y = Mathf.Abs(scale.y);
#if CrossPromotion && UNITY_IOS
            StartCoroutine(KetchappPromo.ShowSquare(new KetchappSquareTransform(position, scale, 0)));
#endif
        }

        public void ShowSquare(Vector2 position, Vector3 scale, float rotation)
        {
            if (KetchappSDK.Advertisement.HasNoAds)
            {
                Debug.Log("No Ads is enabled, won't show square");
                return;
            }
#if CrossPromotion && UNITY_IOS
            StartCoroutine(KetchappPromo.ShowSquare(new KetchappSquareTransform(position, scale, rotation)));
#endif
        }

        public void RequestSquare()
        {
#if CrossPromotion && UNITY_IOS
            StartCoroutine(KetchappPromo.FetchSquare());
#endif
        }

        public void HideSquare()
        {
#if CrossPromotion && UNITY_IOS
            StartCoroutine(KetchappPromo.RemoveSquare());
            StartCoroutine(KetchappPromo.FetchSquare());
#endif
        }

        public void ShowInterstitial(Action onInterstitialClosed)
        {
#if CrossPromotion
            StartCoroutine(KetchappPromo.RequestKetchappPromo(KetchappSDK.Advertisement.HasNoAds));
            KetchappSDK.CrossPromo.Callbacks.PromotionClosed = onInterstitialClosed;
#elif UNITY_EDITOR
            onInterstitialClosed?.Invoke();
#endif
        }

        public bool CheckGdprCountry()
        {
#if CrossPromotion
            return KetchappPromo.IsInGDPRCountry();
#endif
            Debug.Log("Cross Promo sdk is not imported, will return default value: true");
            return true;
        }

        public void ShowGdpr(Action onGdprClosed)
        {
#if CrossPromotion
            StartCoroutine(KetchappPromo.ShowGDPR());
            KetchappSDK.CrossPromo.Callbacks.GdprClosed = onGdprClosed;
#elif UNITY_EDITOR
            onGdprClosed?.Invoke();
#endif
        }

        public bool GetGdprValue()
        {
#if CrossPromotion
            return KetchappPromo.GetGdprValue();
#endif
            return true;
        }
    }
}
