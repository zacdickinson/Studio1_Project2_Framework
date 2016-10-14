// Decompiled with JetBrains decompiler
// Type: PlayVLC
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C9321C04-A7AB-482D-8D92-C80C2E04350E
// Assembly location: C:\Users\PC\Downloads\VLC_for_Unity_Youtube\VLC_for_Unity_Youtube_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayVLC : MonoBehaviour
{
  [Tooltip("Want to use built-in VLC from StreamingAssets? This way your player must not have VLC player installed.")]
  [Header("Use built-in VLC Player in StreamingAssets")]
  public bool UseBuiltInVLC = true;
  [Header("Use installed VLC Player")]
  [Tooltip("If you don't want to bundle VLC with your app, but use the installed VLC Player on your users PC. Recommended VLC version is 2.0.8. Smaller Build: Delete vlc from StreamingAssets in Build!")]
  public string InstallPath = "C:\\Program Files\\VideoLAN\\VLC\\vlc.exe";
  [Tooltip("Want to play from StreamingAssets? Use this option if you package the video with the game.")]
  [Header("Play from Streaming Assets")]
  public bool PlayFromStreamingAssets = true;
  [Tooltip("Path or name with extension relative from StreamingAssets folder, eg. test.mp4")]
  public string StreamingAssetsVideoFilename = string.Empty;
  [Tooltip("Where is the video you want to play? Nearly all video formats are supported by VLC")]
  [Header("Alternative: External video Path")]
  public string VideoPath = string.Empty;
  [Tooltip("Skip Video with any key. Forces Unity to remain the focused window.")]
  public bool SkipVideoWithAnyKey = true;
  [Tooltip("In case video start flickers slightly, use this.")]
  [Header("Additional Settings")]
  public bool FlickerFix = true;
  [Tooltip("Obsolete if you check PinVideo. Otherwise, if you use a higher version than 2.0.8, you have to check this box - Otherwise there might be a problem introduced by a unfixed bug in VLC releases since 2.1.0.")]
  [Header("New features in 1.03 (Using VLC 2.2.1: Youtube streaming)")]
  public bool UseVlc210OrHigher = true;
  [Tooltip("Pin the video to the UI panel or Unity window. You can then scale or move the UI elements dynamically, and the video will do the same and handle aspect automatically.")]
  public bool PinVideo = true;
  private bool _unityIsFocused = true;
  public Text Debtext;
  [Header(" Display Modes - Direct3D recommended")]
  public PlayVLC.RenderMode UsedRenderMode;
  [Tooltip("Use as Intro?")]
  [Header("Playback Options")]
  public bool PlayOnStart;
  [Tooltip("Video will loop, make sure to enable skipping or call Kill.")]
  public bool LoopVideo;
  [Tooltip("Call Play, Pause, Stop etc. fuctions from code or gui buttons. Only possible for 1 video at a time.")]
  public bool EnableVlcControls;
  [Tooltip("If enabled, video will be fullscreen even if Unity is windowed. If disabled, video will be shown over the whole unity window when playing it fullscreen.")]
  public bool CompleteFullscreen;
  [Tooltip("Render \"windowed\" video on GUI RectTransform?.")]
  [Header("Windowed playback")]
  public bool UseGUIVideoPanelPosition;
  public RectTransform GuiVideoPanel;
  [Header("Skip Video Hint")]
  [Tooltip("Show a skip hint under the video.")]
  public bool ShowBottomSkipHint;
  public GameObject BottomSkipHint;
  private bool _focusInUpdate;
  [Tooltip("If enabled, fullsceen video will be played under the rendered unity window. 3D Objects and UI will remain visible. Uses keying, modify VideoInBackgroundCameraPrefab prefab for a different color key, if there are any problems.")]
  public bool VideoInBackground;
  [Tooltip("Drag the Camera Prefab that comes with this package here, or create your own keying Camera.")]
  public GameObject VideoInBackgroundCameraPrefab;
  private int _playlistCurrentID;
  private Process _vlc;
  private IntPtr _unityHwnd;
  private IntPtr _vlcHwnd;
  private PlayVLC.RECT _unityWindowRect;
  private PlayVLC.RECT _vlcWindowRect;
  private uint _unityWindowID;
  private float _mainMonitorWidth;
  private Vector2 _realCurrentMonitorDeskopResolution;
  private Rect _realCurrentMonitorBounds;
  private bool _pinToGuiRectDistanceTaken;
  private int _pinToGuiRectLeftOffset;
  private int _pinToGuiRectTopOffset;
  private bool _isPlaying;
  private bool _thisVlcProcessWasEnded;
  private bool _qtCheckEnabled;
  private Camera[] allCameras;
  private GameObject VideoInBackgroundCamera;
  private float _nXpos;
  private float _nYpos;
  private float _nWidth;
  private float _nHeight;
  private Rect _oldPrect;
  private float _prev_nXpos;
  private float _prev_nYpos;
  private float _prev_nWidth;
  private float _prev_nHeight;
  private float highdpiscale;
  private PlayVLC[] videos;
  private float bottomSkipHintSize;

  public bool IsPlaying
  {
    get
    {
      return this._isPlaying;
    }
    set
    {
      this._isPlaying = value;
    }
  }

  [DllImport("user32.dll")]
  public static extern IntPtr FindWindow(string className, string windowName);

  [DllImport("user32.dll")]
  internal static extern IntPtr SetForegroundWindow(IntPtr hWnd);

  [DllImport("user32.dll")]
  internal static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

  [DllImport("user32.dll")]
  private static extern uint GetActiveWindow();

  [DllImport("user32.dll")]
  private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);

  [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
  internal static extern void MoveWindow(IntPtr hwnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

  [DllImport("User32.dll")]
  private static extern IntPtr MonitorFromPoint([In] Point pt, [In] uint dwFlags);

  [DllImport("Shcore.dll")]
  private static extern IntPtr GetDpiForMonitor([In] IntPtr hmonitor, [In] PlayVLC.DpiType dpiType, out uint dpiX, out uint dpiY);

  [DllImport("user32.dll", SetLastError = true)]
  [return: MarshalAs(UnmanagedType.Bool)]
  private static extern bool GetWindowRect(IntPtr hWnd, ref PlayVLC.RECT lpRect);

  [DllImport("gdi32.dll")]
  private static extern int GetDeviceCaps(IntPtr hdc, int nIndex);

  [DllImport("user32.dll")]
  private static extern IntPtr GetDC(IntPtr hWnd);

  private float GetMainSceenUserScalingFactor()
  {
    IntPtr hdc = System.Drawing.Graphics.FromHwnd(IntPtr.Zero).GetHdc();
    int deviceCaps = PlayVLC.GetDeviceCaps(hdc, 10);
    return (float) PlayVLC.GetDeviceCaps(hdc, 117) / (float) deviceCaps;
  }

  private bool UnityIsOnPrimaryScreen()
  {
    this._unityWindowRect = new PlayVLC.RECT();
    PlayVLC.GetWindowRect(this._unityHwnd, ref this._unityWindowRect);
    return this._unityWindowRect.Left + 10 >= 0 && (double) (this._unityWindowRect.Left + 10) <= (double) this._mainMonitorWidth;
  }

  private void UpdateUnityWindowRect()
  {
    this._unityWindowRect = new PlayVLC.RECT();
    PlayVLC.GetWindowRect(this._unityHwnd, ref this._unityWindowRect);
  }

  private Vector2 GetCurrentMonitorDesktopResolution()
  {
    Form form = new Form();
    form.BackColor = System.Drawing.Color.Black;
    form.ForeColor = System.Drawing.Color.Black;
    form.ShowInTaskbar = false;
    form.Opacity = 0.0;
    form.Show();
    form.StartPosition = FormStartPosition.Manual;
    this.UpdateUnityWindowRect();
    form.Location = new Point(this._unityWindowRect.Left, this._unityWindowRect.Top);
    form.WindowState = FormWindowState.Maximized;
    float userScalingFactor = this.GetMainSceenUserScalingFactor();
    Vector2 vector2;
    if (this.UnityIsOnPrimaryScreen() && (double) this.GetMainSceenUserScalingFactor() != 1.0)
    {
      vector2 = new Vector2((float) (form.DesktopBounds.Width - 16) * userScalingFactor, (float) (form.DesktopBounds.Height + 40 - 16) * userScalingFactor);
      this._realCurrentMonitorBounds = new Rect((float) (form.DesktopBounds.Left + 8) * userScalingFactor, (float) form.DesktopBounds.Top * userScalingFactor, (float) (form.DesktopBounds.Width - 16) * userScalingFactor, (float) (form.DesktopBounds.Height + 40) * userScalingFactor);
    }
    else
    {
      vector2 = new Vector2((float) (form.DesktopBounds.Width - 16), (float) (form.DesktopBounds.Height + 40 - 16));
      this._realCurrentMonitorBounds = new Rect((float) (form.DesktopBounds.Left + 8), (float) form.DesktopBounds.Top, (float) (form.DesktopBounds.Width - 16), (float) (form.DesktopBounds.Height + 40));
    }
    form.Close();
    this._realCurrentMonitorDeskopResolution = vector2;
    return vector2;
  }

  private void CheckErrors()
  {
    if (this.VideoPath.Length > 5 && this.VideoPath.StartsWith("https://www.youtube.com/watch?"))
      UnityEngine.Debug.LogWarning((object) "You are streaming from youtube, make sure you've got a internet connection. Seeking might be less performant, depending on your internet speed.");
    if (this.StreamingAssetsVideoFilename.Length < 5 && this.PlayFromStreamingAssets)
      UnityEngine.Debug.LogError((object) "Please enter a valid video file name!");
    if (this.VideoPath.Length < 5 && !this.PlayFromStreamingAssets)
      UnityEngine.Debug.LogError((object) "Please enter a valid video file name!");
    if (!this.VideoInBackground && this.LoopVideo && (this.CompleteFullscreen || !this.UseGUIVideoPanelPosition) && (!this.SkipVideoWithAnyKey && !this.ShowBottomSkipHint) || this.UsedRenderMode == PlayVLC.RenderMode.FullScreenOverlayModePrimaryDisplay && !this.SkipVideoWithAnyKey)
      UnityEngine.Debug.LogWarning((object) "You are possibly playing a looping fullscreen video you can't skip! Consider using skipping features, or your players won't be able to get past this video.");
    if (this.UseGUIVideoPanelPosition && !(bool) ((UnityEngine.Object) this.GuiVideoPanel))
      UnityEngine.Debug.LogError((object) "If you want to play on a Gui Panel, get the one from the prefabs folder and assign it to this script.");
    if (this.ShowBottomSkipHint && !(bool) ((UnityEngine.Object) this.BottomSkipHint))
      UnityEngine.Debug.LogError((object) "If you want to show the prefab skip hint, place the prefab in your GUI and assign it to this script.");
    if (this.UsedRenderMode != PlayVLC.RenderMode.Direct3DMode)
      UnityEngine.Debug.LogWarning((object) "Please consider using Direct3D Mode. Other modes are experimental or less performant and will be updated with V 1.1 or later.");
    if (this.UseBuiltInVLC)
      return;
    UnityEngine.Debug.LogWarning((object) "Consider using built-in VLC, unless you know you'll have it installed on your target machine.");
  }

  private void Awake()
  {
    this.CheckErrors();
    this._mainMonitorWidth = (float) SystemInformation.PrimaryMonitorMaximizedWindowSize.Width * this.GetMainSceenUserScalingFactor();
    this._unityHwnd = (IntPtr) ((long) PlayVLC.GetActiveWindow());
    this._unityWindowRect = new PlayVLC.RECT();
    PlayVLC.GetWindowRect(this._unityHwnd, ref this._unityWindowRect);
    this._realCurrentMonitorDeskopResolution = this.GetCurrentMonitorDesktopResolution();
    this._unityWindowID = PlayVLC.GetActiveWindow();
  }

  private void Start()
  {
    this.videos = UnityEngine.Object.FindObjectsOfType<PlayVLC>();
    if (!this.PlayOnStart)
      return;
    this.Play();
  }

  public static Rect RectTransformToScreenSpace(RectTransform transform)
  {
    Vector2 vector2 = Vector2.Scale(transform.rect.size, (Vector2) transform.lossyScale);
    return new Rect(transform.position.x, (float) UnityEngine.Screen.height - transform.position.y, vector2.x, vector2.y);
  }

  private uint GetCurrentMonitorDPI()
  {
    uint dpiX;
    uint dpiY;
    PlayVLC.GetDpiForMonitor(PlayVLC.MonitorFromPoint(new Point(this._unityWindowRect.Left + 15, this._unityWindowRect.Top), 2U), PlayVLC.DpiType.Raw, out dpiX, out dpiY);
    return dpiX;
  }

  public void QuitAllVideos()
  {
    foreach (PlayVLC video in this.videos)
    {
      if (video.IsPlaying)
        video.StopVideo();
    }
  }

  private string GetShortCutCodes()
  {
    string str = string.Empty;
    if (this.EnableVlcControls)
      str = str + " --global-key-play-pause \"p\" " + " --global-key-jump+short \"4\" " + " --global-key-jump+medium \"5\" " + " --global-key-jump+long \"6\" " + " --global-key-jump-long \"1\" " + " --global-key-jump-medium \"2\" " + " --global-key-jump-short \"3\" " + " --global-key-vol-down \"7\" " + " --global-key-vol-up \"8\" " + " --global-key-vol-mute \"9\" ";
    return str;
  }

  public void VolumeUp()
  {
    if (!this._isPlaying || !this.EnableVlcControls)
      return;
    PlayVLC.keybd_event((byte) 56, (byte) 137, 1U, 0);
    PlayVLC.keybd_event((byte) 56, (byte) 137, 3U, 0);
  }

  public void VolumeDown()
  {
    if (!this._isPlaying || !this.EnableVlcControls)
      return;
    PlayVLC.keybd_event((byte) 55, (byte) 136, 1U, 0);
    PlayVLC.keybd_event((byte) 55, (byte) 136, 3U, 0);
  }

  public void ToggleMute()
  {
    if (!this._isPlaying || !this.EnableVlcControls)
      return;
    PlayVLC.keybd_event((byte) 57, (byte) 138, 1U, 0);
    PlayVLC.keybd_event((byte) 57, (byte) 138, 3U, 0);
  }

  public void Pause()
  {
    if (!this._isPlaying || !this.EnableVlcControls)
      return;
    PlayVLC.keybd_event((byte) 80, (byte) 153, 1U, 0);
    PlayVLC.keybd_event((byte) 80, (byte) 153, 3U, 0);
  }

  public void SeekForwardShort()
  {
    if (!this._isPlaying || !this.EnableVlcControls)
      return;
    PlayVLC.keybd_event((byte) 52, (byte) 133, 1U, 0);
    PlayVLC.keybd_event((byte) 52, (byte) 133, 3U, 0);
  }

  public void SeekForwardMedium()
  {
    if (!this._isPlaying || !this.EnableVlcControls)
      return;
    PlayVLC.keybd_event((byte) 53, (byte) 134, 1U, 0);
    PlayVLC.keybd_event((byte) 53, (byte) 134, 3U, 0);
  }

  public void SeekForwardLong()
  {
    if (!this._isPlaying || !this.EnableVlcControls)
      return;
    PlayVLC.keybd_event((byte) 54, (byte) 135, 1U, 0);
    PlayVLC.keybd_event((byte) 54, (byte) 135, 3U, 0);
  }

  public void SeekBackwardShort()
  {
    if (!this._isPlaying || !this.EnableVlcControls)
      return;
    PlayVLC.keybd_event((byte) 51, (byte) 132, 1U, 0);
    PlayVLC.keybd_event((byte) 51, (byte) 132, 3U, 0);
  }

  public void SeekBackwardMedium()
  {
    if (!this._isPlaying || !this.EnableVlcControls)
      return;
    PlayVLC.keybd_event((byte) 50, (byte) 131, 1U, 0);
    PlayVLC.keybd_event((byte) 50, (byte) 131, 3U, 0);
  }

  public void SeekBackwardLong()
  {
    if (!this._isPlaying || !this.EnableVlcControls)
      return;
    PlayVLC.keybd_event((byte) 49, (byte) 130, 1U, 0);
    PlayVLC.keybd_event((byte) 49, (byte) 130, 3U, 0);
  }

  public void OpenFile(string path)
  {
  }

  public void FullscreenToggle()
  {
    PlayVLC.MoveWindow(this._vlcHwnd, (int) this._realCurrentMonitorBounds.left, (int) this._realCurrentMonitorBounds.top, (int) this._realCurrentMonitorBounds.width, (int) this._realCurrentMonitorBounds.height, true);
  }

  private bool CheckQTAllowed()
  {
    if (this.UsedRenderMode == PlayVLC.RenderMode.VLC_QT_InterfaceFullscreen)
      return UnityEngine.Screen.fullScreen;
    return true;
  }

  private float GetHighDPIScale()
  {
    float num = 1f;
    if (this.UnityIsOnPrimaryScreen() && (double) this.GetMainSceenUserScalingFactor() > 1.0)
      num = this.GetMainSceenUserScalingFactor();
    return num;
  }

  private Rect GetPanelRect()
  {
    float highDpiScale = this.GetHighDPIScale();
    Rect screenSpace = PlayVLC.RectTransformToScreenSpace(this.GuiVideoPanel);
    float num1 = 0.0f;
    float num2 = 0.0f;
    if (!UnityEngine.Screen.fullScreen)
    {
      num1 = 3f;
      num2 = 20f;
    }
    float num3 = 1f;
    float num4 = 1f;
    float num5 = 0.0f;
    if (UnityEngine.Screen.fullScreen)
    {
      num3 = this._realCurrentMonitorDeskopResolution.x / (float) UnityEngine.Screen.currentResolution.width;
      num4 = this._realCurrentMonitorDeskopResolution.y / (float) UnityEngine.Screen.currentResolution.height;
      num5 = (float) (((double) this._realCurrentMonitorDeskopResolution.x - (double) UnityEngine.Screen.currentResolution.width / (double) UnityEngine.Screen.currentResolution.height / (double) (this._realCurrentMonitorDeskopResolution.x / this._realCurrentMonitorDeskopResolution.y) * (double) this._realCurrentMonitorDeskopResolution.x) / 2.0);
    }
    float num6 = (this._realCurrentMonitorDeskopResolution.x - num5 * 2f) / this._realCurrentMonitorDeskopResolution.x;
    float num7 = screenSpace.left * num3 * num6 + (float) this._unityWindowRect.Left + num1;
    float num8 = screenSpace.top * num4 + (float) this._unityWindowRect.Top + num2;
    this._nXpos = num5 + num7 * highDpiScale;
    this._nYpos = num8 * highDpiScale;
    this._nWidth = screenSpace.width * num3 * num6 * highDpiScale;
    this._nHeight = screenSpace.height * highDpiScale * num4;
    return new Rect(this._nXpos, this._nYpos, this._nWidth, this._nHeight);
  }

  private Rect GetFullscreenRect()
  {
    return new Rect();
  }

  private Rect GetCompleteFullscreenRect()
  {
    return new Rect();
  }

  public void Play()
  {
    if (this._isPlaying || !this.CheckQTAllowed())
      return;
    this.QuitAllVideos();
    this._realCurrentMonitorDeskopResolution = this.GetCurrentMonitorDesktopResolution();
    this._isPlaying = true;
    this._thisVlcProcessWasEnded = false;

    string str1 = string.Empty;
    if (this.PlayFromStreamingAssets)
    {
      if (this.StreamingAssetsVideoFilename.Length > 0)
        str1 = "\"" + UnityEngine.Application.dataPath.Replace("/", "\\") + "\\StreamingAssets\\" + this.StreamingAssetsVideoFilename + "\"";
      else
        UnityEngine.Debug.LogError((object) "ERROR: No StreamingAssets video path set.");
    }
    else if (this.VideoPath.Length > 0)
      str1 = this.VideoPath;
    else
      UnityEngine.Debug.LogError((object) "ERROR: No video path set.");
    string str2 = str1 + " --ignore-config --no-crashdump " + this.GetShortCutCodes();
    if (this.UsedRenderMode == PlayVLC.RenderMode.Direct3DMode)
    {
      this._unityWindowRect = new PlayVLC.RECT();
      PlayVLC.GetWindowRect(this._unityHwnd, ref this._unityWindowRect);
      int num1 = Mathf.Abs(this._unityWindowRect.Right - this._unityWindowRect.Left);
      int num2 = Mathf.Abs(this._unityWindowRect.Bottom - this._unityWindowRect.Top);
      this.highdpiscale = this.GetHighDPIScale();
      string str3 = str2 + " -I=dummy --no-mouse-events --no-interact --no-video-deco ";
      if (!this.VideoInBackground)
        str3 += " --video-on-top ";
      if (this.UseGUIVideoPanelPosition && (bool) ((UnityEngine.Object) this.GuiVideoPanel))
      {
        Rect panelRect = this.GetPanelRect();
        if (!this.UseVlc210OrHigher)
          str2 = str3 + " --video-x=" + (object) panelRect.left + " --video-y=" + (object) panelRect.top + " --width=" + (object) (float) ((double) panelRect.xMax - (double) panelRect.xMin) + " --height=" + (object) (float) ((double) panelRect.yMax - (double) panelRect.yMin) + " ";
        else
          str2 = str3 + " --video-x=" + (object) 6000 + " --video-y=" + (object) 6000;
      }
      else
      {
        float num3 = 0.0f;
        if (this.ShowBottomSkipHint)
        {
          this.BottomSkipHint.SetActive(true);
          this.BottomSkipHint.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(new UnityAction(this.KillVLCProcess));
          num3 = !this.UnityIsOnPrimaryScreen() || (double) this.GetMainSceenUserScalingFactor() <= 1.0 ? PlayVLC.RectTransformToScreenSpace(this.BottomSkipHint.GetComponent<RectTransform>()).height : PlayVLC.RectTransformToScreenSpace(this.BottomSkipHint.GetComponent<RectTransform>()).height * this.GetMainSceenUserScalingFactor();
        }
        if (this._unityWindowRect.Top == 0)
        {
          this._unityWindowRect.Top = -1;
          num2 += 2;
        }
        if (this._unityWindowRect.Left == 0)
        {
          this._unityWindowRect.Left = -1;
          num1 += 2;
        }
        if (this.CompleteFullscreen)
        {
          this.GetCurrentMonitorDesktopResolution();
          str2 = str3 + " --video-x=" + (object) this._realCurrentMonitorBounds.left + " --video-y=" + (object) this._realCurrentMonitorBounds.top + " --width=" + (object) this._realCurrentMonitorBounds.width + " --height=" + (object) (float) ((double) this._realCurrentMonitorBounds.height + 4.0) + " ";
        }
        else if (UnityEngine.Screen.fullScreen)
        {
          str2 = str3 + " --video-x=" + (object) (float) ((double) this._unityWindowRect.Left * (double) this.highdpiscale) + " --video-y=" + (object) (float) ((double) (this._unityWindowRect.Top - 1) * (double) this.highdpiscale) + " --width=" + (object) (float) ((double) num1 * (double) this.highdpiscale) + " --height=" + (object) (float) (((double) num2 - (double) num3) * (double) this.highdpiscale) + " ";
        }
        else
        {
          float num4 = 3f;
          str2 = str3 + " --video-x=" + (object) (float) (((double) this._unityWindowRect.Left + (double) num4) * (double) this.highdpiscale) + " --video-y=" + (object) (float) ((double) (this._unityWindowRect.Top - 1) * (double) this.highdpiscale) + " --width=" + (object) (float) (((double) num1 - (double) num4 * 2.0) * (double) this.highdpiscale) + " --height=" + (object) (float) (((double) num2 - (double) num3) * (double) this.highdpiscale) + " ";
        }
      }
    }
    string str4;
    if (this.UsedRenderMode == PlayVLC.RenderMode.FullScreenOverlayModePrimaryDisplay || this.UsedRenderMode == PlayVLC.RenderMode.VLC_QT_InterfaceFullscreen)
    {
      string str3 = str2 + "--fullscreen ";
      if (this.UsedRenderMode == PlayVLC.RenderMode.FullScreenOverlayModePrimaryDisplay)
      {
        str4 = str3 + " -I=dummy ";
      }
      else
      {
        str4 = str3 + " --no-qt-privacy-ask --no-interact ";
        int @int = PlayerPrefs.GetInt("UnitySelectMonitor");
        if (@int == 1 && this.UnityIsOnPrimaryScreen())
          str4 += " --qt-fullscreen-screennumber=0";
        if (@int == 0 && !this.UnityIsOnPrimaryScreen())
          str4 += " --qt-fullscreen-screennumber=1";
        if (@int == 1 && !this.UnityIsOnPrimaryScreen())
          str4 += " --qt-fullscreen-screennumber=1";
        if (@int == 0 && this.UnityIsOnPrimaryScreen())
          str4 += " --qt-fullscreen-screennumber=0";
      }
    }
    else
      str4 = str2 + " --no-qt-privacy-ask --qt-minimal-view ";
    string str5 = str4 + " --play-and-exit --no-keyboard-events --video-title-timeout=0 --no-interact   ";
    string str6 = this.LoopVideo || this.VideoInBackground ? str5 + "  --loop --repeat" : str5 + " --no-repeat --no-loop";
    this._vlc = new Process();
    this._vlc.StartInfo.FileName = !this.UseBuiltInVLC ? "C:\\Program Files\\VideoLAN\\VLC\\vlc.exe" : UnityEngine.Application.dataPath + "/StreamingAssets/vlc/vlc.exe";
    this._vlc.StartInfo.Arguments = str6;
    this._vlc.StartInfo.CreateNoWindow = true;
    this._vlc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
    this._vlc.Start();
    if (this.UsedRenderMode != PlayVLC.RenderMode.VLC_QT_InterfaceFullscreen)
    {
      if (this.FlickerFix)
        this._focusInUpdate = true;
      else
        this.InvokeRepeating("FocusUnity", 0.025f, 0.05f);
    }
    if (!this.VideoInBackground)
      return;
    this.StartCoroutine("HandleBackgroundVideo");
  }

  [DebuggerHidden]
  private IEnumerator HandleBackgroundVideo()
  {
    /*// ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new PlayVLC.\u003CHandleBackgroundVideo\u003Ec__Iterator0()
    {
      \u003C\u003Ef__this = this
    };*/
	 	/*
		yield return new WaitForSeconds (3f);

		allCameras = FindObjectsOfType (Camera);
		foreach (Camera cam in allCameras) {
			cam.gameObject.SetActive (false);
		}*/

		yield return null;
  }

  private void ResetBackgroundVideo()
  {
    foreach (Camera allCamera in this.allCameras)
    {
      if ((UnityEngine.Object) allCamera != (UnityEngine.Object) this.VideoInBackgroundCamera)
        allCamera.gameObject.SetActive(true);
    }
    if (!((UnityEngine.Object) this.VideoInBackgroundCamera != (UnityEngine.Object) null))
      return;
    this.VideoInBackgroundCamera.GetComponent<BackgroundKey>().DisableTransparency();
    UnityEngine.Object.Destroy((UnityEngine.Object) this.VideoInBackgroundCamera);
  }

  private void FocusUnity()
  {
    this.GetFocus();
  }

  public void StopVideo()
  {
    if (this.VideoInBackground)
      this.ResetBackgroundVideo();
    if (!this._isPlaying)
      return;
    this.KillVLCProcess();
  }

  private void KillVLCProcess()
  {
    try
    {
      this._vlc.Kill();
    }
    catch (Exception ex)
    {
    }
  }

  private bool VLCWindowIsRendered()
  {
    PlayVLC.GetWindowRect(this._vlcHwnd, ref this._vlcWindowRect);
    return this._vlcWindowRect.Top - this._vlcWindowRect.Bottom != 0;
  }

  private void Pin()
  {
    if (!this._isPlaying || !this.PinVideo && !this.UseVlc210OrHigher)
      return;
    if (this._vlcHwnd == IntPtr.Zero)
      this._vlcHwnd = PlayVLC.FindWindow((string) null, "VLC (Direct3D output)");
    PlayVLC.GetWindowRect(this._vlcHwnd, ref this._vlcWindowRect);
    if (!this.VLCWindowIsRendered())
      return;
    if (this.UseGUIVideoPanelPosition && (bool) ((UnityEngine.Object) this.GuiVideoPanel))
    {
      Rect panelRect = this.GetPanelRect();
      if (Math.Abs(this._vlcWindowRect.Top - (int) panelRect.top) <= 3 && Math.Abs(this._vlcWindowRect.Bottom - (int) panelRect.bottom) <= 3 && (Math.Abs(this._vlcWindowRect.Left - (int) panelRect.left) <= 3 && Math.Abs(this._vlcWindowRect.Right - (int) panelRect.right) <= 3))
        return;
      PlayVLC.MoveWindow(this._vlcHwnd, (int) panelRect.xMin, (int) panelRect.yMin, (int) ((double) panelRect.xMax - (double) panelRect.xMin), (int) ((double) panelRect.yMax - (double) panelRect.yMin), true);
    }
    else if (this.CompleteFullscreen)
    {
      if (Math.Abs(this._vlcWindowRect.Top - (int) this._realCurrentMonitorBounds.top) <= 3 && Math.Abs(this._vlcWindowRect.Bottom - (int) this._realCurrentMonitorBounds.bottom) <= 3 && (Math.Abs(this._vlcWindowRect.Left - (int) this._realCurrentMonitorBounds.left) <= 3 && Math.Abs(this._vlcWindowRect.Right - (int) this._realCurrentMonitorBounds.right) <= 3))
        return;
      PlayVLC.MoveWindow(this._vlcHwnd, (int) this._realCurrentMonitorBounds.left, (int) this._realCurrentMonitorBounds.top, (int) this._realCurrentMonitorBounds.width, (int) this._realCurrentMonitorBounds.height, true);
    }
    else
    {
      this.bottomSkipHintSize = !this.ShowBottomSkipHint ? 0.0f : (!this.UnityIsOnPrimaryScreen() ? PlayVLC.RectTransformToScreenSpace(this.BottomSkipHint.GetComponent<RectTransform>()).height : PlayVLC.RectTransformToScreenSpace(this.BottomSkipHint.GetComponent<RectTransform>()).height * this.GetMainSceenUserScalingFactor());
      if (Math.Abs(this._vlcWindowRect.Top - this._unityWindowRect.Top) <= 3 && Math.Abs(this._vlcWindowRect.Bottom - (this._unityWindowRect.Bottom - (int) this.bottomSkipHintSize)) <= 3 && (Math.Abs(this._vlcWindowRect.Left - this._unityWindowRect.Left) <= 3 && Math.Abs(this._vlcWindowRect.Right - this._unityWindowRect.Right) <= 3))
        return;
      PlayVLC.MoveWindow(this._vlcHwnd, this._unityWindowRect.Left, this._unityWindowRect.Top, this._unityWindowRect.Right - this._unityWindowRect.Left, this._unityWindowRect.Bottom - this._unityWindowRect.Top - (int) this.bottomSkipHintSize, true);
    }
  }

  private void LateUpdate()
  {
    this.Pin();
  }

  private void Update()
  {
    if (!this._isPlaying)
      return;
    if (this._focusInUpdate)
    {
      if (this.FlickerFix)
      {
        if (this.UsedRenderMode == PlayVLC.RenderMode.Direct3DMode)
          this.FocusUnity();
      }
    }
    try
    {
      if (this._vlc.HasExited)
      {
        if (!this._thisVlcProcessWasEnded)
        {
          PlayVLC.ShowWindow(this._unityHwnd, 1);
          this._thisVlcProcessWasEnded = true;
          this._pinToGuiRectDistanceTaken = false;
          this.CancelInvoke("FocusUnity");
          this._isPlaying = false;
          this._qtCheckEnabled = false;
          this._focusInUpdate = false;
          this._vlcHwnd = IntPtr.Zero;
          this._oldPrect = new Rect(1f, 1f, 1f, 1f);
          if ((bool) ((UnityEngine.Object) this.BottomSkipHint))
          {
            this.BottomSkipHint.GetComponent<UnityEngine.UI.Button>().onClick.RemoveAllListeners();
            this.BottomSkipHint.SetActive(false);
          }
          if ((UnityEngine.Object) this.GuiVideoPanel != (UnityEngine.Object) null)
            this.GuiVideoPanel.GetComponent<UnityEngine.UI.Image>().enabled = true;
        }
      }
    }
    catch (Exception ex)
    {
    }
    if (!this.SkipVideoWithAnyKey || !this._isPlaying || (Input.GetKeyDown(KeyCode.LeftAlt) || Input.GetKeyDown(KeyCode.RightAlt) || !Input.anyKeyDown) && !Input.GetKeyUp(KeyCode.Space))
      return;
    this.KillVLCProcess();
    PlayVLC.ShowWindow(this._unityHwnd, 5);
  }

  private void QTCheckFullScreenEnd()
  {
    float num = 1f;
    if (this.UnityIsOnPrimaryScreen())
      num = this.GetMainSceenUserScalingFactor();
    PlayVLC.SetForegroundWindow(this._vlcHwnd);
    PlayVLC.ShowWindow(this._vlcHwnd, 5);
    PlayVLC.RECT lpRect = new PlayVLC.RECT();
    PlayVLC.GetWindowRect(this._vlcHwnd, ref lpRect);
    if ((double) (lpRect.Right - lpRect.Left) * (double) num <= 0.0 || ((double) (lpRect.Right - lpRect.Left) * (double) num == (double) (int) this.GetCurrentMonitorDesktopResolution().x || (double) (lpRect.Bottom - lpRect.Top) * (double) num <= 0.0) || (double) (lpRect.Bottom - lpRect.Top) * (double) num == (double) this.GetCurrentMonitorDesktopResolution().y)
      return;
    this.KillVLCProcess();

  }

  private void GetFocus()
  {
    if ((int) this._unityWindowID != (int) PlayVLC.GetActiveWindow() && this._isPlaying)
    {
      PlayVLC.keybd_event((byte) 164, (byte) 69, 1U, 0);
      PlayVLC.keybd_event((byte) 164, (byte) 69, 3U, 0);
      if (this.UsedRenderMode == PlayVLC.RenderMode.VLC_QT_InterfaceFullscreen && !this._qtCheckEnabled)
      {
        this.QTCheckFullScreenEnd();
        this._qtCheckEnabled = true;
        this._vlcHwnd = PlayVLC.FindWindow((string) null, this.StreamingAssetsVideoFilename + " - VLC media player");
      }
      else if (!this._qtCheckEnabled)
      {
        PlayVLC.SetForegroundWindow(this._unityHwnd);
        PlayVLC.ShowWindow(this._unityHwnd, 5);
      }
    }
    if (!this._isPlaying || this.UsedRenderMode != PlayVLC.RenderMode.VLC_QT_InterfaceFullscreen || !this._qtCheckEnabled)
      return;
    this.QTCheckFullScreenEnd();
  }

  private void OnApplicationQuit()
  {
    try
    {
      if (!this._isPlaying || this._thisVlcProcessWasEnded)
        return;
      this._vlc.Kill();
    }
    catch (Exception ex)
    {
    }
  }

  public enum RenderMode
  {
    Direct3DMode,
    VLC_QT_InterfaceFullscreen,
    FullScreenOverlayModePrimaryDisplay,
  }

  public struct RECT
  {
    public int Left;
    public int Top;
    public int Right;
    public int Bottom;
  }

  public enum DeviceCap
  {
    VERTRES = 10,
    DESKTOPVERTRES = 117,
  }

  public enum DpiType
  {
    Effective,
    Angular,
    Raw,
  }
}
