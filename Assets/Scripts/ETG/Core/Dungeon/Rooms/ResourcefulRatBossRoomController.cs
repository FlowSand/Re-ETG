using System.Collections;
using System.Diagnostics;

using UnityEngine;

using Dungeonator;

#nullable disable

public class ResourcefulRatBossRoomController : MonoBehaviour
    {
        public tk2dSpriteAnimator grateAnimator;
        private RoomHandler m_ratRoom;
        private bool m_isRoomSealed;

        [DebuggerHidden]
        public IEnumerator Start()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new ResourcefulRatBossRoomController__Startc__Iterator0()
            {
                _this = this
            };
        }

        [DebuggerHidden]
        public IEnumerator LateStart()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new ResourcefulRatBossRoomController__LateStartc__Iterator1()
            {
                _this = this
            };
        }

        public void Update()
        {
            if (!GameManager.HasInstance || !this.m_isRoomSealed)
                return;
            PlayerController bestActivePlayer = GameManager.Instance.BestActivePlayer;
            if (!(bool) (Object) bestActivePlayer)
                return;
            CellArea area = this.m_ratRoom.area;
            if (bestActivePlayer.specRigidbody.GetUnitCenter(ColliderType.Ground).IsWithin(area.UnitBottomLeft + new Vector2(-2f, -8f), area.UnitTopRight + new Vector2(2f, 2f)))
                return;
            this.SpecialSealRoom(false);
        }

        public void OpenGrate()
        {
            this.SpecialSealRoom(true);
            this.grateAnimator.Play("rat_grate");
        }

        public void OnDarkSoulsReset()
        {
            this.SpecialSealRoom(false);
            this.grateAnimator.StopAndResetFrameToDefault();
            this.EnablePitfalls(false);
        }

        private void SpecialSealRoom(bool seal)
        {
            this.m_isRoomSealed = seal;
            this.m_ratRoom.npcSealState = !seal ? RoomHandler.NPCSealState.SealNone : RoomHandler.NPCSealState.SealAll;
            foreach (DungeonDoorController connectedDoor in this.m_ratRoom.connectedDoors)
                connectedDoor.KeepBossDoorSealed = seal;
        }

        public void EnablePitfalls(bool value)
        {
            IntVector2 intVector2 = this.transform.position.GetAbsoluteRoom().area.basePosition + new IntVector2(16 /*0x10*/, 10);
            for (int x = 0; x < 4; ++x)
            {
                for (int y = 0; y < 4; ++y)
                    GameManager.Instance.Dungeon.data[intVector2 + new IntVector2(x, y)].fallingPrevented = !value;
            }
        }

        private void PitfallIntoMetalGearRatRoom()
        {
            for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
            {
                PlayerController allPlayer = GameManager.Instance.AllPlayers[index];
                allPlayer.IsOnFire = false;
                allPlayer.CurrentFireMeterValue = 0.0f;
                allPlayer.CurrentPoisonMeterValue = 0.0f;
            }
            this.SpecialSealRoom(false);
            GameManager.Instance.StartCoroutine(this.DoMetalGearPitfall());
        }

        [DebuggerHidden]
        private IEnumerator DoMetalGearPitfall()
        {
            // ISSUE: object of a compiler-generated type is created
            // ISSUE: variable of a compiler-generated type
            ResourcefulRatBossRoomController__DoMetalGearPitfallc__Iterator2 pitfallCIterator2 = new ResourcefulRatBossRoomController__DoMetalGearPitfallc__Iterator2();
            return (IEnumerator) pitfallCIterator2;
        }
    }

