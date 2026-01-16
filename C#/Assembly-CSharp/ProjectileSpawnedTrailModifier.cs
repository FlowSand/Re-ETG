// Decompiled with JetBrains decompiler
// Type: ProjectileSpawnedTrailModifier
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class ProjectileSpawnedTrailModifier : MonoBehaviour
{
  public GameObject TrailPrefab;
  public string spawnAudioEvent;
  public Transform InFlightSpawnTransform;
  public Vector3 WorldSpaceSpawnOffset;
  public float SpawnPeriod;
  private float m_elapsed;
  private Projectile m_projectile;
  private SpeculativeRigidbody m_srb;

  private void Start()
  {
    this.m_projectile = this.GetComponent<Projectile>();
    this.m_srb = this.GetComponent<SpeculativeRigidbody>();
  }

  private void Update()
  {
    this.m_elapsed += BraveTime.DeltaTime;
    if ((double) this.m_elapsed <= (double) this.SpawnPeriod)
      return;
    if ((bool) (Object) this.InFlightSpawnTransform)
    {
      this.m_elapsed -= this.SpawnPeriod;
      SpawnManager.SpawnVFX(this.TrailPrefab, this.InFlightSpawnTransform.position + this.WorldSpaceSpawnOffset, Quaternion.identity);
    }
    else
    {
      this.m_elapsed -= this.SpawnPeriod;
      SpawnManager.SpawnVFX(this.TrailPrefab, this.m_srb.UnitCenter.ToVector3ZisY() + this.WorldSpaceSpawnOffset, Quaternion.identity);
    }
    if (string.IsNullOrEmpty(this.spawnAudioEvent))
      return;
    int num = (int) AkSoundEngine.PostEvent(this.spawnAudioEvent, this.gameObject);
  }
}
