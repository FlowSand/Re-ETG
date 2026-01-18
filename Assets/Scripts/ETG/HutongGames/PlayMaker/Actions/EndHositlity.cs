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
