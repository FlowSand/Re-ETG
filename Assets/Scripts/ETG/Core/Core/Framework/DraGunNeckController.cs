// Decompiled with JetBrains decompiler
// Type: DraGunNeckController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable

public class DraGunNeckController : BraveBehaviour
  {
    public void Start() => this.aiActor = this.transform.parent.GetComponent<AIActor>();

    protected override void OnDestroy() => base.OnDestroy();

    public void TriggerAnimationEvent(string eventInfo)
    {
      this.aiActor.behaviorSpeculator.TriggerAnimationEvent(eventInfo);
    }
  }

