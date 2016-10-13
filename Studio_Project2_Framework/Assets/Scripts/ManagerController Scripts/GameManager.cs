using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public string tag_LoadScene;
    public string tag_IntroScene;
    public string tag_GameplayScene;
    public string tag_OutroScene;

    public int PlayScore;

    // Called before start
    void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    // Used to change scenes, can be called from anywhere
    public void ChangeScene(string _sceneName)
    {
        SceneManager.LoadScene(_sceneName);
    }
}
