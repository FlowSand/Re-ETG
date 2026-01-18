using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

#nullable disable

[Serializable]
public class AkEventPlayable : PlayableAsset, ITimelineClipAsset
  {
    private readonly WwiseEventTracker eventTracker = new WwiseEventTracker();
    public AK.Wwise.Event akEvent;
    private float blendInDuration;
    private float blendOutDuration;
    private float easeInDuration;
    private float easeOutDuration;
    public ExposedReference<GameObject> emitterObjectRef;
    [SerializeField]
    private float eventDurationMax = -1f;
    [SerializeField]
    private float eventDurationMin = -1f;
    public bool overrideTrackEmitterObject;
    private TimelineClip owningClip;
    public bool retriggerEvent;

    public TimelineClip OwningClip
    {
      get => this.owningClip;
      set => this.owningClip = value;
    }

    public override double duration
    {
      get => this.akEvent == null ? base.duration : (double) this.eventDurationMax;
    }

    public ClipCaps clipCaps => !this.retriggerEvent ? ClipCaps.All : ClipCaps.None;

    public void setEaseInDuration(float d) => this.easeInDuration = d;

    public void setEaseOutDuration(float d) => this.easeOutDuration = d;

    public void setBlendInDuration(float d) => this.blendInDuration = d;

    public void setBlendOutDuration(float d) => this.blendOutDuration = d;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
      ScriptPlayable<AkEventPlayableBehavior> playable = ScriptPlayable<AkEventPlayableBehavior>.Create(graph);
      AkEventPlayableBehavior behaviour = playable.GetBehaviour();
      this.initializeBehaviour(graph, behaviour, owner);
      behaviour.akEventMinDuration = this.eventDurationMin;
      behaviour.akEventMaxDuration = this.eventDurationMax;
      return (Playable) playable;
    }

    public void initializeBehaviour(PlayableGraph graph, AkEventPlayableBehavior b, GameObject owner)
    {
      b.akEvent = this.akEvent;
      b.eventTracker = this.eventTracker;
      b.easeInDuration = this.easeInDuration;
      b.easeOutDuration = this.easeOutDuration;
      b.blendInDuration = this.blendInDuration;
      b.blendOutDuration = this.blendOutDuration;
      b.eventShouldRetrigger = this.retriggerEvent;
      b.overrideTrackEmittorObject = this.overrideTrackEmitterObject;
      if (this.overrideTrackEmitterObject)
        b.eventObject = this.emitterObjectRef.Resolve(graph.GetResolver());
      else
        b.eventObject = owner;
    }
  }

