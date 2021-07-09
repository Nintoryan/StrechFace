using System;
#if MEDIATION_FairBid
using Fyber;
#endif
namespace Ketchapp.Internal.Advertisement
{
#if MEDIATION_FairBid
internal class FairBidBannerListener : BannerListener
{
    public static event Action BannerShown;
    public static event Action BannerHidden;
    public static event Action BannerFailed;
    public static event Action BannerClicked;
    public static event Action BannerAvailable;
    public static event Action BannerRequested;

    public void OnError(string placementId, string error)
    {
        // Called when an error from placement 'placementId' arises when loading an ad
        BannerFailed?.Invoke();
    }

    public void OnLoad(string placementId)
    {
        // Called when an ad from placement 'placementId' is loaded
        BannerAvailable?.Invoke();
    }

    public void OnShow(string placementId, ImpressionData impressionData)
    {
        // Called when banner from placement 'placementId' shows up
            BannerShown?.Invoke();
            KetchappInternal.Analytics.BannerLoaded(impressionData.netPayout);
    }

    public void OnClick(string placementId)
    {
        // Called when banner from placement 'placementId' is clicked
        BannerClicked?.Invoke();
    }

    public void OnRequestStart(string placementId)
    {
        // Called when a banner from placement 'placementId' is going to be requested
        BannerRequested?.Invoke();
    }
}
#endif
}