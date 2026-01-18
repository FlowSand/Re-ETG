using System.Collections.Generic;

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [HutongGames.PlayMaker.Tooltip("Unlocks all truth chests in the current room")]
    [ActionCategory(".NPCs")]
    public class OpenTruthChest : FsmStateAction
    {
        [HutongGames.PlayMaker.Tooltip("Seconds to wait before opening the chest.")]
        public FsmFloat delay;
        [HutongGames.PlayMaker.Tooltip("If true, the chest will open if this action ends early.")]
        public FsmBool openOnEarlyFinish;
        private float m_vanishTimer;
        private bool m_opened;

        public override void Reset()
        {
            this.delay = (FsmFloat) 0.0f;
            this.openOnEarlyFinish = (FsmBool) true;
        }

        public override void OnEnter()
        {
            this.m_opened = false;
            if ((double) this.delay.Value <= 0.0)
            {
                this.OpenChest();
                this.Finish();
            }
            else
                this.m_vanishTimer = this.delay.Value;
        }

        public override void OnUpdate()
        {
            this.m_vanishTimer -= BraveTime.DeltaTime;
            if ((double) this.m_vanishTimer > 0.0)
                return;
            this.OpenChest();
            this.Finish();
        }

        public override void OnExit()
        {
            if (!this.openOnEarlyFinish.Value || this.m_opened)
                return;
            this.OpenChest();
        }

        private void OpenChest()
        {
            if (this.m_opened)
                return;
            List<Chest> componentsInRoom = GameManager.Instance.Dungeon.GetRoomFromPosition(this.Owner.transform.position.IntXY(VectorConversions.Floor)).GetComponentsInRoom<Chest>();
            for (int index = 0; index < componentsInRoom.Count; ++index)
            {
                if (componentsInRoom[index].name.ToLowerInvariant().Contains("truth"))
                {
                    componentsInRoom[index].IsLocked = false;
                    componentsInRoom[index].IsSealed = false;
                    tk2dSpriteAnimator componentInChildren = componentsInRoom[index].transform.Find("lock").GetComponentInChildren<tk2dSpriteAnimator>();
                    if ((Object) componentInChildren != (Object) null)
                    {
                        componentInChildren.StopAndResetFrame();
                        componentInChildren.PlayAndDestroyObject("truth_lock_open");
                    }
                }
            }
            this.m_opened = true;
        }
    }
}
