// Decompiled with JetBrains decompiler
// Type: InfinilichDeathController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

public class InfinilichDeathController : BraveBehaviour
  {
    public GameObject bigExplosionVfx;
    public GameObject finalExplosionVfx;
    public GameObject eyePos;

    public void Start()
    {
      this.healthHaver.ManualDeathHandling = true;
      this.healthHaver.OnPreDeath += new Action<Vector2>(this.OnBossDeath);
      this.healthHaver.SuppressContinuousKillCamBulletDestruction = true;
    }

    protected override void OnDestroy() => base.OnDestroy();

    private void OnBossDeath(Vector2 dir)
    {
      if (ChallengeManager.CHALLENGE_MODE_ACTIVE)
        ChallengeManager.Instance.ForceStop();
      this.aiAnimator.PlayUntilCancelled("death", true);
      GameManager.Instance.StartCoroutine(this.OnDeathExplosionsCR());
    }

    protected Vector2 GetTargetClockhairPosition(Vector2 currentClockhairPosition)
    {
      BraveInput instanceForPlayer = BraveInput.GetInstanceForPlayer(GameManager.Instance.PrimaryPlayer.PlayerIDX);
      return Vector2.Min(GameManager.Instance.MainCameraController.MaxVisiblePoint, Vector2.Max(GameManager.Instance.MainCameraController.MinVisiblePoint, !instanceForPlayer.IsKeyboardAndMouse() ? currentClockhairPosition + instanceForPlayer.ActiveActions.Aim.Vector * 10f * BraveTime.DeltaTime : GameManager.Instance.MainCameraController.Camera.ScreenToWorldPoint(Input.mousePosition).XY() + new Vector2(0.375f, -0.25f)));
    }

    private bool CheckTarget(GameActor target, Transform clockhairTransform)
    {
      return (double) Vector2.Distance(clockhairTransform.position.XY() + new Vector2(-0.375f, 0.25f), target.CenterPosition + new Vector2(0.0f, -1.25f)) < 0.875;
    }

    [DebuggerHidden]
    private IEnumerator HandleClockhair(PlayerController interactor)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new InfinilichDeathController__HandleClockhairc__Iterator0()
      {
        _this = this
      };
    }

    private void UpdateEyes(Vector2 clockhairPosition, bool isInDanger)
    {
      Vector2 vector = clockhairPosition - this.eyePos.transform.position.XY();
      if (isInDanger && (double) vector.magnitude < 7.0)
      {
        if (this.aiAnimator.IsPlaying("clockhair_target"))
          return;
        this.aiAnimator.PlayUntilCancelled("clockhair_target");
      }
      else
      {
        this.aiAnimator.LockFacingDirection = true;
        this.aiAnimator.FacingDirection = vector.ToAngle();
        if ((double) Mathf.Abs(vector.x) < 4.0 && (double) Mathf.Abs(vector.y) < 5.0)
        {
          if ((double) vector.y > 2.0)
          {
            if (this.aiAnimator.IsPlaying("clockhair_up"))
              return;
            this.aiAnimator.PlayUntilCancelled("clockhair_up");
          }
          else if ((double) vector.y < -2.0)
          {
            if (this.aiAnimator.IsPlaying("clockhair_down"))
              return;
            this.aiAnimator.PlayUntilCancelled("clockhair_down");
          }
          else
          {
            if (this.aiAnimator.IsPlaying("clockhair_mid"))
              return;
            this.aiAnimator.PlayUntilCancelled("clockhair_mid");
          }
        }
        else
        {
          if (this.aiAnimator.IsPlaying("clockhair"))
            return;
          this.aiAnimator.PlayUntilCancelled("clockhair");
        }
      }
    }

    [DebuggerHidden]
    private IEnumerator OnDeathExplosionsCR()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new InfinilichDeathController__OnDeathExplosionsCRc__Iterator1()
      {
        _this = this
      };
    }

    [DebuggerHidden]
    private IEnumerator HandlePastBeingShot()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new InfinilichDeathController__HandlePastBeingShotc__Iterator2()
      {
        _this = this
      };
    }

    [DebuggerHidden]
    private IEnumerator HandleSplashBody(
      PlayerController sourcePlayer,
      bool isPrimaryPlayer,
      TitleDioramaController diorama)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new InfinilichDeathController__HandleSplashBodyc__Iterator3()
      {
        sourcePlayer = sourcePlayer,
        diorama = diorama
      };
    }
  }

