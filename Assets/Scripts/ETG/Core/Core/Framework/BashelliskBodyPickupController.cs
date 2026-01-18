// Decompiled with JetBrains decompiler
// Type: BashelliskBodyPickupController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

public class BashelliskBodyPickupController : BraveBehaviour
  {
    public Transform center;
    public AIActorBuffEffect buffEffect;

    public void Awake() => this.aiActor.PreventBlackPhantom = true;

    public void Update()
    {
      ShootBehavior attackBehavior = this.aiActor.behaviorSpeculator.AttackBehaviors[0] as ShootBehavior;
      if (this.aiActor.CanTargetEnemies)
      {
        attackBehavior.Cooldown = 0.15f;
        attackBehavior.BulletName = "fast";
      }
      else
      {
        attackBehavior.Cooldown = 1.5f;
        attackBehavior.BulletName = "default";
      }
    }

    protected override void OnDestroy() => base.OnDestroy();
  }

