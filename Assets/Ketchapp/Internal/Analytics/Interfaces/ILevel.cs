using System;

namespace Ketchapp.MayoSDK.Analytics
{
    public interface ILevel
    {
        ILevel ProgressionStart();
        ILevel ProgressionComplete();
        ILevel ProgressionFailed();

        ILevel PostTime();
        ILevel Score(int score);
        ILevel Progression(int percentage);
        ILevel EarnMoney(int earnedValue, int totalValue);
        ILevel Reset();
        ILevel Skip();

        ILevel CustomEvent(string eventName);
        ILevel CustomEvent(string eventName, float eventValue);
    }
}