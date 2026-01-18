using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [HutongGames.PlayMaker.Tooltip("Gets the position of a vertex in a GameObject's mesh. Hint: Use GetVertexCount to get the number of vertices in a mesh.")]
    [ActionCategory("Mesh")]
    public class GetVertexPosition : FsmStateAction
    {
        [RequiredField]
        [HutongGames.PlayMaker.Tooltip("The GameObject to check.")]
        [CheckForComponent(typeof (MeshFilter))]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("The index of the vertex.")]
        [RequiredField]
        public FsmInt vertexIndex;
        [HutongGames.PlayMaker.Tooltip("Coordinate system to use.")]
        public Space space;
        [HutongGames.PlayMaker.Tooltip("Store the vertex position in a variable.")]
        [UIHint(UIHint.Variable)]
        [RequiredField]
        public FsmVector3 storePosition;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame. Useful if the mesh is animated.")]
        public bool everyFrame;

        public override void Reset()
        {
            this.gameObject = (FsmOwnerDefault) null;
            this.space = Space.World;
            this.storePosition = (FsmVector3) null;
            this.everyFrame = false;
        }

        public override void OnEnter()
        {
            this.DoGetVertexPosition();
            if (this.everyFrame)
                return;
            this.Finish();
        }

        public override void OnUpdate() => this.DoGetVertexPosition();

        private void DoGetVertexPosition()
        {
            GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (!((Object) ownerDefaultTarget != (Object) null))
                return;
            MeshFilter component = ownerDefaultTarget.GetComponent<MeshFilter>();
            if ((Object) component == (Object) null)
            {
                this.LogError("Missing MeshFilter!");
            }
            else
            {
                switch (this.space)
                {
                    case Space.World:
                        Vector3 vertex = component.mesh.vertices[this.vertexIndex.Value];
                        this.storePosition.Value = ownerDefaultTarget.transform.TransformPoint(vertex);
                        break;
                    case Space.Self:
                        this.storePosition.Value = component.mesh.vertices[this.vertexIndex.Value];
                        break;
                }
            }
        }
    }
}
