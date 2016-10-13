using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameStateDisplay : MonoBehaviour
{
	// Update is called once per frame
	void Update ()
    {
        GetComponent<Text>().text = GamePlayManager.instance.currentGameState.ToString();
	}
}
