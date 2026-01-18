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

