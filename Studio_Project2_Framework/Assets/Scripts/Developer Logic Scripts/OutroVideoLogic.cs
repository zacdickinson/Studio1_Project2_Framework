using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OutroVideoLogic : MonoBehaviour {

    [SerializeField]
    VideoController videoController;
    
	// Use this for initialization
	void Start ()
    {
        videoController.PlayVideo(DetermineOutroVideo(videoController.videoClips));
        GameManager.instance.PlayScore = 0;
	}

    VideoClip DetermineOutroVideo(List<VideoClip> _videoClips)
    {
        //Checks if the players score is above zero and chooses one of the clips based on that.
        if(GameManager.instance.PlayScore > 0)
        {
            return _videoClips[0];
        }
        else
        {
            return _videoClips[1];
        }
    }
	
}
