// Decompiled with JetBrains decompiler
// Type: GrapplingHookItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

namespace ETG.Core.Items.Active
{
    public class GrapplingHookItem : PlayerItem
    {
      public GameObject GrapplePrefab;
      public float GrappleSpeed = 10f;
      public float GrappleRetractSpeed = 10f;
      public float DamageToEnemies = 10f;
      public float EnemyKnockbackForce = 10f;
      private GrappleModule m_grappleModule;

      private void Awake() => this.InitializeModule();

      private void InitializeModule()
      {
        this.m_grappleModule = new GrappleModule();
        this.m_grappleModule.GrapplePrefab = this.GrapplePrefab;
        this.m_grappleModule.GrappleSpeed = this.GrappleSpeed;
        this.m_grappleModule.GrappleRetractSpeed = this.GrappleRetractSpeed;
        this.m_grappleModule.DamageToEnemies = this.DamageToEnemies;
        this.m_grappleModule.EnemyKnockbackForce = this.EnemyKnockbackForce;
        this.m_grappleModule.sourceGameObject = this.gameObject;
        this.m_grappleModule.FinishedCallback += new System.Action(this.GrappleEndedNaturally);
      }

      protected override void DoEffect(PlayerController user)
      {
        int num = (int) AkSoundEngine.PostEvent("Play_OBJ_hook_shot_01", this.gameObject);
        this.IsCurrentlyActive = true;
        this.m_grappleModule.Trigger(user);
      }

      protected void GrappleEndedNaturally() => this.IsCurrentlyActive = false;

      protected override void DoActiveEffect(PlayerController user) => this.m_grappleModule.MarkDone();

      protected override void OnPreDrop(PlayerController user)
      {
        this.m_grappleModule.ClearExtantGrapple();
        this.IsCurrentlyActive = false;
      }

      public override void OnItemSwitched(PlayerController user)
      {
        this.m_grappleModule.ForceEndGrapple();
        this.m_grappleModule.ClearExtantGrapple();
        this.IsCurrentlyActive = false;
      }

      protected override void OnDestroy()
      {
        this.m_grappleModule.ClearExtantGrapple();
        base.OnDestroy();
      }
    }

}
