// Decompiled with JetBrains decompiler
// Type: BulletSourceKiller
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class BulletSourceKiller : BraveBehaviour
{
  public BulletScriptSource BraveSource;
  public SpeculativeRigidbody TrackRigidbody;

  public void Start()
  {
    if ((bool) (Object) this.BraveSource)
      return;
    this.BraveSource = this.GetComponent<BulletScriptSource>();
  }

  public void Update()
  {
    if ((bool) (Object) this.TrackRigidbody)
      this.transform.position = (Vector3) this.TrackRigidbody.GetUnitCenter(ColliderType.HitBox);
    if ((bool) (Object) this.BraveSource && this.BraveSource.IsEnded)
      Object.Destroy((Object) this.gameObject);
    if ((bool) (Object) this.BraveSource)
      return;
    Object.Destroy((Object) this.gameObject);
  }

  protected override void OnDestroy() => base.OnDestroy();
}
