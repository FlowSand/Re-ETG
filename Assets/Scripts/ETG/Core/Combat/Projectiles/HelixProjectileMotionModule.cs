// Decompiled with JetBrains decompiler
// Type: HelixProjectileMotionModule
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

namespace ETG.Core.Combat.Projectiles
{
    public class HelixProjectileMotionModule : ProjectileAndBeamMotionModule
    {
      public float helixWavelength = 3f;
      public float helixAmplitude = 1f;
      public float helixBeamOffsetPerSecond = 6f;
      public bool ForceInvert;
      private bool m_helixInitialized;
      private Vector2 m_initialRightVector;
      private Vector2 m_initialUpVector;
      private Vector2 m_privateLastPosition;
      private float m_displacement;
      private float m_yDisplacement;

      public override void UpdateDataOnBounce(float angleDiff)
      {
        if (float.IsNaN(angleDiff))
          return;
        this.m_initialUpVector = (Vector2) (Quaternion.Euler(0.0f, 0.0f, angleDiff) * (Vector3) this.m_initialUpVector);
        this.m_initialRightVector = (Vector2) (Quaternion.Euler(0.0f, 0.0f, angleDiff) * (Vector3) this.m_initialRightVector);
      }

      public override void AdjustRightVector(float angleDiff)
      {
        if (float.IsNaN(angleDiff))
          return;
        this.m_initialUpVector = (Vector2) (Quaternion.Euler(0.0f, 0.0f, angleDiff) * (Vector3) this.m_initialUpVector);
        this.m_initialRightVector = (Vector2) (Quaternion.Euler(0.0f, 0.0f, angleDiff) * (Vector3) this.m_initialRightVector);
      }

      public override void Move(
        Projectile source,
        Transform projectileTransform,
        tk2dBaseSprite projectileSprite,
        SpeculativeRigidbody specRigidbody,
        ref float m_timeElapsed,
        ref Vector2 m_currentDirection,
        bool Inverted,
        bool shouldRotate)
      {
        ProjectileData baseData = source.baseData;
        Vector2 vector2_1 = !(bool) (Object) projectileSprite ? projectileTransform.position.XY() : projectileSprite.WorldCenter;
        if (!this.m_helixInitialized)
        {
          this.m_helixInitialized = true;
          this.m_initialRightVector = !shouldRotate ? m_currentDirection : projectileTransform.right.XY();
          this.m_initialUpVector = (Vector2) (!shouldRotate ? Quaternion.Euler(0.0f, 0.0f, 90f) * (Vector3) m_currentDirection : projectileTransform.up);
          this.m_privateLastPosition = vector2_1;
          this.m_displacement = 0.0f;
          this.m_yDisplacement = 0.0f;
        }
        m_timeElapsed += BraveTime.DeltaTime;
        int num1 = !(Inverted ^ this.ForceInvert) ? 1 : -1;
        float num2 = m_timeElapsed * baseData.speed;
        float num3 = (float) num1 * this.helixAmplitude * Mathf.Sin(m_timeElapsed * 3.14159274f * baseData.speed / this.helixWavelength);
        Vector2 vector2_2 = this.m_privateLastPosition + this.m_initialRightVector * (num2 - this.m_displacement) + this.m_initialUpVector * (num3 - this.m_yDisplacement);
        this.m_privateLastPosition = vector2_2;
        if (shouldRotate)
        {
          float num4 = (m_timeElapsed + 0.01f) * baseData.speed;
          float num5 = BraveMathCollege.Atan2Degrees((float) num1 * this.helixAmplitude * Mathf.Sin((float) (((double) m_timeElapsed + 0.0099999997764825821) * 3.1415927410125732) * baseData.speed / this.helixWavelength) - num3, num4 - num2);
          projectileTransform.localRotation = Quaternion.Euler(0.0f, 0.0f, num5 + this.m_initialRightVector.ToAngle());
        }
        Vector2 v = (vector2_2 - vector2_1) / BraveTime.DeltaTime;
        if (!float.IsNaN(BraveMathCollege.Atan2Degrees(v)))
          m_currentDirection = v.normalized;
        this.m_displacement = num2;
        this.m_yDisplacement = num3;
        specRigidbody.Velocity = v;
      }

      public override void SentInDirection(
        ProjectileData baseData,
        Transform projectileTransform,
        tk2dBaseSprite projectileSprite,
        SpeculativeRigidbody specRigidbody,
        ref float m_timeElapsed,
        ref Vector2 m_currentDirection,
        bool shouldRotate,
        Vector2 dirVec,
        bool resetDistance,
        bool updateRotation)
      {
        Vector2 vector2 = !(bool) (Object) projectileSprite ? projectileTransform.position.XY() : projectileSprite.WorldCenter;
        this.m_helixInitialized = true;
        this.m_initialRightVector = !shouldRotate ? m_currentDirection : projectileTransform.right.XY();
        this.m_initialUpVector = (Vector2) (!shouldRotate ? Quaternion.Euler(0.0f, 0.0f, 90f) * (Vector3) m_currentDirection : projectileTransform.up);
        this.m_privateLastPosition = vector2;
        this.m_displacement = 0.0f;
        this.m_yDisplacement = 0.0f;
        m_timeElapsed = 0.0f;
      }

      public override Vector2 GetBoneOffset(
        BasicBeamController.BeamBone bone,
        BeamController sourceBeam,
        bool inverted)
      {
        float to = (!(inverted ^ this.ForceInvert) ? 1f : -1f) * this.helixAmplitude * Mathf.Sin((bone.PosX - this.helixBeamOffsetPerSecond * (UnityEngine.Time.timeSinceLevelLoad % 600000f)) * 3.14159274f / this.helixWavelength);
        return BraveMathCollege.DegreesToVector(bone.RotationAngle + 90f, Mathf.SmoothStep(0.0f, to, bone.PosX));
      }
    }

}
