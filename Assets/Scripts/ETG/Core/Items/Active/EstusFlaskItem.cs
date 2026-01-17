// Decompiled with JetBrains decompiler
// Type: EstusFlaskItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Items.Active
{
    public class EstusFlaskItem : PlayerItem
    {
      public int numDrinksPerFloor = 2;
      public float healingAmount = 1f;
      public float drinkDuration = 1f;
      public string HasDrinkSprite;
      public string NoDrinkSprite;
      public GameObject healVFX;
      private PlayerController m_owner;
      private int m_remainingDrinksThisFloor;

      public int RemainingDrinks => this.m_remainingDrinksThisFloor;

      public override void Pickup(PlayerController player)
      {
        this.m_owner = player;
        if (!this.m_pickedUpThisRun)
          this.m_remainingDrinksThisFloor = this.numDrinksPerFloor;
        player.OnNewFloorLoaded += new Action<PlayerController>(this.ResetFlaskForFloor);
        base.Pickup(player);
      }

      protected override void OnPreDrop(PlayerController user)
      {
        user.OnNewFloorLoaded -= new Action<PlayerController>(this.ResetFlaskForFloor);
        this.m_owner = (PlayerController) null;
        base.OnPreDrop(user);
      }

      private void ResetFlaskForFloor(PlayerController obj)
      {
        this.m_remainingDrinksThisFloor = this.numDrinksPerFloor;
        this.sprite.SetSprite(this.HasDrinkSprite);
      }

      public override bool CanBeUsed(PlayerController user) => this.m_remainingDrinksThisFloor > 0;

      protected override void DoEffect(PlayerController user)
      {
        if (this.m_remainingDrinksThisFloor > 0)
        {
          --this.m_remainingDrinksThisFloor;
          user.StartCoroutine(this.HandleDrinkEstus(user));
        }
        if (this.m_remainingDrinksThisFloor > 0)
          return;
        this.sprite.SetSprite(this.NoDrinkSprite);
      }

      [DebuggerHidden]
      private IEnumerator HandleDrinkEstus(PlayerController user)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new EstusFlaskItem__HandleDrinkEstusc__Iterator0()
        {
          user = user,
          _this = this
        };
      }

      protected override void CopyStateFrom(PlayerItem other)
      {
        base.CopyStateFrom(other);
        EstusFlaskItem estusFlaskItem = other as EstusFlaskItem;
        if (!(bool) (UnityEngine.Object) estusFlaskItem)
          return;
        this.m_remainingDrinksThisFloor = estusFlaskItem.m_remainingDrinksThisFloor;
      }

      protected override void OnDestroy()
      {
        if ((UnityEngine.Object) this.m_owner != (UnityEngine.Object) null)
          this.m_owner.OnNewFloorLoaded -= new Action<PlayerController>(this.ResetFlaskForFloor);
        base.OnDestroy();
      }
    }

}
