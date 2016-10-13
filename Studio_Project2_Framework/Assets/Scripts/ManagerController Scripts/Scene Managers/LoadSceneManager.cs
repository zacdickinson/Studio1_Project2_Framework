using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LoadSceneManager : MonoBehaviour {

    // A timer used to track when to automatically progress to the next scene
    public float loadSceneTimer;
	
	// Update is called once per frame
	void Update ()
    {
        #region Scene Timer
        loadSceneTimer -= Time.deltaTime;
        if (loadSceneTimer <= 0)
        {
            GameManager.instance.ChangeScene(GameManager.instance.tag_IntroScene);
            loadSceneTimer = 0;
        }
        #endregion

        #region Load Scene space to Skip
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameManager.instance.ChangeScene(GameManager.instance.tag_IntroScene);
        }
        #endregion
    }
}
