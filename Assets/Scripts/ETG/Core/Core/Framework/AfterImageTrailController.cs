// Decompiled with JetBrains decompiler
// Type: AfterImageTrailController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Core.Framework
{
    public class AfterImageTrailController : BraveBehaviour
    {
      public bool spawnShadows = true;
      public float shadowTimeDelay = 0.1f;
      public float shadowLifetime = 0.6f;
      public float minTranslation = 0.2f;
      public float maxEmission = 800f;
      public float minEmission = 100f;
      public float targetHeight = -2f;
      public Color dashColor = new Color(1f, 0.0f, 1f, 1f);
      public Shader OptionalImageShader;
      public bool UseTargetLayer;
      public string TargetLayer;
      [NonSerialized]
      public Shader OverrideImageShader;
      private readonly LinkedList<AfterImageTrailController.Shadow> m_activeShadows = new LinkedList<AfterImageTrailController.Shadow>();
      private readonly LinkedList<AfterImageTrailController.Shadow> m_inactiveShadows = new LinkedList<AfterImageTrailController.Shadow>();
      private float m_spawnTimer;
      private Vector2 m_lastSpawnPosition;
      private bool m_previousFrameSpawnShadows;

      public void Start()
      {
        if ((UnityEngine.Object) this.OptionalImageShader != (UnityEngine.Object) null)
          this.OverrideImageShader = this.OptionalImageShader;
        if ((UnityEngine.Object) this.transform.parent != (UnityEngine.Object) null && (UnityEngine.Object) this.transform.parent.GetComponent<Projectile>() != (UnityEngine.Object) null)
          this.transform.parent.GetComponent<Projectile>().OnDestruction += new Action<Projectile>(this.projectile_OnDestruction);
        this.m_lastSpawnPosition = (Vector2) this.transform.position;
      }

      private void projectile_OnDestruction(Projectile source)
      {
        if (this.m_activeShadows.Count <= 0)
          return;
        GameManager.Instance.StartCoroutine(this.HandleDeathShadowCleanup());
      }

      public void LateUpdate()
      {
        if (this.spawnShadows && !this.m_previousFrameSpawnShadows)
          this.m_spawnTimer = this.shadowTimeDelay;
        this.m_previousFrameSpawnShadows = this.spawnShadows;
        LinkedListNode<AfterImageTrailController.Shadow> next;
        for (LinkedListNode<AfterImageTrailController.Shadow> node = this.m_activeShadows.First; node != null; node = next)
        {
          next = node.Next;
          node.Value.timer -= BraveTime.DeltaTime;
          if ((double) node.Value.timer <= 0.0)
          {
            this.m_activeShadows.Remove(node);
            this.m_inactiveShadows.AddLast(node);
            if ((bool) (UnityEngine.Object) node.Value.sprite)
              node.Value.sprite.renderer.enabled = false;
          }
          else if ((bool) (UnityEngine.Object) node.Value.sprite)
          {
            float t = node.Value.timer / this.shadowLifetime;
            Material sharedMaterial = node.Value.sprite.renderer.sharedMaterial;
            sharedMaterial.SetFloat("_EmissivePower", Mathf.Lerp(this.maxEmission, this.minEmission, t));
            sharedMaterial.SetFloat("_Opacity", t);
          }
        }
        if (!this.spawnShadows)
          return;
        if ((double) this.m_spawnTimer > 0.0)
          this.m_spawnTimer -= BraveTime.DeltaTime;
        if ((double) this.m_spawnTimer > 0.0 || (double) Vector2.Distance(this.m_lastSpawnPosition, (Vector2) this.transform.position) <= (double) this.minTranslation)
          return;
        this.SpawnNewShadow();
        this.m_spawnTimer += this.shadowTimeDelay;
        this.m_lastSpawnPosition = (Vector2) this.transform.position;
      }

      [DebuggerHidden]
      private IEnumerator HandleDeathShadowCleanup()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new AfterImageTrailController__HandleDeathShadowCleanupc__Iterator0()
        {
          _this = this
        };
      }

      protected override void OnDestroy() => base.OnDestroy();

      private void SpawnNewShadow()
      {
        if (this.m_inactiveShadows.Count == 0)
          this.CreateInactiveShadow();
        LinkedListNode<AfterImageTrailController.Shadow> first = this.m_inactiveShadows.First;
        tk2dSprite sprite = first.Value.sprite;
        this.m_inactiveShadows.RemoveFirst();
        if (!(bool) (UnityEngine.Object) sprite || !(bool) (UnityEngine.Object) sprite.renderer)
          return;
        first.Value.timer = this.shadowLifetime;
        sprite.SetSprite(this.sprite.Collection, this.sprite.spriteId);
        sprite.transform.position = this.sprite.transform.position;
        sprite.transform.rotation = this.sprite.transform.rotation;
        sprite.scale = this.sprite.scale;
        sprite.usesOverrideMaterial = true;
        sprite.IsPerpendicular = true;
        if ((bool) (UnityEngine.Object) sprite.renderer)
        {
          sprite.renderer.enabled = true;
          sprite.renderer.material.shader = this.OverrideImageShader ?? ShaderCache.Acquire("Brave/Internal/HighPriestAfterImage");
          sprite.renderer.sharedMaterial.SetFloat("_EmissivePower", this.minEmission);
          sprite.renderer.sharedMaterial.SetFloat("_Opacity", 1f);
          sprite.renderer.sharedMaterial.SetColor("_DashColor", this.dashColor);
        }
        sprite.HeightOffGround = this.targetHeight;
        sprite.UpdateZDepth();
        this.m_activeShadows.AddLast(first);
      }

      private void CreateInactiveShadow()
      {
        GameObject gameObject = new GameObject("after image");
        if (this.UseTargetLayer)
          gameObject.layer = LayerMask.NameToLayer(this.TargetLayer);
        tk2dSprite tk2dSprite = gameObject.AddComponent<tk2dSprite>();
        gameObject.transform.parent = SpawnManager.Instance.VFX;
        this.m_inactiveShadows.AddLast(new AfterImageTrailController.Shadow()
        {
          timer = this.shadowLifetime,
          sprite = tk2dSprite
        });
      }

      private class Shadow
      {
        public float timer;
        public tk2dSprite sprite;
      }
    }

}
