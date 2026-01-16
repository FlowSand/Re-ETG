// Decompiled with JetBrains decompiler
// Type: BraveDFTextureAnimator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class BraveDFTextureAnimator : MonoBehaviour
{
  public Texture2D[] textures;
  public float fps = 1f;
  public bool IsOneShot;
  public float OneShotDelayTime;
  public bool randomLoop;
  public bool timeless;
  public int arbitraryLoopTarget = -1;
  private dfTextureSprite m_sprite;
  private float m_elapsed;
  private int m_currentFrame;

  private void Start()
  {
    this.m_sprite = this.GetComponent<dfTextureSprite>();
    if (!this.IsOneShot || (double) this.OneShotDelayTime <= 0.0)
      return;
    this.m_sprite.IsVisible = false;
  }

  private void OnEnable()
  {
    this.m_currentFrame = 0;
    this.m_elapsed = 0.0f;
  }

  private void Update()
  {
    if (!this.enabled)
      return;
    if (this.IsOneShot && (double) this.OneShotDelayTime > 0.0)
    {
      this.OneShotDelayTime -= GameManager.INVARIANT_DELTA_TIME;
    }
    else
    {
      if (this.IsOneShot)
        this.m_sprite.IsVisible = true;
      if (this.timeless)
        this.m_elapsed += GameManager.INVARIANT_DELTA_TIME;
      else
        this.m_elapsed += BraveTime.DeltaTime;
      int currentFrame = this.m_currentFrame;
      while ((double) this.m_elapsed > 1.0 / (double) this.fps)
      {
        this.m_elapsed -= 1f / this.fps;
        if (this.randomLoop)
          this.m_currentFrame += Random.Range(0, this.textures.Length);
        else
          ++this.m_currentFrame;
      }
      if (currentFrame == this.m_currentFrame)
        return;
      if (this.IsOneShot && this.m_currentFrame >= this.textures.Length)
      {
        this.enabled = false;
      }
      else
      {
        if (this.m_currentFrame >= this.textures.Length)
        {
          if (this.arbitraryLoopTarget > 0)
          {
            this.m_currentFrame %= this.textures.Length;
            this.m_currentFrame = Mathf.Max(this.arbitraryLoopTarget, this.m_currentFrame);
          }
          else
            this.m_currentFrame %= this.textures.Length;
        }
        this.m_sprite.Texture = (Texture) this.textures[this.m_currentFrame];
      }
    }
  }
}
