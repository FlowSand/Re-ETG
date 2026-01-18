using System;
using UnityEngine;

#nullable disable

public class RatchetScouterItem : PassiveItem
  {
    public GameObject VFXHealthBar;

    public override void Pickup(PlayerController player)
    {
      player.OnAnyEnemyReceivedDamage += new Action<float, bool, HealthHaver>(this.AnyDamageDealt);
      base.Pickup(player);
    }

    private void AnyDamageDealt(float damageAmount, bool fatal, HealthHaver target)
    {
      int a = Mathf.RoundToInt(damageAmount);
      Vector3 worldPosition = target.transform.position;
      float heightOffGround = 1f;
      SpeculativeRigidbody component1 = target.GetComponent<SpeculativeRigidbody>();
      if ((bool) (UnityEngine.Object) component1)
      {
        worldPosition = component1.UnitCenter.ToVector3ZisY();
        heightOffGround = worldPosition.y - component1.UnitBottomCenter.y;
        if ((bool) (UnityEngine.Object) component1.healthHaver && !component1.healthHaver.HasHealthBar && !component1.healthHaver.HasRatchetHealthBar && !component1.healthHaver.IsBoss)
        {
          component1.healthHaver.HasRatchetHealthBar = true;
          UnityEngine.Object.Instantiate<GameObject>(this.VFXHealthBar).GetComponent<SimpleHealthBarController>().Initialize(component1, component1.healthHaver);
        }
      }
      else
      {
        AIActor component2 = target.GetComponent<AIActor>();
        if ((bool) (UnityEngine.Object) component2)
        {
          worldPosition = component2.CenterPosition.ToVector3ZisY();
          if ((bool) (UnityEngine.Object) component2.sprite)
            heightOffGround = worldPosition.y - component2.sprite.WorldBottomCenter.y;
        }
      }
      int damage = Mathf.Max(a, 1);
      GameUIRoot.Instance.DoDamageNumber(worldPosition, heightOffGround, damage);
    }

    public override DebrisObject Drop(PlayerController player)
    {
      if ((bool) (UnityEngine.Object) player)
        player.OnAnyEnemyReceivedDamage -= new Action<float, bool, HealthHaver>(this.AnyDamageDealt);
      return base.Drop(player);
    }

    protected override void OnDestroy()
    {
      if ((bool) (UnityEngine.Object) this.Owner)
        this.Owner.OnAnyEnemyReceivedDamage -= new Action<float, bool, HealthHaver>(this.AnyDamageDealt);
      base.OnDestroy();
    }
  }

