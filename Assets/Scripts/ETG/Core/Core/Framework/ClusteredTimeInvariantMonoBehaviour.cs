// Decompiled with JetBrains decompiler
// Type: ClusteredTimeInvariantMonoBehaviour
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable

namespace ETG.Core.Core.Framework
{
    public class ClusteredTimeInvariantMonoBehaviour : BraveBehaviour
    {
      protected float m_deltaTime;

      protected virtual void Awake()
      {
        StaticReferenceManager.AllClusteredTimeInvariantBehaviours.Add(this);
      }

      public void DoUpdate(float realDeltaTime)
      {
        this.m_deltaTime = realDeltaTime;
        this.InvariantUpdate(realDeltaTime);
      }

      protected virtual void InvariantUpdate(float realDeltaTime)
      {
      }

      protected override void OnDestroy()
      {
        StaticReferenceManager.AllClusteredTimeInvariantBehaviours.Remove(this);
        base.OnDestroy();
      }
    }

}
