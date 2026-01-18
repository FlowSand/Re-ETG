// Decompiled with JetBrains decompiler
// Type: CellCreepTeaser
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using UnityEngine;

#nullable disable

public class CellCreepTeaser : MonoBehaviour
  {
    public tk2dSpriteAnimator bodySprite;
    public tk2dSprite shadowSprite;
    private bool isPlaying;

    public void Update()
    {
      if (!this.isPlaying)
      {
        if (GameManager.Instance.IsPaused || Dungeon.IsGenerating || GameManager.Instance.IsLoadingLevel)
          return;
        this.bodySprite.Play();
        this.isPlaying = true;
      }
      else
      {
        this.shadowSprite.color = this.shadowSprite.color.WithAlpha(Mathf.InverseLerp(3.75f, 3.17f, this.bodySprite.ClipTimeSeconds));
        if (this.bodySprite.Playing)
          return;
        Object.Destroy((Object) this.gameObject);
      }
    }
  }

