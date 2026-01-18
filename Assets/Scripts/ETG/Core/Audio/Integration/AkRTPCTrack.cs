using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

using AK.Wwise;

#nullable disable

[TrackColor(0.32f, 0.13f, 0.13f)]
[TrackClipType(typeof (AkRTPCPlayable))]
[TrackBindingType(typeof (GameObject))]
public class AkRTPCTrack : TrackAsset
    {
        public RTPC Parameter;

        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            ScriptPlayable<AkRTPCPlayableBehaviour> trackMixer = ScriptPlayable<AkRTPCPlayableBehaviour>.Create(graph, inputCount);
            this.setPlayableProperties();
            return (Playable) trackMixer;
        }

        public void setPlayableProperties()
        {
            foreach (TimelineClip clip in this.GetClips())
            {
                AkRTPCPlayable asset = (AkRTPCPlayable) clip.asset;
                asset.Parameter = this.Parameter;
                asset.OwningClip = clip;
            }
        }

        public void OnValidate()
        {
            foreach (TimelineClip clip in this.GetClips())
                ((AkRTPCPlayable) clip.asset).Parameter = this.Parameter;
        }
    }

