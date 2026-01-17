// Decompiled with JetBrains decompiler
// Type: AkRoomParams
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable
public class AkRoomParams : IDisposable
{
  private IntPtr swigCPtr;
  protected bool swigCMemOwn;

  internal AkRoomParams(IntPtr cPtr, bool cMemoryOwn)
  {
    this.swigCMemOwn = cMemoryOwn;
    this.swigCPtr = cPtr;
  }

  public AkRoomParams()
    : this(AkSoundEnginePINVOKE.CSharp_new_AkRoomParams(), true)
  {
  }

  internal static IntPtr getCPtr(AkRoomParams obj) => obj == null ? IntPtr.Zero : obj.swigCPtr;

  internal virtual void setCPtr(IntPtr cPtr)
  {
    this.Dispose();
    this.swigCPtr = cPtr;
  }

  ~AkRoomParams() => this.Dispose();

  public virtual void Dispose()
  {
    lock ((object) this)
    {
      if (this.swigCPtr != IntPtr.Zero)
      {
        if (this.swigCMemOwn)
        {
          this.swigCMemOwn = false;
          AkSoundEnginePINVOKE.CSharp_delete_AkRoomParams(this.swigCPtr);
        }
        this.swigCPtr = IntPtr.Zero;
      }
      GC.SuppressFinalize((object) this);
    }
  }

  public AkVector Up
  {
    set => AkSoundEnginePINVOKE.CSharp_AkRoomParams_Up_set(this.swigCPtr, AkVector.getCPtr(value));
    get
    {
      IntPtr cPtr = AkSoundEnginePINVOKE.CSharp_AkRoomParams_Up_get(this.swigCPtr);
      return !(cPtr == IntPtr.Zero) ? new AkVector(cPtr, false) : (AkVector) null;
    }
  }

  public AkVector Front
  {
    set
    {
      AkSoundEnginePINVOKE.CSharp_AkRoomParams_Front_set(this.swigCPtr, AkVector.getCPtr(value));
    }
    get
    {
      IntPtr cPtr = AkSoundEnginePINVOKE.CSharp_AkRoomParams_Front_get(this.swigCPtr);
      return !(cPtr == IntPtr.Zero) ? new AkVector(cPtr, false) : (AkVector) null;
    }
  }

  public uint ReverbAuxBus
  {
    set => AkSoundEnginePINVOKE.CSharp_AkRoomParams_ReverbAuxBus_set(this.swigCPtr, value);
    get => AkSoundEnginePINVOKE.CSharp_AkRoomParams_ReverbAuxBus_get(this.swigCPtr);
  }

  public float ReverbLevel
  {
    set => AkSoundEnginePINVOKE.CSharp_AkRoomParams_ReverbLevel_set(this.swigCPtr, value);
    get => AkSoundEnginePINVOKE.CSharp_AkRoomParams_ReverbLevel_get(this.swigCPtr);
  }

  public float WallOcclusion
  {
    set => AkSoundEnginePINVOKE.CSharp_AkRoomParams_WallOcclusion_set(this.swigCPtr, value);
    get => AkSoundEnginePINVOKE.CSharp_AkRoomParams_WallOcclusion_get(this.swigCPtr);
  }

  public int Priority
  {
    set => AkSoundEnginePINVOKE.CSharp_AkRoomParams_Priority_set(this.swigCPtr, value);
    get => AkSoundEnginePINVOKE.CSharp_AkRoomParams_Priority_get(this.swigCPtr);
  }

  public float RoomGameObj_AuxSendLevelToSelf
  {
    set
    {
      AkSoundEnginePINVOKE.CSharp_AkRoomParams_RoomGameObj_AuxSendLevelToSelf_set(this.swigCPtr, value);
    }
    get
    {
      return AkSoundEnginePINVOKE.CSharp_AkRoomParams_RoomGameObj_AuxSendLevelToSelf_get(this.swigCPtr);
    }
  }

  public bool RoomGameObj_KeepRegistered
  {
    set
    {
      AkSoundEnginePINVOKE.CSharp_AkRoomParams_RoomGameObj_KeepRegistered_set(this.swigCPtr, value);
    }
    get => AkSoundEnginePINVOKE.CSharp_AkRoomParams_RoomGameObj_KeepRegistered_get(this.swigCPtr);
  }
}
