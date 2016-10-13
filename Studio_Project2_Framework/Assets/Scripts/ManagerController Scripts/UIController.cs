using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public InputField inputField_Response;
    public InputField inputField_Incident;

    // Use this for initialization
    void Start ()
    {
        DeActivateInputFields();
    }
	
    public void ActivateResponseInputField()
    {
        //Focuses the inputfield allowing for direct input
        inputField_Response.gameObject.SetActive(true);
        inputField_Incident.gameObject.SetActive(false);
        inputField_Response.Select();
        inputField_Response.ActivateInputField();
    }

    public void ActivateIncidentInputField()
    {
        //Focuses the inputfield allowing for direct input
        inputField_Response.gameObject.SetActive(false);
        inputField_Incident.gameObject.SetActive(true);
        inputField_Incident.Select();
        inputField_Incident.ActivateInputField();
    }

    public void DeActivateInputFields()
    {
        if(inputField_Incident != null)
        inputField_Incident.gameObject.SetActive(false);

        if(inputField_Response != null)
        inputField_Response.gameObject.SetActive(false);
    }

    public void ClearResponseInputFields()
    {
        //Clears the inputfield
        inputField_Response.text = "";
        inputField_Response.Select();
        inputField_Response.ActivateInputField();
    }

    public void ClearIncidentInputFields()
    {
        //Clears the inputfield
        inputField_Incident.text = "";
        inputField_Incident.Select();
        inputField_Incident.ActivateInputField();
    }
}
