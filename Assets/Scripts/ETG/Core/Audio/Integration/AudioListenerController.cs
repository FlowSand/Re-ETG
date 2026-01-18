using UnityEngine;

#nullable disable

public class AudioListenerController : MonoBehaviour
    {
        private Transform m_transform;

        private void Start() => this.m_transform = this.transform;

        private void LateUpdate()
        {
            this.m_transform.position = this.m_transform.position.WithZ(this.m_transform.position.y);
        }
    }

