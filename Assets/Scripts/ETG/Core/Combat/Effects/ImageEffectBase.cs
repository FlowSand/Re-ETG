// Decompiled with JetBrains decompiler
// Type: ImageEffectBase
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

namespace ETG.Core.Combat.Effects
{
    [RequireComponent(typeof (Camera))]
    [AddComponentMenu("")]
    public class ImageEffectBase : MonoBehaviour
    {
      public Shader shader;
      private Material m_Material;

      protected virtual void Start()
      {
        if (!SystemInfo.supportsImageEffects)
        {
          this.enabled = false;
        }
        else
        {
          if ((bool) (Object) this.shader && this.shader.isSupported)
            return;
          this.enabled = false;
        }
      }

      protected Material material
      {
        get
        {
          if ((Object) this.m_Material == (Object) null)
          {
            this.m_Material = new Material(this.shader);
            this.m_Material.hideFlags = HideFlags.HideAndDontSave;
          }
          return this.m_Material;
        }
      }

      protected virtual void OnDisable()
      {
        if (!(bool) (Object) this.m_Material)
          return;
        Object.DestroyImmediate((Object) this.m_Material);
      }
    }

}
