using System;
using System.Collections.Generic;

using UnityEngine;

using Dungeonator;

#nullable disable

[RequireComponent(typeof (GenericIntroDoer))]
public class AgunimIntroDoer : SpecificIntroDoer
    {
        public GameObject cameraPoint;
        private int m_cachedCameraMinY;
        private bool m_isMotionRestricted;

        public void Start()
        {
            this.aiActor.ParentRoom.Entered += new RoomHandler.OnEnteredEventHandler(this.PlayerEnteredRoom);
            this.aiActor.healthHaver.OnPreDeath += new Action<Vector2>(this.OnPreDeath);
        }

        public void Update()
        {
            this.m_cachedCameraMinY = PhysicsEngine.UnitToPixel(BraveUtility.ViewportToWorldpoint(new Vector2(0.5f, 0.0f), ViewportType.Camera).y);
        }

        protected override void OnDestroy()
        {
            this.RestrictMotion(false);
            this.ModifyCamera(false);
            base.OnDestroy();
        }

        public override Vector2? OverrideOutroPosition
        {
            get
            {
                if (!(bool) (UnityEngine.Object) this)
                    this.ModifyCamera(false);
                else
                    this.ModifyCamera(true);
                return new Vector2?();
            }
        }

        public override void PlayerWalkedIn(PlayerController player, List<tk2dSpriteAnimator> animators)
        {
            RoomHandler parentRoom = this.aiActor.ParentRoom;
            if (parentRoom == null)
                return;
            List<TorchController> componentsInRoom = parentRoom.GetComponentsInRoom<TorchController>();
            for (int index = 0; index < componentsInRoom.Count; ++index)
            {
                TorchController torchController = componentsInRoom[index];
                if ((bool) (UnityEngine.Object) torchController && (bool) (UnityEngine.Object) torchController.specRigidbody)
                    torchController.specRigidbody.CollideWithOthers = false;
            }
        }

        private void PlayerMovementRestrictor(
            SpeculativeRigidbody playerSpecRigidbody,
            IntVector2 prevPixelOffset,
            IntVector2 pixelOffset,
            ref bool validLocation)
        {
            if (!validLocation || (pixelOffset - prevPixelOffset).y >= 0 || playerSpecRigidbody.PixelColliders[0].MinY + pixelOffset.y >= this.m_cachedCameraMinY + 20)
                return;
            validLocation = false;
        }

        private void PlayerEnteredRoom(PlayerController playerController)
        {
            this.RestrictMotion(true);
            BulletPastRoomController[] objectsOfType = UnityEngine.Object.FindObjectsOfType<BulletPastRoomController>();
            for (int index = 0; index < objectsOfType.Length; ++index)
            {
                if (objectsOfType[index].RoomIdentifier == BulletPastRoomController.BulletRoomCategory.ROOM_C)
                {
                    this.StartCoroutine(objectsOfType[index].HandleAgunimIntro(this.transform));
                    break;
                }
            }
        }

        private void OnPreDeath(Vector2 finalDirection) => this.RestrictMotion(false);

        private void RestrictMotion(bool value)
        {
            if (this.m_isMotionRestricted == value)
                return;
            if (value)
            {
                if (!GameManager.HasInstance || GameManager.Instance.IsLoadingLevel || GameManager.IsReturningToBreach)
                    return;
                for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
                    GameManager.Instance.AllPlayers[index].specRigidbody.MovementRestrictor += new SpeculativeRigidbody.MovementRestrictorDelegate(this.PlayerMovementRestrictor);
            }
            else
            {
                if (!GameManager.HasInstance || GameManager.IsReturningToBreach)
                    return;
                for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
                {
                    PlayerController allPlayer = GameManager.Instance.AllPlayers[index];
                    if ((bool) (UnityEngine.Object) allPlayer)
                        allPlayer.specRigidbody.MovementRestrictor -= new SpeculativeRigidbody.MovementRestrictorDelegate(this.PlayerMovementRestrictor);
                }
            }
            this.m_isMotionRestricted = value;
        }

        private void ModifyCamera(bool value)
        {
            if (!GameManager.HasInstance || GameManager.Instance.IsLoadingLevel || GameManager.IsReturningToBreach)
                return;
            if (value)
            {
                CameraController cameraController = GameManager.Instance.MainCameraController;
                cameraController.LockToRoom = true;
                cameraController.PreventAimLook = true;
                cameraController.AddFocusPoint(this.cameraPoint.gameObject);
            }
            else
            {
                CameraController cameraController = GameManager.Instance.MainCameraController;
                cameraController.LockToRoom = false;
                cameraController.PreventAimLook = false;
                cameraController.RemoveFocusPoint(this.cameraPoint.gameObject);
            }
        }
    }

