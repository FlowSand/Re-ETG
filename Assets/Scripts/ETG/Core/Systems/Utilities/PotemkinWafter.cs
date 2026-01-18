using UnityEngine;

#nullable disable

public class PotemkinWafter : MonoBehaviour
    {
        private static bool invert;
        private Vector2 m_currentVelocity = Vector2.zero;
        private float m_elapsed_x;
        private float m_elapsed_y;
        private float xSpeed = 1f;
        private float ySpeed = 1f;

        private void Start()
        {
            this.xSpeed = Random.Range(0.7f, 1.3f) / 3f;
            this.ySpeed = Random.Range(0.7f, 1.3f);
            if (PotemkinWafter.invert)
            {
                this.m_elapsed_x = 1f;
                PotemkinWafter.invert = false;
            }
            else
            {
                this.m_elapsed_x = 0.0f;
                PotemkinWafter.invert = true;
            }
        }

        private void Update()
        {
            this.m_elapsed_x += BraveTime.DeltaTime * this.xSpeed;
            this.m_elapsed_y += BraveTime.DeltaTime * this.ySpeed;
            this.m_currentVelocity.x = Mathf.Lerp(1f, -1f, Mathf.SmoothStep(0.0f, 1f, Mathf.PingPong(this.m_elapsed_x, 1f)));
            this.m_currentVelocity.y = Mathf.Lerp(0.25f, -0.25f, Mathf.SmoothStep(0.0f, 1f, Mathf.PingPong(this.m_elapsed_y + 0.25f, 1f)));
            this.m_currentVelocity.x /= 3f;
            this.transform.position += this.m_currentVelocity.ToVector3ZUp() * BraveTime.DeltaTime;
        }
    }

