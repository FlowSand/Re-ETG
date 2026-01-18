using System;

#nullable disable

[Serializable]
public class tk2dSpriteAnimationFrame
  {
    public tk2dSpriteCollectionData spriteCollection;
    public int spriteId;
    public bool invulnerableFrame;
    public bool groundedFrame = true;
    public bool requiresOffscreenUpdate;
    public string eventAudio = string.Empty;
    public string eventVfx = string.Empty;
    public string eventStopVfx = string.Empty;
    public bool eventLerpEmissive;
    public float eventLerpEmissiveTime = 0.5f;
    public float eventLerpEmissivePower = 30f;
    public bool forceMaterialUpdate;
    public bool finishedSpawning;
    public bool triggerEvent;
    public string eventInfo = string.Empty;
    public int eventInt;
    public float eventFloat;
    public tk2dSpriteAnimationFrame.OutlineModifier eventOutline;

    public void CopyFrom(tk2dSpriteAnimationFrame source) => this.CopyFrom(source, true);

    public void CopyTriggerFrom(tk2dSpriteAnimationFrame source)
    {
      this.triggerEvent = source.triggerEvent;
      this.eventInfo = source.eventInfo;
      this.eventInt = source.eventInt;
      this.eventFloat = source.eventFloat;
      this.eventAudio = source.eventAudio;
      this.eventVfx = source.eventVfx;
      this.eventStopVfx = source.eventStopVfx;
      this.eventOutline = source.eventOutline;
      this.forceMaterialUpdate = source.forceMaterialUpdate;
      this.finishedSpawning = source.finishedSpawning;
      this.eventLerpEmissive = source.eventLerpEmissive;
      this.eventLerpEmissivePower = source.eventLerpEmissivePower;
      this.eventLerpEmissiveTime = source.eventLerpEmissiveTime;
    }

    public void ClearTrigger()
    {
      this.triggerEvent = false;
      this.eventInt = 0;
      this.eventFloat = 0.0f;
      this.eventInfo = string.Empty;
      this.eventAudio = string.Empty;
      this.eventVfx = string.Empty;
      this.eventStopVfx = string.Empty;
      this.eventOutline = tk2dSpriteAnimationFrame.OutlineModifier.Unspecified;
      this.forceMaterialUpdate = false;
      this.finishedSpawning = false;
      this.eventLerpEmissive = false;
      this.eventLerpEmissivePower = 30f;
      this.eventLerpEmissiveTime = 0.5f;
    }

    public void CopyFrom(tk2dSpriteAnimationFrame source, bool full)
    {
      this.spriteCollection = source.spriteCollection;
      this.spriteId = source.spriteId;
      this.invulnerableFrame = source.invulnerableFrame;
      this.groundedFrame = source.groundedFrame;
      this.requiresOffscreenUpdate = source.requiresOffscreenUpdate;
      if (!full)
        return;
      this.CopyTriggerFrom(source);
    }

    public enum OutlineModifier
    {
      Unspecified = 0,
      TurnOn = 10, // 0x0000000A
      TurnOff = 20, // 0x00000014
    }
  }

