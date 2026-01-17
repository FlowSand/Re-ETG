// Decompiled with JetBrains decompiler
// Type: AuraSynergyProcessor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    public class AuraSynergyProcessor : MonoBehaviour
    {
      [LongNumericEnum]
      public CustomSynergyType RequiredSynergy;
      public bool TriggeredOnReload;
      public float AuraRadius = 5f;
      public bool HasOverrideDuration;
      public float OverrideDuration = 0.05f;
      public bool DoPoison;
      public GameActorHealthEffect PoisonEffect;
      public bool DoFreeze;
      public GameActorFreezeEffect FreezeEffect;
      public bool DoBurn;
      public GameActorFireEffect FireEffect;
      public bool DoCharm;
      public GameActorCharmEffect CharmEffect;
      public bool DoSlow;
      public GameActorSpeedEffect SpeedEffect;
      public bool DoStun;
      public float StunDuration = 1f;
      private Gun m_gun;

      private void Awake()
      {
        this.m_gun = this.GetComponent<Gun>();
        this.m_gun.OnReloadPressed += new Action<PlayerController, Gun, bool>(this.HandleReload);
      }

      private void HandleReload(PlayerController sourcePlayer, Gun arg2, bool arg3)
      {
        if (!(bool) (UnityEngine.Object) sourcePlayer || !sourcePlayer.HasActiveBonusSynergy(this.RequiredSynergy) || !this.TriggeredOnReload)
          return;
        this.StartCoroutine(this.HandleReloadTrigger());
      }

      [DebuggerHidden]
      private IEnumerator HandleReloadTrigger()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new AuraSynergyProcessor.<HandleReloadTrigger>c__Iterator0()
        {
          $this = this
        };
      }

      private void ProcessEnemy(AIActor enemy, float distance)
      {
        if (this.DoPoison)
          enemy.ApplyEffect((GameActorEffect) this.PoisonEffect);
        if (this.DoFreeze)
          enemy.ApplyEffect((GameActorEffect) this.FreezeEffect, BraveTime.DeltaTime);
        if (this.DoBurn)
          enemy.ApplyEffect((GameActorEffect) this.FireEffect);
        if (this.DoCharm)
          enemy.ApplyEffect((GameActorEffect) this.CharmEffect);
        if (this.DoSlow)
          enemy.ApplyEffect((GameActorEffect) this.SpeedEffect);
        if (!this.DoStun || !(bool) (UnityEngine.Object) enemy.behaviorSpeculator)
          return;
        if (enemy.behaviorSpeculator.IsStunned)
          enemy.behaviorSpeculator.UpdateStun(this.StunDuration);
        else
          enemy.behaviorSpeculator.Stun(this.StunDuration);
      }
    }

}
