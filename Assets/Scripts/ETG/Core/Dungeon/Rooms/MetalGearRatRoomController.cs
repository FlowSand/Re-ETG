using System.Collections;
using System.Diagnostics;

using UnityEngine;

using Dungeonator;

#nullable disable

public class MetalGearRatRoomController : MonoBehaviour
    {
        public GameObject brokenMetalGear;
        public GameObject floorCover;

        [DebuggerHidden]
        public IEnumerator Start()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new MetalGearRatRoomController__Startc__Iterator0()
            {
                _this = this
            };
        }

        private void HandlePitfallIntoReward()
        {
            GameManager.Instance.Dungeon.StartCoroutine(this.HandlePitfallIntoRewardCR());
        }

        [DebuggerHidden]
        private IEnumerator HandlePitfallIntoRewardCR()
        {
            // ISSUE: object of a compiler-generated type is created
            // ISSUE: variable of a compiler-generated type
            MetalGearRatRoomController__HandlePitfallIntoRewardCRc__Iterator1 rewardCrCIterator1 = new MetalGearRatRoomController__HandlePitfallIntoRewardCRc__Iterator1();
            return (IEnumerator) rewardCrCIterator1;
        }

        public void EnablePitfalls(bool value)
        {
            this.floorCover.SetActive(!value);
            IntVector2 intVector2 = this.transform.position.GetAbsoluteRoom().area.basePosition + new IntVector2(19, 12);
            for (int x = 0; x < 8; ++x)
            {
                for (int y = 0; y < 5; ++y)
                    GameManager.Instance.Dungeon.data[intVector2 + new IntVector2(x, y)].fallingPrevented = !value;
            }
        }

        public void TransformToDestroyedRoom()
        {
            this.brokenMetalGear.SetActive(true);
            this.EnablePitfalls(true);
            RoomHandler absoluteRoom = this.transform.position.GetAbsoluteRoom();
            if (absoluteRoom == null || absoluteRoom.DarkSoulsRoomResetDependencies == null)
                return;
            absoluteRoom.DarkSoulsRoomResetDependencies.Clear();
        }
    }

