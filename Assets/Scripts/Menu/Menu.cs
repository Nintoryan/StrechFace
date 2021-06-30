using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    [SerializeField] private Level[] _levelPresenters;

    private void Awake()
    {
        if (!PlayerPrefs.HasKey("level0"))
        {
            PlayerPrefs.SetInt("level0",1);
        }
    }

    private void Start()
    {
        foreach (var level in _levelPresenters)
        {
            if (PlayerPrefs.GetInt($"level{level.Number}") == 0)
            {
                level.gameObject.SetActive(false);
            }
        }
    }

    public void PlayHome()
    {
        
        SceneManager.LoadScene("Level");
    }
}
