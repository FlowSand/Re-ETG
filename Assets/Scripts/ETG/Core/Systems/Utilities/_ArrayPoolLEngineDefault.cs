// Decompiled with JetBrains decompiler
// Type: _ArrayPoolLEngineDefault
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable

public class _ArrayPoolLEngineDefault : IDisposable
  {
    private IntPtr swigCPtr;
    protected bool swigCMemOwn;

    internal _ArrayPoolLEngineDefault(IntPtr cPtr, bool cMemoryOwn)
    {
      this.swigCMemOwn = cMemoryOwn;
      this.swigCPtr = cPtr;
    }

    public _ArrayPoolLEngineDefault()
      : this(AkSoundEnginePINVOKE.CSharp_new__ArrayPoolLEngineDefault(), true)
    {
    }

    internal static IntPtr getCPtr(_ArrayPoolLEngineDefault obj)
    {
      return obj == null ? IntPtr.Zero : obj.swigCPtr;
    }

    internal virtual void setCPtr(IntPtr cPtr)
    {
      this.Dispose();
      this.swigCPtr = cPtr;
    }

    ~_ArrayPoolLEngineDefault() => this.Dispose();

    public virtual void Dispose()
    {
      lock ((object) this)
      {
        if (this.swigCPtr != IntPtr.Zero)
        {
          if (this.swigCMemOwn)
          {
            this.swigCMemOwn = false;
            AkSoundEnginePINVOKE.CSharp_delete__ArrayPoolLEngineDefault(this.swigCPtr);
          }
          this.swigCPtr = IntPtr.Zero;
        }
        GC.SuppressFinalize((object) this);
      }
    }

    public static int Get() => AkSoundEnginePINVOKE.CSharp__ArrayPoolLEngineDefault_Get();
  }

