using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [HutongGames.PlayMaker.Tooltip("Set a named color property in a Substance material. NOTE: Use Rebuild Textures after setting Substance properties.")]
    [ActionCategory("Substance")]
    public class SetProceduralColor : FsmStateAction
    {
        [HutongGames.PlayMaker.Tooltip("The Substance Material.")]
        [RequiredField]
        public FsmMaterial substanceMaterial;
        [RequiredField]
        [HutongGames.PlayMaker.Tooltip("The named color property in the material.")]
        public FsmString colorProperty;
        [HutongGames.PlayMaker.Tooltip("The value to set the property to.")]
        [RequiredField]
        public FsmColor colorValue;
        [HutongGames.PlayMaker.Tooltip("NOTE: Updating procedural materials every frame can be very slow!")]
        public bool everyFrame;

        public override void Reset()
        {
            this.substanceMaterial = (FsmMaterial) null;
            this.colorProperty = (FsmString) string.Empty;
            this.colorValue = (FsmColor) Color.white;
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
                proceduralMaterial.SetProceduralColor(this.colorProperty.Value, this.colorValue.Value);
        }
    }
}
