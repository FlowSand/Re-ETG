// Decompiled with JetBrains decompiler
// Type: dfMouseTouchSourceComponent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

[AddComponentMenu("Daikon Forge/Input/Debugging/Simulate Touch with Mouse")]
public class dfMouseTouchSourceComponent : dfTouchInputSourceComponent
  {
    public bool editorOnly = true;
    private dfMouseTouchInputSource source;

    public override IDFTouchInputSource Source
    {
      get
      {
        if (this.editorOnly && !Application.isEditor)
          return (IDFTouchInputSource) null;
        if (this.source == null)
          this.source = new dfMouseTouchInputSource();
        return (IDFTouchInputSource) this.source;
      }
    }

    public void Start() => this.useGUILayout = false;

    public void OnGUI()
    {
      if (this.source == null)
        return;
      this.source.MirrorAlt = !Event.current.control && !Event.current.shift;
      this.source.ParallelAlt = !this.source.MirrorAlt && Event.current.shift;
    }
  }

