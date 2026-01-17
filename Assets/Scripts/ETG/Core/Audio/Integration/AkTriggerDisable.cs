// Decompiled with JetBrains decompiler
// Type: AkTriggerDisable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

namespace ETG.Core.Audio.Integration
{
    public class AkTriggerDisable : AkTriggerBase
    {
      private void OnDisable()
      {
        if (this.triggerDelegate == null)
          return;
        this.triggerDelegate((GameObject) null);
      }
    }

}
