// Decompiled with JetBrains decompiler
// Type: AkState
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

[AddComponentMenu("Wwise/AkState")]
public class AkState : AkUnityEventHandler
  {
    public int groupID;
    public int valueID;

    public override void HandleEvent(GameObject in_gameObject)
    {
      int num = (int) AkSoundEngine.SetState((uint) this.groupID, (uint) this.valueID);
    }
  }

