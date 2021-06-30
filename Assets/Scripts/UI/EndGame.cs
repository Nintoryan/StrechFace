using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndGame : MonoBehaviour
{
    [SerializeField] private UIFunctions _uiFunctions;
    [SerializeField] private Canvas _gameplayCanvas;
    [SerializeField] private Canvas _endGameCanvas;
    [SerializeField] private MeshRenderer _streching;

    [Header("End Canvas parts")] 
    [SerializeField] private List<Image> _stars = new List<Image>();
    [SerializeField] private Image _photobackground;
    [SerializeField] private Image _resultImage;
    [SerializeField] private Image _backLights;
    [SerializeField] private List<Button> _buttons = new List<Button>();

    private void Awake()
    {
        _uiFunctions.OnDone += OpenEndGameCanvas;
    }

    private void OpenEndGameCanvas(float progress)
    {
        _streching.gameObject.SetActive(false);
        _gameplayCanvas.gameObject.SetActive(false);
        _endGameCanvas.gameObject.SetActive(true);
        if (progress < 0.68f)
        {
            _stars[0].gameObject.SetActive(true);
            
        }else if (progress < 0.86f)
        {
            _stars[0].gameObject.SetActive(true);
            _stars[1].gameObject.SetActive(true);
        }
        else
        {
            _stars[0].gameObject.SetActive(true);
            _stars[1].gameObject.SetActive(true);
            _stars[2].gameObject.SetActive(true);
        }
    }
}
