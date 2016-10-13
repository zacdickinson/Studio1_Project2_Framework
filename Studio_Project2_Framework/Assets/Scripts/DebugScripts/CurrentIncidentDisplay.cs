using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CurrentIncidentDisplay : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if(GamePlayManager.instance.currentIncident != null)
        {
            GetComponent<Text>().text = GamePlayManager.instance.currentIncident.ToString();
        }
        else
        {
            GetComponent<Text>().text = "No Incident";
        }
    }
}
