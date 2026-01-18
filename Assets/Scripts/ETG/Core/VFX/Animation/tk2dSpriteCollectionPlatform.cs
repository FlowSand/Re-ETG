using System;

#nullable disable

[Serializable]
public class tk2dSpriteCollectionPlatform
  {
    public string name = string.Empty;
    public tk2dSpriteCollection spriteCollection;

    public bool Valid => this.name.Length > 0 && (UnityEngine.Object) this.spriteCollection != (UnityEngine.Object) null;

    public void CopyFrom(tk2dSpriteCollectionPlatform source)
    {
      this.name = source.name;
      this.spriteCollection = source.spriteCollection;
    }
  }

