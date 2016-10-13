using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class VideoController : MonoBehaviour
{
    #region Public Variables
    public bool autoPlay;
    //The clips the video screen can play
    public List<VideoClip> videoClips;
    #endregion

    #region Serialized Variables

    #endregion

    #region Private Variables
    private Material screenMat;
    private AudioSource audioSource;
    #endregion

    void Awake()
    {
        //Links the audio and video components of the Video Screen
        screenMat = GetComponent<Renderer>().material;
        audioSource = GetComponent<AudioSource>();
    }

    //Called at the start of the scene
    void Start()
    {
        //If the auto play is checked, it will automatically pay the first video in the movie screens list.
        if(autoPlay)
        {
            PlayVideo(videoClips[0]);
        }
    }

    //Plays the selected video, this should only be called by PlayClip()
    public void PlayVideo(VideoClip videoClip)
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
