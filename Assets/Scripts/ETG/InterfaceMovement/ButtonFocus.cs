// Decompiled with JetBrains decompiler
// Type: InterfaceMovement.ButtonFocus
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace InterfaceMovement
{
  public class ButtonFocus : MonoBehaviour
  {
    private void Update()
    {
      this.transform.position = Vector3.MoveTowards(this.transform.position, this.transform.parent.GetComponent<ButtonManager>().focusedButton.transform.position, Time.deltaTime * 10f);
    }
  }
}
