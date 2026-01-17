// Decompiled with JetBrains decompiler
// Type: UndertaleGunController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
public class UndertaleGunController : MonoBehaviour
{
  private Gun m_gun;
  private PlayerController m_player;
  private bool m_initialized;

  private void Awake() => this.m_gun = this.GetComponent<Gun>();

  private void Update()
  {
    if (!this.m_initialized && (bool) (UnityEngine.Object) this.m_gun.CurrentOwner)
    {
      this.Initialize();
    }
    else
    {
      if (!this.m_initialized || (bool) (UnityEngine.Object) this.m_gun.CurrentOwner)
        return;
      this.Deinitialize();
    }
  }

  private void Initialize()
  {
    if (this.m_initialized)
      return;
    this.m_player = this.m_gun.CurrentOwner as PlayerController;
    this.m_player.OnDodgedProjectile += new Action<Projectile>(this.HandleDodgedProjectile);
    this.m_initialized = true;
  }

  private void Deinitialize()
  {
    if (!this.m_initialized)
      return;
    this.m_player.OnDodgedProjectile -= new Action<Projectile>(this.HandleDodgedProjectile);
    this.m_player = (PlayerController) null;
    this.m_initialized = false;
  }

  private void HandleDodgedProjectile(Projectile dodgedProjectile)
  {
    if (!(bool) (UnityEngine.Object) dodgedProjectile.Owner || !(dodgedProjectile.Owner is AIActor))
      return;
    this.MakeEnemyNPC(dodgedProjectile.Owner as AIActor);
  }

  private void MakeEnemyNPC(AIActor enemy)
  {
    if ((bool) (UnityEngine.Object) enemy.aiAnimator)
      enemy.aiAnimator.PlayUntilCancelled("idle");
    if ((bool) (UnityEngine.Object) enemy.behaviorSpeculator)
      UnityEngine.Object.Destroy((UnityEngine.Object) enemy.behaviorSpeculator);
    if (!(bool) (UnityEngine.Object) enemy.aiActor)
      return;
    UnityEngine.Object.Destroy((UnityEngine.Object) enemy.aiActor);
  }
}
