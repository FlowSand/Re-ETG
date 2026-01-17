// Decompiled with JetBrains decompiler
// Type: DirectionalAttackActiveItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Items.Active
{
    public class DirectionalAttackActiveItem : PlayerItem
    {
      public float initialWidth = 3f;
      public float finalWidth = 3f;
      public float startDistance = 1f;
      public float attackLength = 10f;
      public GameObject reticleQuad;
      public bool doesGoop;
      public GoopDefinition goopDefinition;
      public bool doesBarrage;
      public int BarrageColumns = 1;
      public GameObject barrageVFX;
      public ExplosionData barrageExplosionData;
      public float barrageRadius = 1.5f;
      public float delayBetweenStrikes = 0.25f;
      public bool SkipTargetingStep;
      public string AudioEvent;
      private PlayerController m_currentUser;
      private tk2dSlicedSprite m_extantReticleQuad;
      private bool m_airstrikeSynergyProcessed;
      private bool m_isDoingBarrage;

      public override void Update()
      {
        base.Update();
        if ((bool) (Object) this.m_currentUser && this.m_currentUser.HasActiveBonusSynergy(CustomSynergyType.EXPLOSIVE_AIRSTRIKE) && this.itemName == "Airstrike" && !this.m_airstrikeSynergyProcessed)
        {
          this.m_airstrikeSynergyProcessed = true;
          this.BarrageColumns = 3;
          this.initialWidth *= 3f;
          this.finalWidth *= 3f;
        }
        else if ((bool) (Object) this.m_currentUser && !this.m_currentUser.HasActiveBonusSynergy(CustomSynergyType.EXPLOSIVE_AIRSTRIKE) && this.m_airstrikeSynergyProcessed)
        {
          this.m_airstrikeSynergyProcessed = false;
          this.BarrageColumns = 1;
          this.initialWidth /= 3f;
          this.finalWidth /= 3f;
        }
        if (!this.IsCurrentlyActive || !(bool) (Object) this.m_extantReticleQuad)
          return;
        Vector2 centerPosition = this.m_currentUser.CenterPosition;
        Vector2 normalized = (this.m_currentUser.unadjustedAimPoint.XY() - centerPosition).normalized;
        this.m_extantReticleQuad.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, BraveMathCollege.Atan2Degrees(normalized));
        this.m_extantReticleQuad.transform.position = (Vector3) (centerPosition + normalized * this.startDistance + (Quaternion.Euler(0.0f, 0.0f, -90f) * (Vector3) normalized * (this.initialWidth / 2f)).XY());
      }

      protected override void OnPreDrop(PlayerController user)
      {
        base.OnPreDrop(user);
        if (!(bool) (Object) this.m_extantReticleQuad)
          return;
        Object.Destroy((Object) this.m_extantReticleQuad.gameObject);
      }

      protected override void DoEffect(PlayerController user)
      {
        this.IsCurrentlyActive = true;
        this.m_currentUser = user;
        Vector2 centerPosition = user.CenterPosition;
        Vector2 normalized = (user.unadjustedAimPoint.XY() - centerPosition).normalized;
        if ((bool) (Object) this.m_currentUser && this.m_currentUser.HasActiveBonusSynergy(CustomSynergyType.EXPLOSIVE_AIRSTRIKE) && this.itemName == "Airstrike" && !this.m_airstrikeSynergyProcessed)
        {
          this.m_airstrikeSynergyProcessed = true;
          this.BarrageColumns = 3;
          this.initialWidth *= 3f;
          this.finalWidth *= 3f;
        }
        else if ((bool) (Object) this.m_currentUser && !this.m_currentUser.HasActiveBonusSynergy(CustomSynergyType.EXPLOSIVE_AIRSTRIKE) && this.m_airstrikeSynergyProcessed)
        {
          this.m_airstrikeSynergyProcessed = false;
          this.BarrageColumns = 1;
          this.initialWidth /= 3f;
          this.finalWidth /= 3f;
        }
        if (this.SkipTargetingStep)
        {
          this.DoActiveEffect(user);
        }
        else
        {
          this.m_extantReticleQuad = Object.Instantiate<GameObject>(this.reticleQuad).GetComponent<tk2dSlicedSprite>();
          this.m_extantReticleQuad.dimensions = new Vector2(this.attackLength * 16f, this.initialWidth * 16f);
          if (normalized != Vector2.zero)
            this.m_extantReticleQuad.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, BraveMathCollege.Atan2Degrees(normalized));
          this.m_extantReticleQuad.transform.position = (Vector3) (centerPosition + normalized * this.startDistance + (Quaternion.Euler(0.0f, 0.0f, -90f) * (Vector3) normalized * (this.initialWidth / 2f)).XY());
        }
      }

      protected override void DoActiveEffect(PlayerController user)
      {
        if (this.m_isDoingBarrage)
          return;
        if ((bool) (Object) this.m_extantReticleQuad)
          Object.Destroy((Object) this.m_extantReticleQuad.gameObject);
        Vector2 centerPosition = user.CenterPosition;
        Vector2 normalized = (user.unadjustedAimPoint.XY() - centerPosition).normalized;
        Vector2 startPoint = centerPosition + normalized * this.startDistance;
        if (this.doesGoop)
          this.HandleEngoopening(startPoint, normalized);
        if (this.doesBarrage)
        {
          List<Vector2> targets = this.AcquireBarrageTargets(startPoint, normalized);
          user.StartCoroutine(this.HandleBarrage(targets));
        }
        else
          this.IsCurrentlyActive = false;
        if (string.IsNullOrEmpty(this.AudioEvent))
          return;
        int num = (int) AkSoundEngine.PostEvent(this.AudioEvent, this.gameObject);
      }

      protected void HandleEngoopening(Vector2 startPoint, Vector2 direction)
      {
        float duration = 1f;
        DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(this.goopDefinition).TimedAddGoopLine(startPoint, startPoint + direction * this.attackLength, this.barrageRadius, duration);
      }

      [DebuggerHidden]
      private IEnumerator HandleBarrage(List<Vector2> targets)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new DirectionalAttackActiveItem__HandleBarragec__Iterator0()
        {
          targets = targets,
          _this = this
        };
      }

      protected List<Vector2> AcquireBarrageTargets(Vector2 startPoint, Vector2 direction)
      {
        List<Vector2> vector2List = new List<Vector2>();
        float num1 = (float) (-(double) this.barrageRadius / 2.0);
        Quaternion quaternion = Quaternion.Euler(0.0f, 0.0f, BraveMathCollege.Atan2Degrees(direction));
        for (; (double) num1 < (double) this.attackLength; num1 += this.barrageRadius)
        {
          float b = Mathf.Lerp(this.initialWidth, this.finalWidth, Mathf.Clamp01(num1 / this.attackLength));
          float x = Mathf.Clamp(num1, 0.0f, this.attackLength);
          for (int index = 0; index < this.BarrageColumns; ++index)
          {
            float num2 = Mathf.Lerp(-b, b, (float) (((double) index + 1.0) / ((double) this.BarrageColumns + 1.0)));
            float num3 = Random.Range((float) (-(double) b / (4.0 * (double) this.BarrageColumns)), b / (4f * (float) this.BarrageColumns));
            Vector2 vector2_1 = new Vector2(x, num2 + num3);
            Vector2 vector2_2 = (quaternion * (Vector3) vector2_1).XY();
            vector2List.Add(startPoint + vector2_2);
          }
        }
        return vector2List;
      }

      protected override void OnDestroy()
      {
        if ((bool) (Object) this.m_extantReticleQuad)
          Object.Destroy((Object) this.m_extantReticleQuad.gameObject);
        base.OnDestroy();
      }
    }

}
