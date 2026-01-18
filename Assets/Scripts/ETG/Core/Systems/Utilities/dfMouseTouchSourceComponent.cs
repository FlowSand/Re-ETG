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

