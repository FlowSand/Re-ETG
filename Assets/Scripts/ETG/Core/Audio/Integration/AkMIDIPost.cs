// Decompiled with JetBrains decompiler
// Type: AkMIDIPost
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable

public class AkMIDIPost : AkMIDIEvent
  {
    private IntPtr swigCPtr;

    internal AkMIDIPost(IntPtr cPtr, bool cMemoryOwn)
      : base(AkSoundEnginePINVOKE.CSharp_AkMIDIPost_SWIGUpcast(cPtr), cMemoryOwn)
    {
      this.swigCPtr = cPtr;
    }

    public AkMIDIPost()
      : this(AkSoundEnginePINVOKE.CSharp_new_AkMIDIPost(), true)
    {
    }

    internal static IntPtr getCPtr(AkMIDIPost obj) => obj == null ? IntPtr.Zero : obj.swigCPtr;

    internal override void setCPtr(IntPtr cPtr)
    {
      base.setCPtr(AkSoundEnginePINVOKE.CSharp_AkMIDIPost_SWIGUpcast(cPtr));
      this.swigCPtr = cPtr;
    }

    ~AkMIDIPost() => this.Dispose();

    public override void Dispose()
    {
      lock ((object) this)
      {
        if (this.swigCPtr != IntPtr.Zero)
        {
          if (this.swigCMemOwn)
          {
            this.swigCMemOwn = false;
            AkSoundEnginePINVOKE.CSharp_delete_AkMIDIPost(this.swigCPtr);
          }
          this.swigCPtr = IntPtr.Zero;
        }
        GC.SuppressFinalize((object) this);
        base.Dispose();
      }
    }

    public uint uOffset
    {
      set => AkSoundEnginePINVOKE.CSharp_AkMIDIPost_uOffset_set(this.swigCPtr, value);
      get => AkSoundEnginePINVOKE.CSharp_AkMIDIPost_uOffset_get(this.swigCPtr);
    }

    public AKRESULT PostOnEvent(uint in_eventID, GameObject in_gameObjectID, uint in_uNumPosts)
    {
      ulong akGameObjectId = AkSoundEngine.GetAkGameObjectID(in_gameObjectID);
      AkSoundEngine.PreGameObjectAPICall(in_gameObjectID, akGameObjectId);
      return (AKRESULT) AkSoundEnginePINVOKE.CSharp_AkMIDIPost_PostOnEvent(this.swigCPtr, in_eventID, akGameObjectId, in_uNumPosts);
    }

    public void Clone(AkMIDIPost other)
    {
      AkSoundEnginePINVOKE.CSharp_AkMIDIPost_Clone(this.swigCPtr, AkMIDIPost.getCPtr(other));
    }

    public static int GetSizeOf() => AkSoundEnginePINVOKE.CSharp_AkMIDIPost_GetSizeOf();
  }

