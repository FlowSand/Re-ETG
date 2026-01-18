using UnityEngine;

#nullable disable
namespace Kvant
{
    [AddComponentMenu("Kvant/Tunnel Scroller")]
    [RequireComponent(typeof (Tunnel))]
    public class TunnelScroller : MonoBehaviour
    {
        [SerializeField]
        private float _speed;
        [SerializeField]
        private float _zRotationSpeed;
        private Transform m_transform;

        public float speed
        {
            get => this._speed;
            set => this._speed = value;
        }

        private void Update()
        {
            if ((Object) this.m_transform == (Object) null)
                this.m_transform = this.transform;
            this.m_transform.Rotate(0.0f, 0.0f, this._zRotationSpeed * BraveTime.DeltaTime);
            this.GetComponent<Tunnel>().offset += this._speed * BraveTime.DeltaTime;
        }
    }
}
