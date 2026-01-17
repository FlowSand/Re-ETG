// Decompiled with JetBrains decompiler
// Type: TorchController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using UnityEngine;

#nullable disable

namespace ETG.Core.Core.Framework
{
    public class TorchController : BraveBehaviour
    {
      [Header("VFX")]
      public VFXPool sparkVfx;
      public VFXPool douseVfx;
      public Transform douseOffset;
      [Header("Animations")]
      public tk2dSpriteAnimator flameAnimator;
      public string flameAnim;
      public string dousedAnim;
      public Renderer[] renderers;
      [Header("Other")]
      public bool canBeRelit = true;
      public bool anyProjectileBreaks;
      public bool disappearAfterDouse;
      public bool igniteGoop = true;
      [ShowInInspectorIf("igniteGoop", false)]
      public float igniteRadius = 0.5f;
      public bool deployGoop;
      [ShowInInspectorIf("deployGoop", false)]
      public GoopDefinition goopToDeploy;
      [ShowInInspectorIf("deployGoop", false)]
      public float goopRadius = 3f;
      public bool createsShardsOnDouse;
      public ShardCluster[] douseShards;
      private bool m_isLit = true;

      public CellData Cell { get; set; }

      public void Start()
      {
        if ((bool) (Object) this.specRigidbody)
          this.specRigidbody.OnEnterTrigger += new SpeculativeRigidbody.OnTriggerDelegate(this.OnEnterTrigger);
        if (this.sprite.FlipX)
          this.douseOffset.transform.localPosition = this.douseOffset.transform.localPosition.Scale(-1f, 1f, 1f);
        if (!(bool) (Object) this.specRigidbody || !this.transform.position.GetAbsoluteRoom().IsWinchesterArcadeRoom)
          return;
        this.specRigidbody.enabled = false;
      }

      protected override void OnDestroy()
      {
        if ((Object) this.specRigidbody != (Object) null)
          this.specRigidbody.OnEnterTrigger += new SpeculativeRigidbody.OnTriggerDelegate(this.OnEnterTrigger);
        base.OnDestroy();
      }

      public void BeamCollision(Projectile p) => this.AnyCollision(p);

      private void AnyCollision(Projectile p)
      {
        if (this.m_isLit && ((p.damageTypes & CoreDamageTypes.Water) == CoreDamageTypes.Water || this.anyProjectileBreaks))
        {
          this.m_isLit = false;
          for (int index = 0; index < this.renderers.Length; ++index)
          {
            this.renderers[index].enabled = false;
            this.visibilityManager.AddIgnoredRenderer(this.renderers[index]);
          }
          if (this.Cell != null && (Object) this.Cell.cellVisualData.lightObject != (Object) null)
            this.Cell.cellVisualData.lightObject.SetActive(false);
          Vector3 vector3_1 = !(bool) (Object) this.douseOffset ? (Vector3) this.specRigidbody.UnitCenter : this.douseOffset.transform.position;
          VFXPool douseVfx = this.douseVfx;
          Vector3 vector3_2 = vector3_1;
          tk2dBaseSprite sprite = this.sprite;
          Vector3 position = vector3_2;
          Vector2? sourceNormal = new Vector2?();
          Vector2? sourceVelocity = new Vector2?();
          float? heightOffGround = new float?();
          tk2dBaseSprite spriteParent = sprite;
          douseVfx.SpawnAtPosition(position, sourceNormal: sourceNormal, sourceVelocity: sourceVelocity, heightOffGround: heightOffGround, keepReferences: true, spriteParent: spriteParent);
          if (this.createsShardsOnDouse)
          {
            for (int index = 0; index < this.douseShards.Length; ++index)
              this.douseShards[index].SpawnShards((Vector2) (this.douseOffset.position + (p.LastVelocity * -1f).normalized.ToVector3ZUp()), p.LastVelocity * -1f, -30f, 30f, 0.0f, 0.5f, 1f, (tk2dSprite) null);
          }
          if (this.disappearAfterDouse)
          {
            if (!this.canBeRelit)
            {
              if (!string.IsNullOrEmpty(this.dousedAnim))
                this.flameAnimator.PlayAndDestroyObject(this.dousedAnim);
              else
                Object.Destroy((Object) this.flameAnimator.gameObject);
            }
            else if (!string.IsNullOrEmpty(this.dousedAnim))
              this.flameAnimator.PlayAndDisableRenderer(this.dousedAnim);
            else
              this.flameAnimator.GetComponent<Renderer>().enabled = false;
          }
          else
          {
            if (string.IsNullOrEmpty(this.dousedAnim))
              return;
            this.flameAnimator.Play(this.dousedAnim);
          }
        }
        else
        {
          if (this.m_isLit || !this.canBeRelit || (p.damageTypes & CoreDamageTypes.Fire) != CoreDamageTypes.Fire)
            return;
          this.m_isLit = true;
          for (int index = 0; index < this.renderers.Length; ++index)
          {
            this.renderers[index].enabled = true;
            this.visibilityManager.RemoveIgnoredRenderer(this.renderers[index]);
          }
          if (this.Cell != null && (Object) this.Cell.cellVisualData.lightObject != (Object) null)
            this.Cell.cellVisualData.lightObject.SetActive(true);
          this.douseVfx.DestroyAll();
          this.flameAnimator.GetComponent<Renderer>().enabled = true;
          this.flameAnimator.Play(this.flameAnim);
        }
      }

      private void OnEnterTrigger(
        SpeculativeRigidbody mySpecRigidbody,
        SpeculativeRigidbody sourceSpecRigidbody,
        CollisionData collisionData)
      {
        if (!(bool) (Object) collisionData.OtherRigidbody.projectile)
          return;
        if (this.m_isLit)
        {
          this.sparkVfx.SpawnAtPosition((Vector3) this.specRigidbody.UnitCenter);
          if (this.deployGoop)
            DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(this.goopToDeploy).TimedAddGoopCircle(this.specRigidbody.UnitBottomCenter, this.goopRadius);
          if (this.igniteGoop)
          {
            for (int index = 0; index < StaticReferenceManager.AllGoops.Count; ++index)
            {
              Vector2 center = new Vector2(this.specRigidbody.UnitCenter.x, this.transform.position.y);
              StaticReferenceManager.AllGoops[index].IgniteGoopCircle(center, this.igniteRadius);
            }
          }
        }
        this.AnyCollision(collisionData.OtherRigidbody.projectile);
      }
    }

}
