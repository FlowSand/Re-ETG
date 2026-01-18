// Decompiled with JetBrains decompiler
// Type: TowerBossEmitterController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

public class TowerBossEmitterController : MonoBehaviour
  {
    public float currentAngle;
    public string eastSpriteName;
    public string westSpriteName;
    public string southSpriteName;
    private tk2dSprite m_sprite;

    private void Start() => this.m_sprite = this.GetComponent<tk2dSprite>();

    public void UpdateAngle(float newAngle)
    {
      this.currentAngle = newAngle;
      float num = this.currentAngle / 3.14159274f;
      if ((double) num > 0.05000000074505806 && (double) num < 0.949999988079071)
        this.m_sprite.renderer.enabled = false;
      else if ((double) num > 1.75 || (double) num <= 0.05000000074505806)
      {
        this.m_sprite.renderer.enabled = true;
        this.m_sprite.SetSprite(this.eastSpriteName);
      }
      else if ((double) num > 1.25)
      {
        this.m_sprite.renderer.enabled = true;
        this.m_sprite.SetSprite(this.southSpriteName);
      }
      else
      {
        this.m_sprite.renderer.enabled = true;
        this.m_sprite.SetSprite(this.westSpriteName);
      }
    }
  }

