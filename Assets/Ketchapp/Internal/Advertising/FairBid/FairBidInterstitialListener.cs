#if MEDIATION_FairBid
using Fyber;
using Ketchapp.MayoSDK;
#endif
using System;
namespace Ketchapp.Internal.Advertisement
{
#if MEDIATION_FairBid
internal class FairBidInterstitialListener : InterstitialListener
{
    public static event Action InterstitialShown;
    public static event Action InterstitialHidden;
    public static event Action InterstitialFailed;
    public static event Action InterstitialClicked;
    public static event Action InterstitialAvailable;

    public void OnShow(string placementId, ImpressionData impressionData)
    {
            // Called when an Interstitial from placement 'placementId' shows up. In case the ad is a video, audio play will start here.
            // On Android, this callback might be called only once the ad is closed.
            KetchappInternal.Analytics.InterstitialShowed(impressionData.netPayout);
            InterstitialShown?.Invoke();
    }

    public void OnClick(string placementId)
    {
        // Called when an Interstitial from placement 'placementId' is clicked
        InterstitialClicked?.Invoke();
    }

    public void OnHide(string placementId)
    {
        // Called when an Interstitial from placement 'placementId' hides.
        InterstitialHidden?.Invoke();
    }

    public void OnShowFailure(string placementId, ImpressionData impressionData)
    {
        // Called when an error arises when showing an Interstitial from placement 'placementId'
        InterstitialFailed?.Invoke();
    }

    public void OnAvailable(string placementId)
    {
        // Called when an Interstitial from placement 'placementId' becomes available
        InterstitialAvailable?.Invoke();
    }

    public void OnUnavailable(string placementId)
    {
        // Called when an Interstitial from placement 'placementId' becomes unavailable
    }

    public void OnRequestStart(string placementId)
    {
        // Called when an Interstitial from placement 'placementId' is going to be requested
    }
}
#endif
}
