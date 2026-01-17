// Decompiled with JetBrains decompiler
// Type: PoisonTrailChallengeModifier
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Data
{
    public class PoisonTrailChallengeModifier : ChallengeModifier
    {
      public GoopDefinition Goop;
      public float GoopRadius = 1f;
      public float DampSmoothTime = 0.25f;
      private float MaxSmoothSpeed = 20f;
      private Vector2[] m_goopVelocities;
      private Vector2[] m_goopPoints;

      private void Update()
      {
        if ((double) BraveTime.DeltaTime <= 0.0)
          return;
        if (this.m_goopPoints == null || this.m_goopPoints.Length != GameManager.Instance.AllPlayers.Length)
          this.InitializePoints();
        for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
        {
          this.m_goopPoints[index] = Vector2.SmoothDamp(this.m_goopPoints[index], GameManager.Instance.AllPlayers[index].specRigidbody.UnitCenter, ref this.m_goopVelocities[index], this.DampSmoothTime, this.MaxSmoothSpeed, UnityEngine.Time.deltaTime);
          if (!GameManager.Instance.AllPlayers[index].IsGhost)
            this.DoGoop(this.m_goopPoints[index]);
        }
      }

      private void InitializePoints()
      {
        this.m_goopPoints = new Vector2[GameManager.Instance.AllPlayers.Length];
        this.m_goopVelocities = new Vector2[GameManager.Instance.AllPlayers.Length];
        for (int index = 0; index < this.m_goopPoints.Length; ++index)
          this.m_goopPoints[index] = GameManager.Instance.AllPlayers[index].CenterPosition;
      }

      private void DoGoop(Vector2 position)
      {
        if (BossKillCam.BossDeathCamRunning)
          return;
        DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(this.Goop).AddGoopCircle(position, this.GoopRadius);
      }
    }

}
