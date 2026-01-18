using UnityEngine;

#nullable disable

public class PlayMakerRPCProxy : MonoBehaviour
    {
        public PlayMakerFSM[] fsms;

        public void Reset() => this.fsms = this.GetComponents<PlayMakerFSM>();

        public void ForwardEvent(string eventName)
        {
            foreach (PlayMakerFSM fsm in this.fsms)
                fsm.SendEvent(eventName);
        }
    }

