using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [HutongGames.PlayMaker.Tooltip("Draw Gizmos in the Scene View.")]
    [ActionCategory(ActionCategory.Debug)]
    public class DebugDrawShape : FsmStateAction
    {
        [RequiredField]
        [HutongGames.PlayMaker.Tooltip("Draw the Gizmo at a GameObject's position.")]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("The type of Gizmo to draw:\nSphere, Cube, WireSphere, or WireCube.")]
        public DebugDrawShape.ShapeType shape;
        [HutongGames.PlayMaker.Tooltip("The color to use.")]
        public FsmColor color;
        [HutongGames.PlayMaker.Tooltip("Use this for sphere gizmos")]
        public FsmFloat radius;
        [HutongGames.PlayMaker.Tooltip("Use this for cube gizmos")]
        public FsmVector3 size;

        public override void Reset()
        {
            this.gameObject = (FsmOwnerDefault) null;
            this.shape = DebugDrawShape.ShapeType.Sphere;
            this.color = (FsmColor) Color.grey;
            this.radius = (FsmFloat) 1f;
            this.size = (FsmVector3) new Vector3(1f, 1f, 1f);
        }

        public override void OnDrawActionGizmos()
        {
            Transform transform = this.Fsm.GetOwnerDefaultTarget(this.gameObject).transform;
            if ((Object) transform == (Object) null)
                return;
            Gizmos.color = this.color.Value;
            switch (this.shape)
            {
                case DebugDrawShape.ShapeType.Sphere:
                    Gizmos.DrawSphere(transform.position, this.radius.Value);
                    break;
                case DebugDrawShape.ShapeType.Cube:
                    Gizmos.DrawCube(transform.position, this.size.Value);
                    break;
                case DebugDrawShape.ShapeType.WireSphere:
                    Gizmos.DrawWireSphere(transform.position, this.radius.Value);
                    break;
                case DebugDrawShape.ShapeType.WireCube:
                    Gizmos.DrawWireCube(transform.position, this.size.Value);
                    break;
            }
        }

        public enum ShapeType
        {
            Sphere,
            Cube,
            WireSphere,
            WireCube,
        }
    }
}
