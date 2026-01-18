// Decompiled with JetBrains decompiler
// Type: BulletVeilController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

public class BulletVeilController : BraveBehaviour
  {
    public tk2dSpriteAnimator VeilAnimator;
    public string OpenVeilAnimName;
    public string CloseVeilAnimName;
    public BulletCurtainParticleController[] ParticleControllers;
    public GameObject DepartureVFX;
    public GameObject ArrivalVFX;
    private bool m_isOpen;
    private bool m_hasWarped;
    private RoomHandler m_parentRoom;

    [DebuggerHidden]
    private IEnumerator Start()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new BulletVeilController__Startc__Iterator0()
      {
        _this = this
      };
    }

    private float OnPreWarp(PlayerController p)
    {
      if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER)
      {
        PlayerController otherPlayer = GameManager.Instance.GetOtherPlayer(p);
        if ((bool) (UnityEngine.Object) otherPlayer && otherPlayer.IsGhost)
          otherPlayer.ResurrectFromBossKill();
      }
      p.IsOnFire = false;
      p.CurrentFireMeterValue = 0.0f;
      p.CurrentPoisonMeterValue = 0.0f;
      if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER)
      {
        PlayerController otherPlayer = GameManager.Instance.GetOtherPlayer(p);
        if ((bool) (UnityEngine.Object) otherPlayer)
        {
          otherPlayer.IsOnFire = false;
          otherPlayer.CurrentFireMeterValue = 0.0f;
          otherPlayer.CurrentPoisonMeterValue = 0.0f;
        }
      }
      p.specRigidbody.Velocity = Vector2.zero;
      p.SetInputOverride("bullet veil");
      p.ToggleHandRenderers(false, "bullet veil");
      p.ToggleGunRenderers(false, "bullet veil");
      p.ToggleShadowVisiblity(false);
      p.ForceMoveToPoint(p.CenterPosition + Vector2.up * 3f, maximumTime: 0.5f);
      if ((UnityEngine.Object) this.DepartureVFX != (UnityEngine.Object) null)
        SpawnManager.SpawnVFX(this.DepartureVFX, p.CenterPosition.ToVector3ZisY(), Quaternion.identity);
      Minimap.Instance.ToggleMinimap(false);
      Minimap.Instance.TemporarilyPreventMinimap = true;
      GameUIRoot.Instance.HideCoreUI(string.Empty);
      GameUIRoot.Instance.ToggleLowerPanels(false, source: string.Empty);
      Pixelator.Instance.FadeToBlack(0.25f, holdTime: 0.25f);
      return 0.5f;
    }

    private void HandleDoorwayAnimationComplete(tk2dSpriteAnimator anim, tk2dSpriteAnimationClip clip)
    {
      anim.AnimationCompleted -= new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.HandleDoorwayAnimationComplete);
      anim.transform.parent.GetComponent<PlayerController>().IsVisible = false;
    }

    private void Update()
    {
      if (!this.m_isOpen && !this.m_hasWarped)
      {
        PlayerController playerClosestToPoint = GameManager.Instance.GetActivePlayerClosestToPoint(this.specRigidbody.UnitCenter);
        if (!((UnityEngine.Object) playerClosestToPoint != (UnityEngine.Object) null) || playerClosestToPoint.CurrentRoom != this.m_parentRoom || (double) Vector2.Distance(this.specRigidbody.UnitCenter, playerClosestToPoint.CenterPosition) >= 6.0)
          return;
        this.m_isOpen = true;
        this.VeilAnimator.Play(this.OpenVeilAnimName);
        this.StartCoroutine(this.HandleVeilParticles(false));
      }
      else
      {
        if (!this.m_isOpen || this.m_hasWarped)
          return;
        PlayerController playerClosestToPoint = GameManager.Instance.GetActivePlayerClosestToPoint(this.specRigidbody.UnitCenter);
        if (!((UnityEngine.Object) playerClosestToPoint != (UnityEngine.Object) null) || playerClosestToPoint.CurrentRoom != this.m_parentRoom || (double) Vector2.Distance(this.specRigidbody.UnitCenter, playerClosestToPoint.CenterPosition) <= 6.0)
          return;
        this.m_isOpen = false;
        this.StartCoroutine(this.HandleVeilParticles(true));
      }
    }

    [DebuggerHidden]
    private IEnumerator HandleVeilParticles(bool reverse)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new BulletVeilController__HandleVeilParticlesc__Iterator1()
      {
        reverse = reverse,
        _this = this
      };
    }

    private void ActivateEndTimes()
    {
      Minimap.Instance.ToggleMinimap(false);
      GameManager.Instance.Dungeon.IsEndTimes = true;
      Minimap.Instance.TemporarilyPreventMinimap = true;
      GameUIRoot.Instance.HideCoreUI(string.Empty);
      GameUIRoot.Instance.ToggleLowerPanels(false, source: string.Empty);
      for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
        GameManager.Instance.AllPlayers[index].CurrentInputState = PlayerInputState.FoyerInputOnly;
      UnityEngine.Object.FindObjectOfType<EndTimesNebulaController>().BecomeActive();
      Pixelator.Instance.DoOcclusionLayer = false;
    }

    private float OnWarping(PlayerController player)
    {
      player.ClearInputOverride("bullet veil");
      player.ToggleShadowVisiblity(true);
      player.ToggleHandRenderers(true, "bullet veil");
      player.ToggleGunRenderers(true, "bullet veil");
      player.IsVisible = true;
      this.GetComponent<WarpPointHandler>().OnWarping = (Func<PlayerController, float>) null;
      this.ActivateEndTimes();
      TimeTubeCreditsController.AcquireTunnelInstanceInAdvance();
      GameManager.Instance.DungeonMusicController.SwitchToEndTimesMusic();
      return 0.5f;
    }

    private float OnWarpDone(PlayerController player)
    {
      int num = (int) AkSoundEngine.PostEvent("State_ENV_Dimension_01", this.gameObject);
      Pixelator.Instance.FadeToBlack(0.25f, true, 0.1f);
      this.GetComponent<WarpPointHandler>().OnWarpDone = (Func<PlayerController, float>) null;
      for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
        GameManager.Instance.AllPlayers[index].DoSpinfallSpawn(0.1f * (float) (index + 1));
      if ((UnityEngine.Object) this.ArrivalVFX != (UnityEngine.Object) null)
        SpawnManager.SpawnVFX(this.ArrivalVFX, player.CenterPosition.ToVector3ZisY(), Quaternion.identity);
      return 0.4f;
    }

    protected override void OnDestroy() => base.OnDestroy();
  }

