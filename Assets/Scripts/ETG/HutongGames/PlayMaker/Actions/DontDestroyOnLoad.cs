using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.Level)]
  [HutongGames.PlayMaker.Tooltip("Makes the Game Object not be destroyed automatically when loading a new scene.")]
  public class DontDestroyOnLoad : FsmStateAction
  {
    [HutongGames.PlayMaker.Tooltip("GameObject to mark as DontDestroyOnLoad.")]
    [RequiredField]
    public FsmOwnerDefault gameObject;

    public override void Reset() => this.gameObject = (FsmOwnerDefault) null;

    public override void OnEnter()
    {
      Object.DontDestroyOnLoad((Object) this.Owner.transform.root.gameObject);
      this.Finish();
    }
  }
}
