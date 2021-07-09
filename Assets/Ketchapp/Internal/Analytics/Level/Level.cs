using System;
using System.Linq;
using Ketchapp.Internal.Analytics;
using UnityEngine;

namespace Ketchapp.MayoSDK.Analytics
{
    public class Level : ILevel
    {
        private readonly Internal.Analytics.AnalyticsManager _analyticsManager;
        public string LevelNumber { get; set; }

        private string _formattedLevelNumber;

        private float _levelStartedDate;

        internal Level(string levelNumber, Internal.Analytics.AnalyticsManager manager)
        {
            _analyticsManager = manager;
            LevelNumber = levelNumber;

            if (LevelNumber.All(char.IsDigit))
            {
                _formattedLevelNumber = LevelNumber.ToString().PadLeft(4, '0');
            }
            else
            {
                _formattedLevelNumber = LevelNumber;
            }

            _formattedLevelNumber = _formattedLevelNumber.Insert(0, "Level_");
        }

        public ILevel EarnMoney(int earnedValue, int totalValue)
        {
            _analyticsManager.CustomEvent($"ClaimableMoney:{_formattedLevelNumber}", earnedValue);
            _analyticsManager.CustomEvent($"TotalMoney:{_formattedLevelNumber}", totalValue);

            return this;
        }

        public ILevel Progression(int percentage)
        {
            _analyticsManager.CustomEvent($"Progression:{_formattedLevelNumber}", percentage);
            return this;
        }

        public ILevel ProgressionComplete()
        {
            _analyticsManager.LevelPassed($"{_formattedLevelNumber}", 0);
            PostTime();
            return this;
        }

        public ILevel ProgressionFailed()
        {
            _analyticsManager.LevelFailed($"{_formattedLevelNumber}", 0);
            PostTime();
            return this;
        }

        public ILevel ProgressionStart()
        {
            _levelStartedDate = Time.unscaledTime;
            _analyticsManager.LevelStarted($"{_formattedLevelNumber}");
            return this;
        }

        public ILevel Reset()
        {
            _analyticsManager.CustomEvent($"{_formattedLevelNumber}", 0);
            return this;
        }

        public ILevel Score(int score)
        {
            _analyticsManager.CustomEvent($"{_formattedLevelNumber}", score);
            return this;
        }

        public ILevel Skip()
        {
            _analyticsManager.CustomEvent($"Skip:{_formattedLevelNumber}", 0);
            return this;
        }

        public ILevel PostTime()
        {
            _analyticsManager.CustomEvent($"Time:{_formattedLevelNumber}", (float)(Time.unscaledTime - _levelStartedDate));
            return this;
        }

        public ILevel CustomEvent(string eventName)
        {
            _analyticsManager.CustomEvent($"{_formattedLevelNumber}:{eventName}");
            return this;
        }

        public ILevel CustomEvent(string eventName, float eventValue)
        {
            _analyticsManager.CustomEvent($"{_formattedLevelNumber}:{eventName}", eventValue);
            return this;
        }
    }
}
