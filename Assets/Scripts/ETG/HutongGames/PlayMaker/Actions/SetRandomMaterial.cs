using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Sets a Game Object's material randomly from an array of Materials.")]
  [ActionCategory(ActionCategory.Material)]
  public class SetRandomMaterial : ComponentAction<Renderer>
  {
    [RequiredField]
    [CheckForComponent(typeof (Renderer))]
    public FsmOwnerDefault gameObject;
    public FsmInt materialIndex;
    public FsmMaterial[] materials;

    public override void Reset()
    {
      this.gameObject = (FsmOwnerDefault) null;
      this.materialIndex = (FsmInt) 0;
      this.materials = new FsmMaterial[3];
    }

    public override void OnEnter()
    {
      this.DoSetRandomMaterial();
      this.Finish();
    }

    private void DoSetRandomMaterial()
    {
      if (this.materials == null || this.materials.Length == 0 || !this.UpdateCache(this.Fsm.GetOwnerDefaultTarget(this.gameObject)))
        return;
      if ((Object) this.renderer.material == (Object) null)
        this.LogError("Missing Material!");
      else if (this.materialIndex.Value == 0)
      {
        this.renderer.material = this.materials[Random.Range(0, this.materials.Length)].Value;
      }
      else
      {
        if (this.renderer.materials.Length <= this.materialIndex.Value)
          return;
        Material[] materials = this.renderer.materials;
        materials[this.materialIndex.Value] = this.materials[Random.Range(0, this.materials.Length)].Value;
        this.renderer.materials = materials;
      }
    }
  }
}
