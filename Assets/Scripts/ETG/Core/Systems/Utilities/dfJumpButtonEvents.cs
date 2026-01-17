// Decompiled with JetBrains decompiler
// Type: dfJumpButtonEvents
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    public class dfJumpButtonEvents : MonoBehaviour
    {
      public bool isMouseDown;

      public void OnMouseDown(dfControl control, dfMouseEventArgs mouseEvent)
      {
        this.isMouseDown = true;
      }

      public void OnMouseUp(dfControl control, dfMouseEventArgs mouseEvent) => this.isMouseDown = false;
    }

}
