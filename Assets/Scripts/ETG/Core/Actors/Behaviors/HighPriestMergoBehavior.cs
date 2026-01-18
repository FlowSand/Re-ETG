// Decompiled with JetBrains decompiler
// Type: HighPriestMergoBehavior
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using FullInspector;
using System.Collections.Generic;
using UnityEngine;

#nullable disable

[InspectorDropdownName("Bosses/HighPriest/MergoBehavior")]
public class HighPriestMergoBehavior : BasicAttackBehavior
  {
    public BulletScriptSelector shootBulletScript;
    public BulletScriptSelector wallBulletScript;
    public float darknessFadeTime = 1f;
    public float fireTime = 8f;
    public float fireMainMidTime = 0.8f;
    public float fireMainDist = 16f;
    public float fireMainDistVariance = 3f;
    public float fireWallMidTime = 0.5f;
    [InspectorCategory("Visuals")]
    public string teleportOutAnim;
    [InspectorCategory("Visuals")]
    public string teleportInAnim;
    private const float c_wallBuffer = 5f;
    private HighPriestMergoBehavior.State m_state;
    private tk2dBaseSprite m_shadowSprite;
    private float m_timer;
    private float m_mainShotTimer;
    private float m_wallShotTimer;
    private List<BulletScriptSource> m_shootBulletSources = new List<BulletScriptSource>();
    private BulletScriptSource m_wallBulletSource;

    public override void Start() => base.Start();

    public override void Upkeep()
    {
      base.Upkeep();
      this.DecrementTimer(ref this.m_timer);
    }

    public override BehaviorResult Update()
    {
      BehaviorResult behaviorResult = base.Update();
      if (behaviorResult != BehaviorResult.Continue)
        return behaviorResult;
      if (!this.IsReady())
        return BehaviorResult.Continue;
      this.m_shadowSprite = (tk2dBaseSprite) this.m_aiActor.ShadowObject.GetComponent<tk2dSprite>();
      this.m_state = HighPriestMergoBehavior.State.OutAnim;
      if (!this.m_aiActor.IsBlackPhantom)
      {
        this.m_aiActor.sprite.usesOverrideMaterial = true;
        this.m_aiActor.renderer.material.shader = ShaderCache.Acquire("Brave/LitBlendUber");
      }
      SpriteOutlineManager.ToggleOutlineRenderers(this.m_aiActor.sprite, false);
      this.m_aiAnimator.PlayUntilCancelled(this.teleportOutAnim);
      this.m_aiActor.specRigidbody.enabled = false;
      this.m_aiActor.ClearPath();
      this.m_aiActor.knockbackDoer.SetImmobile(true, "CrosshairBehavior");
      this.m_updateEveryFrame = true;
      return BehaviorResult.RunContinuous;
    }

    public override ContinuousBehaviorResult ContinuousUpdate()
    {
      int num = (int) base.ContinuousUpdate();
      if (this.m_state == HighPriestMergoBehavior.State.OutAnim)
      {
        this.m_shadowSprite.color = this.m_shadowSprite.color.WithAlpha(Mathf.Lerp(1f, 0.0f, this.m_aiAnimator.CurrentClipProgress));
        if (!this.m_aiAnimator.IsPlaying(this.teleportOutAnim))
        {
          this.m_state = HighPriestMergoBehavior.State.Fading;
          this.m_aiActor.ToggleRenderers(false);
          this.m_shadowSprite.color = this.m_shadowSprite.color.WithAlpha(0.0f);
          this.m_timer = this.darknessFadeTime;
          this.m_aiActor.ParentRoom.BecomeTerrifyingDarkRoom();
          return ContinuousBehaviorResult.Continue;
        }
      }
      else if (this.m_state == HighPriestMergoBehavior.State.Fading)
      {
        if ((double) this.m_timer <= 0.0)
        {
          this.m_state = HighPriestMergoBehavior.State.Firing;
          this.m_timer = this.fireTime;
          this.m_mainShotTimer = this.fireMainMidTime;
          this.m_wallShotTimer = this.fireWallMidTime;
          return ContinuousBehaviorResult.Continue;
        }
      }
      else if (this.m_state == HighPriestMergoBehavior.State.Firing)
      {
        if ((double) this.m_timer <= 0.0)
        {
          this.m_state = HighPriestMergoBehavior.State.Unfading;
          this.m_timer = this.darknessFadeTime;
          this.m_aiActor.ParentRoom.EndTerrifyingDarkRoom();
          return ContinuousBehaviorResult.Continue;
        }
        this.m_mainShotTimer -= this.m_deltaTime;
        if ((double) this.m_mainShotTimer < 0.0)
        {
          this.ShootBulletScript();
          this.m_mainShotTimer += this.fireMainMidTime;
        }
        this.m_wallShotTimer -= this.m_deltaTime;
        if ((double) this.m_wallShotTimer < 0.0)
        {
          this.ShootWallBulletScript();
          this.m_wallShotTimer += this.fireWallMidTime;
        }
      }
      else if (this.m_state == HighPriestMergoBehavior.State.Unfading)
      {
        if ((double) this.m_timer <= 0.0)
        {
          this.m_state = HighPriestMergoBehavior.State.InAnim;
          this.m_aiActor.ToggleRenderers(true);
          this.m_aiAnimator.PlayUntilFinished(this.teleportInAnim);
          return ContinuousBehaviorResult.Continue;
        }
      }
      else if (this.m_state == HighPriestMergoBehavior.State.InAnim)
      {
        this.m_shadowSprite.color = this.m_shadowSprite.color.WithAlpha(Mathf.Lerp(0.0f, 1f, this.m_aiAnimator.CurrentClipProgress));
        if (!this.m_aiAnimator.IsPlaying(this.teleportInAnim))
        {
          if (!this.m_aiActor.IsBlackPhantom)
          {
            this.m_aiActor.sprite.usesOverrideMaterial = false;
            this.m_aiActor.renderer.material.shader = ShaderCache.Acquire("Brave/LitCutoutUber");
          }
          SpriteOutlineManager.ToggleOutlineRenderers(this.m_aiActor.sprite, true);
          this.m_shadowSprite.color = this.m_shadowSprite.color.WithAlpha(1f);
          this.m_aiActor.specRigidbody.enabled = true;
          return ContinuousBehaviorResult.Finished;
        }
      }
      return ContinuousBehaviorResult.Continue;
    }

    public override void EndContinuousUpdate()
    {
      base.EndContinuousUpdate();
      this.m_aiAnimator.EndAnimationIf(this.teleportInAnim);
      this.m_aiAnimator.EndAnimationIf(this.teleportOutAnim);
      this.m_aiActor.ToggleRenderers(true);
      this.m_shadowSprite.color = this.m_shadowSprite.color.WithAlpha(1f);
      if (!this.m_aiActor.IsBlackPhantom)
      {
        this.m_aiActor.sprite.usesOverrideMaterial = false;
        this.m_aiActor.renderer.material.shader = ShaderCache.Acquire("Brave/LitCutoutUber");
      }
      SpriteOutlineManager.ToggleOutlineRenderers(this.m_aiActor.sprite, true);
      this.m_aiActor.ParentRoom.EndTerrifyingDarkRoom();
      this.m_aiActor.specRigidbody.enabled = true;
      this.m_state = HighPriestMergoBehavior.State.Idle;
      this.m_updateEveryFrame = false;
      this.UpdateCooldowns();
    }

    public override bool IsOverridable() => false;

    public override void OnActorPreDeath()
    {
      if (this.m_state == HighPriestMergoBehavior.State.Fading || this.m_state == HighPriestMergoBehavior.State.Firing)
        this.m_aiActor.ParentRoom.EndTerrifyingDarkRoom();
      base.OnActorPreDeath();
    }

    private void ShootBulletScript()
    {
      BulletScriptSource bulletScriptSource = (BulletScriptSource) null;
      for (int index = 0; index < this.m_shootBulletSources.Count; ++index)
      {
        if (this.m_shootBulletSources[index].IsEnded)
        {
          bulletScriptSource = this.m_shootBulletSources[index];
          break;
        }
      }
      if ((Object) bulletScriptSource == (Object) null)
      {
        bulletScriptSource = new GameObject("Mergo shoot point").AddComponent<BulletScriptSource>();
        this.m_shootBulletSources.Add(bulletScriptSource);
      }
      bulletScriptSource.transform.position = (Vector3) this.RandomShootPoint();
      bulletScriptSource.BulletManager = this.m_aiActor.bulletBank;
      bulletScriptSource.BulletScript = this.shootBulletScript;
      bulletScriptSource.Initialize();
    }

    private void ShootWallBulletScript()
    {
      float rotation;
      Vector2 a = this.RandomWallPoint(out rotation);
      for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
      {
        PlayerController allPlayer = GameManager.Instance.AllPlayers[index];
        if (!(bool) (Object) allPlayer || allPlayer.healthHaver.IsDead || (double) Vector2.Distance(a, allPlayer.CenterPosition) < 8.0)
          return;
      }
      if (!(bool) (Object) this.m_wallBulletSource)
        this.m_wallBulletSource = new GameObject("Mergo wall shoot point").AddComponent<BulletScriptSource>();
      this.m_wallBulletSource.transform.position = (Vector3) a;
      this.m_wallBulletSource.transform.rotation = Quaternion.Euler(0.0f, 0.0f, rotation);
      this.m_wallBulletSource.BulletManager = this.m_aiActor.bulletBank;
      this.m_wallBulletSource.BulletScript = this.wallBulletScript;
      this.m_wallBulletSource.Initialize();
    }

    private Vector2 RandomShootPoint()
    {
      Vector2 center = this.m_aiActor.ParentRoom.area.Center;
      if ((bool) (Object) this.m_aiActor.TargetRigidbody)
        this.m_aiActor.TargetRigidbody.GetUnitCenter(ColliderType.HitBox);
      float magnitude = this.fireMainDist + Random.Range(-this.fireMainDistVariance, this.fireMainDistVariance);
      List<Vector2> list = new List<Vector2>();
      DungeonData data = GameManager.Instance.Dungeon.data;
      for (int index = 0; index < 36; ++index)
      {
        Vector2 vector2 = center + BraveMathCollege.DegreesToVector((float) (index * 10), magnitude);
        if (!data.isWall((int) vector2.x, (int) vector2.y) && !data.isTopWall((int) vector2.x, (int) vector2.y))
          list.Add(vector2);
      }
      return BraveUtility.RandomElement<Vector2>(list);
    }

    private Vector2 RandomWallPoint(out float rotation)
    {
      float num = 4f;
      CellArea area = this.m_aiActor.ParentRoom.area;
      Vector2 vector2_1 = area.basePosition.ToVector2() + new Vector2(0.5f, 1.5f);
      Vector2 vector2_2 = (area.basePosition + area.dimensions).ToVector2() - new Vector2(0.5f, 0.5f);
      if (BraveUtility.RandomBool())
      {
        if (BraveUtility.RandomBool())
        {
          rotation = -90f;
          return new Vector2(Random.Range(vector2_1.x + 5f, vector2_2.x - 5f), (float) ((double) vector2_2.y + (double) num + 2.0));
        }
        rotation = 90f;
        return new Vector2(Random.Range(vector2_1.x + 5f, vector2_2.x - 5f), vector2_1.y - num);
      }
      if (BraveUtility.RandomBool())
      {
        rotation = 0.0f;
        return new Vector2(vector2_1.x - num, Random.Range(vector2_1.y + 5f, vector2_2.y - 5f));
      }
      rotation = 180f;
      return new Vector2(vector2_2.x + num, Random.Range(vector2_1.y + 5f, vector2_2.y - 5f));
    }

    private enum State
    {
      Idle,
      OutAnim,
      Fading,
      Firing,
      Unfading,
      InAnim,
    }
  }

