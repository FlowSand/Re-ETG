using UnityEngine;

#nullable disable

public class ProjectileSpriteFollower : MonoBehaviour
    {
        public tk2dBaseSprite TargetSprite;
        public float SmoothTime = 0.25f;
        private Vector3 m_currentVelocity;

        private void Awake()
        {
            this.transform.parent = (Transform) null;
            this.transform.localRotation = Quaternion.identity;
        }

        private void LateUpdate()
        {
            if (!(bool) (Object) this.TargetSprite)
                Object.Destroy((Object) this.gameObject);
            else
                this.transform.position = Vector3.SmoothDamp(this.transform.position, this.TargetSprite.transform.position, ref this.m_currentVelocity, this.SmoothTime);
        }
    }

