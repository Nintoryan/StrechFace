using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class OkayWindow : MonoBehaviour
{
    [SerializeField] private RectTransform _window;
    [SerializeField] private Image Background;
    void Start()
    {
        Background.DOFade(0, 0f);
        _window.localScale = Vector3.zero;
    }

    public void Open()
    {
        Background.raycastTarget = true;
        Background.DOFade(0.8f, 0.5f);
        _window.DOScale(Vector3.one, 0.5f);
    }

    public void Close()
    {
        Background.raycastTarget = false;
        Background.DOFade(0, 0.5f);
        _window.DOScale(Vector3.zero, 0.5f);
    }
}
