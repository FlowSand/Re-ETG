// Decompiled with JetBrains decompiler
// Type: AkAuxSendArray
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Runtime.InteropServices;
using UnityEngine;

#nullable disable

public class AkAuxSendArray : IDisposable
  {
    private const int MAX_COUNT = 4;
    private readonly int SIZE_OF_AKAUXSENDVALUE = AkSoundEnginePINVOKE.CSharp_AkAuxSendValue_GetSizeOf();
    private IntPtr m_Buffer;
    private int m_Count;

    public AkAuxSendArray()
    {
      this.m_Buffer = Marshal.AllocHGlobal(4 * this.SIZE_OF_AKAUXSENDVALUE);
      this.m_Count = 0;
    }

    public AkAuxSendValue this[int index]
    {
      get
      {
        return index < this.m_Count ? new AkAuxSendValue(this.GetObjectPtr(index), false) : throw new IndexOutOfRangeException("Out of range access in AkAuxSendArray");
      }
    }

    public bool isFull => this.m_Count >= 4 || this.m_Buffer == IntPtr.Zero;

    public void Dispose()
    {
      if (!(this.m_Buffer != IntPtr.Zero))
        return;
      Marshal.FreeHGlobal(this.m_Buffer);
      this.m_Buffer = IntPtr.Zero;
      this.m_Count = 0;
    }

    ~AkAuxSendArray() => this.Dispose();

    public void Reset() => this.m_Count = 0;

    public bool Add(GameObject in_listenerGameObj, uint in_AuxBusID, float in_fValue)
    {
      if (this.isFull)
        return false;
      AkSoundEnginePINVOKE.CSharp_AkAuxSendValue_Set(this.GetObjectPtr(this.m_Count), AkSoundEngine.GetAkGameObjectID(in_listenerGameObj), in_AuxBusID, in_fValue);
      ++this.m_Count;
      return true;
    }

    public bool Add(uint in_AuxBusID, float in_fValue)
    {
      if (this.isFull)
        return false;
      AkSoundEnginePINVOKE.CSharp_AkAuxSendValue_Set(this.GetObjectPtr(this.m_Count), ulong.MaxValue, in_AuxBusID, in_fValue);
      ++this.m_Count;
      return true;
    }

    public bool Contains(GameObject in_listenerGameObj, uint in_AuxBusID)
    {
      if (this.m_Buffer == IntPtr.Zero)
        return false;
      for (int index = 0; index < this.m_Count; ++index)
      {
        if (AkSoundEnginePINVOKE.CSharp_AkAuxSendValue_IsSame(this.GetObjectPtr(index), AkSoundEngine.GetAkGameObjectID(in_listenerGameObj), in_AuxBusID))
          return true;
      }
      return false;
    }

    public bool Contains(uint in_AuxBusID)
    {
      if (this.m_Buffer == IntPtr.Zero)
        return false;
      for (int index = 0; index < this.m_Count; ++index)
      {
        if (AkSoundEnginePINVOKE.CSharp_AkAuxSendValue_IsSame(this.GetObjectPtr(index), ulong.MaxValue, in_AuxBusID))
          return true;
      }
      return false;
    }

    public AKRESULT SetValues(GameObject gameObject)
    {
      return (AKRESULT) AkSoundEnginePINVOKE.CSharp_AkAuxSendValue_SetGameObjectAuxSendValues(this.m_Buffer, AkSoundEngine.GetAkGameObjectID(gameObject), (uint) this.m_Count);
    }

    public AKRESULT GetValues(GameObject gameObject)
    {
      uint jarg3 = 4;
      AKRESULT objectAuxSendValues = (AKRESULT) AkSoundEnginePINVOKE.CSharp_AkAuxSendValue_GetGameObjectAuxSendValues(this.m_Buffer, AkSoundEngine.GetAkGameObjectID(gameObject), ref jarg3);
      this.m_Count = (int) jarg3;
      return objectAuxSendValues;
    }

    public IntPtr GetBuffer() => this.m_Buffer;

    public int Count() => this.m_Count;

    private IntPtr GetObjectPtr(int index)
    {
      return (IntPtr) (this.m_Buffer.ToInt64() + (long) (this.SIZE_OF_AKAUXSENDVALUE * index));
    }
  }

