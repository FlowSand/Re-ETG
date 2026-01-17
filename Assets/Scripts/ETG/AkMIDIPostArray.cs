// Decompiled with JetBrains decompiler
// Type: AkMIDIPostArray
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Runtime.InteropServices;
using UnityEngine;

#nullable disable
public class AkMIDIPostArray
{
  private readonly int m_Count;
  private readonly int SIZE_OF = AkSoundEnginePINVOKE.CSharp_AkMIDIPost_GetSizeOf();
  private IntPtr m_Buffer = IntPtr.Zero;

  public AkMIDIPostArray(int size)
  {
    this.m_Count = size;
    this.m_Buffer = Marshal.AllocHGlobal(this.m_Count * this.SIZE_OF);
  }

  public AkMIDIPost this[int index]
  {
    get
    {
      return index < this.m_Count ? new AkMIDIPost(this.GetObjectPtr(index), false) : throw new IndexOutOfRangeException("Out of range access in AkMIDIPostArray");
    }
    set
    {
      if (index >= this.m_Count)
        throw new IndexOutOfRangeException("Out of range access in AkMIDIPostArray");
      AkSoundEnginePINVOKE.CSharp_AkMIDIPost_Clone(this.GetObjectPtr(index), AkMIDIPost.getCPtr(value));
    }
  }

  ~AkMIDIPostArray()
  {
    Marshal.FreeHGlobal(this.m_Buffer);
    this.m_Buffer = IntPtr.Zero;
  }

  public void PostOnEvent(uint in_eventID, GameObject gameObject)
  {
    ulong akGameObjectId = AkSoundEngine.GetAkGameObjectID(gameObject);
    AkSoundEngine.PreGameObjectAPICall(gameObject, akGameObjectId);
    AkSoundEnginePINVOKE.CSharp_AkMIDIPost_PostOnEvent(this.m_Buffer, in_eventID, akGameObjectId, (uint) this.m_Count);
  }

  public void PostOnEvent(uint in_eventID, GameObject gameObject, int count)
  {
    if (count >= this.m_Count)
      throw new IndexOutOfRangeException("Out of range access in AkMIDIPostArray");
    ulong akGameObjectId = AkSoundEngine.GetAkGameObjectID(gameObject);
    AkSoundEngine.PreGameObjectAPICall(gameObject, akGameObjectId);
    AkSoundEnginePINVOKE.CSharp_AkMIDIPost_PostOnEvent(this.m_Buffer, in_eventID, akGameObjectId, (uint) count);
  }

  public IntPtr GetBuffer() => this.m_Buffer;

  public int Count() => this.m_Count;

  private IntPtr GetObjectPtr(int index)
  {
    return (IntPtr) (this.m_Buffer.ToInt64() + (long) (this.SIZE_OF * index));
  }
}
