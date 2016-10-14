// Decompiled with JetBrains decompiler
// Type: QuitVLCHelper
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C9321C04-A7AB-482D-8D92-C80C2E04350E
// Assembly location: C:\Users\PC\Downloads\VLC_for_Unity_Youtube\VLC_for_Unity_Youtube_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

public class QuitVLCHelper : MonoBehaviour
{
  private PlayVLC[] videos;

  private void Start()
  {
    this.videos = Object.FindObjectsOfType<PlayVLC>();
  }

  public void QuitAllVideos()
  {
    foreach (PlayVLC video in this.videos)
      video.StopVideo();
  }

  public void QuitApplication()
  {
    foreach (PlayVLC video in this.videos)
      video.StopVideo();
    Application.Quit();
  }

  private void OnApplicationQuit()
  {
    foreach (PlayVLC video in this.videos)
      video.StopVideo();
  }
}
