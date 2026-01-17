// Decompiled with JetBrains decompiler
// Type: DistortionWake
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable

namespace ETG.Core.Core.Framework
{
    public class DistortionWake : BraveBehaviour
    {
      public float maxLength = 3f;
      public float initialIntensity;
      public float maxIntensity = 1f;
      public float initialRadius;
      public float maxRadius = 0.5f;
      public float initialOffset;
      public float offsetVariance;
      public float offsetVarianceSpeed = 1f;
      [NonSerialized]
      private Material m_material;
      [NonSerialized]
      private List<Vector2> m_positions = new List<Vector2>();

      private void Start()
      {
        this.m_material = new Material(ShaderCache.Acquire("Brave/Internal/DistortionLine"));
        this.m_material.SetVector("_WavePoint1", this.CalculateSettings(this.specRigidbody.UnitCenter, 0.0f));
        this.m_material.SetVector("_WavePoint2", this.CalculateSettings(this.specRigidbody.UnitCenter, 0.0f));
        this.m_material.SetFloat("_DistortProgress", this.initialOffset);
        Pixelator.Instance.RegisterAdditionalRenderPass(this.m_material);
      }

      private Vector4 CalculateSettings(Vector2 worldPoint, float t)
      {
        Vector3 viewportPoint = GameManager.Instance.MainCameraController.Camera.WorldToViewportPoint(worldPoint.ToVector3ZUp());
        return new Vector4(viewportPoint.x, viewportPoint.y, Mathf.Lerp(this.initialRadius, this.maxRadius, t), Mathf.Lerp(this.initialIntensity, this.maxIntensity, t));
      }

      private void LateUpdate()
      {
        this.m_positions.Add(this.specRigidbody.UnitCenter);
        if (this.m_positions.Count == 1)
          return;
        if (this.m_positions.Count != 2)
        {
          Vector2 position = this.m_positions[this.m_positions.Count - 1];
          while ((double) Vector2.Distance(position, this.m_positions[1]) > (double) this.maxLength)
            this.m_positions.RemoveAt(0);
        }
        this.m_material.SetVector("_WavePoint1", this.CalculateSettings(this.m_positions[this.m_positions.Count - 1], 0.0f));
        this.m_material.SetVector("_WavePoint2", this.CalculateSettings(this.m_positions[0], Mathf.Clamp01(Vector2.Distance(this.m_positions[this.m_positions.Count - 1], this.m_positions[0]) / this.maxLength)));
        float initialOffset = this.initialOffset;
        if ((double) this.offsetVariance > 0.0)
          initialOffset += Mathf.Sin(UnityEngine.Time.realtimeSinceStartup * this.offsetVarianceSpeed) * this.offsetVariance;
        this.m_material.SetFloat("_DistortProgress", initialOffset);
      }

      protected override void OnDestroy()
      {
        if (Pixelator.HasInstance && (bool) (UnityEngine.Object) this.m_material)
          Pixelator.Instance.DeregisterAdditionalRenderPass(this.m_material);
        if (!(bool) (UnityEngine.Object) this.m_material)
          return;
        UnityEngine.Object.Destroy((UnityEngine.Object) this.m_material);
      }
    }

}
