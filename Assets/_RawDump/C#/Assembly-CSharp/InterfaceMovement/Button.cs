// Decompiled with JetBrains decompiler
// Type: InterfaceMovement.Button
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace InterfaceMovement;

public class Button : MonoBehaviour
{
  private Renderer cachedRenderer;
  public Button up;
  public Button down;
  public Button left;
  public Button right;

  private void Start() => this.cachedRenderer = this.GetComponent<Renderer>();

  private void Update()
  {
    bool flag = (Object) this.transform.parent.GetComponent<ButtonManager>().focusedButton == (Object) this;
    Color color = this.cachedRenderer.material.color;
    color.a = Mathf.MoveTowards(color.a, !flag ? 0.5f : 1f, Time.deltaTime * 3f);
    this.cachedRenderer.material.color = color;
  }
}
