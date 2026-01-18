using UnityEngine;

#nullable disable

public class BreakableObject : MonoBehaviour
  {
    public string breakAnimName = string.Empty;
    private SpeculativeRigidbody m_srb;

    private void Start()
    {
      this.m_srb = this.GetComponent<SpeculativeRigidbody>();
      this.m_srb.OnRigidbodyCollision += new SpeculativeRigidbody.OnRigidbodyCollisionDelegate(this.OnRigidbodyCollision);
    }

    private void OnRigidbodyCollision(CollisionData rigidbodyCollision) => this.Break();

    private void Break()
    {
      tk2dSpriteAnimator component = this.GetComponent<tk2dSpriteAnimator>();
      if (this.breakAnimName != string.Empty)
        component.Play(this.breakAnimName);
      else
        component.Play();
      Object.Destroy((Object) this.m_srb);
    }
  }

