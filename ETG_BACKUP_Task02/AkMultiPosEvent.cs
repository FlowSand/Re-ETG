// Decompiled with JetBrains decompiler
// Type: AkMultiPosEvent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections.Generic;

#nullable disable
public class AkMultiPosEvent
{
  public bool eventIsPlaying;
  public List<AkAmbient> list = new List<AkAmbient>();

  public void FinishedPlaying(object in_cookie, AkCallbackType in_type, object in_info)
  {
    this.eventIsPlaying = false;
  }
}
