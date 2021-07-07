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
    

    public void PlayHome()
    {
        GlobalData.LoadableLevel = GlobalData.ProgressLevel;
        SceneManager.LoadScene("Level");
    }
}
