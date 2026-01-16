// Decompiled with JetBrains decompiler
// Type: FakeGameActorEffectHandler
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable
public class FakeGameActorEffectHandler : BraveBehaviour
{
  public bool OverrideColorOverridden;
  private int m_overrideColorID;
  private List<string> m_overrideColorSources = new List<string>();
  private List<Color> m_overrideColorStack = new List<Color>();
  private List<GameActorEffect> m_activeEffects = new List<GameActorEffect>();
  private List<FakeGameActorEffectHandler.RuntimeGameActorEffectData> m_activeEffectData = new List<FakeGameActorEffectHandler.RuntimeGameActorEffectData>();

  public bool IsGone { get; set; }

  public virtual void Awake()
  {
    this.m_overrideColorID = Shader.PropertyToID("_OverrideColor");
    this.RegisterOverrideColor(new Color(1f, 1f, 1f, 0.0f), "base");
  }

  public virtual void Update()
  {
    for (int index = 0; index < this.m_activeEffects.Count; ++index)
    {
      GameActorEffect activeEffect = this.m_activeEffects[index];
      if (activeEffect != null && this.m_activeEffectData != null && index < this.m_activeEffectData.Count)
      {
        FakeGameActorEffectHandler.RuntimeGameActorEffectData gameActorEffectData = this.m_activeEffectData[index];
        if (gameActorEffectData != null)
        {
          if ((Object) gameActorEffectData.instanceOverheadVFX != (Object) null)
          {
            if (!this.IsGone)
            {
              Vector2 vector2 = this.transform.position.XY();
              if (activeEffect.PlaysVFXOnActor)
              {
                if ((bool) (Object) this.specRigidbody && this.specRigidbody.HitboxPixelCollider != null)
                  vector2 = this.specRigidbody.HitboxPixelCollider.UnitBottomCenter.Quantize(1f / 16f);
                gameActorEffectData.instanceOverheadVFX.transform.position = (Vector3) vector2;
              }
              else
              {
                if ((bool) (Object) this.specRigidbody && this.specRigidbody.HitboxPixelCollider != null)
                  vector2 = this.specRigidbody.HitboxPixelCollider.UnitTopCenter.Quantize(1f / 16f);
                gameActorEffectData.instanceOverheadVFX.transform.position = (Vector3) vector2;
              }
              gameActorEffectData.instanceOverheadVFX.renderer.enabled = true;
            }
            else if ((bool) (Object) gameActorEffectData.instanceOverheadVFX)
              Object.Destroy((Object) gameActorEffectData.instanceOverheadVFX.gameObject);
          }
          gameActorEffectData.elapsed += BraveTime.DeltaTime;
          if ((double) gameActorEffectData.elapsed >= (double) activeEffect.duration)
            this.RemoveEffect(activeEffect);
        }
      }
    }
  }

  protected override void OnDestroy() => base.OnDestroy();

  public void ApplyEffect(GameActorEffect effect)
  {
    FakeGameActorEffectHandler.RuntimeGameActorEffectData gameActorEffectData = new FakeGameActorEffectHandler.RuntimeGameActorEffectData();
    if (effect.AppliesTint)
      this.RegisterOverrideColor(effect.TintColor, effect.effectIdentifier);
    if ((Object) effect.OverheadVFX != (Object) null)
    {
      GameObject gameObject = Object.Instantiate<GameObject>(effect.OverheadVFX);
      tk2dBaseSprite component = gameObject.GetComponent<tk2dBaseSprite>();
      gameObject.transform.parent = this.transform;
      if ((bool) (Object) this.healthHaver && this.healthHaver.IsBoss)
      {
        gameObject.transform.position = (Vector3) this.specRigidbody.HitboxPixelCollider.UnitTopCenter;
      }
      else
      {
        Bounds bounds = this.sprite.GetBounds();
        Vector3 vector3 = this.transform.position + new Vector3((float) (((double) bounds.max.x + (double) bounds.min.x) / 2.0), bounds.max.y, 0.0f).Quantize(1f / 16f);
        if (effect.PlaysVFXOnActor)
          vector3.y = this.transform.position.y + bounds.min.y;
        gameObject.transform.position = this.sprite.WorldCenter.ToVector3ZUp().WithY(vector3.y);
      }
      component.HeightOffGround = 0.5f;
      this.sprite.AttachRenderer(component);
      gameActorEffectData.instanceOverheadVFX = gameObject.GetComponent<tk2dBaseSprite>();
    }
    this.m_activeEffects.Add(effect);
    this.m_activeEffectData.Add(gameActorEffectData);
  }

  public void RemoveEffect(GameActorEffect effect)
  {
    for (int index = 0; index < this.m_activeEffects.Count; ++index)
    {
      if (this.m_activeEffects[index].effectIdentifier == effect.effectIdentifier)
      {
        this.RemoveEffect(index);
        break;
      }
    }
  }

  private void RemoveEffect(int index, bool ignoreDeathCheck = false)
  {
    if (!ignoreDeathCheck && (bool) (Object) this.healthHaver && this.healthHaver.IsDead)
      return;
    GameActorEffect activeEffect = this.m_activeEffects[index];
    if (activeEffect.AppliesTint)
      this.DeregisterOverrideColor(activeEffect.effectIdentifier);
    this.m_activeEffects.RemoveAt(index);
    if ((bool) (Object) this.m_activeEffectData[index].instanceOverheadVFX)
      Object.Destroy((Object) this.m_activeEffectData[index].instanceOverheadVFX.gameObject);
    this.m_activeEffectData.RemoveAt(index);
  }

  public void RemoveAllEffects(bool ignoreDeathCheck = false)
  {
    for (int index = this.m_activeEffects.Count - 1; index >= 0; --index)
      this.RemoveEffect(index, ignoreDeathCheck);
  }

  public Color CurrentOverrideColor
  {
    get
    {
      if (this.m_overrideColorStack.Count == 0)
        this.RegisterOverrideColor(new Color(1f, 1f, 1f, 0.0f), "base");
      return this.m_overrideColorStack[this.m_overrideColorStack.Count - 1];
    }
  }

  public bool HasSourcedOverrideColor(string source)
  {
    return this.m_overrideColorSources.Contains(source);
  }

  public void RegisterOverrideColor(Color overrideColor, string source)
  {
    int index = this.m_overrideColorSources.IndexOf(source);
    if (index >= 0)
    {
      this.m_overrideColorStack[index] = overrideColor;
    }
    else
    {
      this.m_overrideColorSources.Add(source);
      this.m_overrideColorStack.Add(overrideColor);
    }
    this.OnOverrideColorsChanged();
  }

  public void DeregisterOverrideColor(string source)
  {
    int index = this.m_overrideColorSources.IndexOf(source);
    if (index >= 0)
    {
      this.m_overrideColorStack.RemoveAt(index);
      this.m_overrideColorSources.RemoveAt(index);
    }
    this.OnOverrideColorsChanged();
  }

  public void OnOverrideColorsChanged()
  {
    if (this.OverrideColorOverridden)
      return;
    if ((bool) (Object) this.healthHaver)
    {
      for (int index = 0; index < this.healthHaver.bodySprites.Count; ++index)
      {
        if ((bool) (Object) this.healthHaver.bodySprites[index])
        {
          this.healthHaver.bodySprites[index].usesOverrideMaterial = true;
          this.healthHaver.bodySprites[index].renderer.material.SetColor(this.m_overrideColorID, this.CurrentOverrideColor);
        }
      }
    }
    else
    {
      if (!(bool) (Object) this.sprite)
        return;
      this.sprite.usesOverrideMaterial = true;
      this.sprite.renderer.material.SetColor(this.m_overrideColorID, this.CurrentOverrideColor);
    }
  }

  private class RuntimeGameActorEffectData
  {
    public float elapsed;
    public tk2dBaseSprite instanceOverheadVFX;
  }
}
