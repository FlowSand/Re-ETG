// Decompiled with JetBrains decompiler
// Type: ShaderProjModifier
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable
public class ShaderProjModifier : BraveBehaviour
{
  public bool ProcessProperty = true;
  public string ShaderProperty;
  [HideInInspectorIf("ColorAttribute", false)]
  public float StartValue;
  [HideInInspectorIf("ColorAttribute", false)]
  public float EndValue = 1f;
  public float LerpTime;
  public bool ColorAttribute;
  [ShowInInspectorIf("ColorAttribute", false)]
  public Color StartColor;
  [ShowInInspectorIf("ColorAttribute", false)]
  public Color EndColor;
  public bool OnDeath;
  public bool PreventCorpse;
  public bool DisablesOutlines;
  public bool EnableEmission;
  public bool GlobalSparks;
  public Color GlobalSparksColor;
  public float GlobalSparksForce = 3f;
  public float GlobalSparksOverrideLifespan = -1f;
  public bool AddMaterialPass;
  public Material AddPass;
  public bool IsGlitter;
  public bool ShouldAffectBosses;
  public bool AddsEncircler;
  [Header("Combine Rifle")]
  public bool AppliesLocalSlowdown;
  public float LocalTimescaleMultiplier = 0.5f;
  public bool AppliesParticleSystem;
  public GameObject ParticleSystemToSpawn;
  private bool DoesScaleAmounts;
  public GlobalSparksDoer.SparksType GlobalSparkType;
  public bool GlobalSparksRepeat;

  private float GetStartValue() => this.StartValue;

  private float GetEndValue(HealthHaver hitEnemy)
  {
    float endValue = this.EndValue;
    if (this.DoesScaleAmounts && (bool) (UnityEngine.Object) hitEnemy && (bool) (UnityEngine.Object) hitEnemy.specRigidbody && hitEnemy.specRigidbody.HitboxPixelCollider != null)
      endValue = Mathf.Lerp(this.EndValue, Mathf.Max(this.StartValue, this.EndValue / 10f), (float) ((double) hitEnemy.specRigidbody.HitboxPixelCollider.UnitWidth * (double) hitEnemy.specRigidbody.HitboxPixelCollider.UnitHeight / 5.0));
    return endValue;
  }

  private void Start()
  {
    if (this.ShaderProperty == "_EmissivePower")
      this.DoesScaleAmounts = true;
    this.projectile.OnHitEnemy += new Action<Projectile, SpeculativeRigidbody, bool>(this.projectile_OnHitEnemy);
  }

  private void projectile_OnHitEnemy(
    Projectile proj,
    SpeculativeRigidbody enemyRigidbody,
    bool killed)
  {
    if (this.ColorAttribute && (bool) (UnityEngine.Object) enemyRigidbody.gameActor && enemyRigidbody.gameActor.HasSourcedOverrideColor(this.ShaderProperty) && !this.GlobalSparksRepeat)
      return;
    HealthHaver healthHaver = enemyRigidbody.healthHaver;
    if (!(bool) (UnityEngine.Object) healthHaver)
      return;
    if (killed && this.AppliesLocalSlowdown)
    {
      AIActor component1 = enemyRigidbody.GetComponent<AIActor>();
      if ((bool) (UnityEngine.Object) component1 && (!(bool) (UnityEngine.Object) component1.healthHaver || !component1.healthHaver.IsBoss))
      {
        component1.LocalTimeScale *= this.LocalTimescaleMultiplier;
        if (component1.ParentRoom != null)
          component1.ParentRoom.DeregisterEnemy(component1);
        if ((bool) (UnityEngine.Object) component1.aiAnimator)
          component1.aiAnimator.FpsScale *= this.LocalTimescaleMultiplier;
        if ((bool) (UnityEngine.Object) component1.specRigidbody)
        {
          for (int index = 0; index < component1.specRigidbody.PixelColliders.Count; ++index)
            component1.specRigidbody.PixelColliders[index].Enabled = false;
        }
        if ((bool) (UnityEngine.Object) component1.knockbackDoer)
          component1.knockbackDoer.timeScalar = 0.0f;
        if ((bool) (UnityEngine.Object) component1.GetComponent<SpawnEnemyOnDeath>())
          component1.GetComponent<SpawnEnemyOnDeath>().chanceToSpawn = 0.0f;
        if (this.AppliesParticleSystem)
        {
          GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.ParticleSystemToSpawn, component1.CenterPosition.ToVector3ZisY(), Quaternion.identity);
          ParticleSystem component2 = gameObject.GetComponent<ParticleSystem>();
          gameObject.transform.parent = component1.transform;
          if ((bool) (UnityEngine.Object) component1.sprite)
          {
            gameObject.transform.position = (Vector3) component1.sprite.WorldCenter;
            Bounds bounds = component1.sprite.GetBounds();
            component2.shape.scale = new Vector3(bounds.extents.x * 2f, bounds.extents.y * 2f, 0.125f);
          }
        }
      }
    }
    if (this.OnDeath && !killed)
      return;
    if ((bool) (UnityEngine.Object) enemyRigidbody.aiActor && (this.IsGlitter || this.ShouldAffectBosses || !enemyRigidbody.healthHaver.IsBoss))
    {
      if (this.PreventCorpse)
      {
        if ((bool) (UnityEngine.Object) enemyRigidbody.aiActor)
          enemyRigidbody.aiActor.CorpseObject = (GameObject) null;
        FreezeOnDeath component = enemyRigidbody.GetComponent<FreezeOnDeath>();
        if ((bool) (UnityEngine.Object) component)
          component.HandleDisintegration();
      }
      if (this.DisablesOutlines && (bool) (UnityEngine.Object) enemyRigidbody.sprite)
        SpriteOutlineManager.RemoveOutlineFromSprite(enemyRigidbody.sprite);
      if (this.ProcessProperty)
      {
        if ((double) this.LerpTime <= 0.0)
        {
          for (int index = 0; index < healthHaver.bodySprites.Count; ++index)
          {
            tk2dBaseSprite bodySprite = healthHaver.bodySprites[index];
            if (!(bool) (UnityEngine.Object) bodySprite)
              return;
            bodySprite.usesOverrideMaterial = true;
            if (this.EnableEmission)
            {
              bodySprite.renderer.material.EnableKeyword("EMISSIVE_ON");
              bodySprite.renderer.material.DisableKeyword("EMISSIVE_OFF");
            }
            if (this.GlobalSparks)
            {
              int numPerSecond = 100;
              if (this.GlobalSparksRepeat)
                numPerSecond = 20;
              GlobalSparksDoer.EmitFromRegion(GlobalSparksDoer.EmitRegionStyle.RADIAL, (float) numPerSecond, this.LerpTime + 0.1f, bodySprite.WorldBottomLeft.ToVector3ZisY(), bodySprite.WorldTopRight.ToVector3ZisY(), new Vector3(this.GlobalSparksForce, 0.0f, 0.0f), 15f, 0.5f, startLifetime: (double) this.GlobalSparksOverrideLifespan <= 0.0 ? new float?() : new float?(this.GlobalSparksOverrideLifespan), startColor: new Color?(this.GlobalSparksColor), systemType: this.GlobalSparkType);
            }
            bodySprite.renderer.material.SetFloat(this.ShaderProperty, this.GetEndValue(healthHaver));
            if (this.AddsEncircler)
              bodySprite.gameObject.GetOrAddComponent<Encircler>();
          }
        }
        else
          GameManager.Instance.StartCoroutine(this.ApplyEffect(healthHaver, killed));
      }
      if (this.AddMaterialPass)
      {
        for (int index = 0; index < healthHaver.bodySprites.Count; ++index)
        {
          MeshRenderer component = healthHaver.bodySprites[index].GetComponent<MeshRenderer>();
          Material[] sharedMaterials = component.sharedMaterials;
          Array.Resize<Material>(ref sharedMaterials, sharedMaterials.Length + 1);
          Material material = UnityEngine.Object.Instantiate<Material>(this.AddPass);
          material.SetTexture("_MainTex", sharedMaterials[0].GetTexture("_MainTex"));
          sharedMaterials[sharedMaterials.Length - 1] = material;
          component.sharedMaterials = sharedMaterials;
        }
      }
    }
    if (!this.IsGlitter || !(bool) (UnityEngine.Object) enemyRigidbody.aiActor)
      return;
    enemyRigidbody.aiActor.HasBeenGlittered = true;
  }

  [DebuggerHidden]
  private IEnumerator ApplyEffect(HealthHaver hh, bool killed)
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new ShaderProjModifier.\u003CApplyEffect\u003Ec__Iterator0()
    {
      hh = hh,
      \u0024this = this
    };
  }

  protected override void OnDestroy() => base.OnDestroy();
}
