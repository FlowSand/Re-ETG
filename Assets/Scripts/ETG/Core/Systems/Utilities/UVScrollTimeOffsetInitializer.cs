using UnityEngine;

#nullable disable

public class UVScrollTimeOffsetInitializer : MonoBehaviour
  {
    public int NumberFrames;
    public float TimePerFrame;

    public void OnSpawned()
    {
      float num = UnityEngine.Time.realtimeSinceStartup % ((float) this.NumberFrames * this.TimePerFrame);
      this.GetComponent<MeshRenderer>().material.SetFloat("_TimeOffset", num);
    }
  }

