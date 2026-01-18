// Decompiled with JetBrains decompiler
// Type: AkPropagationPathInfoProxy
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable

public class AkPropagationPathInfoProxy : AkPropagationPathInfo
  {
    private IntPtr swigCPtr;

    internal AkPropagationPathInfoProxy(IntPtr cPtr, bool cMemoryOwn)
      : base(AkSoundEnginePINVOKE.CSharp_AkPropagationPathInfoProxy_SWIGUpcast(cPtr), cMemoryOwn)
    {
      this.swigCPtr = cPtr;
    }

    public AkPropagationPathInfoProxy()
      : this(AkSoundEnginePINVOKE.CSharp_new_AkPropagationPathInfoProxy(), true)
    {
    }

    internal static IntPtr getCPtr(AkPropagationPathInfoProxy obj)
    {
      return obj == null ? IntPtr.Zero : obj.swigCPtr;
    }

    internal override void setCPtr(IntPtr cPtr)
    {
      base.setCPtr(AkSoundEnginePINVOKE.CSharp_AkPropagationPathInfoProxy_SWIGUpcast(cPtr));
      this.swigCPtr = cPtr;
    }

    ~AkPropagationPathInfoProxy() => this.Dispose();

    public override void Dispose()
    {
      lock ((object) this)
      {
        if (this.swigCPtr != IntPtr.Zero)
        {
          if (this.swigCMemOwn)
          {
            this.swigCMemOwn = false;
            AkSoundEnginePINVOKE.CSharp_delete_AkPropagationPathInfoProxy(this.swigCPtr);
          }
          this.swigCPtr = IntPtr.Zero;
        }
        GC.SuppressFinalize((object) this);
        base.Dispose();
      }
    }

    public static int GetSizeOf()
    {
      return AkSoundEnginePINVOKE.CSharp_AkPropagationPathInfoProxy_GetSizeOf();
    }

    public AkVector GetNodePoint(uint idx)
    {
      IntPtr nodePoint = AkSoundEnginePINVOKE.CSharp_AkPropagationPathInfoProxy_GetNodePoint(this.swigCPtr, idx);
      return !(nodePoint == IntPtr.Zero) ? new AkVector(nodePoint, false) : (AkVector) null;
    }
  }

