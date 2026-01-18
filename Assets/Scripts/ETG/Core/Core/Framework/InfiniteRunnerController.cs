using System;
using System.Collections;
using System.Diagnostics;

using Dungeonator;

#nullable disable

public class InfiniteRunnerController : BraveBehaviour, IPlaceConfigurable
    {
        [NonSerialized]
        public RoomHandler TargetRoom;

        public void ConfigureOnPlacement(RoomHandler room)
        {
            this.StartCoroutine(this.HandleDelayedInitialization(room));
        }

        [DebuggerHidden]
        private IEnumerator HandleDelayedInitialization(RoomHandler room)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new InfiniteRunnerController__HandleDelayedInitializationc__Iterator0()
            {
                room = room,
                _this = this
            };
        }

        public void StartQuest()
        {
            this.talkDoer.PathfindToPosition(this.TargetRoom.GetCenterCell().ToVector2());
        }

        protected override void OnDestroy() => base.OnDestroy();
    }

