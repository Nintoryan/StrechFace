using System;
using Ketchapp.Internal;
using Ketchapp.Internal.CrossPromo;
using UnityEngine;

namespace Ketchapp.MayoSDK.CrossPromo
{
    public class CrossPromoManager : MonoBehaviour
    {
        private Vector2 DefaultSquarePosition => new Vector2(400, 600);

#if CrossPromotion
        public CrossPromoCallbacks Callbacks { get; set; }
#endif

        internal CrossPromoManager()
        {
        }

        public void OnEnable()
        {
            var manager = gameObject.AddComponent(typeof(Internal.CrossPromo.CrossPromoManager)) as Internal.CrossPromo.CrossPromoManager;
            KetchappInternal.CrossPromo = manager;
#if CrossPromotion
            gameObject.AddComponent(typeof(CrossPromoCallbacks));
            Callbacks = gameObject.GetComponent<CrossPromoCallbacks>();
#endif
        }

        public void Initialize(Action onDone = null)
        {
            KetchappInternal.CrossPromo.Initialize(onDone);
            KetchappInternal.CrossPromo.RequestSquare();
        }

        /// <summary>
        /// Show the CrossPromo interstitial. It will show the GDPR popup on first launch.
        /// </summary>
        public void ShowInterstitial(Action onInterstitialClosed = null)
        {
            KetchappInternal.CrossPromo.ShowInterstitial(onInterstitialClosed);
        }

        /// <summary>
        /// Show the GDPR popup.
        /// </summary>
        public void ShowGdpr(Action onGdprClosed = null)
        {
            KetchappInternal.CrossPromo.ShowGdpr(onGdprClosed);
        }

        /// <summary>
        /// Get the current GDPR opt-in value.
        /// </summary>
        /// <returns></returns>
        public bool GetGdprValue()
        {
            return KetchappInternal.CrossPromo.GetGdprValue();
        }

        /// <summary>
        /// Returns whether or not the country the user is located in applies GDPR.
        /// </summary>
        /// <returns> true if user is in GDPR country.</returns>
        public bool CheckGdprCountry()
        {
            return KetchappInternal.CrossPromo.CheckGdprCountry();
        }

        /// <summary>
        /// Request a new promotion ad.
        /// </summary>
        public void RequestSquare()
        {
            KetchappInternal.CrossPromo.RequestSquare();
        }

        /// <summary>
        /// Show the cross promotion square video.
        /// </summary>
        public void ShowSquare()
        {
            KetchappInternal.CrossPromo.ShowSquare(DefaultSquarePosition);
            RequestSquare();
        }

        /// <summary>
        /// Show the cross promotion square video in a defined location with normalized coordinates.
        /// To unify coordinates between Android and iOS. Show the Ketchapp square with Normalized coordinates(from 0 to 1). calculates by offset from x and y.
        /// </summary>
        /// <param name="x">The square X position (in normals from 0 to 1). You can use anchor position from RectTransform.</param>
        /// <param name="y">The square Y position (in normals from 0 to 1). You can use anchor position from RectTransform.</param>
        public void ShowSquare(float x, float y)
        {
            KetchappInternal.CrossPromo.ShowSquare(x, y);
        }

        /// <summary>
        /// Show the cross promotion square video in a defined location.
        /// </summary>
        /// <param name="position">The square position (in screen size)</param>
        public void ShowSquare(Vector2 position)
        {
            KetchappInternal.CrossPromo.ShowSquare(position);
        }

        /// <summary>
        /// Show the cross promotion square video in a defined location with a specific scale.
        /// </summary>
        /// <param name="position">The square position (in screen size).</param>
        /// <param name="scale">The square scale.</param>
        public void ShowSquare(Vector2 position, Vector3 scale)
        {
            KetchappInternal.CrossPromo.ShowSquare(position, scale);
        }

        /// <summary>
        /// Show the cross promotion square video in a defined location with a specific scale and rotation.
        /// </summary>
        /// <param name="position">The square position (in screen size).</param>
        /// <param name="scale">The square scale.</param>
        /// <param name="rotation">The square rotation.</param>
        public void ShowSquare(Vector2 position, Vector3 scale, float rotation)
        {
            KetchappInternal.CrossPromo.ShowSquare(position, scale, rotation);
        }

        /// <summary>
        /// Hide the cross promotion square video.
        /// </summary>
        public void HideSquare()
        {
            KetchappInternal.CrossPromo.HideSquare();
        }
    }
}
