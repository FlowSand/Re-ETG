// Decompiled with JetBrains decompiler
// Type: ReticleRiserEffect
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

namespace ETG.Core.Combat.Effects
{
    public class ReticleRiserEffect : MonoBehaviour
    {
      public int NumRisers = 4;
      public float RiserHeight = 1f;
      public float RiseTime = 1.5f;
      private tk2dSlicedSprite m_sprite;
      private tk2dSlicedSprite[] m_risers;
      private Shader m_shader;
      private float m_localElapsed;

      private void Start()
      {
        this.m_sprite = this.GetComponent<tk2dSlicedSprite>();
        this.m_sprite.usesOverrideMaterial = true;
        this.m_shader = ShaderCache.Acquire("tk2d/BlendVertexColorUnlitTilted");
        this.m_sprite.renderer.material.shader = this.m_shader;
        GameObject original = Object.Instantiate<GameObject>(this.gameObject);
        Object.Destroy((Object) original.GetComponent<ReticleRiserEffect>());
        this.m_risers = new tk2dSlicedSprite[this.NumRisers];
        this.m_risers[0] = original.GetComponent<tk2dSlicedSprite>();
        for (int index = 0; index < this.NumRisers - 1; ++index)
          this.m_risers[index + 1] = Object.Instantiate<GameObject>(original).GetComponent<tk2dSlicedSprite>();
        this.OnSpawned();
      }

      private void OnSpawned()
      {
        this.m_localElapsed = 0.0f;
        if (this.m_risers == null)
          return;
        for (int index = 0; index < this.m_risers.Length; ++index)
        {
          this.m_risers[index].transform.parent = this.transform;
          this.m_risers[index].transform.localPosition = Vector3.zero;
          this.m_risers[index].transform.localRotation = Quaternion.identity;
          this.m_risers[index].dimensions = this.m_sprite.dimensions;
          this.m_risers[index].usesOverrideMaterial = true;
          this.m_risers[index].renderer.material.shader = this.m_shader;
          this.m_risers[index].gameObject.SetLayerRecursively(LayerMask.NameToLayer("FG_Critical"));
        }
      }

      private void Update()
      {
        if (!(bool) (Object) this.m_sprite)
          return;
        this.m_localElapsed += BraveTime.DeltaTime;
        this.m_sprite.ForceRotationRebuild();
        this.m_sprite.UpdateZDepth();
        this.m_sprite.renderer.material.shader = this.m_shader;
        if (this.m_risers == null)
          return;
        for (int index = 0; index < this.m_risers.Length; ++index)
        {
          float t = Mathf.Max(0.0f, this.m_localElapsed - this.RiseTime / (float) this.NumRisers * (float) index) % this.RiseTime / this.RiseTime;
          this.m_risers[index].color = Color.Lerp(new Color(1f, 1f, 1f, 0.75f), new Color(1f, 1f, 1f, 0.0f), t);
          float y = Mathf.Lerp(0.0f, this.RiserHeight, t);
          this.m_risers[index].transform.localPosition = Vector3.zero;
          this.m_risers[index].transform.position += Vector3.zero.WithY(y);
          this.m_risers[index].ForceRotationRebuild();
          this.m_risers[index].UpdateZDepth();
          this.m_risers[index].renderer.material.shader = this.m_shader;
        }
      }
    }

}
