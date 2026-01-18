using UnityEngine;

#nullable disable
namespace DaikonForge.Tween
{
    [ExecuteInEditMode]
    public class SplineNode : MonoBehaviour
    {
        public void OnDestroy()
        {
            if (Application.isPlaying || (Object) this.transform.parent == (Object) null)
                return;
            SplineObject component = this.transform.parent.GetComponent<SplineObject>();
            if ((Object) component == (Object) null)
                return;
            component.ControlPoints.Remove(this.transform);
        }
    }
}
