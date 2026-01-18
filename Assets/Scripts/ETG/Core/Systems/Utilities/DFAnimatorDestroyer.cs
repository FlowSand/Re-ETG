using UnityEngine;

#nullable disable

public class DFAnimatorDestroyer : MonoBehaviour
    {
        protected dfSpriteAnimation m_animator;

        private void Start() => this.m_animator = this.GetComponent<dfSpriteAnimation>();

        private void Update()
        {
            if (!this.m_animator.IsPlaying && !this.m_animator.AutoRun)
                Object.Destroy((Object) this.gameObject);
            if (!this.m_animator.IsPlaying)
                return;
            this.m_animator.AutoRun = false;
        }
    }

