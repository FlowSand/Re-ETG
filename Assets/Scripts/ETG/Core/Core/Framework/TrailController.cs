// Decompiled with JetBrains decompiler
// Type: TrailController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable

namespace ETG.Core.Core.Framework
{
    public class TrailController : BraveBehaviour
    {
      public bool usesStartAnimation;
      public string startAnimation;
      public bool usesAnimation;
      public string animation;
      [TogglesProperty("cascadeTimer", "Cascade Timer")]
      public bool usesCascadeTimer;
      [HideInInspector]
      public float cascadeTimer;
      [TogglesProperty("softMaxLength", "Soft Max Length")]
      public bool usesSoftMaxLength;
      [HideInInspector]
      public float softMaxLength;
      [TogglesProperty("globalTimer", "Global Timer")]
      public bool usesGlobalTimer;
      [HideInInspector]
      public float globalTimer;
      public bool destroyOnEmpty = true;
      [HideInInspector]
      public bool FlipUvsY;
      public bool rampHeight;
      public float rampStartHeight = 2f;
      public float rampTime = 1f;
      public Vector2 boneSpawnOffset;
      public bool UsesDispersalParticles;
      [ShowInInspectorIf("UsesDispersalParticles", false)]
      public float DispersalDensity = 3f;
      [ShowInInspectorIf("UsesDispersalParticles", false)]
      public float DispersalMinCoherency = 0.2f;
      [ShowInInspectorIf("UsesDispersalParticles", false)]
      public float DispersalMaxCoherency = 1f;
      [ShowInInspectorIf("UsesDispersalParticles", false)]
      public GameObject DispersalParticleSystemPrefab;
      private tk2dTiledSprite m_sprite;
      private tk2dSpriteAnimationClip m_startAnimationClip;
      private tk2dSpriteAnimationClip m_animationClip;
      private float m_projectileScale = 1f;
      private int m_spriteSubtileWidth;
      private readonly LinkedList<TrailController.Bone> m_bones = new LinkedList<TrailController.Bone>();
      private ParticleSystem m_dispersalParticles;
      private Vector2 m_minBonePosition;
      private Vector2 m_maxBonePosition;
      private bool m_isDirty;
      private float m_globalTimer;
      private float m_rampTimer;
      private float m_maxPosX;
      private const int c_bonePixelLength = 4;
      private const float c_boneUnitLength = 0.25f;
      private const float c_trailHeightOffset = -0.5f;

      public void Start()
      {
        this.specRigidbody = this.transform.parent.GetComponent<SpeculativeRigidbody>();
        this.specRigidbody.Initialize();
        this.transform.parent = SpawnManager.Instance.VFX;
        this.transform.rotation = Quaternion.identity;
        this.transform.position = Vector3.zero;
        if (this.specRigidbody.projectile.Owner is PlayerController)
          this.m_projectileScale = (this.specRigidbody.projectile.Owner as PlayerController).BulletScaleModifier;
        this.gameObject.SetLayerRecursively(LayerMask.NameToLayer("FG_Critical"));
        this.m_sprite = this.GetComponent<tk2dTiledSprite>();
        this.m_sprite.OverrideGetTiledSpriteGeomDesc = new tk2dTiledSprite.OverrideGetTiledSpriteGeomDescDelegate(this.GetTiledSpriteGeomDesc);
        this.m_sprite.OverrideSetTiledSpriteGeom = new tk2dTiledSprite.OverrideSetTiledSpriteGeomDelegate(this.SetTiledSpriteGeom);
        tk2dSpriteDefinition currentSpriteDef = this.m_sprite.GetCurrentSpriteDef();
        this.m_spriteSubtileWidth = Mathf.RoundToInt(currentSpriteDef.untrimmedBoundsDataExtents.x / currentSpriteDef.texelSize.x) / 4;
        float heightOffset = !this.rampHeight ? 0.0f : this.rampStartHeight;
        this.m_bones.AddLast(new TrailController.Bone(this.specRigidbody.Position.UnitPosition + this.boneSpawnOffset, 0.0f, heightOffset));
        this.m_bones.AddLast(new TrailController.Bone(this.specRigidbody.Position.UnitPosition + this.boneSpawnOffset, 0.0f, heightOffset));
        if (this.usesStartAnimation)
          this.m_startAnimationClip = this.spriteAnimator.GetClipByName(this.startAnimation);
        if (this.usesAnimation)
          this.m_animationClip = this.spriteAnimator.GetClipByName(this.animation);
        if ((this.usesStartAnimation || this.usesAnimation) && this.usesCascadeTimer)
          this.m_bones.First.Value.IsAnimating = true;
        if (this.UsesDispersalParticles)
          this.m_dispersalParticles = GlobalDispersalParticleManager.GetSystemForPrefab(this.DispersalParticleSystemPrefab);
        this.specRigidbody.OnCollision += new Action<CollisionData>(this.UpdateOnCollision);
        this.specRigidbody.OnPostRigidbodyMovement += new Action<SpeculativeRigidbody, Vector2, IntVector2>(this.PostRigidbodyMovement);
      }

      public void Update()
      {
        int subtilesPerTile = Mathf.RoundToInt(this.m_sprite.GetCurrentSpriteDef().untrimmedBoundsDataExtents.x / this.m_sprite.GetCurrentSpriteDef().texelSize.x) / 4;
        this.m_globalTimer += BraveTime.DeltaTime;
        this.m_rampTimer += BraveTime.DeltaTime;
        if (this.usesAnimation)
        {
          LinkedListNode<TrailController.Bone> boneNode = this.m_bones.First;
          float num1 = 0.0f;
    label_23:
          while (boneNode != null)
          {
            bool flag = false;
            if (boneNode.Value.IsAnimating)
            {
              tk2dSpriteAnimationClip spriteAnimationClip = !this.usesStartAnimation || boneNode != this.m_bones.First ? this.m_animationClip : this.m_startAnimationClip;
              boneNode.Value.AnimationTimer += BraveTime.DeltaTime;
              num1 = boneNode.Value.AnimationTimer;
              int num2 = Mathf.FloorToInt((boneNode.Value.AnimationTimer - BraveTime.DeltaTime) * spriteAnimationClip.fps);
              if (Mathf.FloorToInt(boneNode.Value.AnimationTimer * spriteAnimationClip.fps) != num2)
                this.m_isDirty = true;
              if ((double) boneNode.Value.AnimationTimer > (double) spriteAnimationClip.frames.Length / (double) spriteAnimationClip.fps)
              {
                if (this.usesStartAnimation && boneNode == this.m_bones.First)
                  this.usesStartAnimation = false;
                for (int index = 0; index < subtilesPerTile && boneNode != null; ++index)
                {
                  LinkedListNode<TrailController.Bone> node = boneNode;
                  boneNode = boneNode.Next;
                  this.m_bones.Remove(node);
                }
                flag = true;
                this.m_isDirty = true;
              }
            }
            if (boneNode != null)
            {
              if (!boneNode.Value.IsAnimating && this.usesGlobalTimer && (double) this.m_globalTimer > (double) this.globalTimer)
              {
                boneNode.Value.IsAnimating = true;
                boneNode.Value.AnimationTimer = this.m_globalTimer - this.globalTimer;
                this.DoDispersalParticles(boneNode, subtilesPerTile);
                this.m_isDirty = true;
              }
              if (!boneNode.Value.IsAnimating && this.usesCascadeTimer && (boneNode == this.m_bones.First || (double) num1 >= (double) this.cascadeTimer))
              {
                boneNode.Value.IsAnimating = true;
                num1 = 0.0f;
                this.DoDispersalParticles(boneNode, subtilesPerTile);
                this.m_isDirty = true;
              }
              if (!boneNode.Value.IsAnimating && this.usesSoftMaxLength && (double) this.m_maxPosX - (double) boneNode.Value.posX > (double) this.softMaxLength)
              {
                boneNode.Value.IsAnimating = true;
                num1 = 0.0f;
                this.DoDispersalParticles(boneNode, subtilesPerTile);
                this.m_isDirty = true;
              }
            }
            if (!flag && boneNode != null)
            {
              int num3 = 0;
              while (true)
              {
                if (num3 < subtilesPerTile && boneNode != null)
                {
                  boneNode = boneNode.Next;
                  ++num3;
                }
                else
                  goto label_23;
              }
            }
          }
        }
        if (this.m_bones.Count != 0)
          return;
        UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
      }

      public void LateUpdate() => this.UpdateIfDirty();

      protected override void OnDestroy()
      {
        if ((bool) (UnityEngine.Object) this.specRigidbody)
        {
          this.specRigidbody.OnCollision -= new Action<CollisionData>(this.UpdateOnCollision);
          this.specRigidbody.OnPostRigidbodyMovement -= new Action<SpeculativeRigidbody, Vector2, IntVector2>(this.PostRigidbodyMovement);
        }
        base.OnDestroy();
      }

      public void DisconnectFromSpecRigidbody()
      {
        this.specRigidbody.OnCollision -= new Action<CollisionData>(this.UpdateOnCollision);
        this.specRigidbody.OnPostRigidbodyMovement -= new Action<SpeculativeRigidbody, Vector2, IntVector2>(this.PostRigidbodyMovement);
      }

      private void UpdateOnCollision(CollisionData obj)
      {
        this.HandleExtension(this.specRigidbody.Position.UnitPosition + PhysicsEngine.PixelToUnit(obj.NewPixelsToMove));
        this.m_bones.Last.Value.Hide = true;
        this.m_bones.AddLast(new TrailController.Bone(this.m_bones.Last.Value.pos, this.m_bones.Last.Value.posX, this.m_bones.Last.Value.HeightOffset));
        this.m_bones.AddLast(new TrailController.Bone(this.m_bones.Last.Value.pos, this.m_bones.Last.Value.posX, this.m_bones.Last.Value.HeightOffset));
      }

      private void PostRigidbodyMovement(
        SpeculativeRigidbody rigidbody,
        Vector2 unitDelta,
        IntVector2 pixelDelta)
      {
        this.HandleExtension(this.specRigidbody.Position.UnitPosition);
        this.UpdateIfDirty();
      }

      private void UpdateIfDirty()
      {
        if (!this.m_isDirty)
          return;
        this.m_minBonePosition = new Vector2(float.MaxValue, float.MaxValue);
        this.m_maxBonePosition = new Vector2(float.MinValue, float.MinValue);
        for (LinkedListNode<TrailController.Bone> linkedListNode = this.m_bones.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
        {
          this.m_minBonePosition = Vector2.Min(this.m_minBonePosition, linkedListNode.Value.pos);
          this.m_maxBonePosition = Vector2.Max(this.m_maxBonePosition, linkedListNode.Value.pos);
        }
        this.transform.position = new Vector3(this.m_minBonePosition.x, this.m_minBonePosition.y, this.m_minBonePosition.y - 0.5f);
        this.m_sprite.ForceBuild();
        this.m_sprite.UpdateZDepth();
        this.m_isDirty = false;
      }

      private void HandleExtension(Vector2 specRigidbodyPosition)
      {
        if (!this.destroyOnEmpty && this.m_bones.Count == 0)
        {
          float heightOffset = !this.rampHeight ? 0.0f : this.rampStartHeight;
          this.m_bones.AddLast(new TrailController.Bone(this.specRigidbody.Position.UnitPosition + this.boneSpawnOffset, this.m_maxPosX, heightOffset));
          this.m_bones.AddLast(new TrailController.Bone(this.specRigidbody.Position.UnitPosition + this.boneSpawnOffset, this.m_maxPosX, heightOffset));
        }
        Vector2 vector2 = specRigidbodyPosition;
        if ((bool) (UnityEngine.Object) this.specRigidbody.projectile && this.specRigidbody.projectile.OverrideTrailPoint.HasValue)
          vector2 = this.specRigidbody.projectile.OverrideTrailPoint.Value;
        this.ExtendBonesTo(vector2 + this.boneSpawnOffset);
        this.m_isDirty = true;
      }

      private void ExtendBonesTo(Vector2 newPos)
      {
        if (this.m_bones == null || this.m_bones.Last == null || this.m_bones.Last.Value == null || this.m_bones.Last.Previous == null || this.m_bones.Last.Previous.Value == null)
          return;
        Vector2 vector2_1 = newPos - this.m_bones.Last.Value.pos;
        Vector2 v = this.m_bones.Last.Value.pos - this.m_bones.Last.Previous.Value.pos;
        float magnitude1 = v.magnitude;
        LinkedListNode<TrailController.Bone> previous = this.m_bones.Last.Previous;
        float num1 = Vector3.Distance((Vector3) newPos, (Vector3) this.m_bones.Last.Previous.Value.pos);
        if ((double) num1 < 0.25)
        {
          this.m_bones.Last.Value.pos = newPos;
          this.m_bones.Last.Value.posX = this.m_bones.Last.Previous.Value.posX + num1;
        }
        else
        {
          if (Mathf.Approximately(magnitude1, 0.0f))
          {
            this.m_bones.Last.Value.pos = this.m_bones.Last.Previous.Value.pos + vector2_1.normalized * 0.25f;
            this.m_bones.Last.Value.posX = this.m_bones.Last.Previous.Value.posX + 0.25f;
          }
          else
          {
            float num2 = 0.25f;
            float num3 = magnitude1;
            float f1 = BraveMathCollege.ClampAnglePi(Mathf.Atan2(vector2_1.y, vector2_1.x) - Mathf.Atan2(-v.y, -v.x));
            float f2 = Mathf.Abs(f1);
            float num4 = 3.14159274f - Mathf.Asin(num3 / num2 * Mathf.Sin(f2)) - f2;
            this.m_bones.Last.Value.pos = this.m_bones.Last.Previous.Value.pos + v.Rotate((float) ((double) Mathf.Sign(f1) * -(double) num4 * 57.295780181884766)).normalized * 0.25f;
            this.m_bones.Last.Value.posX = this.m_bones.Last.Previous.Value.posX + 0.25f;
          }
          Vector2 pos = this.m_bones.Last.Value.pos;
          Vector2 vector2_2 = newPos - pos;
          float magnitude2 = vector2_2.magnitude;
          float heightOffset = !this.rampHeight ? 0.0f : Mathf.Lerp(this.rampStartHeight, 0.0f, this.m_rampTimer / this.rampTime);
          vector2_2.Normalize();
          while ((double) magnitude2 > 0.0)
          {
            if ((double) magnitude2 < 0.25)
            {
              this.m_bones.AddLast(new TrailController.Bone(newPos, this.m_bones.Last.Value.posX + magnitude2, heightOffset));
              break;
            }
            pos += vector2_2 * 0.25f;
            this.m_bones.AddLast(new TrailController.Bone(pos, this.m_bones.Last.Value.posX + 0.25f, heightOffset));
            magnitude2 -= 0.25f;
            if (this.usesGlobalTimer && (double) this.m_globalTimer > (double) this.globalTimer)
              this.m_bones.Last.Value.AnimationTimer = this.m_globalTimer - this.globalTimer;
          }
        }
        this.m_maxPosX = this.m_bones.Last.Value.posX;
        for (LinkedListNode<TrailController.Bone> linkedListNode = previous; linkedListNode != null && linkedListNode.Next != null; linkedListNode = linkedListNode.Next)
          linkedListNode.Value.normal = (Vector2) (Quaternion.Euler(0.0f, 0.0f, 90f) * (Vector3) (linkedListNode.Next.Value.pos - linkedListNode.Value.pos)).normalized;
        this.m_bones.Last.Value.normal = this.m_bones.Last.Previous.Value.normal;
        this.m_isDirty = true;
      }

      private void DoDispersalParticles(
        LinkedListNode<TrailController.Bone> boneNode,
        int subtilesPerTile)
      {
        if (!this.UsesDispersalParticles || boneNode.Value == null || boneNode.Next == null || boneNode.Next.Value == null)
          return;
        Vector3 vector3Zup1 = boneNode.Value.pos.ToVector3ZUp(boneNode.Value.pos.y);
        LinkedListNode<TrailController.Bone> linkedListNode = boneNode;
        for (int index = 0; index < subtilesPerTile && linkedListNode.Next != null; ++index)
          linkedListNode = linkedListNode.Next;
        Vector3 vector3Zup2 = linkedListNode.Value.pos.ToVector3ZUp(linkedListNode.Value.pos.y);
        int num = Mathf.Max(Mathf.CeilToInt(Vector2.Distance(vector3Zup1.XY(), vector3Zup2.XY()) * this.DispersalDensity), 1);
        for (int index = 0; index < num; ++index)
        {
          float t = (float) index / (float) num;
          Vector3 vector3_1 = Vector3.Lerp(vector3Zup1, vector3Zup2, t) + Vector3.back;
          Vector3 vector3_2 = Vector3.Lerp(Quaternion.Euler(0.0f, 0.0f, Mathf.PerlinNoise(vector3_1.x / 3f, vector3_1.y / 3f) * 360f) * Vector3.right, UnityEngine.Random.insideUnitSphere, UnityEngine.Random.Range(this.DispersalMinCoherency, this.DispersalMaxCoherency));
          this.m_dispersalParticles.Emit(new ParticleSystem.EmitParams()
          {
            position = vector3_1,
            velocity = vector3_2 * this.m_dispersalParticles.startSpeed,
            startSize = this.m_dispersalParticles.startSize,
            startLifetime = this.m_dispersalParticles.startLifetime,
            startColor = (Color32) this.m_dispersalParticles.startColor
          }, 1);
        }
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
        LinkedListNode<TrailController.Bone> linkedListNode = this.m_bones.First;
        int num4 = 0;
        for (int index1 = 0; index1 < num3; ++index1)
        {
          int num5 = 0;
          int num6 = num1 - 1;
          if (index1 == num3 - 1 && num2 % num1 != 0)
            num6 = num2 % num1 - 1;
          tk2dSpriteDefinition spriteDefinition = spriteDef;
          if (this.usesStartAnimation && index1 == 0)
            spriteDefinition = this.m_sprite.Collection.spriteDefinitions[this.m_startAnimationClip.frames[Mathf.Clamp(Mathf.FloorToInt(linkedListNode.Value.AnimationTimer * this.m_startAnimationClip.fps), 0, this.m_startAnimationClip.frames.Length - 1)].spriteId];
          else if (this.usesAnimation && linkedListNode.Value.IsAnimating)
            spriteDefinition = this.m_sprite.Collection.spriteDefinitions[this.m_animationClip.frames[Mathf.Min((int) ((double) linkedListNode.Value.AnimationTimer * (double) this.m_animationClip.fps), this.m_animationClip.frames.Length - 1)].spriteId];
          float t = 0.0f;
          for (int index2 = num5; index2 <= num6; ++index2)
          {
            float num7 = 1f;
            if (index1 == num3 - 1 && index2 == num6)
              num7 = Vector2.Distance(linkedListNode.Next.Value.pos, linkedListNode.Value.pos);
            int num8 = offset + num4;
            Vector3[] vector3Array1 = pos;
            int index3 = num8;
            int num9 = index3 + 1;
            vector3Array1[index3] = (Vector3) (linkedListNode.Value.pos + linkedListNode.Value.normal * (spriteDefinition.position0.y * this.m_projectileScale) - this.m_minBonePosition);
            Vector3[] vector3Array2 = pos;
            int index4 = num9;
            int num10 = index4 + 1;
            vector3Array2[index4] = (Vector3) (linkedListNode.Next.Value.pos + linkedListNode.Next.Value.normal * (spriteDefinition.position1.y * this.m_projectileScale) - this.m_minBonePosition);
            Vector3[] vector3Array3 = pos;
            int index5 = num10;
            int num11 = index5 + 1;
            vector3Array3[index5] = (Vector3) (linkedListNode.Value.pos + linkedListNode.Value.normal * (spriteDefinition.position2.y * this.m_projectileScale) - this.m_minBonePosition);
            Vector3[] vector3Array4 = pos;
            int index6 = num11;
            int num12 = index6 + 1;
            vector3Array4[index6] = (Vector3) (linkedListNode.Next.Value.pos + linkedListNode.Next.Value.normal * (spriteDefinition.position3.y * this.m_projectileScale) - this.m_minBonePosition);
            int num13 = offset + num4;
            Vector3[] vector3Array5 = pos;
            int index7 = num13;
            int num14 = index7 + 1;
            vector3Array5[index7] += new Vector3(0.0f, 0.0f, -linkedListNode.Value.HeightOffset);
            Vector3[] vector3Array6 = pos;
            int index8 = num14;
            int num15 = index8 + 1;
            vector3Array6[index8] += new Vector3(0.0f, 0.0f, -linkedListNode.Next.Value.HeightOffset);
            Vector3[] vector3Array7 = pos;
            int index9 = num15;
            int num16 = index9 + 1;
            vector3Array7[index9] += new Vector3(0.0f, 0.0f, -linkedListNode.Value.HeightOffset);
            Vector3[] vector3Array8 = pos;
            int index10 = num16;
            num12 = index10 + 1;
            vector3Array8[index10] += new Vector3(0.0f, 0.0f, -linkedListNode.Next.Value.HeightOffset);
            Vector2 vector2_1 = Vector2.Lerp(spriteDefinition.uvs[0], spriteDefinition.uvs[1], t);
            Vector2 vector2_2 = Vector2.Lerp(spriteDefinition.uvs[2], spriteDefinition.uvs[3], t + num7 / (float) num1);
            int num17 = offset + num4;
            int num18;
            if (spriteDefinition.flipped == tk2dSpriteDefinition.FlipMode.Tk2d)
            {
              Vector2[] vector2Array1 = uv;
              int index11 = num17;
              int num19 = index11 + 1;
              vector2Array1[index11] = new Vector2(vector2_1.x, vector2_1.y);
              Vector2[] vector2Array2 = uv;
              int index12 = num19;
              int num20 = index12 + 1;
              vector2Array2[index12] = new Vector2(vector2_1.x, vector2_2.y);
              Vector2[] vector2Array3 = uv;
              int index13 = num20;
              int num21 = index13 + 1;
              vector2Array3[index13] = new Vector2(vector2_2.x, vector2_1.y);
              Vector2[] vector2Array4 = uv;
              int index14 = num21;
              num18 = index14 + 1;
              vector2Array4[index14] = new Vector2(vector2_2.x, vector2_2.y);
            }
            else if (spriteDefinition.flipped == tk2dSpriteDefinition.FlipMode.TPackerCW)
            {
              Vector2[] vector2Array5 = uv;
              int index15 = num17;
              int num22 = index15 + 1;
              vector2Array5[index15] = new Vector2(vector2_1.x, vector2_1.y);
              Vector2[] vector2Array6 = uv;
              int index16 = num22;
              int num23 = index16 + 1;
              vector2Array6[index16] = new Vector2(vector2_2.x, vector2_1.y);
              Vector2[] vector2Array7 = uv;
              int index17 = num23;
              int num24 = index17 + 1;
              vector2Array7[index17] = new Vector2(vector2_1.x, vector2_2.y);
              Vector2[] vector2Array8 = uv;
              int index18 = num24;
              num18 = index18 + 1;
              vector2Array8[index18] = new Vector2(vector2_2.x, vector2_2.y);
            }
            else
            {
              Vector2[] vector2Array9 = uv;
              int index19 = num17;
              int num25 = index19 + 1;
              vector2Array9[index19] = new Vector2(vector2_1.x, vector2_1.y);
              Vector2[] vector2Array10 = uv;
              int index20 = num25;
              int num26 = index20 + 1;
              vector2Array10[index20] = new Vector2(vector2_2.x, vector2_1.y);
              Vector2[] vector2Array11 = uv;
              int index21 = num26;
              int num27 = index21 + 1;
              vector2Array11[index21] = new Vector2(vector2_1.x, vector2_2.y);
              Vector2[] vector2Array12 = uv;
              int index22 = num27;
              num18 = index22 + 1;
              vector2Array12[index22] = new Vector2(vector2_2.x, vector2_2.y);
            }
            if (linkedListNode.Value.Hide)
            {
              uv[num18 - 4] = Vector2.zero;
              uv[num18 - 3] = Vector2.zero;
              uv[num18 - 2] = Vector2.zero;
              uv[num18 - 1] = Vector2.zero;
            }
            if (this.FlipUvsY)
            {
              Vector2 vector2_3 = uv[num18 - 4];
              uv[num18 - 4] = uv[num18 - 2];
              uv[num18 - 2] = vector2_3;
              Vector2 vector2_4 = uv[num18 - 3];
              uv[num18 - 3] = uv[num18 - 1];
              uv[num18 - 1] = vector2_4;
            }
            num4 += 4;
            t += num7 / (float) this.m_spriteSubtileWidth;
            if (linkedListNode != null)
              linkedListNode = linkedListNode.Next;
          }
        }
      }

      private class Bone
      {
        public float posX;
        public Vector2 normal;
        public bool IsAnimating;
        public float AnimationTimer;
        public readonly float HeightOffset;
        public bool Hide;

        public Bone(Vector2 pos, float posX, float heightOffset)
        {
          this.pos = pos;
          this.posX = posX;
          this.HeightOffset = heightOffset;
        }

        public Vector2 pos { get; set; }
      }
    }

}
