using System;

using UnityEngine;

using Dungeonator;

#nullable disable

public class TeleporterController : 
        DungeonPlaceableBehaviour,
        IPlaceConfigurable,
        IPlayerInteractable
    {
        public GameObject teleporterIcon;
        public GameObject teleportDepartureVFX;
        public GameObject teleportArrivalVFX;
        public GameObject onetimeActivateVFX;
        public GameObject extantActiveVFX;
        public tk2dSpriteAnimator portalVFX;
        private bool m_wasJustWarpedTo;
        private RoomHandler m_room;
        private bool m_activated;

        public void Start()
        {
            IntVector2 intVector2 = (this.transform.position.XY() + new Vector2(0.5f, 0.5f)).ToIntVector2(VectorConversions.Floor);
            for (int x = intVector2.x; x < intVector2.x + this.placeableWidth; ++x)
            {
                for (int y = intVector2.y; y < intVector2.y + this.placeableHeight; ++y)
                    GameManager.Instance.Dungeon.data[x, y].PreventRewardSpawn = true;
            }
        }

        public void Update()
        {
            if (this.m_activated || !((UnityEngine.Object) GameManager.Instance.PrimaryPlayer != (UnityEngine.Object) null) || GameManager.Instance.PrimaryPlayer.CurrentRoom == null || GameManager.Instance.PrimaryPlayer.CurrentRoom != this.m_room && (GameManager.Instance.CurrentGameType != GameManager.GameType.COOP_2_PLAYER || !((UnityEngine.Object) GameManager.Instance.SecondaryPlayer != (UnityEngine.Object) null) || GameManager.Instance.SecondaryPlayer.CurrentRoom == null || GameManager.Instance.SecondaryPlayer.CurrentRoom != this.m_room) || this.m_room.HasActiveEnemies(RoomHandler.ActiveEnemyType.RoomClear))
                return;
            this.Activate();
        }

        protected override void OnDestroy() => base.OnDestroy();

        public void SetReturnActive()
        {
            this.m_wasJustWarpedTo = true;
            this.portalVFX.gameObject.SetActive(true);
        }

        public void ClearReturnActive() => this.portalVFX.gameObject.SetActive(false);

        public void ConfigureOnPlacement(RoomHandler room)
        {
            this.m_room = room;
            Minimap.Instance.RegisterTeleportIcon(this.m_room, this.teleporterIcon, this.sprite.WorldCenter);
        }

        public float GetDistanceToPoint(Vector2 point)
        {
            return !this.portalVFX.gameObject.activeSelf ? 10000f : Vector2.Distance(point, this.sprite.WorldCenter) / 3f;
        }

        public float GetOverrideMaxDistance() => -1f;

        public void OnEnteredRange(PlayerController interactor)
        {
            if (this.m_wasJustWarpedTo || !this.m_activated || !interactor.CanReturnTeleport)
                return;
            this.portalVFX.sprite.HeightOffGround = -1f;
            SpriteOutlineManager.AddOutlineToSprite(this.sprite, Color.white, 0.1f);
        }

        public void OnExitRange(PlayerController interactor)
        {
            this.m_wasJustWarpedTo = false;
            if (!this.m_activated)
                return;
            SpriteOutlineManager.RemoveOutlineFromSprite(this.sprite);
        }

        public void Interact(PlayerController interactor)
        {
            if (GameManager.Instance.AllPlayers != null)
            {
                for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
                {
                    PlayerController allPlayer = GameManager.Instance.AllPlayers[index];
                    if ((bool) (UnityEngine.Object) allPlayer && allPlayer.IsTalking)
                        return;
                }
            }
            if (!this.m_activated || !interactor.CanReturnTeleport)
                return;
            for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
                GameManager.Instance.AllPlayers[index].AttemptReturnTeleport(this);
        }

        public string GetAnimationState(PlayerController interactor, out bool shouldBeFlipped)
        {
            shouldBeFlipped = false;
            return string.Empty;
        }

        private void Activate()
        {
            this.m_activated = true;
            this.spriteAnimator.Play("teleport_pad_activate");
            this.spriteAnimator.AnimationCompleted += new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.TriggerActiveVFX);
            if (!((UnityEngine.Object) this.onetimeActivateVFX != (UnityEngine.Object) null))
                return;
            this.onetimeActivateVFX.SetActive(true);
            this.onetimeActivateVFX.GetComponent<tk2dSprite>().IsPerpendicular = false;
        }

        private void TriggerActiveVFX(tk2dSpriteAnimator arg1, tk2dSpriteAnimationClip arg2)
        {
            this.spriteAnimator.AnimationCompleted -= new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.TriggerActiveVFX);
            this.extantActiveVFX.SetActive(true);
        }
    }

