// Decompiled with JetBrains decompiler
// Type: AK.Wwise.Event
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
namespace AK.Wwise
{
  [Serializable]
  public class Event : BaseType
  {
    private void VerifyPlayingID(uint playingId)
    {
    }

    public uint Post(GameObject gameObject)
    {
      if (!this.IsValid())
        return 0;
      uint playingId = AkSoundEngine.PostEvent(this.GetID(), gameObject);
      this.VerifyPlayingID(playingId);
      return playingId;
    }

    public uint Post(
      GameObject gameObject,
      CallbackFlags flags,
      AkCallbackManager.EventCallback callback,
      object cookie = null)
    {
      if (!this.IsValid())
        return 0;
      uint playingId = AkSoundEngine.PostEvent(this.GetID(), gameObject, flags.value, callback, cookie);
      this.VerifyPlayingID(playingId);
      return playingId;
    }

    public uint Post(
      GameObject gameObject,
      uint flags,
      AkCallbackManager.EventCallback callback,
      object cookie = null)
    {
      if (!this.IsValid())
        return 0;
      uint playingId = AkSoundEngine.PostEvent(this.GetID(), gameObject, flags, callback, cookie);
      this.VerifyPlayingID(playingId);
      return playingId;
    }

    public void Stop(
      GameObject gameObject,
      int transitionDuration = 0,
      AkCurveInterpolation curveInterpolation = AkCurveInterpolation.AkCurveInterpolation_Linear)
    {
      this.ExecuteAction(gameObject, AkActionOnEventType.AkActionOnEventType_Stop, transitionDuration, curveInterpolation);
    }

    public void ExecuteAction(
      GameObject gameObject,
      AkActionOnEventType actionOnEventType,
      int transitionDuration,
      AkCurveInterpolation curveInterpolation)
    {
      if (!this.IsValid())
        return;
      this.Verify(AkSoundEngine.ExecuteActionOnEvent(this.GetID(), actionOnEventType, gameObject, transitionDuration, curveInterpolation));
    }

    public void PostMIDI(GameObject gameObject, AkMIDIPostArray array)
    {
      if (!this.IsValid())
        return;
      array.PostOnEvent(this.GetID(), gameObject);
    }

    public void PostMIDI(GameObject gameObject, AkMIDIPostArray array, int count)
    {
      if (!this.IsValid())
        return;
      array.PostOnEvent(this.GetID(), gameObject, count);
    }

    public void StopMIDI(GameObject gameObject)
    {
      if (!this.IsValid())
        return;
      int num = (int) AkSoundEngine.StopMIDIOnEvent(this.GetID(), gameObject);
    }

    public void StopMIDI()
    {
      if (!this.IsValid())
        return;
      int num = (int) AkSoundEngine.StopMIDIOnEvent(this.GetID());
    }
  }
}
