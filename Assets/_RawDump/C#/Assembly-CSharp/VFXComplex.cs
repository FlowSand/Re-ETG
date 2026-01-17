// Decompiled with JetBrains decompiler
// Type: VFXComplex
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
[Serializable]
public class VFXComplex
{
  public VFXObject[] effects;
  private List<GameObject> m_spawnedObjects = new List<GameObject>();

  public void SpawnAtPosition(
    float xPosition,
    float yPositionAtGround,
    float heightOffGround,
    float zRotation = 0.0f,
    Transform parent = null,
    Vector2? sourceNormal = null,
    Vector2? sourceVelocity = null,
    bool keepReferences = false,
    VFXComplex.SpawnMethod spawnMethod = null,
    bool ignoresPools = false)
  {
    Vector3 position = new Vector3(xPosition, yPositionAtGround + heightOffGround, yPositionAtGround - heightOffGround);
    Action<VFXObject, tk2dSprite> vfxSpriteManipulator = (Action<VFXObject, tk2dSprite>) ((effect, vfxSprite) =>
    {
      vfxSprite.HeightOffGround = 2f * heightOffGround;
      vfxSprite.UpdateZDepth();
    });
    this.InternalSpawnAtLocation(position, zRotation, parent, sourceNormal, sourceVelocity, vfxSpriteManipulator, keepReferences, spawnMethod, ignoresPools);
  }

  public void SpawnAtPosition(
    Vector3 position,
    float zRotation = 0.0f,
    Transform parent = null,
    Vector2? sourceNormal = null,
    Vector2? sourceVelocity = null,
    float? heightOffGround = null,
    bool keepReferences = false,
    VFXComplex.SpawnMethod spawnMethod = null,
    tk2dBaseSprite spriteParent = null,
    bool ignoresPools = false)
  {
    Action<VFXObject, tk2dSprite> vfxSpriteManipulator = (Action<VFXObject, tk2dSprite>) ((effect, vfxSprite) =>
    {
      if ((UnityEngine.Object) spriteParent != (UnityEngine.Object) null)
      {
        spriteParent.AttachRenderer((tk2dBaseSprite) vfxSprite);
        vfxSprite.HeightOffGround = 0.05f;
        vfxSprite.UpdateZDepth();
      }
      else
      {
        if (!((UnityEngine.Object) vfxSprite.Collection != (UnityEngine.Object) null))
          return;
        DepthLookupManager.ProcessRenderer(vfxSprite.renderer);
        if ((double) Mathf.Abs(zRotation) > 90.0)
          vfxSprite.FlipY = true;
        if (heightOffGround.HasValue)
          vfxSprite.HeightOffGround = heightOffGround.Value;
        else if (effect.usesZHeight)
          vfxSprite.HeightOffGround = effect.zHeight;
        else
          vfxSprite.HeightOffGround = 0.9f;
        vfxSprite.UpdateZDepth();
      }
    });
    this.InternalSpawnAtLocation(position, zRotation, parent, sourceNormal, sourceVelocity, vfxSpriteManipulator, keepReferences, spawnMethod, ignoresPools);
  }

  public void SpawnAtLocalPosition(
    Vector3 localPosition,
    float zRotation,
    Transform parent,
    Vector2? sourceNormal = null,
    Vector2? sourceVelocity = null,
    bool keepReferences = false,
    VFXComplex.SpawnMethod spawnMethod = null,
    bool ignoresPools = false)
  {
    Vector3 position = parent.transform.position + localPosition;
    Action<VFXObject, tk2dSprite> vfxSpriteManipulator = (Action<VFXObject, tk2dSprite>) ((effect, vfxSprite) =>
    {
      if (effect.usesZHeight)
        vfxSprite.HeightOffGround = effect.zHeight;
      else if (effect.orphaned && !vfxSprite.IsPerpendicular)
        vfxSprite.HeightOffGround = 0.0f;
      else
        vfxSprite.HeightOffGround = 0.9f;
      vfxSprite.UpdateZDepth();
    });
    this.InternalSpawnAtLocation(position, zRotation, parent, sourceNormal, sourceVelocity, vfxSpriteManipulator, keepReferences, spawnMethod, ignoresPools);
  }

  public void RemoveDespawnedVfx()
  {
    for (int index = this.m_spawnedObjects.Count - 1; index >= 0; --index)
    {
      if (!(bool) (UnityEngine.Object) this.m_spawnedObjects[index] || !this.m_spawnedObjects[index].activeSelf)
        this.m_spawnedObjects.RemoveAt(index);
    }
  }

  public void DestroyAll()
  {
    for (int index = 0; index < this.m_spawnedObjects.Count; ++index)
    {
      if ((bool) (UnityEngine.Object) this.m_spawnedObjects[index])
      {
        if ((bool) (UnityEngine.Object) SpawnManager.Instance)
          this.m_spawnedObjects[index].transform.parent = SpawnManager.Instance.VFX;
        SpawnManager.Despawn(this.m_spawnedObjects[index]);
      }
    }
    this.m_spawnedObjects.Clear();
  }

  public void ForEach(Action<GameObject> action)
  {
    for (int index = 0; index < this.m_spawnedObjects.Count; ++index)
    {
      if ((bool) (UnityEngine.Object) this.m_spawnedObjects[index])
        action(this.m_spawnedObjects[index]);
    }
  }

  public void ToggleRenderers(bool value)
  {
    for (int index = 0; index < this.m_spawnedObjects.Count; ++index)
    {
      if ((bool) (UnityEngine.Object) this.m_spawnedObjects[index])
      {
        foreach (Renderer componentsInChild in this.m_spawnedObjects[index].GetComponentsInChildren<Renderer>())
          componentsInChild.enabled = value;
      }
    }
  }

  public void SetHeightOffGround(float height)
  {
    for (int index = 0; index < this.m_spawnedObjects.Count; ++index)
    {
      if ((bool) (UnityEngine.Object) this.m_spawnedObjects[index])
      {
        foreach (tk2dBaseSprite componentsInChild in this.m_spawnedObjects[index].GetComponentsInChildren<tk2dBaseSprite>())
        {
          componentsInChild.HeightOffGround = height;
          if ((UnityEngine.Object) componentsInChild.attachParent == (UnityEngine.Object) null)
            componentsInChild.UpdateZDepth();
        }
      }
    }
  }

  protected void HandleDebris(
    GameObject vfx,
    float heightOffGround,
    Vector2? sourceNormal,
    Vector2? sourceVelocity)
  {
    DebrisObject component = vfx.GetComponent<DebrisObject>();
    if (!((UnityEngine.Object) component != (UnityEngine.Object) null) || !sourceNormal.HasValue || !sourceVelocity.HasValue)
      return;
    if (!sourceNormal.HasValue)
      Debug.LogWarning((object) "Trying to create debris for an effect with no normal.");
    if (!sourceVelocity.HasValue)
      Debug.LogWarning((object) "Trying to create debris for an effect with no velocity.");
    tk2dBaseSprite sprite = component.sprite;
    sprite.IsPerpendicular = false;
    sprite.usesOverrideMaterial = true;
    Bounds bounds = sprite.GetBounds();
    component.transform.position = component.transform.position + new Vector3(BraveMathCollege.ActualSign(sourceNormal.Value.x) * bounds.size.x, 0.0f, 0.0f);
    float z = Mathf.Atan2(sourceVelocity.Value.y, sourceVelocity.Value.x) * 57.29578f;
    vfx.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, z);
    Vector2 normalized = BraveMathCollege.ReflectVectorAcrossNormal(sourceVelocity.Value, sourceNormal.Value).normalized;
    Vector2 vector = (Vector2) (Quaternion.Euler(0.0f, 0.0f, UnityEngine.Random.Range(-20f, 20f)) * (Vector3) normalized);
    component.Trigger(vector.ToVector3ZUp(0.1f), heightOffGround + 1f);
  }

  protected void HandleAttachment(tk2dSprite vfxSprite, Transform parent)
  {
    if (!((UnityEngine.Object) parent != (UnityEngine.Object) null) || !((UnityEngine.Object) parent.parent != (UnityEngine.Object) null))
      return;
    tk2dSprite componentInChildren = parent.parent.GetComponentInChildren<tk2dSprite>();
    if (!((UnityEngine.Object) vfxSprite != (UnityEngine.Object) null) || !((UnityEngine.Object) componentInChildren != (UnityEngine.Object) null))
      return;
    componentInChildren.AttachRenderer((tk2dBaseSprite) vfxSprite);
  }

  protected void InternalSpawnAtLocation(
    Vector3 position,
    float zRotation,
    Transform parent,
    Vector2? sourceNormal,
    Vector2? sourceVelocity,
    Action<VFXObject, tk2dSprite> vfxSpriteManipulator,
    bool keepReferences,
    VFXComplex.SpawnMethod spawnMethod,
    bool ignoresPools)
  {
    if (spawnMethod == null)
    {
      // ISSUE: reference to a compiler-generated field
      if (VFXComplex.\u003C\u003Ef__mg\u0024cache0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        VFXComplex.\u003C\u003Ef__mg\u0024cache0 = new VFXComplex.SpawnMethod(SpawnManager.SpawnVFX);
      }
      // ISSUE: reference to a compiler-generated field
      spawnMethod = VFXComplex.\u003C\u003Ef__mg\u0024cache0;
    }
    this.m_spawnedObjects.RemoveAll((Predicate<GameObject>) (go => !(bool) (UnityEngine.Object) go));
    for (int index = 0; index < this.effects.Length; ++index)
    {
      if (!((UnityEngine.Object) this.effects[index].effect == (UnityEngine.Object) null))
      {
        if (this.effects[index].alignment == VFXAlignment.NormalAligned && sourceNormal.HasValue)
          zRotation = Mathf.Atan2(sourceNormal.Value.y, sourceNormal.Value.x) * 57.29578f;
        if (this.effects[index].alignment == VFXAlignment.VelocityAligned && sourceVelocity.HasValue)
          zRotation = (float) ((double) Mathf.Atan2(sourceVelocity.Value.y, sourceVelocity.Value.x) * 57.295780181884766 + 180.0);
        Vector3 position1 = position.Quantize(1f / 16f);
        GameObject vfx = spawnMethod(this.effects[index].effect, position1, Quaternion.identity, ignoresPools);
        if ((bool) (UnityEngine.Object) vfx)
        {
          if (keepReferences && !this.effects[index].persistsOnDeath)
            this.m_spawnedObjects.Add(vfx);
          tk2dSprite componentInChildren = vfx.GetComponentInChildren<tk2dSprite>();
          if ((UnityEngine.Object) componentInChildren != (UnityEngine.Object) null)
          {
            vfxSpriteManipulator(this.effects[index], componentInChildren);
            if (this.effects[index].usesZHeight)
            {
              componentInChildren.HeightOffGround = this.effects[index].zHeight;
              componentInChildren.UpdateZDepth();
            }
          }
          if ((bool) (UnityEngine.Object) vfx.GetComponent<ParticleSystem>())
          {
            ParticleKiller component = vfx.GetComponent<ParticleKiller>();
            vfx.transform.localRotation = !(bool) (UnityEngine.Object) component || !component.overrideXRotation ? Quaternion.Euler(-90f, 0.0f, 0.0f) : Quaternion.Euler(component.xRotation, 0.0f, 0.0f);
            vfx.transform.position = vfx.transform.position.WithZ(vfx.transform.position.y);
          }
          else
            vfx.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, zRotation);
          this.HandleDebris(vfx, 0.5f, sourceNormal, sourceVelocity);
          if ((UnityEngine.Object) parent != (UnityEngine.Object) null)
          {
            if (!this.effects[index].orphaned)
            {
              vfx.transform.parent = parent;
              vfx.transform.localScale = Vector3.one;
              PersistentVFXManagerBehaviour managerBehaviour = parent.GetComponentInChildren<PersistentVFXManagerBehaviour>() ?? parent.GetComponentInParent<PersistentVFXManagerBehaviour>();
              if ((UnityEngine.Object) managerBehaviour != (UnityEngine.Object) null && !(bool) (UnityEngine.Object) vfx.GetComponent<SpriteAnimatorKiller>())
              {
                if (this.effects[index].destructible)
                  managerBehaviour.AttachDestructibleVFX(vfx);
                else
                  managerBehaviour.AttachPersistentVFX(vfx);
              }
              if (this.effects[index].attached)
                this.HandleAttachment(componentInChildren, parent);
            }
            else
              vfx.transform.localScale = parent.localScale;
          }
        }
      }
    }
  }

  public delegate GameObject SpawnMethod(
    GameObject prefab,
    Vector3 position,
    Quaternion rotation,
    bool ignoresPools);
}
