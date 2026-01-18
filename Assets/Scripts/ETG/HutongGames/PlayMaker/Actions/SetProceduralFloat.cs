using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("Substance")]
    [HutongGames.PlayMaker.Tooltip("Set a named float property in a Substance material. NOTE: Use Rebuild Textures after setting Substance properties.")]
    public class SetProceduralFloat : FsmStateAction
    {
        [HutongGames.PlayMaker.Tooltip("The Substance Material.")]
        [RequiredField]
        public FsmMaterial substanceMaterial;
        [RequiredField]
        [HutongGames.PlayMaker.Tooltip("The named float property in the material.")]
        public FsmString floatProperty;
        [RequiredField]
        [HutongGames.PlayMaker.Tooltip("The value to set the property to.")]
        public FsmFloat floatValue;
        [HutongGames.PlayMaker.Tooltip("NOTE: Updating procedural materials every frame can be very slow!")]
        public bool everyFrame;

        public override void Reset()
        {
            this.substanceMaterial = (FsmMaterial) null;
            this.floatProperty = (FsmString) string.Empty;
            this.floatValue = (FsmFloat) 0.0f;
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
                proceduralMaterial.SetProceduralFloat(this.floatProperty.Value, this.floatValue.Value);
        }
    }
}
