using UnityEngine;
using System.Collections;

public class Incident : MonoBehaviour
{
    //The Indetifier of the Incident. Used to match the incident from playing input.
    public string ID;
    //This is the name of the video that is accociated with the incident.
    public string VideoID;

    //If your incident cards have effects, put them here
    public void Activate()
    {
        //An example of something that an incident card could do when play, currently has no effect, obviously.
        GameManager.instance.PlayScore += 0;
    }
}
