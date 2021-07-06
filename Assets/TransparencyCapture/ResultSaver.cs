using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(TransparencyImageCapture))]
public class ResultSaver:MonoBehaviour
{
    [SerializeField] private Image _resultPresenter;
    [SerializeField] private Camera main;
    [SerializeField] private Camera _resultSaverCamera;
    private Sprite _savedResult;
    private TransparencyImageCapture _imageCapture;

    public event UnityAction OnSaved;

    private void Awake()
    {
        _imageCapture = GetComponent<TransparencyImageCapture>();
    }

    private IEnumerator Save()
    {
        main.enabled = false;
        yield return new WaitForEndOfFrame();
        _savedResult = _imageCapture.captureScreenshot($"{GlobalData.LoadableLevel}.png",_resultSaverCamera);
        main.enabled = true;
        _resultPresenter.sprite = _savedResult;
        OnSaved?.Invoke();
    }

    public void SaveResult()
    {
        StartCoroutine(Save());
    }
}