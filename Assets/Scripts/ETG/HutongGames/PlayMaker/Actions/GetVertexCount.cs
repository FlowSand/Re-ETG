using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("Mesh")]
    [HutongGames.PlayMaker.Tooltip("Gets the number of vertices in a GameObject's mesh. Useful in conjunction with GetVertexPosition.")]
    public class GetVertexCount : FsmStateAction
    {
        [HutongGames.PlayMaker.Tooltip("The GameObject to check.")]
        [CheckForComponent(typeof (MeshFilter))]
        [RequiredField]
        public FsmOwnerDefault gameObject;
        [UIHint(UIHint.Variable)]
        [RequiredField]
        [HutongGames.PlayMaker.Tooltip("Store the vertex count in a variable.")]
        public FsmInt storeCount;
        public bool everyFrame;

        public override void Reset()
        {
            this.gameObject = (FsmOwnerDefault) null;
            this.storeCount = (FsmInt) null;
            this.everyFrame = false;
        }

        public override void OnEnter()
        {
            this.DoGetVertexCount();
            if (this.everyFrame)
                return;
            this.Finish();
        }

        public override void OnUpdate() => this.DoGetVertexCount();

        private void DoGetVertexCount()
        {
            GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (!((Object) ownerDefaultTarget != (Object) null))
                return;
            MeshFilter component = ownerDefaultTarget.GetComponent<MeshFilter>();
            if ((Object) component == (Object) null)
                this.LogError("Missing MeshFilter!");
            else
                this.storeCount.Value = component.mesh.vertexCount;
        }
    }
}
