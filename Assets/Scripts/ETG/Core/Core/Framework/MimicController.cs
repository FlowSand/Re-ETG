// Decompiled with JetBrains decompiler
// Type: MimicController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable

public class MimicController : BraveBehaviour
  {
    public void Awake()
    {
      this.aiActor.enabled = false;
      this.aiShooter.enabled = false;
      this.behaviorSpeculator.enabled = false;
    }

    protected override void OnDestroy() => base.OnDestroy();
  }

