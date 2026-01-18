using System;

#nullable disable

[Serializable]
public struct CommentModule
  {
    public string stringKey;
    public float duration;
    public CommentModule.CommentTarget target;
    public float delay;

    public enum CommentTarget
    {
      PRIMARY,
      SECONDARY,
      DOG,
    }
  }

