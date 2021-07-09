#if MEDIATION_FairBid
using Fyber;
#endif
using System;

namespace Ketchapp.Internal.Advertisement
{
#if MEDIATION_FairBid
internal class FairBidRewardedListener : RewardedListener
{
    public static event Action<bool> RewardedShown;
    public static event Action RewardedHidden;
    public static event Action RewardedFailed;
    public static event Action RewardedClicked;
    public static event Action RewardedAvailable;

    private bool _completedRv;
    public void OnShow(string placementId, ImpressionData impressionData)
    {
        KetchappInternal.Analytics.RewardVideoValidated(impressionData.netPayout);
    }

    public void OnClick(string placementId)
    {
        // Called when a rewarded ad from placement 'placementId' is clicked
        RewardedClicked?.Invoke();
    }

    public void OnHide(string placementId)
    {
        RewardedShown?.Invoke(_completedRv);
        RewardedHidden?.Invoke();
    }

    public void OnShowFailure(string placementId, ImpressionData impressionData)
    {
        // Called when an error arises when showing a rewarded ad from placement 'placementId'
        RewardedFailed?.Invoke();
    }

    public void OnAvailable(string placementId)
    {
        // Called when a rewarded ad from placement 'placementId' becomes available
        RewardedAvailable?.Invoke();
    }

    public void OnUnavailable(string placementId)
    {
        // Called when a rewarded ad from placement 'placementId' becomes unavailable
    }

    public void OnCompletion(string placementId, bool userRewarded)
    {
        // Called when a rewarded ad from placement 'placementId' finishes playing. In case the ad is a video, audio play will stop here.
        _completedRv = userRewarded;
    }

    public void OnRequestStart(string placementId)
    {
        // Called when a rewarded ad from placement 'placementId' is going to be requested
    }
}
#endif
}
