// Decompiled with JetBrains decompiler
// Type: ElectricMushroom
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

public class ElectricMushroom : DungeonPlaceableBehaviour
  {
    public string[] ValidIdleAnims;
    public string[] ValidHitAnims;
    public float MinEmissive = 10f;
    public float MaxEmissive = 30f;
    public float PulsesPerSecond = 1f;
    public float DamageToEnemies = 6f;
    public tk2dSpriteAnimator ElectrifyVFX;
    private int AnimIndex = -1;
    private int EmissivePowerID = -1;
    private static bool m_updatedEmissiveThisFrame;
    private bool m_isFiring;
    private float m_fireCooldownTime;
    private float m_remainingFireTime;

    private void Awake()
    {
      this.EmissivePowerID = Shader.PropertyToID("_EmissivePower");
      this.AnimIndex = Random.Range(0, this.ValidIdleAnims.Length);
      IntVector2 key = this.transform.position.IntXY();
      if (!StaticReferenceManager.MushroomMap.ContainsKey(key))
        StaticReferenceManager.MushroomMap.Add(key, this);
      else
        UnityEngine.Debug.LogError((object) ("Duplicate mushroom at: " + (object) key));
    }

    [DebuggerHidden]
    private IEnumerator Start()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new ElectricMushroom__Startc__Iterator0()
      {
        _this = this
      };
    }

    private void Update()
    {
      if (ElectricMushroom.m_updatedEmissiveThisFrame)
        return;
      ElectricMushroom.m_updatedEmissiveThisFrame = true;
      this.sprite.renderer.sharedMaterial.SetFloat(this.EmissivePowerID, Mathf.Lerp(this.MinEmissive, this.MaxEmissive, Mathf.SmoothStep(0.0f, 1f, Mathf.PingPong(UnityEngine.Time.realtimeSinceStartup * this.PulsesPerSecond, 1f))));
    }

    private void LateUpdate() => ElectricMushroom.m_updatedEmissiveThisFrame = false;

    protected override void OnDestroy()
    {
      StaticReferenceManager.MushroomMap.Remove(this.transform.position.IntXY());
      base.OnDestroy();
    }

    private void TriggerNearby()
    {
      IntVector2 intVector2 = this.transform.position.IntXY();
      for (int index = 0; index < 4; ++index)
      {
        if (StaticReferenceManager.MushroomMap.ContainsKey(intVector2 + IntVector2.Cardinals[index]))
          StaticReferenceManager.MushroomMap[intVector2 + IntVector2.Cardinals[index]].Trigger();
      }
    }

    public void Trigger(bool IsPrimaryTarget = false)
    {
      if ((double) this.m_fireCooldownTime > 0.0)
      {
        if (!IsPrimaryTarget)
          return;
        this.StartCoroutine(this.FrameDelayedBreak());
      }
      else
        this.StartCoroutine(this.Trigger_CR(IsPrimaryTarget));
    }

    [DebuggerHidden]
    public IEnumerator Trigger_CR(bool IsPrimaryTarget = false)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new ElectricMushroom__Trigger_CRc__Iterator1()
      {
        IsPrimaryTarget = IsPrimaryTarget,
        _this = this
      };
    }

    [DebuggerHidden]
    private IEnumerator FrameDelayedBreak()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new ElectricMushroom__FrameDelayedBreakc__Iterator2()
      {
        _this = this
      };
    }

    [DebuggerHidden]
    private IEnumerator HandleFiring()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new ElectricMushroom__HandleFiringc__Iterator3()
      {
        _this = this
      };
    }

    public void Electrify()
    {
      this.ElectrifyVFX.renderer.enabled = true;
      this.ElectrifyVFX.PlayAndDisableRenderer(string.Empty);
      for (int index = 0; index < this.specRigidbody.PrimaryPixelCollider.TriggerCollisions.Count; ++index)
      {
        TriggerCollisionData triggerCollision = this.specRigidbody.PrimaryPixelCollider.TriggerCollisions[index];
        if ((Object) triggerCollision.SpecRigidbody.gameActor != (Object) null && !triggerCollision.SpecRigidbody.gameActor.IsFlying)
        {
          if (triggerCollision.SpecRigidbody.gameActor is AIActor)
          {
            if ((bool) (Object) triggerCollision.SpecRigidbody.healthHaver)
              triggerCollision.SpecRigidbody.healthHaver.ApplyDamage(this.DamageToEnemies, Vector2.zero, StringTableManager.GetEnemiesString("#MUSHROOM"), CoreDamageTypes.Electric, DamageCategory.Environment);
          }
          else if ((bool) (Object) triggerCollision.SpecRigidbody.healthHaver)
            triggerCollision.SpecRigidbody.healthHaver.ApplyDamage(0.5f, Vector2.zero, StringTableManager.GetEnemiesString("#MUSHROOM"), CoreDamageTypes.Electric, DamageCategory.Environment);
        }
      }
    }

    private void HandleTriggerEnter(
      SpeculativeRigidbody specRigidbody,
      SpeculativeRigidbody sourceSpecRigidbody,
      CollisionData collisionData)
    {
      if (!((Object) specRigidbody.gameActor != (Object) null) || specRigidbody.gameActor.IsFlying || !(bool) (Object) specRigidbody.spriteAnimator && specRigidbody.spriteAnimator.QueryGroundedFrame())
        return;
      this.spriteAnimator.PlayForDuration(this.ValidHitAnims[this.AnimIndex], -1f, this.ValidIdleAnims[this.AnimIndex]);
      this.Trigger(true);
    }
  }

