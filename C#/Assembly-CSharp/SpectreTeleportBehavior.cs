// Decompiled with JetBrains decompiler
// Type: SpectreTeleportBehavior
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using FullInspector;
using Pathfinding;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
public class SpectreTeleportBehavior : BasicAttackBehavior
{
  public bool AttackableDuringAnimation;
  public bool AvoidWalls;
  public float GoneTime = 1f;
  public float HauntTime = 1f;
  public float HauntDistance = 5f;
  public List<AIAnimator> HauntCopies;
  [InspectorCategory("Attack")]
  public GameObject ShootPoint;
  [InspectorCategory("Attack")]
  public BulletScriptSelector hauntBulletScript;
  [InspectorCategory("Visuals")]
  public string teleportOutAnim = "teleport_out";
  [InspectorCategory("Visuals")]
  public string teleportInAttackAnim = "teleport_in";
  [InspectorCategory("Visuals")]
  public string teleportOutAttackAnim = "teleport_out";
  [InspectorCategory("Visuals")]
  public string teleportInAnim = "teleport_in";
  [InspectorCategory("Visuals")]
  public bool teleportRequiresTransparency;
  [InspectorCategory("Visuals")]
  public SpectreTeleportBehavior.ShadowSupport shadowSupport;
  [InspectorCategory("Visuals")]
  [InspectorShowIf("ShowShadowAnimationNames")]
  public string shadowOutAnim;
  [InspectorCategory("Visuals")]
  [InspectorShowIf("ShowShadowAnimationNames")]
  public string shadowInAnim;
  private Shader m_cachedShader;
  private List<SpectreTeleportBehavior.SpecterInfo> m_allSpectres;
  private BulletScriptSource m_bulletSource;
  private float m_timer;
  private float m_hauntAngle;
  private Vector2 m_centerOffset;
  private SpectreTeleportBehavior.TeleportState m_state;

  private bool ShowShadowAnimationNames()
  {
    return this.shadowSupport == SpectreTeleportBehavior.ShadowSupport.Animate;
  }

  public override void Start()
  {
    base.Start();
    PhysicsEngine.Instance.OnPostRigidbodyMovement += new System.Action(this.OnPostRigidbodyMovement);
    for (int index = 0; index < this.HauntCopies.Count; ++index)
    {
      this.HauntCopies[index].aiActor = this.m_aiActor;
      this.HauntCopies[index].healthHaver = this.m_aiActor.healthHaver;
    }
  }

  public override void Upkeep()
  {
    base.Upkeep();
    this.DecrementTimer(ref this.m_timer);
  }

  public override BehaviorResult Update()
  {
    int num = (int) base.Update();
    if (this.m_allSpectres == null)
    {
      this.m_allSpectres = new List<SpectreTeleportBehavior.SpecterInfo>(this.HauntCopies.Count + 1);
      tk2dBaseSprite component = this.m_aiActor.ShadowObject.GetComponent<tk2dBaseSprite>();
      this.m_allSpectres.Add(new SpectreTeleportBehavior.SpecterInfo()
      {
        aiAnimator = this.m_aiAnimator,
        shadowSprite = component
      });
      for (int index = 0; index < this.HauntCopies.Count; ++index)
      {
        tk2dBaseSprite attachment = UnityEngine.Object.Instantiate<tk2dBaseSprite>(component);
        attachment.transform.parent = this.HauntCopies[index].transform;
        attachment.transform.localPosition = component.transform.localPosition;
        this.HauntCopies[index].sprite.AttachRenderer(attachment);
        attachment.UpdateZDepth();
        this.m_allSpectres.Add(new SpectreTeleportBehavior.SpecterInfo()
        {
          aiAnimator = this.HauntCopies[index],
          shadowSprite = attachment
        });
      }
    }
    if (!this.IsReady() || !(bool) (UnityEngine.Object) this.m_aiActor.TargetRigidbody)
      return BehaviorResult.Continue;
    this.State = SpectreTeleportBehavior.TeleportState.TeleportOut;
    this.m_updateEveryFrame = true;
    return BehaviorResult.RunContinuous;
  }

  public override ContinuousBehaviorResult ContinuousUpdate()
  {
    int num = (int) base.ContinuousUpdate();
    if (this.State == SpectreTeleportBehavior.TeleportState.TeleportOut)
    {
      if (this.shadowSupport == SpectreTeleportBehavior.ShadowSupport.Fade)
      {
        for (int index = 0; index < this.m_allSpectres.Count; ++index)
        {
          if ((bool) (UnityEngine.Object) this.m_allSpectres[index].shadowSprite)
            this.m_allSpectres[index].shadowSprite.color = this.m_allSpectres[index].shadowSprite.color.WithAlpha(1f - this.m_aiAnimator.CurrentClipProgress);
        }
      }
      if (!this.m_aiAnimator.IsPlaying(this.teleportOutAnim))
        this.State = (double) this.GoneTime <= 0.0 ? SpectreTeleportBehavior.TeleportState.HauntIn : SpectreTeleportBehavior.TeleportState.Gone1;
    }
    else if (this.State == SpectreTeleportBehavior.TeleportState.Gone1)
    {
      if ((double) this.m_timer <= 0.0)
        this.State = SpectreTeleportBehavior.TeleportState.HauntIn;
    }
    else if (this.State == SpectreTeleportBehavior.TeleportState.HauntIn)
    {
      if (this.shadowSupport == SpectreTeleportBehavior.ShadowSupport.Fade)
      {
        for (int index = 0; index < this.m_allSpectres.Count; ++index)
        {
          if ((bool) (UnityEngine.Object) this.m_allSpectres[index].shadowSprite)
            this.m_allSpectres[index].shadowSprite.color = this.m_allSpectres[index].shadowSprite.color.WithAlpha(this.m_aiAnimator.CurrentClipProgress);
        }
      }
      this.m_aiShooter.ToggleGunAndHandRenderers(false, nameof (SpectreTeleportBehavior));
      if (!this.m_aiAnimator.IsPlaying(this.teleportInAttackAnim))
        this.State = SpectreTeleportBehavior.TeleportState.Haunt;
    }
    else if (this.State == SpectreTeleportBehavior.TeleportState.Haunt)
    {
      if ((double) this.m_timer <= 0.0)
        this.State = SpectreTeleportBehavior.TeleportState.HauntOut;
    }
    else if (this.State == SpectreTeleportBehavior.TeleportState.HauntOut)
    {
      if (this.shadowSupport == SpectreTeleportBehavior.ShadowSupport.Fade)
      {
        for (int index = 0; index < this.m_allSpectres.Count; ++index)
        {
          if ((bool) (UnityEngine.Object) this.m_allSpectres[index].shadowSprite)
            this.m_allSpectres[index].shadowSprite.color = this.m_allSpectres[index].shadowSprite.color.WithAlpha(1f - this.m_aiAnimator.CurrentClipProgress);
        }
      }
      if (!this.m_aiAnimator.IsPlaying(this.teleportOutAttackAnim))
        this.State = (double) this.GoneTime <= 0.0 ? SpectreTeleportBehavior.TeleportState.TeleportIn : SpectreTeleportBehavior.TeleportState.Gone2;
    }
    else if (this.State == SpectreTeleportBehavior.TeleportState.Gone2)
    {
      if ((double) this.m_timer <= 0.0)
        this.State = SpectreTeleportBehavior.TeleportState.TeleportIn;
    }
    else if (this.State == SpectreTeleportBehavior.TeleportState.TeleportIn)
    {
      if (this.shadowSupport == SpectreTeleportBehavior.ShadowSupport.Fade)
      {
        for (int index = 0; index < this.m_allSpectres.Count; ++index)
        {
          if ((bool) (UnityEngine.Object) this.m_allSpectres[index].shadowSprite)
            this.m_allSpectres[index].shadowSprite.color = this.m_allSpectres[index].shadowSprite.color.WithAlpha(this.m_aiAnimator.CurrentClipProgress);
        }
      }
      this.m_aiShooter.ToggleGunAndHandRenderers(false, nameof (SpectreTeleportBehavior));
      if (!this.m_aiAnimator.IsPlaying(this.teleportInAnim))
      {
        this.State = SpectreTeleportBehavior.TeleportState.None;
        return ContinuousBehaviorResult.Finished;
      }
    }
    return ContinuousBehaviorResult.Continue;
  }

  public override void EndContinuousUpdate()
  {
    base.EndContinuousUpdate();
    this.m_updateEveryFrame = false;
    this.UpdateCooldowns();
  }

  public override bool IsOverridable() => false;

  public override void OnActorPreDeath()
  {
    for (int index = 0; index < this.HauntCopies.Count; ++index)
      this.HauntCopies[index].PlayUntilCancelled("die", true);
    PhysicsEngine.Instance.OnPostRigidbodyMovement -= new System.Action(this.OnPostRigidbodyMovement);
    base.OnActorPreDeath();
  }

  public void OnPostRigidbodyMovement()
  {
    if (this.State != SpectreTeleportBehavior.TeleportState.HauntIn && this.State != SpectreTeleportBehavior.TeleportState.Haunt && this.State != SpectreTeleportBehavior.TeleportState.HauntOut)
      return;
    Vector2 unitCenter = this.m_aiActor.TargetRigidbody.GetUnitCenter(ColliderType.HitBox);
    float num = 360f / (float) this.m_allSpectres.Count;
    for (int index = 0; index < this.m_allSpectres.Count; ++index)
    {
      Vector2 vector = BraveMathCollege.DegreesToVector(this.m_hauntAngle + (float) index * num, this.HauntDistance);
      this.m_allSpectres[index].transform.position = (Vector3) (unitCenter + vector + this.m_centerOffset);
      this.m_allSpectres[index].specRigidbody.Reinitialize();
    }
  }

  private SpectreTeleportBehavior.TeleportState State
  {
    get => this.m_state;
    set
    {
      this.EndState(this.m_state);
      this.m_state = value;
      this.BeginState(this.m_state);
    }
  }

  private void BeginState(SpectreTeleportBehavior.TeleportState state)
  {
    switch (state)
    {
      case SpectreTeleportBehavior.TeleportState.TeleportOut:
      case SpectreTeleportBehavior.TeleportState.HauntOut:
        if (this.teleportRequiresTransparency)
          this.m_cachedShader = this.m_aiActor.renderer.material.shader;
        for (int index = 0; index < this.m_allSpectres.Count; ++index)
        {
          if (this.teleportRequiresTransparency)
          {
            this.m_allSpectres[index].sprite.usesOverrideMaterial = true;
            this.m_allSpectres[index].renderer.material.shader = ShaderCache.Acquire("Brave/LitBlendUber");
          }
          string name = state != SpectreTeleportBehavior.TeleportState.TeleportOut ? this.teleportOutAttackAnim : this.teleportOutAnim;
          this.m_allSpectres[index].aiAnimator.PlayUntilCancelled(name, true);
          if (this.shadowSupport == SpectreTeleportBehavior.ShadowSupport.Animate && (bool) (UnityEngine.Object) this.m_allSpectres[index].shadowSprite)
            this.m_allSpectres[index].shadowSprite.spriteAnimator.PlayAndForceTime(this.shadowOutAnim, this.m_aiAnimator.CurrentClipLength);
          if (!this.AttackableDuringAnimation)
            this.m_allSpectres[index].specRigidbody.CollideWithOthers = false;
        }
        this.m_aiShooter.ToggleGunAndHandRenderers(false, nameof (SpectreTeleportBehavior));
        this.m_aiActor.ClearPath();
        break;
      case SpectreTeleportBehavior.TeleportState.Gone1:
      case SpectreTeleportBehavior.TeleportState.Gone2:
        this.m_timer = this.GoneTime;
        for (int index = 0; index < this.m_allSpectres.Count; ++index)
        {
          this.m_allSpectres[index].specRigidbody.CollideWithOthers = false;
          this.m_allSpectres[index].sprite.renderer.enabled = false;
        }
        break;
      case SpectreTeleportBehavior.TeleportState.HauntIn:
      case SpectreTeleportBehavior.TeleportState.TeleportIn:
        if (state == SpectreTeleportBehavior.TeleportState.TeleportIn)
        {
          this.DoTeleport();
          this.m_aiAnimator.PlayUntilFinished(this.teleportInAnim, true);
        }
        else
        {
          Vector2 unitCenter = this.m_aiActor.TargetRigidbody.GetUnitCenter(ColliderType.HitBox);
          this.m_hauntAngle = (float) UnityEngine.Random.Range(0, 360);
          this.m_centerOffset = this.m_aiActor.transform.position.XY() - this.m_aiActor.specRigidbody.GetUnitCenter(ColliderType.HitBox);
          float num = 360f / (float) this.m_allSpectres.Count;
          for (int index = 0; index < this.m_allSpectres.Count; ++index)
          {
            Vector2 vector = BraveMathCollege.DegreesToVector(this.m_hauntAngle + (float) index * num, this.HauntDistance);
            if (index > 0)
            {
              this.m_allSpectres[index].gameObject.SetActive(true);
              this.m_allSpectres[index].specRigidbody.enabled = true;
            }
            this.m_allSpectres[index].transform.position = (Vector3) (unitCenter + vector + this.m_centerOffset);
            this.m_allSpectres[index].specRigidbody.Reinitialize();
            this.m_allSpectres[index].aiAnimator.PlayUntilFinished(this.teleportInAttackAnim, true);
          }
        }
        for (int index = 0; index < this.m_allSpectres.Count; ++index)
        {
          if ((bool) (UnityEngine.Object) this.m_allSpectres[index].shadowSprite)
          {
            if (this.shadowSupport == SpectreTeleportBehavior.ShadowSupport.Animate)
              this.m_allSpectres[index].shadowSprite.spriteAnimator.PlayAndForceTime(this.shadowInAnim, this.m_aiAnimator.CurrentClipLength);
            this.m_allSpectres[index].shadowSprite.renderer.enabled = true;
          }
          if (this.AttackableDuringAnimation)
            this.m_allSpectres[index].specRigidbody.CollideWithOthers = true;
          this.m_allSpectres[index].sprite.renderer.enabled = true;
          SpriteOutlineManager.ToggleOutlineRenderers(this.m_allSpectres[index].sprite, true);
        }
        break;
      case SpectreTeleportBehavior.TeleportState.Haunt:
        this.Fire();
        this.m_timer = this.HauntTime;
        for (int index = 0; index < this.m_allSpectres.Count; ++index)
        {
          this.m_allSpectres[index].specRigidbody.CollideWithOthers = true;
          this.m_allSpectres[index].specRigidbody.CollideWithTileMap = false;
          this.m_allSpectres[index].aiAnimator.LockFacingDirection = true;
          this.m_allSpectres[index].aiAnimator.FacingDirection = -90f;
        }
        break;
    }
  }

  private void EndState(SpectreTeleportBehavior.TeleportState state)
  {
    switch (state)
    {
      case SpectreTeleportBehavior.TeleportState.TeleportOut:
      case SpectreTeleportBehavior.TeleportState.HauntOut:
        for (int index = 0; index < this.m_allSpectres.Count; ++index)
        {
          if ((bool) (UnityEngine.Object) this.m_allSpectres[index].shadowSprite)
            this.m_allSpectres[index].renderer.enabled = false;
          SpriteOutlineManager.ToggleOutlineRenderers(this.m_allSpectres[index].sprite, false);
        }
        if (state != SpectreTeleportBehavior.TeleportState.HauntOut)
          break;
        for (int index = 1; index < this.m_allSpectres.Count; ++index)
        {
          this.m_allSpectres[index].gameObject.SetActive(false);
          this.m_allSpectres[index].specRigidbody.enabled = false;
        }
        break;
      case SpectreTeleportBehavior.TeleportState.HauntIn:
      case SpectreTeleportBehavior.TeleportState.TeleportIn:
        for (int index = 0; index < this.m_allSpectres.Count; ++index)
        {
          if (this.teleportRequiresTransparency)
          {
            this.m_allSpectres[index].sprite.usesOverrideMaterial = false;
            this.m_allSpectres[index].renderer.material.shader = this.m_cachedShader;
          }
          if (this.shadowSupport == SpectreTeleportBehavior.ShadowSupport.Fade && (bool) (UnityEngine.Object) this.m_allSpectres[index].shadowSprite)
            this.m_allSpectres[index].shadowSprite.color = this.m_allSpectres[index].shadowSprite.color.WithAlpha(1f);
          this.m_allSpectres[index].specRigidbody.CollideWithOthers = true;
        }
        if (state != SpectreTeleportBehavior.TeleportState.TeleportIn)
          break;
        this.m_aiShooter.ToggleGunAndHandRenderers(true, nameof (SpectreTeleportBehavior));
        break;
      case SpectreTeleportBehavior.TeleportState.Haunt:
        for (int index = 0; index < this.m_allSpectres.Count; ++index)
        {
          this.m_allSpectres[index].specRigidbody.CollideWithTileMap = true;
          this.m_allSpectres[index].aiAnimator.LockFacingDirection = false;
        }
        break;
    }
  }

  private void Fire()
  {
    if (!(bool) (UnityEngine.Object) this.m_bulletSource)
      this.m_bulletSource = this.ShootPoint.GetOrAddComponent<BulletScriptSource>();
    this.m_bulletSource.BulletManager = this.m_aiActor.bulletBank;
    this.m_bulletSource.BulletScript = this.hauntBulletScript;
    this.m_bulletSource.Initialize();
  }

  private void DoTeleport()
  {
    IntVector2? targetCenter = new IntVector2?();
    if ((bool) (UnityEngine.Object) this.m_aiActor.TargetRigidbody)
      targetCenter = new IntVector2?(this.m_aiActor.TargetRigidbody.UnitCenter.ToIntVector2(VectorConversions.Floor));
    Vector2 worldpoint1 = (Vector2) BraveUtility.ViewportToWorldpoint(new Vector2(0.0f, 0.0f), ViewportType.Gameplay);
    Vector2 worldpoint2 = (Vector2) BraveUtility.ViewportToWorldpoint(new Vector2(1f, 1f), ViewportType.Gameplay);
    IntVector2 bottomLeft = worldpoint1.ToIntVector2(VectorConversions.Ceil);
    IntVector2 topRight = worldpoint2.ToIntVector2(VectorConversions.Floor) - IntVector2.One;
    CellValidator cellValidator = (CellValidator) (c =>
    {
      for (int index1 = 0; index1 < this.m_aiActor.Clearance.x; ++index1)
      {
        for (int index2 = 0; index2 < this.m_aiActor.Clearance.y; ++index2)
        {
          if (GameManager.Instance.Dungeon.data.isTopWall(c.x + index1, c.y + index2) || this.State == SpectreTeleportBehavior.TeleportState.TeleportIn && targetCenter.HasValue && (double) IntVector2.DistanceSquared(targetCenter.Value, c.x + index1, c.y + index2) < 16.0 || this.State == SpectreTeleportBehavior.TeleportState.HauntIn && targetCenter.HasValue && (double) IntVector2.DistanceSquared(targetCenter.Value, c.x + index1, c.y + index2) > 4.0)
            return false;
        }
      }
      if (c.x < bottomLeft.x || c.y < bottomLeft.y || c.x + this.m_aiActor.Clearance.x - 1 > topRight.x || c.y + this.m_aiActor.Clearance.y - 1 > topRight.y)
        return false;
      if (this.AvoidWalls)
      {
        int num1 = -1;
        for (int index = -1; index < this.m_aiActor.Clearance.y + 1; ++index)
        {
          if (GameManager.Instance.Dungeon.data.isWall(c.x + num1, c.y + index))
            return false;
        }
        int x = this.m_aiActor.Clearance.x;
        for (int index = -1; index < this.m_aiActor.Clearance.y + 1; ++index)
        {
          if (GameManager.Instance.Dungeon.data.isWall(c.x + x, c.y + index))
            return false;
        }
        int num2 = -1;
        for (int index = -1; index < this.m_aiActor.Clearance.x + 1; ++index)
        {
          if (GameManager.Instance.Dungeon.data.isWall(c.x + index, c.y + num2))
            return false;
        }
        int y = this.m_aiActor.Clearance.y;
        for (int index = -1; index < this.m_aiActor.Clearance.x + 1; ++index)
        {
          if (GameManager.Instance.Dungeon.data.isWall(c.x + index, c.y + y))
            return false;
        }
      }
      return true;
    });
    Vector2 vector2 = this.m_aiActor.specRigidbody.UnitCenter - this.m_aiActor.transform.position.XY();
    IntVector2? randomAvailableCell = this.m_aiActor.ParentRoom.GetRandomAvailableCell(new IntVector2?(this.m_aiActor.Clearance), new CellTypes?(this.m_aiActor.PathableTiles), cellValidator: cellValidator);
    if (randomAvailableCell.HasValue)
    {
      this.m_aiActor.transform.position = (Vector3) (Pathfinder.GetClearanceOffset(randomAvailableCell.Value, this.m_aiActor.Clearance) - vector2);
      this.m_aiActor.specRigidbody.Reinitialize();
    }
    else
      Debug.LogWarning((object) "TELEPORT FAILED!", (UnityEngine.Object) this.m_aiActor);
  }

  public enum ShadowSupport
  {
    None,
    Fade,
    Animate,
  }

  private enum TeleportState
  {
    None,
    TeleportOut,
    Gone1,
    HauntIn,
    Haunt,
    HauntOut,
    Gone2,
    TeleportIn,
  }

  private class SpecterInfo
  {
    public AIAnimator aiAnimator;
    public tk2dBaseSprite shadowSprite;

    public GameObject gameObject => this.aiAnimator.gameObject;

    public Transform transform => this.aiAnimator.transform;

    public Renderer renderer => this.aiAnimator.renderer;

    public SpeculativeRigidbody specRigidbody => this.aiAnimator.specRigidbody;

    public tk2dBaseSprite sprite => this.aiAnimator.sprite;
  }
}
