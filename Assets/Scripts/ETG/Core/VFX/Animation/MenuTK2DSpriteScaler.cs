// Decompiled with JetBrains decompiler
// Type: MenuTK2DSpriteScaler
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable

namespace ETG.Core.VFX.Animation
{
    public class MenuTK2DSpriteScaler : MonoBehaviour
    {
      [NonSerialized]
      protected float TargetResolution = 1080f;
      protected Transform m_transform;
      protected dfGUIManager m_manager;
      protected int m_cachedWidth;
      protected int m_cachedHeight;
      protected bool m_cachedFullscreen;
      protected float m_cachedParentScale = 1f;

      private void Start()
      {
        this.m_manager = UnityEngine.Object.FindObjectOfType<dfGUIManager>();
        this.m_transform = this.transform;
        this.m_cachedFullscreen = Screen.fullScreen;
      }

      private void LateUpdate()
      {
        float num1 = 1f;
        if ((UnityEngine.Object) this.m_transform.parent != (UnityEngine.Object) null)
          num1 = this.m_transform.parent.lossyScale.x;
        if (this.m_cachedWidth == Screen.width && this.m_cachedHeight == Screen.height && this.m_cachedFullscreen == Screen.fullScreen && (double) this.m_cachedParentScale == (double) num1)
          return;
        float num2 = (float) ((double) Screen.height * (double) this.m_manager.RenderCamera.rect.height / (double) this.TargetResolution * 4.0);
        float num3 = num2 * 16f * this.m_manager.PixelsToUnits();
        this.m_transform.localScale = new Vector3(num3 / num1, num3 / num1, 1f);
        this.m_transform.position = this.m_transform.position.Quantize(this.m_manager.PixelsToUnits() * num2);
        this.m_cachedParentScale = num1;
        this.m_cachedWidth = Screen.width;
        this.m_cachedHeight = Screen.height;
        this.m_cachedFullscreen = Screen.fullScreen;
      }
    }

}
