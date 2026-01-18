// Decompiled with JetBrains decompiler
// Type: PlayMakerRPCProxy
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

public class PlayMakerRPCProxy : MonoBehaviour
  {
    public PlayMakerFSM[] fsms;

    public void Reset() => this.fsms = this.GetComponents<PlayMakerFSM>();

    public void ForwardEvent(string eventName)
    {
      foreach (PlayMakerFSM fsm in this.fsms)
        fsm.SendEvent(eventName);
    }
  }

