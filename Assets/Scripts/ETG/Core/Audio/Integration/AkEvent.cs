using UnityEngine;

#nullable disable

[RequireComponent(typeof (AkGameObj))]
[AddComponentMenu("Wwise/AkEvent")]
public class AkEvent : AkUnityEventHandler
  {
    public AkActionOnEventType actionOnEventType;
    public AkCurveInterpolation curveInterpolation = AkCurveInterpolation.AkCurveInterpolation_Linear;
    public bool enableActionOnEvent;
    public int eventID;
    public AkEventCallbackData m_callbackData;
    public uint playingId;
    public GameObject soundEmitterObject;
    public float transitionDuration;

    private void Callback(object in_cookie, AkCallbackType in_type, AkCallbackInfo in_info)
    {
      for (int index = 0; index < this.m_callbackData.callbackFunc.Count; ++index)
      {
        if ((in_type & (AkCallbackType) this.m_callbackData.callbackFlags[index]) != (AkCallbackType) 0 && (Object) this.m_callbackData.callbackGameObj[index] != (Object) null)
          this.m_callbackData.callbackGameObj[index].SendMessage(this.m_callbackData.callbackFunc[index], (object) new AkEventCallbackMsg()
          {
            type = in_type,
            sender = this.gameObject,
            info = in_info
          });
      }
    }

    public override void HandleEvent(GameObject in_gameObject)
    {
      GameObject in_gameObjectID = !this.useOtherObject || !((Object) in_gameObject != (Object) null) ? this.gameObject : in_gameObject;
      this.soundEmitterObject = in_gameObjectID;
      if (this.enableActionOnEvent)
      {
        int num = (int) AkSoundEngine.ExecuteActionOnEvent((uint) this.eventID, this.actionOnEventType, in_gameObjectID, (int) this.transitionDuration * 1000, this.curveInterpolation);
      }
      else
      {
        this.playingId = !((Object) this.m_callbackData != (Object) null) ? AkSoundEngine.PostEvent((uint) this.eventID, in_gameObjectID) : AkSoundEngine.PostEvent((uint) this.eventID, in_gameObjectID, (uint) this.m_callbackData.uFlags, new AkCallbackManager.EventCallback(this.Callback), (object) null, 0U, (AkExternalSourceInfo) null, 0U);
        if (this.playingId != 0U || !AkSoundEngine.IsInitialized())
          return;
        Debug.LogError((object) $"Could not post event ID \"{(object) (uint) this.eventID}\". Did you make sure to load the appropriate SoundBank?");
      }
    }

    public void Stop(int _transitionDuration, AkCurveInterpolation _curveInterpolation = AkCurveInterpolation.AkCurveInterpolation_Linear)
    {
      int num = (int) AkSoundEngine.ExecuteActionOnEvent((uint) this.eventID, AkActionOnEventType.AkActionOnEventType_Stop, this.soundEmitterObject, _transitionDuration, _curveInterpolation);
    }
  }

