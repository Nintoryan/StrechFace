using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIFunctions : MonoBehaviour
{
    [Header("Links")]
    [SerializeField] private Streching _streching;
    [SerializeField] private MeshFilter _meshFilter;
    [SerializeField] private ProgressChecker _progressChecker;
    [Header("Buttons")]
    [SerializeField] private Button _undoButton;
    [SerializeField] private Button _doneButton;
    private Stack<Vector3[]> _actionsStack = new Stack<Vector3[]>();
    
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

    public void OpenGallery()
    {
        
    }

    public void Done()
    {
        
    }
}
