// Decompiled with JetBrains decompiler
// Type: EphemeralObject
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;

#nullable disable
public abstract class EphemeralObject : ClusteredTimeInvariantMonoBehaviour
{
  public EphemeralObject.EphemeralPriority Priority = EphemeralObject.EphemeralPriority.Middling;
  private float m_destructionTimer;
  private bool m_isRegistered;
  private const float c_destroyTime = 1f;

  public virtual void Start() => this.OnSpawned();

  public virtual void OnSpawned()
  {
    if (this.m_isRegistered)
      return;
    SpawnManager.RegisterEphemeralObject(this);
    this.m_isRegistered = true;
  }

  protected override void OnDestroy()
  {
    this.OnDespawned();
    base.OnDestroy();
  }

  public virtual void OnDespawned()
  {
    if (!this.m_isRegistered)
      return;
    if (SpawnManager.HasInstance)
      SpawnManager.DeregisterEphemeralObject(this);
    this.m_isRegistered = false;
  }

  public void TriggerDestruction(bool forceImmediate = false)
  {
    SpawnManager.DeregisterEphemeralObject(this);
    if (this.gameObject.activeInHierarchy && !forceImmediate)
      this.StartCoroutine(this.DestroyCR());
    else
      SpawnManager.Despawn(this.gameObject);
  }

  [DebuggerHidden]
  private IEnumerator DestroyCR()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new EphemeralObject.\u003CDestroyCR\u003Ec__Iterator0()
    {
      \u0024this = this
    };
  }

  public enum EphemeralPriority
  {
    Critical,
    Important,
    Middling,
    Minor,
    Ephemeral,
  }
}
