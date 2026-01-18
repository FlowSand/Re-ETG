using UnityEngine;

#nullable disable

public class ObjectHeightController : MonoBehaviour
    {
        public float heightOffGround = 0.5f;
        private Transform m_transform;

        private void Start() => this.m_transform = this.transform;

        private void Update()
        {
            this.m_transform.position = this.m_transform.position.WithZ(this.m_transform.position.y - this.heightOffGround);
        }
    }

