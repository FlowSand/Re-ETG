// Decompiled with JetBrains decompiler
// Type: PlayParticleSystemDuringBossIntro
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class PlayParticleSystemDuringBossIntro : MonoBehaviour
{
  private bool m_isSimulating;
  private ParticleSystem m_particleSystem;

  public void Start() => this.m_particleSystem = this.GetComponent<ParticleSystem>();

  public void Update()
  {
    if (GameManager.IsBossIntro)
    {
      this.m_particleSystem.Simulate(GameManager.INVARIANT_DELTA_TIME, true, false);
      this.m_isSimulating = true;
    }
    else
    {
      if (!this.m_isSimulating)
        return;
      this.m_particleSystem.Play();
      this.m_isSimulating = false;
    }
  }
}
