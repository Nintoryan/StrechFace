using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIFunctions : MonoBehaviour
{
    [SerializeField] private Streching _streching;
    [SerializeField] private MeshFilter _meshFilter;
    [SerializeField] private Button _undoButton;
    private Stack<Vector3[]> _actionsStack = new Stack<Vector3[]>();
    
    private void Awake()
    {
        _streching.OnChangeVertices += Record;
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
}
