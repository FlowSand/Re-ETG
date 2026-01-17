// Decompiled with JetBrains decompiler
// Type: AkCallbackSerializer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable
public class AkCallbackSerializer : IDisposable
{
  private IntPtr swigCPtr;
  protected bool swigCMemOwn;

  internal AkCallbackSerializer(IntPtr cPtr, bool cMemoryOwn)
  {
    this.swigCMemOwn = cMemoryOwn;
    this.swigCPtr = cPtr;
  }

  public AkCallbackSerializer()
    : this(AkSoundEnginePINVOKE.CSharp_new_AkCallbackSerializer(), true)
  {
  }

  internal static IntPtr getCPtr(AkCallbackSerializer obj)
  {
    return obj == null ? IntPtr.Zero : obj.swigCPtr;
  }

  internal virtual void setCPtr(IntPtr cPtr)
  {
    this.Dispose();
    this.swigCPtr = cPtr;
  }

  ~AkCallbackSerializer() => this.Dispose();

  public virtual void Dispose()
  {
    lock ((object) this)
    {
      if (this.swigCPtr != IntPtr.Zero)
      {
        if (this.swigCMemOwn)
        {
          this.swigCMemOwn = false;
          AkSoundEnginePINVOKE.CSharp_delete_AkCallbackSerializer(this.swigCPtr);
        }
        this.swigCPtr = IntPtr.Zero;
      }
      GC.SuppressFinalize((object) this);
    }
  }

  public static AKRESULT Init(IntPtr in_pMemory, uint in_uSize)
  {
    return (AKRESULT) AkSoundEnginePINVOKE.CSharp_AkCallbackSerializer_Init(in_pMemory, in_uSize);
  }

  public static void Term() => AkSoundEnginePINVOKE.CSharp_AkCallbackSerializer_Term();

  public static IntPtr Lock() => AkSoundEnginePINVOKE.CSharp_AkCallbackSerializer_Lock();

  public static void SetLocalOutput(uint in_uErrorLevel)
  {
    AkSoundEnginePINVOKE.CSharp_AkCallbackSerializer_SetLocalOutput(in_uErrorLevel);
  }

  public static void Unlock() => AkSoundEnginePINVOKE.CSharp_AkCallbackSerializer_Unlock();

  public static AKRESULT AudioSourceChangeCallbackFunc(
    bool in_bOtherAudioPlaying,
    object in_pCookie)
  {
    return (AKRESULT) AkSoundEnginePINVOKE.CSharp_AkCallbackSerializer_AudioSourceChangeCallbackFunc(in_bOtherAudioPlaying, in_pCookie == null ? IntPtr.Zero : (IntPtr) in_pCookie.GetHashCode());
  }
}
