// Decompiled with JetBrains decompiler
// Type: TimeInvariantMonoBehaviour
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable

public class TimeInvariantMonoBehaviour : BraveBehaviour
  {
    protected float m_deltaTime;

    protected virtual void Update()
    {
      this.m_deltaTime = GameManager.INVARIANT_DELTA_TIME;
      this.InvariantUpdate(GameManager.INVARIANT_DELTA_TIME);
    }

    protected virtual void InvariantUpdate(float realDeltaTime)
    {
    }

    protected override void OnDestroy() => base.OnDestroy();
  }

