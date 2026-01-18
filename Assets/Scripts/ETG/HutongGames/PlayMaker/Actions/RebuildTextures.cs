using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("Substance")]
    [HutongGames.PlayMaker.Tooltip("Rebuilds all dirty textures. By default the rebuild is spread over multiple frames so it won't halt the game. Check Immediately to rebuild all textures in a single frame.")]
    public class RebuildTextures : FsmStateAction
    {
        [RequiredField]
        public FsmMaterial substanceMaterial;
        [RequiredField]
        public FsmBool immediately;
        public bool everyFrame;

        public override void Reset()
        {
            this.substanceMaterial = (FsmMaterial) null;
            this.immediately = (FsmBool) false;
            this.everyFrame = false;
        }

        public override void OnEnter()
        {
            this.DoRebuildTextures();
            if (this.everyFrame)
                return;
            this.Finish();
        }

        public override void OnUpdate() => this.DoRebuildTextures();

        private void DoRebuildTextures()
        {
            ProceduralMaterial proceduralMaterial = this.substanceMaterial.Value as ProceduralMaterial;
            if ((Object) proceduralMaterial == (Object) null)
                this.LogError("Not a substance material!");
            else if (!this.immediately.Value)
                proceduralMaterial.RebuildTextures();
            else
                proceduralMaterial.RebuildTexturesImmediately();
        }
    }
}
