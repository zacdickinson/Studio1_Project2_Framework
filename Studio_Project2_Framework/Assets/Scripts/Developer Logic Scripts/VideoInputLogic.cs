using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VideoInputLogic
{
    public VideoClip DetermineIncidentVideo(List<VideoClip> _videoClips)
    {
        //Grabs the 2 most common variables used for determining outcome videos.
        Response currentResponse = GamePlayManager.instance.currentResponse;
        Incident currentIncident = GamePlayManager.instance.currentIncident;

        //Loops through the list of video cips to find a match using the videoName.
        for (int index = 0; index < _videoClips.Count; index++)
        {
            if (_videoClips[index].videoName == currentIncident.VideoID)
            {
                return _videoClips[index];
            }
        }
        return null;
    }

    public VideoClip DetermineResponseVideo(List<VideoClip> _videoClips)
    {
        //Grabs the 2 most common variables used for determining outcome videos.
        Response currentResponse = GamePlayManager.instance.currentResponse;
        Incident currentIncident = GamePlayManager.instance.currentIncident;

        //This is where you may compare the current incident card against the current response card and determine a different video.

        //Loops through the list of video cips to find a match using the videoName.
        for (int index = 0; index < _videoClips.Count; index++)
        {
            if (_videoClips[index].videoName == currentResponse.VideoID)
            {
                return _videoClips[index];
            }
        }
        return null;
    }
}
