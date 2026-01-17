// Decompiled with JetBrains decompiler
// Type: dfPooledObject
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
[AddComponentMenu("Daikon Forge/Examples/Object Pooling/Pooled Object")]
public class dfPooledObject : MonoBehaviour
{
  public event dfPooledObject.SpawnEventHandler Spawned;

  public event dfPooledObject.SpawnEventHandler Despawned;

  public dfPoolManager.ObjectPool Pool { get; set; }

  private void Awake()
  {
  }

  private void OnEnable()
  {
  }

  private void OnDisable()
  {
  }

  private void OnDestroy()
  {
  }

  public void Despawn() => this.Pool.Despawn(this.gameObject);

  internal void OnSpawned()
  {
    if (this.Spawned != null)
      this.Spawned(this.gameObject);
    this.SendMessage("OnObjectSpawned", SendMessageOptions.DontRequireReceiver);
  }

  internal void OnDespawned()
  {
    if (this.Despawned != null)
      this.Despawned(this.gameObject);
    this.SendMessage("OnObjectDespawned", SendMessageOptions.DontRequireReceiver);
  }

  public delegate void SpawnEventHandler(GameObject instance);
}
