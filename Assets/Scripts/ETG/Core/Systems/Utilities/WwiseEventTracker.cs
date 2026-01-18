// Decompiled with JetBrains decompiler
// Type: WwiseEventTracker
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable

public class WwiseEventTracker
  {
    public float currentDuration = -1f;
    public float currentDurationProportion = 1f;
    public bool eventIsPlaying;
    public bool fadeoutTriggered;
    public uint playingID;
    public float previousEventStartTime;

    public void CallbackHandler(object in_cookie, AkCallbackType in_type, object in_info)
    {
      if (in_type == AkCallbackType.AK_EndOfEvent)
      {
        this.eventIsPlaying = false;
        this.fadeoutTriggered = false;
      }
      else
      {
        if (in_type != AkCallbackType.AK_Duration)
          return;
        this.currentDuration = (float) ((double) ((AkDurationCallbackInfo) in_info).fEstimatedDuration * (double) this.currentDurationProportion / 1000.0);
      }
    }
  }

