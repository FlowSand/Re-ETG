// Decompiled with JetBrains decompiler
// Type: NoBlankChecker
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

namespace ETG.Core.Core.Framework
{
    public class NoBlankChecker : BraveBehaviour
    {
      public void Update()
      {
        if (!((Object) GameManager.Instance.BestActivePlayer != (Object) null) || GameManager.Instance.BestActivePlayer.Blanks != 0)
          return;
        GameManager.BroadcastRoomTalkDoerFsmEvent("hasNoBlanks");
      }
    }

}
