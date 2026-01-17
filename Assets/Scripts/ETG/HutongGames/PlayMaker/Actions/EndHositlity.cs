// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.EndHositlity
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(".NPCs")]
  [HutongGames.PlayMaker.Tooltip("Makes this NPC become an enemy.")]
  public class EndHositlity : FsmStateAction
  {
    public FsmBool DontMoveNPC = (FsmBool) false;

    public override void OnEnter()
    {
      TalkDoerLite component = this.Owner.GetComponent<TalkDoerLite>();
      if (!this.DontMoveNPC.Value)
      {
        component.transform.position += (Vector3) (component.HostileObject.specRigidbody.UnitBottomLeft - component.specRigidbody.UnitBottomLeft);
        component.specRigidbody.Reinitialize();
      }
      SetNpcVisibility.SetVisible(component, true);
      component.aiAnimator.FacingDirection = component.HostileObject.aiAnimator.FacingDirection;
      component.aiAnimator.LockFacingDirection = true;
      this.Finish();
    }
  }
}
