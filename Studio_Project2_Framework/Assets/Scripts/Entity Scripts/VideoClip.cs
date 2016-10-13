using UnityEngine;
using System.Collections;

public class VideoClip : MonoBehaviour
{
    //The unique identifier for the video clip
    public string videoName;
    //The Duration of the clip
    public float duration;
    //The video and audio parts of the full clip
    public MovieTexture movTexture;
    public AudioClip movAudio;
}
