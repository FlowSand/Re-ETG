// Decompiled with JetBrains decompiler
// Type: BooRoomChallengeModifier
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System.Collections.Generic;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Data
{
    public class BooRoomChallengeModifier : ChallengeModifier
    {
      public float ConeAngle = 45f;
      public Shader DarknessEffectShader;
      private Material m_material;

      private void Start()
      {
        if (!((Object) Pixelator.Instance.AdditionalCoreStackRenderPass == (Object) null))
          return;
        this.m_material = new Material(this.DarknessEffectShader);
        Pixelator.Instance.AdditionalCoreStackRenderPass = this.m_material;
      }

      private Vector4 GetCenterPointInScreenUV(Vector2 centerPoint)
      {
        Vector3 viewportPoint = GameManager.Instance.MainCameraController.Camera.WorldToViewportPoint(centerPoint.ToVector3ZUp());
        return new Vector4(viewportPoint.x, viewportPoint.y, 0.0f, 0.0f);
      }

      private void LateUpdate()
      {
        if (!((Object) this.m_material != (Object) null))
          return;
        float facingDirection1 = GameManager.Instance.PrimaryPlayer.FacingDirection;
        if ((double) facingDirection1 > 270.0)
          facingDirection1 -= 360f;
        if ((double) facingDirection1 < -270.0)
          facingDirection1 += 360f;
        this.m_material.SetFloat("_ConeAngle", this.ConeAngle);
        Vector4 centerPointInScreenUv = this.GetCenterPointInScreenUV(GameManager.Instance.PrimaryPlayer.CenterPosition) with
        {
          z = facingDirection1
        };
        Vector4 vector4 = centerPointInScreenUv;
        if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER)
        {
          float facingDirection2 = GameManager.Instance.SecondaryPlayer.FacingDirection;
          if ((double) facingDirection2 > 270.0)
            facingDirection2 -= 360f;
          if ((double) facingDirection2 < -270.0)
            facingDirection2 += 360f;
          vector4 = this.GetCenterPointInScreenUV(GameManager.Instance.SecondaryPlayer.CenterPosition) with
          {
            z = facingDirection2
          };
        }
        this.m_material.SetVector("_Player1ScreenPosition", centerPointInScreenUv);
        this.m_material.SetVector("_Player2ScreenPosition", vector4);
      }

      private void Update()
      {
        List<AIActor> activeEnemies = GameManager.Instance.PrimaryPlayer.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
        Vector2 zero1 = Vector2.zero;
        Vector2 zero2 = Vector2.zero;
        float a1 = !(bool) (Object) GameManager.Instance.PrimaryPlayer.CurrentGun || GameManager.Instance.PrimaryPlayer.IsGhost ? BraveMathCollege.Atan2Degrees(GameManager.Instance.PrimaryPlayer.unadjustedAimPoint.XY()) : GameManager.Instance.PrimaryPlayer.CurrentGun.CurrentAngle;
        Vector2 centerPosition1 = GameManager.Instance.PrimaryPlayer.CenterPosition;
        float a2;
        Vector2 vector2;
        if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER)
        {
          a2 = !(bool) (Object) GameManager.Instance.SecondaryPlayer.CurrentGun || GameManager.Instance.SecondaryPlayer.IsGhost ? BraveMathCollege.Atan2Degrees(GameManager.Instance.SecondaryPlayer.unadjustedAimPoint.XY()) : GameManager.Instance.SecondaryPlayer.CurrentGun.CurrentAngle;
          vector2 = GameManager.Instance.SecondaryPlayer.CenterPosition;
        }
        else
        {
          vector2 = centerPosition1;
          a2 = a1;
        }
        for (int index = 0; index < activeEnemies.Count; ++index)
        {
          AIActor aiActor = activeEnemies[index];
          if ((bool) (Object) aiActor && (bool) (Object) aiActor.healthHaver && aiActor.IsNormalEnemy && !aiActor.healthHaver.IsBoss && !aiActor.healthHaver.IsDead)
          {
            Vector2 centerPosition2 = aiActor.CenterPosition;
            float b1 = BraveMathCollege.Atan2Degrees(centerPosition2 - centerPosition1);
            float b2 = BraveMathCollege.Atan2Degrees(centerPosition2 - vector2);
            if ((double) BraveMathCollege.AbsAngleBetween(a1, b1) < (double) this.ConeAngle || (double) BraveMathCollege.AbsAngleBetween(a2, b2) < (double) this.ConeAngle)
            {
              if ((bool) (Object) aiActor.behaviorSpeculator)
                aiActor.behaviorSpeculator.Stun(0.25f);
              if (aiActor.IsBlackPhantom)
                aiActor.UnbecomeBlackPhantom();
            }
            else if (!aiActor.IsBlackPhantom)
              aiActor.BecomeBlackPhantom();
          }
        }
      }

      private void OnDestroy()
      {
        if (!((Object) this.m_material != (Object) null) || !(bool) (Object) Pixelator.Instance)
          return;
        Pixelator.Instance.AdditionalCoreStackRenderPass = (Material) null;
      }
    }

}
