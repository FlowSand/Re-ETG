// Decompiled with JetBrains decompiler
// Type: DemonWallMovementBehavior
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using FullInspector;
using UnityEngine;

#nullable disable

namespace ETG.Core.Actors.Behaviors
{
    [InspectorDropdownName("Bosses/DemonWall/MovementBehavior")]
    public class DemonWallMovementBehavior : MovementBehaviorBase
    {
      public float speed = 4f;
      public float sinPeriod = 2f;
      public float sinMagnitude = 1f;
      private DemonWallController m_demonWallController;
      private bool m_initialized;
      private float m_startY;
      private float m_startCameraY;
      private float lowestGoalY = float.MaxValue;
      private float m_timer;

      public override void Start()
      {
        base.Start();
        this.m_demonWallController = this.m_aiActor.GetComponent<DemonWallController>();
        this.m_updateEveryFrame = true;
      }

      public override BehaviorResult Update()
      {
        if ((double) this.m_deltaTime > 0.0 && this.m_demonWallController.IsCameraLocked)
        {
          if (!this.m_initialized)
          {
            this.m_startY = this.m_aiActor.specRigidbody.Position.UnitY;
            this.m_startCameraY = this.m_demonWallController.CameraPos.y;
            this.m_initialized = true;
          }
          this.m_timer += this.m_deltaTime;
          float num1 = this.m_startY - this.m_timer * this.speed;
          float y = this.m_startCameraY - this.m_timer * this.speed;
          float num2 = Mathf.Min(this.lowestGoalY, num1 + Mathf.Sin((float) ((double) this.m_timer / (double) this.sinPeriod * 3.1415927410125732)) * this.sinMagnitude);
          this.lowestGoalY = num2;
          this.m_aiActor.BehaviorOverridesVelocity = true;
          if ((double) this.m_deltaTime > 0.0)
            this.m_aiActor.BehaviorVelocity = new Vector2(0.0f, (num2 - this.m_aiActor.specRigidbody.Position.UnitY) / this.m_deltaTime);
          this.m_aiActor.specRigidbody.Velocity = this.m_aiActor.BehaviorVelocity;
          CameraController cameraController = GameManager.Instance.MainCameraController;
          Vector2 vector2 = this.m_demonWallController.CameraPos.WithY(y);
          float b = (float) this.m_aiActor.ParentRoom.area.basePosition.y + cameraController.Camera.orthographicSize;
          vector2.y = Mathf.Max(vector2.y, b);
          cameraController.OverridePosition = (Vector3) vector2;
        }
        return BehaviorResult.Continue;
      }
    }

}
