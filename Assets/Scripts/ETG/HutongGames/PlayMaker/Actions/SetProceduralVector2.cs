// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.SetProceduralVector2
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory("Substance")]
  [HutongGames.PlayMaker.Tooltip("Set a named Vector2 property in a Substance material. NOTE: Use Rebuild Textures after setting Substance properties.")]
  public class SetProceduralVector2 : FsmStateAction
  {
    [HutongGames.PlayMaker.Tooltip("The Substance Material.")]
    [RequiredField]
    public FsmMaterial substanceMaterial;
    [RequiredField]
    [HutongGames.PlayMaker.Tooltip("The named vector property in the material.")]
    public FsmString vector2Property;
    [HutongGames.PlayMaker.Tooltip("The Vector3 value to set the property to.\nNOTE: Use Set Procedural Vector2 for Vector3 values.")]
    [RequiredField]
    public FsmVector2 vector2Value;
    [HutongGames.PlayMaker.Tooltip("NOTE: Updating procedural materials every frame can be very slow!")]
    public bool everyFrame;

    public override void Reset()
    {
      this.substanceMaterial = (FsmMaterial) null;
      this.vector2Property = (FsmString) null;
      this.vector2Value = (FsmVector2) null;
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
        proceduralMaterial.SetProceduralVector(this.vector2Property.Value, (Vector4) this.vector2Value.Value);
    }
  }
}
