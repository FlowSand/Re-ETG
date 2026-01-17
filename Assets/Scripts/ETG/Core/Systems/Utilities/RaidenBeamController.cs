// Decompiled with JetBrains decompiler
// Type: RaidenBeamController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    public class RaidenBeamController : BeamController
    {
      [FormerlySerializedAs("animation")]
      public string beamAnimation;
      public bool usesStartAnimation;
      public string startAnimation;
      public tk2dBaseSprite ImpactRenderer;
      [CheckAnimation(null)]
      public string EnemyImpactAnimation;
      [CheckAnimation(null)]
      public string BossImpactAnimation;
      [CheckAnimation(null)]
      public string OtherImpactAnimation;
      public RaidenBeamController.TargetType targetType = RaidenBeamController.TargetType.Screen;
      public int maxTargets = -1;
      public float endRampHeight;
      public int endRampSteps;
      [HideInInspector]
      public bool FlipUvsY;
      [HideInInspector]
      public bool SelectRandomTarget;
      private List<AIActor> s_enemiesInRoom = new List<AIActor>();
      private tk2dTiledSprite m_sprite;
      private tk2dSpriteAnimationClip m_startAnimationClip;
      private tk2dSpriteAnimationClip m_animationClip;
      private bool m_isCurrentlyFiring = true;
      private bool m_audioPlaying;
      private List<AIActor> m_targets = new List<AIActor>();
      private SpeculativeRigidbody m_hitRigidbody;
      private int m_spriteSubtileWidth;
      private LinkedList<RaidenBeamController.Bone> m_bones = new LinkedList<RaidenBeamController.Bone>();
      private Vector2 m_minBonePosition;
      private Vector2 m_maxBonePosition;
      private bool m_isDirty;
      private float m_globalTimer;
      private const int c_segmentCount = 20;
      private const int c_bonePixelLength = 4;
      private const float c_boneUnitLength = 0.25f;
      private const float c_trailHeightOffset = 0.5f;
      private float m_projectileScale = 1f;

      public override bool ShouldUseAmmo => true;

      public void Start()
      {
        this.transform.parent = SpawnManager.Instance.VFX;
        this.transform.rotation = Quaternion.identity;
        this.transform.position = Vector3.zero;
        this.m_sprite = this.GetComponent<tk2dTiledSprite>();
        this.m_sprite.OverrideGetTiledSpriteGeomDesc = new tk2dTiledSprite.OverrideGetTiledSpriteGeomDescDelegate(this.GetTiledSpriteGeomDesc);
        this.m_sprite.OverrideSetTiledSpriteGeom = new tk2dTiledSprite.OverrideSetTiledSpriteGeomDelegate(this.SetTiledSpriteGeom);
        tk2dSpriteDefinition currentSpriteDef = this.m_sprite.GetCurrentSpriteDef();
        this.m_spriteSubtileWidth = Mathf.RoundToInt(currentSpriteDef.untrimmedBoundsDataExtents.x / currentSpriteDef.texelSize.x) / 4;
        if (this.usesStartAnimation)
          this.m_startAnimationClip = this.spriteAnimator.GetClipByName(this.startAnimation);
        this.m_animationClip = this.spriteAnimator.GetClipByName(this.beamAnimation);
        PlayerController owner = this.projectile.Owner as PlayerController;
        if ((bool) (UnityEngine.Object) owner)
          this.m_projectileScale = owner.BulletScaleModifier;
        if (!(bool) (UnityEngine.Object) this.ImpactRenderer)
          return;
        this.ImpactRenderer.transform.localScale = new Vector3(this.m_projectileScale, this.m_projectileScale, 1f);
      }

      public void Update()
      {
        this.m_globalTimer += BraveTime.DeltaTime;
        for (int index = this.m_targets.Count - 1; index >= 0; --index)
        {
          AIActor target = this.m_targets[index];
          if (!(bool) (UnityEngine.Object) target || !(bool) (UnityEngine.Object) target.healthHaver || target.healthHaver.IsDead)
            this.m_targets.RemoveAt(index);
        }
        this.m_hitRigidbody = (SpeculativeRigidbody) null;
        this.HandleBeamFrame(this.Origin, this.Direction, this.m_isCurrentlyFiring);
        if (this.m_targets == null || this.m_targets.Count == 0)
        {
          if (GameManager.AUDIO_ENABLED && this.m_audioPlaying)
          {
            this.m_audioPlaying = false;
            int num = (int) AkSoundEngine.PostEvent("Stop_WPN_loop_01", this.gameObject);
          }
        }
        else if (GameManager.AUDIO_ENABLED && !this.m_audioPlaying)
        {
          this.m_audioPlaying = true;
          int num = (int) AkSoundEngine.PostEvent("Play_WPN_shot_01", this.gameObject);
        }
        float num1 = this.projectile.baseData.damage + this.DamageModifier;
        PlayerController owner = this.projectile.Owner as PlayerController;
        if ((bool) (UnityEngine.Object) owner)
          num1 = num1 * owner.stats.GetStatValue(PlayerStats.StatType.RateOfFire) * owner.stats.GetStatValue(PlayerStats.StatType.Damage);
        if (this.ChanceBasedShadowBullet)
          num1 *= 2f;
        string name = this.OtherImpactAnimation;
        if (this.m_targets != null && this.m_targets.Count > 0)
        {
          foreach (AIActor target in this.m_targets)
          {
            if ((bool) (UnityEngine.Object) target && (bool) (UnityEngine.Object) target.healthHaver)
            {
              name = string.IsNullOrEmpty(this.BossImpactAnimation) || !target.healthHaver.IsBoss ? this.EnemyImpactAnimation : this.BossImpactAnimation;
              if (target.healthHaver.IsBoss && (bool) (UnityEngine.Object) this.projectile)
                num1 *= this.projectile.BossDamageMultiplier;
              if ((bool) (UnityEngine.Object) this.projectile && (double) this.projectile.BlackPhantomDamageMultiplier != 1.0 && target.IsBlackPhantom)
                num1 *= this.projectile.BlackPhantomDamageMultiplier;
              target.healthHaver.ApplyDamage(num1 * BraveTime.DeltaTime, Vector2.zero, this.Owner.ActorName);
            }
          }
        }
        if ((bool) (UnityEngine.Object) this.m_hitRigidbody)
        {
          if ((bool) (UnityEngine.Object) this.m_hitRigidbody.minorBreakable)
            this.m_hitRigidbody.minorBreakable.Break(this.Direction);
          if ((bool) (UnityEngine.Object) this.m_hitRigidbody.majorBreakable)
            this.m_hitRigidbody.majorBreakable.ApplyDamage(num1 * BraveTime.DeltaTime, this.Direction, false);
        }
        if (!(bool) (UnityEngine.Object) this.ImpactRenderer || !(bool) (UnityEngine.Object) this.ImpactRenderer.spriteAnimator || string.IsNullOrEmpty(name))
          return;
        this.ImpactRenderer.spriteAnimator.Play(name);
      }

      public void LateUpdate()
      {
        if (!this.m_isDirty)
          return;
        this.m_minBonePosition = new Vector2(float.MaxValue, float.MaxValue);
        this.m_maxBonePosition = new Vector2(float.MinValue, float.MinValue);
        for (LinkedListNode<RaidenBeamController.Bone> linkedListNode = this.m_bones.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
        {
          this.m_minBonePosition = Vector2.Min(this.m_minBonePosition, linkedListNode.Value.pos);
          this.m_maxBonePosition = Vector2.Max(this.m_maxBonePosition, linkedListNode.Value.pos);
        }
        Vector2 vector = new Vector2(this.m_minBonePosition.x, this.m_minBonePosition.y) - this.transform.position.XY();
        this.transform.position = new Vector3(this.m_minBonePosition.x, this.m_minBonePosition.y);
        this.m_sprite.HeightOffGround = 0.5f;
        this.ImpactRenderer.transform.position -= vector.ToVector3ZUp();
        this.m_sprite.ForceBuild();
        this.m_sprite.UpdateZDepth();
      }

      protected override void OnDestroy() => base.OnDestroy();

      public void HandleBeamFrame(Vector2 barrelPosition, Vector2 direction, bool isCurrentlyFiring)
      {
        if (this.Owner is PlayerController)
          this.HandleChanceTick();
        if (this.targetType == RaidenBeamController.TargetType.Screen)
        {
          this.m_targets.Clear();
          List<AIActor> allEnemies = StaticReferenceManager.AllEnemies;
          for (int index = 0; index < allEnemies.Count; ++index)
          {
            AIActor aiActor = allEnemies[index];
            if (aiActor.IsNormalEnemy && aiActor.renderer.isVisible && aiActor.healthHaver.IsAlive && !aiActor.IsGone)
              this.m_targets.Add(aiActor);
            if (this.maxTargets > 0 && this.m_targets.Count >= this.maxTargets)
              break;
          }
        }
        else if (this.maxTargets <= 0 || this.m_targets.Count < this.maxTargets)
        {
          GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(barrelPosition.ToIntVector2(VectorConversions.Floor)).GetActiveEnemies(RoomHandler.ActiveEnemyType.All, ref this.s_enemiesInRoom);
          if (this.SelectRandomTarget)
            this.s_enemiesInRoom = this.s_enemiesInRoom.Shuffle<AIActor>();
          else
            this.s_enemiesInRoom.Sort((Comparison<AIActor>) ((a, b) => Vector2.Distance(barrelPosition, a.CenterPosition).CompareTo(Vector2.Distance(barrelPosition, b.CenterPosition))));
          for (int index = 0; index < this.s_enemiesInRoom.Count; ++index)
          {
            AIActor aiActor = this.s_enemiesInRoom[index];
            if (aiActor.IsNormalEnemy && aiActor.renderer.isVisible && aiActor.healthHaver.IsAlive && !aiActor.IsGone)
              this.m_targets.Add(aiActor);
            if (this.maxTargets > 0 && this.m_targets.Count >= this.maxTargets)
              break;
          }
        }
        this.m_bones.Clear();
        Vector3? nullable1 = new Vector3?();
        if (this.m_targets.Count > 0)
        {
          Vector3 vector3_1 = (Vector3) (direction.normalized * 5f);
          Vector3 p0 = (Vector3) barrelPosition;
          Vector3 p1_1 = p0 + vector3_1;
          Vector3 vector3_2 = Quaternion.Euler(0.0f, 0.0f, 180f) * vector3_1;
          Vector3 unitCenter1 = (Vector3) this.m_targets[0].specRigidbody.HitboxPixelCollider.UnitCenter;
          Vector3 p2_1 = unitCenter1 + vector3_2;
          Vector3 vector3_3 = Quaternion.Euler(0.0f, 0.0f, 180f) * vector3_2;
          this.DrawBezierCurve((Vector2) p0, (Vector2) p1_1, (Vector2) p2_1, (Vector2) unitCenter1);
          for (int index = 0; index < this.m_targets.Count - 1; ++index)
          {
            Vector3 unitCenter2 = (Vector3) this.m_targets[index].specRigidbody.HitboxPixelCollider.UnitCenter;
            Vector3 p1_2 = unitCenter2 + vector3_3;
            Vector3 vector3_4 = Quaternion.Euler(0.0f, 0.0f, 90f) * vector3_3;
            Vector3 unitCenter3 = (Vector3) this.m_targets[index + 1].specRigidbody.HitboxPixelCollider.UnitCenter;
            Vector3 p2_2 = unitCenter3 + vector3_4;
            vector3_3 = Quaternion.Euler(0.0f, 0.0f, 180f) * vector3_4;
            this.DrawBezierCurve((Vector2) unitCenter2, (Vector2) p1_2, (Vector2) p2_2, (Vector2) unitCenter3);
          }
          if ((bool) (UnityEngine.Object) this.ImpactRenderer)
            this.ImpactRenderer.renderer.enabled = false;
        }
        else
        {
          Vector3 vector3_5 = Quaternion.Euler(0.0f, 0.0f, Mathf.PingPong(UnityEngine.Time.realtimeSinceStartup * 15f, 60f) - 30f) * (Vector3) direction.normalized * 5f;
          Vector3 p0 = (Vector3) barrelPosition;
          Vector3 p1 = p0 + vector3_5;
          Vector3 vector3_6 = Quaternion.Euler(0.0f, 0.0f, 180f) * vector3_5;
          int num1 = (CollisionLayerMatrix.GetMask(CollisionLayer.Projectile) | CollisionMask.LayerToMask(CollisionLayer.BeamBlocker)) & ~CollisionMask.LayerToMask(CollisionLayer.PlayerCollider, CollisionLayer.PlayerHitBox);
          PhysicsEngine instance = PhysicsEngine.Instance;
          Vector2 vector2_1 = (Vector2) p0;
          Vector2 vector2_2 = direction;
          float num2 = 30f;
          RaycastResult raycastResult;
          ref RaycastResult local1 = ref raycastResult;
          bool flag1 = true;
          bool flag2 = true;
          int num3 = num1;
          CollisionLayer? nullable2 = new CollisionLayer?();
          bool flag3 = false;
          SpeculativeRigidbody[] ignoreRigidbodies = this.GetIgnoreRigidbodies();
          Vector2 unitOrigin = vector2_1;
          Vector2 direction1 = vector2_2;
          double dist = (double) num2;
          ref RaycastResult local2 = ref local1;
          int num4 = flag1 ? 1 : 0;
          int num5 = flag2 ? 1 : 0;
          int rayMask = num3;
          CollisionLayer? sourceLayer = nullable2;
          int num6 = flag3 ? 1 : 0;
          SpeculativeRigidbody[] ignoreList = ignoreRigidbodies;
          bool flag4 = instance.RaycastWithIgnores(unitOrigin, direction1, (float) dist, out local2, num4 != 0, num5 != 0, rayMask, sourceLayer, num6 != 0, ignoreList: (ICollection<SpeculativeRigidbody>) ignoreList);
          Vector3 p3 = p0 + (direction.normalized * 30f).ToVector3ZUp();
          if (flag4)
          {
            p3 = (Vector3) raycastResult.Contact;
            this.m_hitRigidbody = raycastResult.SpeculativeRigidbody;
          }
          RaycastResult.Pool.Free(ref raycastResult);
          nullable1 = new Vector3?(p3);
          Vector3 p2 = p3 + vector3_6;
          Vector3 vector3_7 = Quaternion.Euler(0.0f, 0.0f, 180f) * vector3_6;
          this.DrawBezierCurve((Vector2) p0, (Vector2) p1, (Vector2) p2, (Vector2) p3);
          if ((bool) (UnityEngine.Object) this.ImpactRenderer)
            this.ImpactRenderer.renderer.enabled = false;
        }
        for (LinkedListNode<RaidenBeamController.Bone> linkedListNode = this.m_bones.First; linkedListNode != null && linkedListNode != this.m_bones.Last; linkedListNode = linkedListNode.Next)
          linkedListNode.Value.normal = (Vector2) (Quaternion.Euler(0.0f, 0.0f, 90f) * (Vector3) (linkedListNode.Next.Value.pos - linkedListNode.Value.pos)).normalized;
        if (this.m_bones.Count > 0)
          this.m_bones.Last.Value.normal = this.m_bones.Last.Previous.Value.normal;
        this.m_isDirty = true;
        if (!(bool) (UnityEngine.Object) this.ImpactRenderer)
          return;
        if (this.m_targets.Count == 0)
        {
          this.ImpactRenderer.renderer.enabled = true;
          this.ImpactRenderer.transform.position = (Vector3) (!nullable1.HasValue ? (this.Gun.CurrentOwner as PlayerController).unadjustedAimPoint.XY() : nullable1.Value.XY());
          this.ImpactRenderer.IsPerpendicular = false;
        }
        else
        {
          this.ImpactRenderer.renderer.enabled = true;
          this.ImpactRenderer.transform.position = (Vector3) this.m_targets[this.m_targets.Count - 1].CenterPosition;
          this.ImpactRenderer.IsPerpendicular = true;
        }
        this.ImpactRenderer.HeightOffGround = 6f;
        this.ImpactRenderer.UpdateZDepth();
      }

      public override void LateUpdatePosition(Vector3 origin)
      {
      }

      public override void CeaseAttack() => this.DestroyBeam();

      public override void DestroyBeam() => UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);

      public override void AdjustPlayerBeamTint(Color targetTintColor, int priority, float lerpTime = 0.0f)
      {
      }

      private void DrawBezierCurve(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
      {
        Vector3 a = (Vector3) BraveMathCollege.CalculateBezierPoint(0.0f, p0, p1, p2, p3);
        float num1 = 0.0f;
        for (int index = 1; index <= 20; ++index)
        {
          Vector2 bezierPoint = BraveMathCollege.CalculateBezierPoint((float) index / 20f, p0, p1, p2, p3);
          num1 += Vector2.Distance((Vector2) a, bezierPoint);
          a = (Vector3) bezierPoint;
        }
        float num2 = num1 / 0.25f;
        Vector3 bezierPoint1 = (Vector3) BraveMathCollege.CalculateBezierPoint(0.0f, p0, p1, p2, p3);
        if (this.m_bones.Count == 0)
          this.m_bones.AddLast(new RaidenBeamController.Bone((Vector2) bezierPoint1));
        for (int index = 1; (double) index <= (double) num2; ++index)
          this.m_bones.AddLast(new RaidenBeamController.Bone((Vector2) (Vector3) BraveMathCollege.CalculateBezierPoint((float) index / num2, p0, p1, p2, p3)));
      }

      public void GetTiledSpriteGeomDesc(
        out int numVertices,
        out int numIndices,
        tk2dSpriteDefinition spriteDef,
        Vector2 dimensions)
      {
        int num = Mathf.Max(this.m_bones.Count - 1, 0);
        numVertices = num * 4;
        numIndices = num * 6;
      }

      public float RampHeightOffset { get; set; }

      public void SetTiledSpriteGeom(
        Vector3[] pos,
        Vector2[] uv,
        int offset,
        out Vector3 boundsCenter,
        out Vector3 boundsExtents,
        tk2dSpriteDefinition spriteDef,
        Vector3 scale,
        Vector2 dimensions,
        tk2dBaseSprite.Anchor anchor,
        float colliderOffsetZ,
        float colliderExtentZ)
      {
        int num1 = Mathf.RoundToInt(spriteDef.untrimmedBoundsDataExtents.x / spriteDef.texelSize.x) / 4;
        int num2 = Mathf.Max(this.m_bones.Count - 1, 0);
        int num3 = Mathf.CeilToInt((float) num2 / (float) num1);
        boundsCenter = (Vector3) ((this.m_minBonePosition + this.m_maxBonePosition) / 2f);
        boundsExtents = (Vector3) ((this.m_maxBonePosition - this.m_minBonePosition) / 2f);
        int num4 = 0;
        LinkedListNode<RaidenBeamController.Bone> linkedListNode = this.m_bones.First;
        int num5 = 0;
        for (int index1 = 0; index1 < num3; ++index1)
        {
          int num6 = 0;
          int num7 = num1 - 1;
          if (index1 == num3 - 1 && num2 % num1 != 0)
            num7 = num2 % num1 - 1;
          tk2dSpriteDefinition spriteDefinition = !this.usesStartAnimation || index1 != 0 ? this.m_sprite.Collection.spriteDefinitions[this.m_animationClip.frames[Mathf.FloorToInt(Mathf.Repeat(this.m_globalTimer * this.m_animationClip.fps, (float) this.m_animationClip.frames.Length))].spriteId] : this.m_sprite.Collection.spriteDefinitions[this.m_startAnimationClip.frames[Mathf.FloorToInt(Mathf.Repeat(this.m_globalTimer * this.m_startAnimationClip.fps, (float) this.m_startAnimationClip.frames.Length))].spriteId];
          float t = 0.0f;
          for (int index2 = num6; index2 <= num7; ++index2)
          {
            float num8 = 1f;
            if (index1 == num3 - 1 && index2 == num7)
              num8 = Vector2.Distance(linkedListNode.Next.Value.pos, linkedListNode.Value.pos);
            float num9 = 0.0f;
            if ((double) this.endRampHeight == 0.0)
              ;
            int num10 = offset + num5;
            Vector3[] vector3Array1 = pos;
            int index3 = num10;
            int num11 = index3 + 1;
            vector3Array1[index3] = (linkedListNode.Value.pos + linkedListNode.Value.normal * (spriteDefinition.position0.y * this.m_projectileScale) - this.m_minBonePosition).ToVector3ZUp(-num9);
            Vector3[] vector3Array2 = pos;
            int index4 = num11;
            int num12 = index4 + 1;
            vector3Array2[index4] = (linkedListNode.Next.Value.pos + linkedListNode.Next.Value.normal * (spriteDefinition.position1.y * this.m_projectileScale) - this.m_minBonePosition).ToVector3ZUp(-num9);
            Vector3[] vector3Array3 = pos;
            int index5 = num12;
            int num13 = index5 + 1;
            vector3Array3[index5] = (linkedListNode.Value.pos + linkedListNode.Value.normal * (spriteDefinition.position2.y * this.m_projectileScale) - this.m_minBonePosition).ToVector3ZUp(-num9);
            Vector3[] vector3Array4 = pos;
            int index6 = num13;
            int num14 = index6 + 1;
            vector3Array4[index6] = (linkedListNode.Next.Value.pos + linkedListNode.Next.Value.normal * (spriteDefinition.position3.y * this.m_projectileScale) - this.m_minBonePosition).ToVector3ZUp(-num9);
            Vector2 vector2_1 = Vector2.Lerp(spriteDefinition.uvs[0], spriteDefinition.uvs[1], t);
            Vector2 vector2_2 = Vector2.Lerp(spriteDefinition.uvs[2], spriteDefinition.uvs[3], t + num8 / (float) num1);
            int num15 = offset + num5;
            int num16;
            if (spriteDefinition.flipped == tk2dSpriteDefinition.FlipMode.Tk2d)
            {
              Vector2[] vector2Array1 = uv;
              int index7 = num15;
              int num17 = index7 + 1;
              vector2Array1[index7] = new Vector2(vector2_1.x, vector2_1.y);
              Vector2[] vector2Array2 = uv;
              int index8 = num17;
              int num18 = index8 + 1;
              vector2Array2[index8] = new Vector2(vector2_1.x, vector2_2.y);
              Vector2[] vector2Array3 = uv;
              int index9 = num18;
              int num19 = index9 + 1;
              vector2Array3[index9] = new Vector2(vector2_2.x, vector2_1.y);
              Vector2[] vector2Array4 = uv;
              int index10 = num19;
              num16 = index10 + 1;
              vector2Array4[index10] = new Vector2(vector2_2.x, vector2_2.y);
            }
            else if (spriteDefinition.flipped == tk2dSpriteDefinition.FlipMode.TPackerCW)
            {
              Vector2[] vector2Array5 = uv;
              int index11 = num15;
              int num20 = index11 + 1;
              vector2Array5[index11] = new Vector2(vector2_1.x, vector2_1.y);
              Vector2[] vector2Array6 = uv;
              int index12 = num20;
              int num21 = index12 + 1;
              vector2Array6[index12] = new Vector2(vector2_2.x, vector2_1.y);
              Vector2[] vector2Array7 = uv;
              int index13 = num21;
              int num22 = index13 + 1;
              vector2Array7[index13] = new Vector2(vector2_1.x, vector2_2.y);
              Vector2[] vector2Array8 = uv;
              int index14 = num22;
              num16 = index14 + 1;
              vector2Array8[index14] = new Vector2(vector2_2.x, vector2_2.y);
            }
            else
            {
              Vector2[] vector2Array9 = uv;
              int index15 = num15;
              int num23 = index15 + 1;
              vector2Array9[index15] = new Vector2(vector2_1.x, vector2_1.y);
              Vector2[] vector2Array10 = uv;
              int index16 = num23;
              int num24 = index16 + 1;
              vector2Array10[index16] = new Vector2(vector2_2.x, vector2_1.y);
              Vector2[] vector2Array11 = uv;
              int index17 = num24;
              int num25 = index17 + 1;
              vector2Array11[index17] = new Vector2(vector2_1.x, vector2_2.y);
              Vector2[] vector2Array12 = uv;
              int index18 = num25;
              num16 = index18 + 1;
              vector2Array12[index18] = new Vector2(vector2_2.x, vector2_2.y);
            }
            if (this.FlipUvsY)
            {
              Vector2 vector2_3 = uv[num16 - 4];
              uv[num16 - 4] = uv[num16 - 2];
              uv[num16 - 2] = vector2_3;
              Vector2 vector2_4 = uv[num16 - 3];
              uv[num16 - 3] = uv[num16 - 1];
              uv[num16 - 1] = vector2_4;
            }
            num5 += 4;
            t += num8 / (float) this.m_spriteSubtileWidth;
            if (linkedListNode != null)
              linkedListNode = linkedListNode.Next;
            ++num4;
          }
        }
      }

      public enum TargetType
      {
        Screen = 10, // 0x0000000A
        Room = 20, // 0x00000014
      }

      private class Bone
      {
        public Vector2 pos;
        public Vector2 normal;

        public Bone(Vector2 pos) => this.pos = pos;
      }
    }

}
