using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

public class AmbientChatter : MonoBehaviour
  {
    public float MinTimeBetweenChatter = 10f;
    public float MaxTimeBetweenChatter = 20f;
    public float ChatterDuration = 5f;
    public string ChatterStringKey;
    public Transform SpeakPoint;
    public bool WanderInRadius;
    public float WanderRadius = 3f;
    private Transform m_transform;
    private Vector3 m_startPosition;

    private void Start()
    {
      this.m_transform = this.transform;
      this.m_startPosition = this.m_transform.position;
      if (this.WanderInRadius)
        this.StartCoroutine(this.HandleWander());
      this.StartCoroutine(this.HandleAmbientChatter());
    }

    [DebuggerHidden]
    private IEnumerator HandleWander()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new AmbientChatter__HandleWanderc__Iterator0()
      {
        _this = this
      };
    }

    [DebuggerHidden]
    private IEnumerator HandleAmbientChatter()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new AmbientChatter__HandleAmbientChatterc__Iterator1()
      {
        _this = this
      };
    }
  }

