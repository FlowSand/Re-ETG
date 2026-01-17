// Decompiled with JetBrains decompiler
// Type: tk2dUISoundItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

namespace ETG.Core.UI.HUD
{
    [AddComponentMenu("2D Toolkit/UI/tk2dUISoundItem")]
    public class tk2dUISoundItem : tk2dUIBaseItemControl
    {
      public AudioClip downButtonSound;
      public AudioClip upButtonSound;
      public AudioClip clickButtonSound;
      public AudioClip releaseButtonSound;

      private void OnEnable()
      {
        if (!(bool) (UnityEngine.Object) this.uiItem)
          return;
        if ((UnityEngine.Object) this.downButtonSound != (UnityEngine.Object) null)
          this.uiItem.OnDown += new System.Action(this.PlayDownSound);
        if ((UnityEngine.Object) this.upButtonSound != (UnityEngine.Object) null)
          this.uiItem.OnUp += new System.Action(this.PlayUpSound);
        if ((UnityEngine.Object) this.clickButtonSound != (UnityEngine.Object) null)
          this.uiItem.OnClick += new System.Action(this.PlayClickSound);
        if (!((UnityEngine.Object) this.releaseButtonSound != (UnityEngine.Object) null))
          return;
        this.uiItem.OnRelease += new System.Action(this.PlayReleaseSound);
      }

      private void OnDisable()
      {
        if (!(bool) (UnityEngine.Object) this.uiItem)
          return;
        if ((UnityEngine.Object) this.downButtonSound != (UnityEngine.Object) null)
          this.uiItem.OnDown -= new System.Action(this.PlayDownSound);
        if ((UnityEngine.Object) this.upButtonSound != (UnityEngine.Object) null)
          this.uiItem.OnUp -= new System.Action(this.PlayUpSound);
        if ((UnityEngine.Object) this.clickButtonSound != (UnityEngine.Object) null)
          this.uiItem.OnClick -= new System.Action(this.PlayClickSound);
        if (!((UnityEngine.Object) this.releaseButtonSound != (UnityEngine.Object) null))
          return;
        this.uiItem.OnRelease -= new System.Action(this.PlayReleaseSound);
      }

      private void PlayDownSound() => this.PlaySound(this.downButtonSound);

      private void PlayUpSound() => this.PlaySound(this.upButtonSound);

      private void PlayClickSound() => this.PlaySound(this.clickButtonSound);

      private void PlayReleaseSound() => this.PlaySound(this.releaseButtonSound);

      private void PlaySound(AudioClip source) => tk2dUIAudioManager.Instance.Play(source);
    }

}
