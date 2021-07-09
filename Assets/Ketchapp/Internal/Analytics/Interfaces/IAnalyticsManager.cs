namespace Ketchapp.Internal.Analytics
{
    internal interface IAnalyticsManager
    {
        bool IsInitialized { get; set; }

        void Initialize();
        void InterstitialShowed(string impressionData);
        void RewardVideoValidated(string impressionData);
        void BannerLoaded(string impressionData);
        void InAppPurchaseMade(string item, string currency, string value);
        void ApplicationInstalled();
        void ApplicationSession();
        void SendAttResult(string eventName);
        void LevelStarted(string levelName);
        void LevelPassed(string levelName, int score);
        void LevelFailed(string levelName, int score);

        void CustomEvent(string eventName);
        void CustomEvent(string eventName, float eventValue);
    }
}