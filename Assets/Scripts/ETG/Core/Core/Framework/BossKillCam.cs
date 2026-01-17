// Decompiled with JetBrains decompiler
// Type: BossKillCam
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System.Collections.Generic;
using UnityEngine;

#nullable disable

namespace ETG.Core.Core.Framework
{
    public class BossKillCam : TimeInvariantMonoBehaviour
    {
      public static bool BossDeathCamRunning;
      public float trackToBossTime = 0.75f;
      public float returnToPlayerTime = 1f;
      protected bool m_isRunning;
      protected Projectile m_projectile;
      protected SpeculativeRigidbody m_bossRigidbody;
      protected CameraController m_camera;
      protected Transform m_cameraTransform;
      protected float m_phaseCountdown;
      protected int m_currentPhase;
      protected bool m_phaseComplete = true;
      protected float m_targetTimeScale = 1f;
      protected bool m_suppressContinuousBulletDestruction;
      protected List<CutsceneMotion> activeMotions = new List<CutsceneMotion>();
      public static GatlingGullOutroDoer hackGatlingGullOutroDoer;

      public void ForceCancelSequence()
      {
        Debug.Log((object) "force ending sequence");
        for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
        {
          PlayerController allPlayer = GameManager.Instance.AllPlayers[index];
          allPlayer.healthHaver.IsVulnerable = true;
          allPlayer.gameActor.SuppressEffectUpdates = false;
          allPlayer.ClearInputOverride("bossKillCam");
        }
        this.m_targetTimeScale = 1f;
        BraveTime.ClearMultiplier(this.gameObject);
        StickyFrictionManager.Instance.FrictionEnabled = true;
        this.m_isRunning = false;
        BossKillCam.BossDeathCamRunning = false;
        GameUIRoot.Instance.EndBossKillCam();
        Object.Destroy((Object) this);
      }

      public void SetPhaseCountdown(float value) => this.m_phaseCountdown = value;

      public void TriggerSequence(Projectile p, SpeculativeRigidbody bossSRB)
      {
        this.m_projectile = p;
        this.m_bossRigidbody = bossSRB;
        for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
        {
          PlayerController allPlayer = GameManager.Instance.AllPlayers[index];
          allPlayer.healthHaver.IsVulnerable = false;
          allPlayer.gameActor.SuppressEffectUpdates = true;
          allPlayer.SetInputOverride("bossKillCam");
          allPlayer.IsOnFire = false;
          allPlayer.CurrentFireMeterValue = 0.0f;
          allPlayer.CurrentPoisonMeterValue = 0.0f;
          DeadlyDeadlyGoopManager.DelayedClearGoopsInRadius(allPlayer.specRigidbody.UnitCenter, 1f);
          allPlayer.knockbackDoer.TriggerTemporaryKnockbackInvulnerability(5f);
        }
        this.m_targetTimeScale = 0.2f;
        StickyFrictionManager.Instance.FrictionEnabled = false;
        this.m_camera = GameManager.Instance.MainCameraController;
        this.m_camera.StopTrackingPlayer();
        this.m_camera.SetManualControl(true, false);
        this.m_camera.OverridePosition = this.m_camera.transform.position;
        GenericIntroDoer component = bossSRB.GetComponent<GenericIntroDoer>();
        if ((bool) (Object) component && (bool) (Object) component.cameraFocus)
          this.m_camera.RemoveFocusPoint(component.cameraFocus);
        this.m_cameraTransform = this.m_camera.transform;
        this.m_isRunning = true;
        BossKillCam.BossDeathCamRunning = true;
        if ((Object) this.m_projectile != (Object) null)
        {
          this.m_currentPhase = 0;
        }
        else
        {
          this.m_currentPhase = 1;
          Vector2? overrideKillCamPos = bossSRB.healthHaver.OverrideKillCamPos;
          Vector2 b = !overrideKillCamPos.HasValue ? bossSRB.UnitCenter : overrideKillCamPos.Value;
          this.m_suppressContinuousBulletDestruction = bossSRB.healthHaver.SuppressContinuousKillCamBulletDestruction;
          this.activeMotions.Add(new CutsceneMotion(this.m_cameraTransform, new Vector2?(b), Vector2.Distance(this.m_cameraTransform.position.XY(), b) / this.trackToBossTime)
          {
            camera = this.m_camera
          });
          this.m_phaseComplete = false;
        }
      }

      public static void ClearPerLevelData()
      {
        BossKillCam.hackGatlingGullOutroDoer = (GatlingGullOutroDoer) null;
      }

      private void EndSequence()
      {
        for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
        {
          PlayerController allPlayer = GameManager.Instance.AllPlayers[index];
          allPlayer.healthHaver.IsVulnerable = true;
          allPlayer.gameActor.SuppressEffectUpdates = false;
          allPlayer.ClearInputOverride("bossKillCam");
          allPlayer.IsOnFire = false;
          allPlayer.CurrentFireMeterValue = 0.0f;
          allPlayer.CurrentPoisonMeterValue = 0.0f;
          DeadlyDeadlyGoopManager.DelayedClearGoopsInRadius(allPlayer.specRigidbody.UnitCenter, 1f);
        }
        this.m_targetTimeScale = 1f;
        BraveTime.ClearMultiplier(this.gameObject);
        StickyFrictionManager.Instance.FrictionEnabled = true;
        this.m_camera.StartTrackingPlayer();
        this.m_camera.SetManualControl(false);
        this.m_isRunning = false;
        BossKillCam.BossDeathCamRunning = false;
        GameUIRoot.Instance.EndBossKillCam();
        if ((Object) BossKillCam.hackGatlingGullOutroDoer != (Object) null)
          BossKillCam.hackGatlingGullOutroDoer.TriggerSequence();
        BossKillCam.hackGatlingGullOutroDoer = (GatlingGullOutroDoer) null;
        Object.Destroy((Object) this);
      }

      protected override void InvariantUpdate(float realDeltaTime)
      {
        if (!this.m_isRunning)
          return;
        if (!this.m_suppressContinuousBulletDestruction)
          StaticReferenceManager.DestroyAllEnemyProjectiles();
        this.KillAllEnemies();
        for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
        {
          PlayerController allPlayer = GameManager.Instance.AllPlayers[index];
          allPlayer.healthHaver.IsVulnerable = false;
          allPlayer.gameActor.SuppressEffectUpdates = true;
          allPlayer.SetInputOverride("bossKillCam");
          allPlayer.IsOnFire = false;
          allPlayer.CurrentFireMeterValue = 0.0f;
          allPlayer.CurrentPoisonMeterValue = 0.0f;
          DeadlyDeadlyGoopManager.DelayedClearGoopsInRadius(allPlayer.specRigidbody.UnitCenter, 1f);
        }
        if ((double) UnityEngine.Time.timeScale != (double) this.m_targetTimeScale)
        {
          float max = (double) this.m_targetTimeScale <= (double) UnityEngine.Time.timeScale ? 1f : this.m_targetTimeScale;
          float min = (double) this.m_targetTimeScale <= (double) UnityEngine.Time.timeScale ? this.m_targetTimeScale : 0.0f;
          BraveTime.SetTimeScaleMultiplier(Mathf.Clamp(UnityEngine.Time.timeScale + (float) (((double) this.m_targetTimeScale - (double) UnityEngine.Time.timeScale) * ((double) realDeltaTime / 0.10000000149011612)), min, max), this.gameObject);
        }
        for (int index = 0; index < this.activeMotions.Count; ++index)
        {
          CutsceneMotion activeMotion = this.activeMotions[index];
          Vector2 vector2 = !activeMotion.lerpEnd.HasValue ? GameManager.Instance.MainCameraController.GetIdealCameraPosition() : activeMotion.lerpEnd.Value;
          float num1 = Vector2.Distance(vector2, activeMotion.lerpStart);
          float num2 = activeMotion.speed * realDeltaTime / num1;
          activeMotion.lerpProgress = Mathf.Clamp01(activeMotion.lerpProgress + num2);
          float t = activeMotion.lerpProgress;
          if (activeMotion.isSmoothStepped)
            t = Mathf.SmoothStep(0.0f, 1f, t);
          Vector2 vector = Vector2.Lerp(activeMotion.lerpStart, vector2, t);
          if ((Object) activeMotion.camera != (Object) null)
            activeMotion.camera.OverridePosition = vector.ToVector3ZUp(activeMotion.zOffset);
          else
            activeMotion.transform.position = BraveUtility.QuantizeVector(vector.ToVector3ZUp(activeMotion.zOffset), (float) PhysicsEngine.Instance.PixelsPerUnit);
          if ((double) activeMotion.lerpProgress == 1.0)
          {
            this.activeMotions.RemoveAt(index);
            --index;
            if (this.activeMotions.Count == 0)
            {
              ++this.m_currentPhase;
              this.m_phaseComplete = true;
            }
          }
        }
        if (this.m_currentPhase == 0)
        {
          if (!(bool) (Object) this.m_bossRigidbody || this.m_bossRigidbody.healthHaver.IsDead)
          {
            this.m_currentPhase += 2;
            this.m_phaseComplete = true;
          }
          else
          {
            if (!(bool) (Object) this.m_projectile || !(bool) (Object) this.m_projectile.specRigidbody)
            {
              this.EndSequence();
              return;
            }
            this.m_camera.OverridePosition = this.m_projectile.specRigidbody.UnitCenter.ToVector3ZUp();
          }
        }
        else if (this.m_currentPhase == 2 && (bool) (Object) this.m_bossRigidbody && this.m_bossRigidbody.healthHaver.TrackDuringDeath)
          GameManager.Instance.MainCameraController.OverridePosition = (Vector3) this.m_bossRigidbody.GetUnitCenter(ColliderType.HitBox);
        if (this.m_currentPhase <= 2)
          BraveInput.DoSustainedScreenShakeVibration(this.m_currentPhase >= 2 ? Mathf.Clamp01(this.m_phaseCountdown) : 1f);
        if (this.m_phaseComplete)
        {
          switch (this.m_currentPhase)
          {
            case 1:
              this.m_phaseComplete = false;
              break;
            case 2:
              this.m_targetTimeScale = 1f;
              this.m_phaseCountdown = !(bool) (Object) this.m_bossRigidbody || !(bool) (Object) this.m_bossRigidbody.healthHaver || !this.m_bossRigidbody.healthHaver.OverrideKillCamTime.HasValue ? 3f : this.m_bossRigidbody.healthHaver.OverrideKillCamTime.Value;
              this.m_phaseComplete = false;
              break;
            case 3:
              GameManager.Instance.MainCameraController.ForceUpdateControllerCameraState(CameraController.ControllerCameraState.FollowPlayer);
              Vector2 currentBasePosition = this.m_camera.GetCoreCurrentBasePosition();
              this.activeMotions.Add(new CutsceneMotion(this.m_cameraTransform, new Vector2?(), Vector2.Distance(this.m_cameraTransform.position.XY(), currentBasePosition) / this.returnToPlayerTime)
              {
                camera = this.m_camera
              });
              this.m_phaseComplete = false;
              break;
            case 4:
              this.EndSequence();
              return;
          }
        }
        if ((double) this.m_phaseCountdown <= 0.0)
          return;
        this.m_phaseCountdown -= realDeltaTime;
        if ((double) this.m_phaseCountdown > 0.0)
          return;
        this.m_phaseCountdown = 0.0f;
        ++this.m_currentPhase;
        this.m_phaseComplete = true;
      }

      private void KillAllEnemies()
      {
        if (!(bool) (Object) GameManager.Instance.BestActivePlayer)
          return;
        RoomHandler currentRoom = GameManager.Instance.BestActivePlayer.CurrentRoom;
        currentRoom.ClearReinforcementLayers();
        List<AIActor> activeEnemies = currentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
        if (activeEnemies == null)
          return;
        List<AIActor> aiActorList = new List<AIActor>((IEnumerable<AIActor>) activeEnemies);
        for (int index = 0; index < aiActorList.Count; ++index)
        {
          AIActor aiActor = aiActorList[index];
          if (!aiActor.PreventAutoKillOnBossDeath)
          {
            SpawnEnemyOnDeath component = aiActor.GetComponent<SpawnEnemyOnDeath>();
            if ((bool) (Object) component)
              Object.Destroy((Object) component);
            aiActor.healthHaver.minimumHealth = 0.0f;
            aiActor.healthHaver.ApplyDamage(10000f, Vector2.zero, "Boss Kill", damageCategory: DamageCategory.Unstoppable, ignoreInvulnerabilityFrames: true);
          }
        }
      }

      protected override void OnDestroy() => base.OnDestroy();
    }

}
