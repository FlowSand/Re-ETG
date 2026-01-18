// Decompiled with JetBrains decompiler
// Type: AkAmbient
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable

[AddComponentMenu("Wwise/AkAmbient")]
public class AkAmbient : AkEvent
  {
    public static Dictionary<int, AkMultiPosEvent> multiPosEventTree = new Dictionary<int, AkMultiPosEvent>();
    public List<Vector3> multiPositionArray = new List<Vector3>();
    public AkMultiPositionType MultiPositionType = AkMultiPositionType.MultiPositionType_MultiSources;
    public MultiPositionTypeLabel multiPositionTypeLabel;

    public AkAmbient ParentAkAmbience { get; set; }

    private void OnEnable()
    {
      if (this.multiPositionTypeLabel == MultiPositionTypeLabel.Simple_Mode)
      {
        foreach (Behaviour component in this.gameObject.GetComponents<AkGameObj>())
          component.enabled = true;
      }
      else if (this.multiPositionTypeLabel == MultiPositionTypeLabel.Large_Mode)
      {
        foreach (Behaviour component in this.gameObject.GetComponents<AkGameObj>())
          component.enabled = false;
        AkPositionArray in_pPositions = this.BuildAkPositionArray();
        int num = (int) AkSoundEngine.SetMultiplePositions(this.gameObject, in_pPositions, (ushort) in_pPositions.Count, this.MultiPositionType);
      }
      else
      {
        if (this.multiPositionTypeLabel != MultiPositionTypeLabel.MultiPosition_Mode)
          return;
        foreach (Behaviour component in this.gameObject.GetComponents<AkGameObj>())
          component.enabled = false;
        AkMultiPosEvent eventPosList;
        if (AkAmbient.multiPosEventTree.TryGetValue(this.eventID, out eventPosList))
        {
          if (!eventPosList.list.Contains(this))
            eventPosList.list.Add(this);
        }
        else
        {
          eventPosList = new AkMultiPosEvent();
          eventPosList.list.Add(this);
          AkAmbient.multiPosEventTree.Add(this.eventID, eventPosList);
        }
        AkPositionArray in_pPositions = this.BuildMultiDirectionArray(eventPosList);
        int num = (int) AkSoundEngine.SetMultiplePositions(eventPosList.list[0].gameObject, in_pPositions, (ushort) in_pPositions.Count, this.MultiPositionType);
      }
    }

    private void OnDisable()
    {
      if (this.multiPositionTypeLabel != MultiPositionTypeLabel.MultiPosition_Mode)
        return;
      AkMultiPosEvent eventPosList = AkAmbient.multiPosEventTree[this.eventID];
      if (eventPosList.list.Count == 1)
      {
        AkAmbient.multiPosEventTree.Remove(this.eventID);
      }
      else
      {
        eventPosList.list.Remove(this);
        AkPositionArray in_pPositions = this.BuildMultiDirectionArray(eventPosList);
        int num = (int) AkSoundEngine.SetMultiplePositions(eventPosList.list[0].gameObject, in_pPositions, (ushort) in_pPositions.Count, this.MultiPositionType);
      }
    }

    public override void HandleEvent(GameObject in_gameObject)
    {
      if (this.multiPositionTypeLabel != MultiPositionTypeLabel.MultiPosition_Mode)
      {
        base.HandleEvent(in_gameObject);
      }
      else
      {
        AkMultiPosEvent akMultiPosEvent = AkAmbient.multiPosEventTree[this.eventID];
        if (akMultiPosEvent.eventIsPlaying)
          return;
        akMultiPosEvent.eventIsPlaying = true;
        this.soundEmitterObject = akMultiPosEvent.list[0].gameObject;
        if (this.enableActionOnEvent)
        {
          int num = (int) AkSoundEngine.ExecuteActionOnEvent((uint) this.eventID, this.actionOnEventType, akMultiPosEvent.list[0].gameObject, (int) this.transitionDuration * 1000, this.curveInterpolation);
        }
        else
          this.playingId = AkSoundEngine.PostEvent((uint) this.eventID, akMultiPosEvent.list[0].gameObject, 1U, new AkCallbackManager.EventCallback(akMultiPosEvent.FinishedPlaying), (object) null, 0U, (AkExternalSourceInfo) null, 0U);
      }
    }

    public void OnDrawGizmosSelected()
    {
      Gizmos.DrawIcon(this.transform.position, "WwiseAudioSpeaker.png", false);
    }

    public AkPositionArray BuildMultiDirectionArray(AkMultiPosEvent eventPosList)
    {
      AkPositionArray akPositionArray = new AkPositionArray((uint) eventPosList.list.Count);
      for (int index = 0; index < eventPosList.list.Count; ++index)
        akPositionArray.Add(eventPosList.list[index].transform.position, eventPosList.list[index].transform.forward, eventPosList.list[index].transform.up);
      return akPositionArray;
    }

    private AkPositionArray BuildAkPositionArray()
    {
      AkPositionArray akPositionArray = new AkPositionArray((uint) this.multiPositionArray.Count);
      for (int index = 0; index < this.multiPositionArray.Count; ++index)
        akPositionArray.Add(this.transform.position + this.multiPositionArray[index], this.transform.forward, this.transform.up);
      return akPositionArray;
    }
  }

