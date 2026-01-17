// Decompiled with JetBrains decompiler
// Type: AkTriggerExit
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

namespace ETG.Core.Audio.Integration
{
    public class AkTriggerExit : AkTriggerBase
    {
      public GameObject triggerObject;

      private void OnTriggerExit(Collider in_other)
      {
        if (this.triggerDelegate == null || !((Object) this.triggerObject == (Object) null) && !((Object) this.triggerObject == (Object) in_other.gameObject))
          return;
        this.triggerDelegate(in_other.gameObject);
      }
    }

}
