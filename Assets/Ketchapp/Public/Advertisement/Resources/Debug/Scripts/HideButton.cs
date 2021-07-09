using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Ketchapp.Internal.DebugAds
{
    public class HideButton : MonoBehaviour
    {
        public float HideTimer { get; set; }

        private float _initialTimer;
        private Image HideImage { get; set; }
        private Button _hideButton;

        public Action OnHideButtonClicked { get; set; }

        private void Awake()
        {
            _hideButton = GetComponent<Button>();

            HideImage = GetComponent<Image>();

            _initialTimer = 0;
            HideImage.fillAmount = _initialTimer / HideTimer;
            _hideButton.onClick.AddListener(HideButtonClicked);
        }

        private void Update()
        {
            _hideButton.interactable = _initialTimer >= HideTimer;

            if (_initialTimer < HideTimer)
            {
                _initialTimer += Time.deltaTime;
                HideImage.fillAmount = _initialTimer / HideTimer;
            }
        }

        private void HideButtonClicked()
        {
            OnHideButtonClicked?.Invoke();
        }
    }
}
