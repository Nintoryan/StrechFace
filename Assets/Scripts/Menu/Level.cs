using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Level : MonoBehaviour
{
    [SerializeField] private int _number;
    public int Number => _number;

    private Image backGround;
    [SerializeField] private Image result;

    public Vector2 startPosition;
    private RectTransform _rectTransform;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        backGround = GetComponent<Image>();
        startPosition = _rectTransform.anchoredPosition;
        if (GlobalData.ProgressLevel > Number)
        {
            result.sprite = IMG2Sprite.LoadNewSprite($"{_number}.png");
        }
    }

    public void Hide()
    {
        OpenLevelScreen.Instance.Open(this);
        backGround.enabled = false;
        result.enabled = false;
        
    }

    public void Show()
    {
        backGround.enabled = true;
        result.enabled = true;
    }

    public void Load()
    {
        SceneManager.LoadScene(_number);
    }
}
