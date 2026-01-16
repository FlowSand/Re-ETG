// Decompiled with JetBrains decompiler
// Type: IntroMovieClipPlayer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable
public class IntroMovieClipPlayer : MonoBehaviour
{
  public MovieTexture movieTexture;
  public AudioClip movieAudio;
  public dfTextureSprite guiTexture;
  private AudioSource m_source;

  private void Start()
  {
    GameManager.AttemptSoundEngineInitialization();
    this.m_source = Camera.main.gameObject.AddComponent<AudioSource>();
    this.m_source.clip = this.movieAudio;
    this.m_source.pitch = 1.02f;
    this.movieTexture.loop = false;
    this.guiTexture.Texture = (Texture) this.movieTexture;
  }

  public void TriggerMovie() => this.StartCoroutine(this.Do());

  [DebuggerHidden]
  private IEnumerator Do()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new IntroMovieClipPlayer.\u003CDo\u003Ec__Iterator0()
    {
      \u0024this = this
    };
  }
}
