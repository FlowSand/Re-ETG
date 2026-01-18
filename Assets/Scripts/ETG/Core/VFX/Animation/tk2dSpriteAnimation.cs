using UnityEngine;

#nullable disable

[AddComponentMenu("2D Toolkit/Backend/tk2dSpriteAnimation")]
public class tk2dSpriteAnimation : MonoBehaviour
  {
    public tk2dSpriteAnimationClip[] clips;

    public tk2dSpriteAnimationClip GetClipByName(string name)
    {
      if (string.IsNullOrEmpty(name))
        return (tk2dSpriteAnimationClip) null;
      for (int index = 0; index < this.clips.Length; ++index)
      {
        if (this.clips[index].name == name)
          return this.clips[index];
      }
      return (tk2dSpriteAnimationClip) null;
    }

    public tk2dSpriteAnimationClip GetClipById(int id)
    {
      return id < 0 || id >= this.clips.Length || this.clips[id].Empty ? (tk2dSpriteAnimationClip) null : this.clips[id];
    }

    public int GetClipIdByName(string name)
    {
      if (string.IsNullOrEmpty(name))
        return -1;
      for (int clipIdByName = 0; clipIdByName < this.clips.Length; ++clipIdByName)
      {
        if (this.clips[clipIdByName].name == name)
          return clipIdByName;
      }
      return -1;
    }

    public int GetClipIdByName(tk2dSpriteAnimationClip clip)
    {
      for (int clipIdByName = 0; clipIdByName < this.clips.Length; ++clipIdByName)
      {
        if (this.clips[clipIdByName] == clip)
          return clipIdByName;
      }
      return -1;
    }

    public tk2dSpriteAnimationClip FirstValidClip
    {
      get
      {
        for (int index = 0; index < this.clips.Length; ++index)
        {
          if (!this.clips[index].Empty && (Object) this.clips[index].frames[0].spriteCollection != (Object) null && this.clips[index].frames[0].spriteId != -1)
            return this.clips[index];
        }
        return (tk2dSpriteAnimationClip) null;
      }
    }
  }

