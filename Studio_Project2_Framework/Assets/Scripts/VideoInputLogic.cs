using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VideoInputLogic
{
    public VideoClip DetermineVideo(string _playerInput, List<VideoClip> _videoClips)
    {
        Debug.Log("Single Input Logic");
        //Loops through the list of video cips to find a match using the videoName.
        for (int index = 0; index < _videoClips.Count; index++)
        {
            if (_videoClips[index].videoName == _playerInput)
            {
                return _videoClips[index];
            }
        }
        return null;
    }

    public VideoClip DetermineVideo(List<string> _playerInputs, List<VideoClip> _videoClips)
    {
        //Dev specific logic to determine what video is played, will eventually the call:

        #region String Concatonation Example 
        string combinedString = "";

        for (int index = 0; index < _playerInputs.Count; index++)
        {
            combinedString += _playerInputs[index];
        }
        for (int index = 0; index < _videoClips.Count; index++)
        {
            if (_videoClips[index].videoName == combinedString)
            {
                return _videoClips[index];
            }
        }
        return null;
        #endregion
    }
}
