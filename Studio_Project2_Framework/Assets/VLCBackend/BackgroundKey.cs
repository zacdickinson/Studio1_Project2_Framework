// Decompiled with JetBrains decompiler
// Type: BackgroundKey
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C9321C04-A7AB-482D-8D92-C80C2E04350E
// Assembly location: C:\Users\PC\Downloads\VLC_for_Unity_Youtube\VLC_for_Unity_Youtube_Data\Managed\Assembly-CSharp.dll

using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class BackgroundKey : MonoBehaviour
{
  private bool active = true;
  [SerializeField]
  private Material _keyMaterial;

  [DllImport("Dwmapi.dll")]
  private static extern uint DwmExtendFrameIntoClientArea(IntPtr hWnd, ref BackgroundKey.Margins margins);

  [DllImport("user32.dll")]
  private static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);

  [DllImport("user32.dll")]
  public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

  private void Start()
  {
    this.transform.SetAsFirstSibling();
  }

  public void ActivateTransparency(IntPtr hWnd)
  {
    this.active = true;
    Color32 color32 = (Color32) this.GetComponent<Camera>().backgroundColor;
    this.GetComponent<Camera>().backgroundColor = (Color) new Color32(color32.r, color32.g, color32.b, (byte) 5);

    BackgroundKey.SetWindowLong(hWnd, -16, 2415919104U);
  }

  public void DisableTransparency()
  {
    this.active = false;
    Color32 color32 = (Color32) this.GetComponent<Camera>().backgroundColor;
    this.GetComponent<Camera>().backgroundColor = (Color) new Color32(color32.r, color32.g, color32.b, byte.MaxValue);
  }

  private void OnRenderImage(RenderTexture a, RenderTexture n)
  {
    if (!this.active)
      return;
    Graphics.Blit((Texture) a, n, this._keyMaterial);
  }

  private struct Margins
  {
    public int Left;
    public int Right;
    public int Top;
    public int Bottom;
  }
}
