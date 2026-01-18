using UnityEngine;

#nullable disable

public class SpriteAnimatorChanger : MonoBehaviour
  {
    public float time;
    public string newAnimation;
    private tk2dSpriteAnimator m_animator;
    private float m_timer;

    public void Awake() => this.m_animator = this.GetComponent<tk2dSpriteAnimator>();

    public void Update()
    {
      this.m_timer += BraveTime.DeltaTime;
      if ((double) this.m_timer <= (double) this.time)
        return;
      this.m_animator.Play(this.newAnimation);
      Object.Destroy((Object) this);
    }
  }

