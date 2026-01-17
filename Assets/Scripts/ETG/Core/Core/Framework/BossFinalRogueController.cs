// Decompiled with JetBrains decompiler
// Type: BossFinalRogueController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable

namespace ETG.Core.Core.Framework
{
    public class BossFinalRogueController : BraveBehaviour
    {
      public GameObject cameraPoint;
      public List<BossFinalRogueGunController> BaseGuns;
      public float minPlayerDist = -10f;
      public float maxPlayerDist = 14f;
      public float playerDistOffset = 7f;
      public Vector2 worldCenter;
      public float worldRadius;
      [Header("Background Scrolling")]
      public float minScrollDist = 8f;
      public float maxScrollDist = 20f;
      public float scrollMultiplier = 0.05f;
      private float? m_cameraX;
      private IntVector2 m_cachedCameraLowerLeftPixels;
      private IntVector2 m_cachedCameraUpperRightPixels;
      private PilotPastController m_pastController;
      private CameraController m_camera;
      private bool m_lockCamera;
      private bool m_suppressBaseGuns;

      public void Start()
      {
        PhysicsEngine.Instance.OnPostRigidbodyMovement += new System.Action(this.PostRigidbodyMovement);
        this.gameObject.SetLayerRecursively(LayerMask.NameToLayer("BG_Critical"));
        this.m_pastController = UnityEngine.Object.FindObjectOfType<PilotPastController>();
      }

      public void Update()
      {
        if ((bool) (UnityEngine.Object) this.m_camera && this.m_cameraX.HasValue)
        {
          float x = this.m_camera.GetCoreCurrentBasePosition().x;
          float num = Mathf.InverseLerp(this.minScrollDist, this.maxScrollDist, Mathf.Abs(x - this.m_cameraX.Value));
          this.m_pastController.BackgroundScrollSpeed.x = Mathf.Sign(x - this.m_cameraX.Value) * num * this.scrollMultiplier;
        }
        this.m_cachedCameraLowerLeftPixels = PhysicsEngine.UnitToPixel((Vector2) BraveUtility.ViewportToWorldpoint(new Vector2(0.0f, 0.0f), ViewportType.Gameplay));
        this.m_cachedCameraUpperRightPixels = PhysicsEngine.UnitToPixel((Vector2) BraveUtility.ViewportToWorldpoint(new Vector2(1f, 1f), ViewportType.Gameplay));
      }

      protected override void OnDestroy()
      {
        if (PhysicsEngine.HasInstance)
          PhysicsEngine.Instance.OnPostRigidbodyMovement -= new System.Action(this.PostRigidbodyMovement);
        base.OnDestroy();
      }

      public void InitCamera()
      {
        if ((bool) (UnityEngine.Object) this.m_camera)
          return;
        this.m_camera = GameManager.Instance.MainCameraController;
        this.m_lockCamera = true;
        this.m_camera.SetManualControl(true);
        this.m_camera.OverridePosition = (Vector3) this.CameraPos;
        this.m_cameraX = new float?(this.m_camera.OverridePosition.x);
        GameManager.Instance.PrimaryPlayer.specRigidbody.MovementRestrictor += new SpeculativeRigidbody.MovementRestrictorDelegate(this.CameraBoundsMovementRestrictor);
      }

      public void EndCameraLock() => this.m_lockCamera = false;

      public bool SuppressBaseGuns
      {
        get => this.m_suppressBaseGuns;
        set
        {
          if (this.m_suppressBaseGuns != value)
          {
            for (int index = 0; index < this.BaseGuns.Count; ++index)
              this.BaseGuns[index].fireType = !value ? BossFinalRogueGunController.FireType.Timed : BossFinalRogueGunController.FireType.Triggered;
          }
          this.m_suppressBaseGuns = value;
        }
      }

      private void PostRigidbodyMovement()
      {
        if (!this.m_lockCamera)
          return;
        this.m_camera.OverridePosition = (Vector3) this.CameraPos;
      }

      private void CameraBoundsMovementRestrictor(
        SpeculativeRigidbody specRigidbody,
        IntVector2 prevPixelOffset,
        IntVector2 pixelOffset,
        ref bool validLocation)
      {
        if (!validLocation)
          return;
        if (specRigidbody.PixelColliders[0].LowerLeft.x < this.m_cachedCameraLowerLeftPixels.x && pixelOffset.x < prevPixelOffset.x)
          validLocation = false;
        else if (specRigidbody.PixelColliders[0].UpperRight.x > this.m_cachedCameraUpperRightPixels.x && pixelOffset.x > prevPixelOffset.x)
          validLocation = false;
        else if (specRigidbody.PixelColliders[0].LowerLeft.y < this.m_cachedCameraLowerLeftPixels.y && pixelOffset.y < prevPixelOffset.y)
        {
          validLocation = false;
        }
        else
        {
          if (specRigidbody.PixelColliders[1].UpperRight.y <= this.m_cachedCameraUpperRightPixels.y || pixelOffset.y <= prevPixelOffset.y)
            return;
          validLocation = false;
        }
      }

      public Vector2 CameraPos
      {
        get
        {
          CameraController cameraController = this.m_camera ?? GameManager.Instance.MainCameraController;
          float num = Mathf.SmoothStep(this.playerDistOffset, 0.0f, Mathf.InverseLerp(this.minPlayerDist, this.maxPlayerDist, this.specRigidbody.HitboxPixelCollider.UnitBottom - GameManager.Instance.PrimaryPlayer.specRigidbody.UnitBottom));
          Vector2 cameraPos = this.cameraPoint.transform.position.XY() + new Vector2(0.0f, -cameraController.Camera.orthographicSize + num);
          if (this.m_cameraX.HasValue)
            cameraPos.x = this.m_cameraX.Value;
          return cameraPos;
        }
      }
    }

}
