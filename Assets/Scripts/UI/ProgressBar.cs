using MPUIKIT;
using UnityEngine;

public class ProgressBar : MonoBehaviour
{
    [SerializeField] private MPImage _bar;
    [SerializeField] private ProgressChecker _progressChecker;

    private void FixedUpdate()
    {
        _bar.fillAmount = _progressChecker.Progress;
    }
}
