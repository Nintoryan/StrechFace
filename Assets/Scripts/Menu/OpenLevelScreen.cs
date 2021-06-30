using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class OpenLevelScreen : MonoBehaviour
{
    [SerializeField] private Image _background;
    [SerializeField] private Button[] _buttons;
    [SerializeField] private Transform ParentCanvas;

    public static OpenLevelScreen Instance;
    private Level _current;
    private GameObject _opendInstance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Close();
    }

    public void Open(Level _level)
    {
        _current = _level;
        
        foreach (var button in _buttons)
        {
            button.transform.DOScale(1, 0.5f);
        }
        _background.DOFade(0.35f, 0.5f);
        var s = DOTween.Sequence();
        s.AppendInterval(0.5f);
        s.AppendCallback(() =>
        {
            _background.raycastTarget = true;
        });
        
        
        _opendInstance = Instantiate(_level.gameObject, ParentCanvas, true);
        Destroy(_opendInstance.GetComponent<Button>());
        _opendInstance.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f,0.5f);
        _opendInstance.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f,0.5f);
        _opendInstance.transform.position = transform.position;
        _opendInstance.GetComponent<RectTransform>().DOAnchorPos(new Vector2(), 0.5f);
        _opendInstance.transform.DOScale(1.5f, 0.5f);
    }

    public void Close()
    {
        foreach (var button in _buttons)
        {
            button.transform.DOScale(0, 0.001f);
        }
        
        _background.DOFade(0, 0.001f);
        _background.raycastTarget = false;
        
        if(_opendInstance == null) return;
        _current.Show();
        Destroy(_opendInstance);
    }
}
