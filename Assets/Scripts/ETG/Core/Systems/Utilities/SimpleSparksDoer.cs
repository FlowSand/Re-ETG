// Decompiled with JetBrains decompiler
// Type: SimpleSparksDoer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

public class SimpleSparksDoer : MonoBehaviour
  {
    public Vector3 localMin;
    public Vector3 localMax;
    public GlobalSparksDoer.SparksType sparksType;
    public Vector3 baseDirection = Vector3.up;
    public float magnitudeVariance = 0.5f;
    public float angleVariance = 45f;
    public float LifespanMin = 0.5f;
    public float LifespanMax = 1f;
    public int SparksPerSecond = 60;
    public bool DefineColor;
    public Color Color1;
    public Color Color2;
    private Transform m_transform;
    private float m_particlesToSpawn;

    private void Start() => this.m_transform = this.gameObject.transform;

    private void Update()
    {
      if (GameManager.Options.ShaderQuality == GameOptions.GenericHighMedLowOption.LOW || GameManager.Options.ShaderQuality == GameOptions.GenericHighMedLowOption.VERY_LOW)
        return;
      Color? startColor = new Color?();
      if (this.DefineColor)
        startColor = new Color?(new Color(Random.Range(this.Color1.r, this.Color2.r), Random.Range(this.Color1.g, this.Color2.g), Random.Range(this.Color1.b, this.Color2.b), Random.Range(this.Color1.a, this.Color2.a)));
      this.m_particlesToSpawn += (float) this.SparksPerSecond * BraveTime.DeltaTime;
      GlobalSparksDoer.DoRandomParticleBurst((int) this.m_particlesToSpawn, this.m_transform.position + this.localMin, this.m_transform.position + this.localMax, this.baseDirection, this.angleVariance, this.magnitudeVariance, startLifetime: new float?(Random.Range(this.LifespanMin, this.LifespanMax)), startColor: startColor, systemType: this.sparksType);
      this.m_particlesToSpawn %= 1f;
    }
  }

