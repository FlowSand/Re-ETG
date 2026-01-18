using UnityEngine;

#nullable disable

public class SimpleMover : MonoBehaviour
  {
    public Vector3 velocity;
    public Vector3 acceleration;
    private Transform m_transform;

    private void Start() => this.m_transform = this.transform;

    private void Update()
    {
      this.m_transform.position += this.velocity * BraveTime.DeltaTime;
      this.velocity += this.acceleration * BraveTime.DeltaTime;
    }

    public void OnDespawned() => Object.Destroy((Object) this);
  }

