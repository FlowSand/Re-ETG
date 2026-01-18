using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("Substance")]
    [HutongGames.PlayMaker.Tooltip("Set a named bool property in a Substance material. NOTE: Use Rebuild Textures after setting Substance properties.")]
    public class SetProceduralBoolean : FsmStateAction
    {
        [RequiredField]
        [HutongGames.PlayMaker.Tooltip("The Substance Material.")]
        public FsmMaterial substanceMaterial;
        [HutongGames.PlayMaker.Tooltip("The named bool property in the material.")]
        [RequiredField]
        public FsmString boolProperty;
        [RequiredField]
        [HutongGames.PlayMaker.Tooltip("The value to set the property to.")]
        public FsmBool boolValue;
        [HutongGames.PlayMaker.Tooltip("NOTE: Updating procedural materials every frame can be very slow!")]
        public bool everyFrame;

        public override void Reset()
        {
            this.substanceMaterial = (FsmMaterial) null;
            this.boolProperty = (FsmString) string.Empty;
            this.boolValue = (FsmBool) false;
            this.everyFrame = false;
        }

        public override void OnEnter()
        {
            this.DoSetProceduralFloat();
            if (this.everyFrame)
                return;
            this.Finish();
        }

        public override void OnUpdate() => this.DoSetProceduralFloat();

        private void DoSetProceduralFloat()
        {
            ProceduralMaterial proceduralMaterial = this.substanceMaterial.Value as ProceduralMaterial;
            if ((Object) proceduralMaterial == (Object) null)
                this.LogError("The Material is not a Substance Material!");
            else
                proceduralMaterial.SetProceduralBoolean(this.boolProperty.Value, this.boolValue.Value);
        }
    }
}
