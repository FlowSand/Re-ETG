using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

#nullable disable

[TrackColor(0.855f, 0.8623f, 0.87f)]
[TrackBindingType(typeof (GameObject))]
[TrackClipType(typeof (AkEventPlayable))]
public class AkEventTrack : TrackAsset
  {
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
      ScriptPlayable<AkEventPlayableBehavior> playable = ScriptPlayable<AkEventPlayableBehavior>.Create(graph);
      playable.SetInputCount<ScriptPlayable<AkEventPlayableBehavior>>(inputCount);
      this.setFadeTimes();
      this.setOwnerClips();
      return (Playable) playable;
    }

    public void setFadeTimes()
    {
      foreach (TimelineClip clip in this.GetClips())
      {
        AkEventPlayable asset = (AkEventPlayable) clip.asset;
        asset.setBlendInDuration((float) this.getBlendInTime(asset));
        asset.setBlendOutDuration((float) this.getBlendOutTime(asset));
        asset.setEaseInDuration((float) this.getEaseInTime(asset));
        asset.setEaseOutDuration((float) this.getEaseOutTime(asset));
      }
    }

    public void setOwnerClips()
    {
      foreach (TimelineClip clip in this.GetClips())
        ((AkEventPlayable) clip.asset).OwningClip = clip;
    }

    public double getBlendInTime(AkEventPlayable playableClip)
    {
      foreach (TimelineClip clip in this.GetClips())
      {
        if ((Object) playableClip == clip.asset)
          return clip.blendInDuration;
      }
      return 0.0;
    }

    public double getBlendOutTime(AkEventPlayable playableClip)
    {
      foreach (TimelineClip clip in this.GetClips())
      {
        if ((Object) playableClip == clip.asset)
          return clip.blendOutDuration;
      }
      return 0.0;
    }

    public double getEaseInTime(AkEventPlayable playableClip)
    {
      foreach (TimelineClip clip in this.GetClips())
      {
        if ((Object) playableClip == clip.asset)
          return clip.easeInDuration;
      }
      return 0.0;
    }

    public double getEaseOutTime(AkEventPlayable playableClip)
    {
      foreach (TimelineClip clip in this.GetClips())
      {
        if ((Object) playableClip == clip.asset)
          return clip.easeOutDuration;
      }
      return 0.0;
    }
  }

