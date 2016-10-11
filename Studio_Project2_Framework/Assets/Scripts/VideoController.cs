using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class VideoController : MonoBehaviour
{
    #region Public Variables

    #endregion

    #region Serialized Variables
    //This amount of inputs the player needs to enter to trigger a video
    [SerializeField]
    int numOfInputs;
    //The clips the video screen can play
    [SerializeField]
    List<VideoClip> videoClips;
    #endregion

    #region Private Variables
    private Material screenMat;
    private AudioSource audioSource;
    private int inputCounter;
    private List<string> inputStringList = new List<string>();
    private VideoInputLogic videoInputLogic = new VideoInputLogic();
    #endregion

    #region Singleton Pattern
    public VideoController instance;

    void Awake()
    {
        instance = this;
    }
    #endregion

    //Called at the start of the scene
    void Start()
    {
        //Resets the input counter
        inputCounter = numOfInputs;

        //Links the audio and video components of the Video Screen
        screenMat = GetComponent<Renderer>().material;
        audioSource = GetComponent<AudioSource>();
    }

    //Called when the player enters input
    public void ActivateVideo()
    {
        //If the inputs are set to 1 or less determine the video through single input logic
        if (numOfInputs <= 1)
        {
            Debug.Log(UIController.instance.inputField.text);
            PlayVideo(videoInputLogic.DetermineVideo(UIController.instance.inputField.text, videoClips));
        }
        else
        {
            //If this is the first input on the cycle, clear the current input list
            if(inputCounter == numOfInputs)
            {
                inputStringList.Clear();
            }

            //Add the current input field value to the list of inputs
            inputStringList.Add(UIController.instance.inputField.text);
            //Decrease the input counter
            inputCounter -= 1;

            //If the players have reached the correct amount of inputs
            if(inputCounter == 0)
            {
                //Determine the video to be played based on multi input logic
                PlayVideo(videoInputLogic.DetermineVideo(inputStringList, videoClips));
                //Reset the counter
                inputCounter = numOfInputs;
            }
        }

        //Clears the inputfield
        UIController.instance.inputField.text = "";
        UIController.instance.inputField.Select();
        UIController.instance.inputField.ActivateInputField();
    }

    //Plays the selected video, this should only be called by PlayClip()
    void PlayVideo(VideoClip videoClip)
    {
        //Stop the current video
        if(videoClip != null)
        {
            videoClip.movTexture.Stop();
        }

        //Sets up video
        screenMat.mainTexture = videoClip.movTexture;
        //Sets up Audio
        audioSource.clip = videoClip.movAudio;

        //Plays Video
        videoClip.movTexture.Play();
        //Plays Audio
        audioSource.Play();
    }
}
