// Decompiled with JetBrains decompiler
// Type: MultiplayerWithBindingsExample.Player
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using InControl;
using UnityEngine;

#nullable disable
namespace MultiplayerWithBindingsExample;

public class Player : MonoBehaviour
{
  private Renderer cachedRenderer;

  public PlayerActions Actions { get; set; }

  private void OnDisable()
  {
    if (this.Actions == null)
      return;
    this.Actions.Destroy();
  }

  private void Start() => this.cachedRenderer = this.GetComponent<Renderer>();

  private void Update()
  {
    if (this.Actions == null)
    {
      this.cachedRenderer.material.color = new Color(1f, 1f, 1f, 0.2f);
    }
    else
    {
      this.cachedRenderer.material.color = this.GetColorFromInput();
      this.transform.Rotate(Vector3.down, 500f * Time.deltaTime * this.Actions.Rotate.X, Space.World);
      this.transform.Rotate(Vector3.right, 500f * Time.deltaTime * this.Actions.Rotate.Y, Space.World);
    }
  }

  private Color GetColorFromInput()
  {
    if ((bool) (OneAxisInputControl) this.Actions.Green)
      return Color.green;
    if ((bool) (OneAxisInputControl) this.Actions.Red)
      return Color.red;
    if ((bool) (OneAxisInputControl) this.Actions.Blue)
      return Color.blue;
    return (bool) (OneAxisInputControl) this.Actions.Yellow ? Color.yellow : Color.white;
  }
}
