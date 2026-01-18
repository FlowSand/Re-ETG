using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory("Substance")]
  [HutongGames.PlayMaker.Tooltip("Set a named Vector3 property in a Substance material. NOTE: Use Rebuild Textures after setting Substance properties.")]
  public class SetProceduralVector3 : FsmStateAction
  {
    [HutongGames.PlayMaker.Tooltip("The Substance Material.")]
    [RequiredField]
    public FsmMaterial substanceMaterial;
    [HutongGames.PlayMaker.Tooltip("The named vector property in the material.")]
    [RequiredField]
    public FsmString vector3Property;
    [HutongGames.PlayMaker.Tooltip("The value to set the property to.\nNOTE: Use Set Procedural Vector3 for Vector3 values.")]
    [RequiredField]
    public FsmVector3 vector3Value;
    [HutongGames.PlayMaker.Tooltip("NOTE: Updating procedural materials every frame can be very slow!")]
    public bool everyFrame;

    public override void Reset()
    {
      this.substanceMaterial = (FsmMaterial) null;
      this.vector3Property = (FsmString) null;
      this.vector3Value = (FsmVector3) null;
      this.everyFrame = false;
    }

    public override void OnEnter()
    {
      this.DoSetProceduralVector();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate() => this.DoSetProceduralVector();

    private void DoSetProceduralVector()
    {
      ProceduralMaterial proceduralMaterial = this.substanceMaterial.Value as ProceduralMaterial;
      if ((Object) proceduralMaterial == (Object) null)
        this.LogError("The Material is not a Substance Material!");
      else
        proceduralMaterial.SetProceduralVector(this.vector3Property.Value, (Vector4) this.vector3Value.Value);
    }
  }
}
