using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Ketchapp.Internal;
using UnityEngine;

namespace Ketchapp.MayoSDK.Analytics
{
    public class AnalyticsManager : MonoBehaviour
    {
        private static List<Level> _analyticsLevels = new List<Level>();

        public void Initialize()
        {
            if (KetchappInternal.CrossPromo.GetGdprValue())
            {
                KetchappInternal.Analytics.Initialize();
            }
        }

        /// <summary>
        /// Notify the analytics a level has stared.
        /// </summary>
        /// <param name="levelName">Name of the level.</param>
        [Obsolete("Use KetchappSDK.Analytics.GetLevel(levelNumber).ProgressionStarted() instead")]
        public void LevelStarted(string levelName)
        {
            KetchappInternal.Analytics.LevelStarted(levelName);
        }

        /// <summary>
        /// Notify the analytics a level has been successfuly passed.
        /// </summary>
        /// <param name="levelName">Name of the level.</param>
        /// <param name="score">Score of the level.</param>
        [Obsolete("Use KetchappSDK.Analytics.GetLevel(levelNumber).ProgressionCompleted() instead")]
        public void LevelPassed(string levelName, int score)
        {
            KetchappInternal.Analytics.LevelPassed(levelName, score);
        }

        /// <summary>
        /// Notify the analytisc a level has failed to pass.
        /// </summary>
        /// <param name="levelName">Name of the level.</param>
        /// <param name="score">Score of the level.</param>
        [Obsolete("Use KetchappSDK.Analytics.GetLevel(levelNumber).ProgressionFailed() instead")]
        public void LevelFailed(string levelName, int score)
        {
            KetchappInternal.Analytics.LevelFailed(levelName, score);
        }

        /// <summary>
        /// Send a custom design event to analytics without value parameter.
        /// </summary>
        /// <param name="eventName">Name of the event.</param>
        public void CustomEvent(string eventName)
        {
            KetchappInternal.Analytics.CustomEvent(eventName);
        }

        /// <summary>
        /// Notify a custom design event to analytics.
        /// </summary>
        /// <param name="eventName">Name of the event.</param>
        /// <param name="eventValue">Value of the event.</param>
        public void CustomEvent(string eventName, float eventValue)
        {
            KetchappInternal.Analytics.CustomEvent(eventName, eventValue);
        }

        /// <summary>
        /// Get the current level you are playing.
        /// </summary>
        /// <param name="levelNumber">Level Number.</param>
        /// <returns></returns>
        public ILevel GetLevel(int levelNumber)
        {
            if (!_analyticsLevels.Any(l => l.LevelNumber == levelNumber.ToString()))
            {
                var lvl = new Level(levelNumber.ToString(), KetchappInternal.Analytics);
                _analyticsLevels.Add(lvl);
                return lvl;
            }
            else
            {
                return _analyticsLevels.FirstOrDefault(l => l.LevelNumber == levelNumber.ToString());
            }
        }

        /// <summary>
        /// Get the current level you are playing.
        /// </summary>
        /// <param name="levelName">Level Name</param>
        /// <returns></returns>
        public ILevel GetLevel(string levelName)
        {
            if (!_analyticsLevels.Any(l => l.LevelNumber == levelName))
            {
                var lvl = new Level(levelName, KetchappInternal.Analytics);
                _analyticsLevels.Add(lvl);
                return lvl;
            }
            else
            {
                return _analyticsLevels.FirstOrDefault(l => l.LevelNumber == levelName);
            }
        }
    }
}
