// Decompiled with JetBrains decompiler
// Type: TrackInputDirectionalPad
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
public class TrackInputDirectionalPad : BraveBehaviour
{
  public float InputLifetime = 2f;
  public Projectile HadoukenProjectile;
  public GrappleModule grappleModule;
  private List<TrackInputDirectionalPad.TrackedKeyInput> m_trackedInput;
  private TrackInputDirectionalPad.TrackedKeyInput? m_lastInput;
  private bool m_hasNulledInput;
  private Gun m_gun;
  private int m_hadoukenCounter;

  private void Awake()
  {
    this.m_gun = this.GetComponent<Gun>();
    this.m_trackedInput = new List<TrackInputDirectionalPad.TrackedKeyInput>();
    this.grappleModule.sourceGameObject = this.gameObject;
  }

  private void Update()
  {
    if (!(bool) (UnityEngine.Object) this.m_gun || !(bool) (UnityEngine.Object) this.m_gun.CurrentOwner || !((UnityEngine.Object) this.m_gun.CurrentOwner.CurrentGun == (UnityEngine.Object) this.m_gun))
      return;
    this.AddNewInputs();
    this.CheckSequences();
    this.DropOldInputs();
  }

  private void CheckSequences()
  {
    if (GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.END_TIMES)
      return;
    for (int index = 0; index < this.m_trackedInput.Count; ++index)
    {
      TrackInputDirectionalPad.TrackedKeyInput trackedKeyInput = this.m_trackedInput[index];
      if (trackedKeyInput.sourceKey == TrackInputDirectionalPad.TrackInputSequenceKey.DOWN && index < this.m_trackedInput.Count - 2 && this.m_trackedInput[index + 1].sourceKey == TrackInputDirectionalPad.TrackInputSequenceKey.RIGHT && this.m_trackedInput[index + 2].sourceKey == TrackInputDirectionalPad.TrackInputSequenceKey.A)
      {
        this.m_trackedInput.RemoveAt(index + 2);
        this.m_trackedInput.RemoveAt(index + 1);
        this.m_trackedInput.RemoveAt(index);
        --index;
        this.m_hadoukenCounter = 0;
        this.m_gun.OnPreFireProjectileModifier += new Func<Gun, Projectile, ProjectileModule, Projectile>(this.HadoukenPrefireProjectileModifier);
      }
      if (trackedKeyInput.sourceKey == TrackInputDirectionalPad.TrackInputSequenceKey.LEFT && index < this.m_trackedInput.Count - 2 && this.m_trackedInput[index + 1].sourceKey == TrackInputDirectionalPad.TrackInputSequenceKey.LEFT && this.m_trackedInput[index + 2].sourceKey == TrackInputDirectionalPad.TrackInputSequenceKey.A)
      {
        this.m_trackedInput.RemoveAt(index + 2);
        this.m_trackedInput.RemoveAt(index + 1);
        this.m_trackedInput.RemoveAt(index);
        --index;
        this.grappleModule.ForceEndGrappleImmediate();
        this.grappleModule.Trigger(this.m_gun.CurrentOwner as PlayerController);
      }
    }
  }

  private Projectile HadoukenPrefireProjectileModifier(
    Gun sourceGun,
    Projectile sourceProjectile,
    ProjectileModule sourceModule)
  {
    ++this.m_hadoukenCounter;
    if ((bool) (UnityEngine.Object) this.m_gun && this.m_hadoukenCounter >= sourceGun.Volley.projectiles.Count)
      this.m_gun.OnPreFireProjectileModifier -= new Func<Gun, Projectile, ProjectileModule, Projectile>(this.HadoukenPrefireProjectileModifier);
    return this.HadoukenProjectile;
  }

  private void DropOldInputs()
  {
    float realtimeSinceStartup = UnityEngine.Time.realtimeSinceStartup;
    while (this.m_trackedInput.Count > 0 && (double) realtimeSinceStartup - (double) this.m_trackedInput[0].sourceTime > (double) this.InputLifetime)
      this.m_trackedInput.RemoveAt(0);
  }

  private void AddNewInputs()
  {
    float realtimeSinceStartup = UnityEngine.Time.realtimeSinceStartup;
    if (this.m_trackedInput.Count == 0)
      this.m_lastInput = new TrackInputDirectionalPad.TrackedKeyInput?();
    bool flag = false;
    TrackInputDirectionalPad.TrackedKeyInput trackedKeyInput = new TrackInputDirectionalPad.TrackedKeyInput(TrackInputDirectionalPad.TrackInputSequenceKey.A, realtimeSinceStartup);
    BraveInput instanceForPlayer = BraveInput.GetInstanceForPlayer((this.m_gun.CurrentOwner as PlayerController).PlayerIDX);
    if ((bool) (UnityEngine.Object) instanceForPlayer && instanceForPlayer.ActiveActions != null)
    {
      GungeonActions activeActions = instanceForPlayer.ActiveActions;
      if (activeActions.ShootAction.WasPressed)
      {
        flag = true;
        trackedKeyInput = new TrackInputDirectionalPad.TrackedKeyInput(TrackInputDirectionalPad.TrackInputSequenceKey.A, realtimeSinceStartup);
      }
      if (activeActions.DodgeRollAction.WasPressed)
      {
        flag = true;
        trackedKeyInput = new TrackInputDirectionalPad.TrackedKeyInput(TrackInputDirectionalPad.TrackInputSequenceKey.B, realtimeSinceStartup);
      }
      if (!flag)
      {
        Vector2 vector = activeActions.Move.Vector;
        Vector2 majorAxis = BraveUtility.GetMajorAxis(vector);
        if ((double) Mathf.Abs(vector.x) < 0.10000000149011612 && (double) Mathf.Abs(vector.y) < 0.10000000149011612)
          this.m_hasNulledInput = true;
        else if ((double) majorAxis.x > 0.0)
        {
          flag = true;
          trackedKeyInput = new TrackInputDirectionalPad.TrackedKeyInput(TrackInputDirectionalPad.TrackInputSequenceKey.RIGHT, realtimeSinceStartup);
        }
        else if ((double) majorAxis.x < 0.0)
        {
          flag = true;
          trackedKeyInput = new TrackInputDirectionalPad.TrackedKeyInput(TrackInputDirectionalPad.TrackInputSequenceKey.LEFT, realtimeSinceStartup);
        }
        else if ((double) majorAxis.y > 0.0)
        {
          flag = true;
          trackedKeyInput = new TrackInputDirectionalPad.TrackedKeyInput(TrackInputDirectionalPad.TrackInputSequenceKey.UP, realtimeSinceStartup);
        }
        else if ((double) majorAxis.y < 0.0)
        {
          flag = true;
          trackedKeyInput = new TrackInputDirectionalPad.TrackedKeyInput(TrackInputDirectionalPad.TrackInputSequenceKey.DOWN, realtimeSinceStartup);
        }
      }
    }
    if (flag && !this.m_hasNulledInput && this.m_lastInput.HasValue && this.m_lastInput.Value.sourceKey == trackedKeyInput.sourceKey)
      flag = false;
    if (!flag)
      return;
    this.m_trackedInput.Add(trackedKeyInput);
    this.m_lastInput = new TrackInputDirectionalPad.TrackedKeyInput?(trackedKeyInput);
    this.m_hasNulledInput = false;
  }

  public enum TrackInputSequenceKey
  {
    UP,
    RIGHT,
    DOWN,
    LEFT,
    A,
    B,
  }

  protected struct TrackedKeyInput(TrackInputDirectionalPad.TrackInputSequenceKey key, float t)
  {
    public TrackInputDirectionalPad.TrackInputSequenceKey sourceKey = key;
    public float sourceTime = t;
  }
}
