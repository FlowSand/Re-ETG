// Decompiled with JetBrains decompiler
// Type: MultiplayerBasicExample.Player
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using InControl;
using UnityEngine;

#nullable disable
namespace MultiplayerBasicExample
{
  public class Player : MonoBehaviour
  {
    private Renderer cachedRenderer;

    public InputDevice Device { get; set; }

    private void Start() => this.cachedRenderer = this.GetComponent<Renderer>();

    private void Update()
    {
      if (this.Device == null)
      {
        this.cachedRenderer.material.color = new Color(1f, 1f, 1f, 0.2f);
      }
      else
      {
        this.cachedRenderer.material.color = this.GetColorFromInput();
        this.transform.Rotate(Vector3.down, 500f * Time.deltaTime * this.Device.Direction.X, Space.World);
        this.transform.Rotate(Vector3.right, 500f * Time.deltaTime * this.Device.Direction.Y, Space.World);
      }
    }

    private Color GetColorFromInput()
    {
      if ((bool) (OneAxisInputControl) this.Device.Action1)
        return Color.green;
      if ((bool) (OneAxisInputControl) this.Device.Action2)
        return Color.red;
      if ((bool) (OneAxisInputControl) this.Device.Action3)
        return Color.blue;
      return (bool) (OneAxisInputControl) this.Device.Action4 ? Color.yellow : Color.white;
    }
  }
}
