// Decompiled with JetBrains decompiler
// Type: AkRTPCPlayable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using AK.Wwise;
using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

#nullable disable

[Serializable]
public class AkRTPCPlayable : PlayableAsset, ITimelineClipAsset
  {
    public bool overrideTrackObject;
    private TimelineClip owningClip;
    private RTPC RTPC;
    public ExposedReference<GameObject> RTPCObject;
    public bool setRTPCGlobally;
    public AkRTPCPlayableBehaviour template = new AkRTPCPlayableBehaviour();

    public RTPC Parameter
    {
      get => this.RTPC;
      set => this.RTPC = value;
    }

    public TimelineClip OwningClip
    {
      get => this.owningClip;
      set => this.owningClip = value;
    }

    public ClipCaps clipCaps => ClipCaps.None;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
    {
      ScriptPlayable<AkRTPCPlayableBehaviour> playable = ScriptPlayable<AkRTPCPlayableBehaviour>.Create(graph, this.template);
      AkRTPCPlayableBehaviour behaviour = playable.GetBehaviour();
      this.InitializeBehavior(graph, ref behaviour, go);
      return (Playable) playable;
    }

    public void InitializeBehavior(
      PlayableGraph graph,
      ref AkRTPCPlayableBehaviour b,
      GameObject owner)
    {
      b.overrideTrackObject = this.overrideTrackObject;
      b.setRTPCGlobally = this.setRTPCGlobally;
      b.rtpcObject = !this.overrideTrackObject ? owner : this.RTPCObject.Resolve(graph.GetResolver());
      b.parameter = this.RTPC;
    }
  }

