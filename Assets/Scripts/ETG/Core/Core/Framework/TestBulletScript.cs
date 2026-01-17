// Decompiled with JetBrains decompiler
// Type: TestBulletScript
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable

namespace ETG.Core.Core.Framework
{
    public class TestBulletScript : BraveBehaviour
    {
      public float fireDelay = 1f;
      private BulletScriptSource m_bulletSource;
      private float m_counter;

      public void Awake() => this.m_bulletSource = this.GetComponentInChildren<BulletScriptSource>();

      private void Update()
      {
        if (!this.m_bulletSource.IsEnded)
          return;
        this.m_counter += BraveTime.DeltaTime;
        if ((double) this.m_counter <= (double) this.fireDelay)
          return;
        this.m_counter = 0.0f;
        this.m_bulletSource.Initialize();
      }

      protected override void OnDestroy() => base.OnDestroy();
    }

}
