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

