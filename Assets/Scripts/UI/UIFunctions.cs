using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;

public class UIFunctions : MonoBehaviour
{
    [Header("Links")]
    [SerializeField] private Streching _streching;
    [SerializeField] private EventTrigger _gameplayTouchPad;
    [SerializeField] private MeshRenderer _outLine;
    [SerializeField] private ProgressChecker _progressChecker;
    [SerializeField] private ResultImageSaver _resultImageSaver;
    [SerializeField] private RectTransform _faceSelectionBar;
    [SerializeField] private Canvas _faceSelectionCanvas;
    [SerializeField] private Canvas _gameplayCanvas;
    [Header("Buttons")]
    [SerializeField] private Button _undoButton;
    [SerializeField] private Button _doneButton;
    
    private Stack<Vector3[]> _actionsStack = new Stack<Vector3[]>();

    public event UnityAction<float> OnDone; 
    
    private void Awake()
    {
        _streching.OnChangeVertices += Record;
        _progressChecker.OnLowPass += ActivateDoneButton;
    }

    private void Record(Vector3[] step)
    {
        _actionsStack.Push(step);
        if (_actionsStack.Count > 1)
        {
            _undoButton.interactable = true;
        }
    }
    
    public void Undo()
    {
        _actionsStack.Pop();
        _streching.ApplyOldVertices(_actionsStack.Peek());
        _undoButton.interactable = _actionsStack.Count > 1;
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void ActivateDoneButton()
    {
        _doneButton.gameObject.SetActive(true);
    }

    public void HideSelectionBar()
    {
        var s = DOTween.Sequence();
        s.Append(_faceSelectionBar.DOAnchorPosY(-200f, 0.5f));
        s.AppendCallback(() =>
        {
            _gameplayCanvas.gameObject.SetActive(true);
            _faceSelectionCanvas.gameObject.SetActive(false);
        });
    }

    public void OpenGallery()
    {
        SceneManager.LoadScene("Gallery");
    }

    public void Done()
    {
        _outLine.gameObject.SetActive(false);
        _gameplayTouchPad.gameObject.SetActive(false);
        _resultImageSaver.SaveFaceSprite();
        OnDone?.Invoke(_progressChecker.Progress);
        
        if (GlobalData.LoadableLevel == GlobalData.ProgressLevel)
        {
            GlobalData.ProgressLevel++;
        }
        GlobalData.LoadableLevel++;
    }

    public void Continue()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
