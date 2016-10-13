using UnityEngine;
using System.Collections;

public class Response : MonoBehaviour {

    //Used to match the playing input to the response card
    public string ID;
    //Used to match the response card to the video clip
    public string VideoID;
    //Scroe that is applied when this card is played.
    public int points;

    //If your response cards have effects, put them here
    public void Activate()
    {
        GameManager.instance.PlayScore += points;
    }
}
