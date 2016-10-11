using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

    public InputField inputField;

    #region Singleton Pattern
    public static UIController instance;

    void Awake()
    {
        instance = this;
    }
    #endregion

    // Use this for initialization
    void Start ()
    {
        //Focuses the inputfield allowing for direct input
        inputField.Select();
        inputField.ActivateInputField();
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
