// Decompiled with JetBrains decompiler
// Type: tk2dUIBaseItemControl
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
[AddComponentMenu("2D Toolkit/UI/tk2dUIBaseItemControl")]
public abstract class tk2dUIBaseItemControl : MonoBehaviour
{
  public tk2dUIItem uiItem;

  public GameObject SendMessageTarget
  {
    get
    {
      return (Object) this.uiItem != (Object) null ? this.uiItem.sendMessageTarget : (GameObject) null;
    }
    set
    {
      if (!((Object) this.uiItem != (Object) null))
        return;
      this.uiItem.sendMessageTarget = value;
    }
  }

  public static void ChangeGameObjectActiveState(GameObject go, bool isActive)
  {
    go.SetActive(isActive);
  }

  public static void ChangeGameObjectActiveStateWithNullCheck(GameObject go, bool isActive)
  {
    if (!((Object) go != (Object) null))
      return;
    tk2dUIBaseItemControl.ChangeGameObjectActiveState(go, isActive);
  }

  protected void DoSendMessage(string methodName, object parameter)
  {
    if (!((Object) this.SendMessageTarget != (Object) null) || methodName.Length <= 0)
      return;
    this.SendMessageTarget.SendMessage(methodName, parameter, SendMessageOptions.RequireReceiver);
  }
}
