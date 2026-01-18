// Decompiled with JetBrains decompiler
// Type: dfAnimationClip
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable

[AddComponentMenu("Daikon Forge/User Interface/Animation Clip")]
[Serializable]
public class dfAnimationClip : MonoBehaviour
  {
    [SerializeField]
    private dfAtlas atlas;
    [SerializeField]
    private List<string> sprites = new List<string>();

    public dfAtlas Atlas
    {
      get => this.atlas;
      set => this.atlas = value;
    }

    public List<string> Sprites => this.sprites;
  }

