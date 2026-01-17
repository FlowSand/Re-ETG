// Decompiled with JetBrains decompiler
// Type: TurboModeController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class TurboModeController : MonoBehaviour
{
  public static float sPlayerSpeedMultiplier = 1.4f;
  public static float sPlayerRollSpeedMultiplier = 1.4f;
  public static float sEnemyBulletSpeedMultiplier = 1.3f;
  public static float sEnemyMovementSpeedMultiplier = 1.5f;
  public static float sEnemyCooldownMultiplier = 0.5f;
  public static float sEnemyWakeTimeMultiplier = 4f;
  public static float sEnemyAnimSpeed = 1f;
  public float PlayerSpeedMultiplier = 1.4f;
  public float PlayerRollSpeedMultiplier = 1.4f;
  public float EnemyBulletSpeedMultiplier = 1.3f;
  public float EnemyMovementSpeedMultiplier = 1.5f;
  public float EnemyCooldownMultiplier = 0.5f;
  public float EnemyWakeTimeMultiplier = 4f;
  public float EnemyAnimSpeed = 1f;

  public void Update()
  {
    TurboModeController.sPlayerSpeedMultiplier = this.PlayerSpeedMultiplier;
    TurboModeController.sPlayerRollSpeedMultiplier = this.PlayerRollSpeedMultiplier;
    TurboModeController.sEnemyBulletSpeedMultiplier = this.EnemyBulletSpeedMultiplier;
    TurboModeController.sEnemyMovementSpeedMultiplier = this.EnemyMovementSpeedMultiplier;
    TurboModeController.sEnemyCooldownMultiplier = this.EnemyCooldownMultiplier;
    TurboModeController.sEnemyWakeTimeMultiplier = this.EnemyWakeTimeMultiplier;
    TurboModeController.sEnemyAnimSpeed = this.EnemyAnimSpeed;
  }

  public static bool IsActive => GameManager.IsTurboMode;

  public static float MaybeModifyEnemyBulletSpeed(float speed)
  {
    return GameManager.IsTurboMode ? speed * TurboModeController.sEnemyBulletSpeedMultiplier : speed;
  }

  public static float MaybeModifyEnemyMovementSpeed(float speed)
  {
    return GameManager.IsTurboMode ? speed * TurboModeController.sEnemyMovementSpeedMultiplier : speed;
  }
}
