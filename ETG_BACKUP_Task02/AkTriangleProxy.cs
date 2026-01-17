// Decompiled with JetBrains decompiler
// Type: AkTriangleProxy
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable
public class AkTriangleProxy : AkTriangle
{
  private IntPtr swigCPtr;

  internal AkTriangleProxy(IntPtr cPtr, bool cMemoryOwn)
    : base(AkSoundEnginePINVOKE.CSharp_AkTriangleProxy_SWIGUpcast(cPtr), cMemoryOwn)
  {
    this.swigCPtr = cPtr;
  }

  public AkTriangleProxy()
    : this(AkSoundEnginePINVOKE.CSharp_new_AkTriangleProxy(), true)
  {
  }

  internal static IntPtr getCPtr(AkTriangleProxy obj) => obj == null ? IntPtr.Zero : obj.swigCPtr;

  internal override void setCPtr(IntPtr cPtr)
  {
    base.setCPtr(AkSoundEnginePINVOKE.CSharp_AkTriangleProxy_SWIGUpcast(cPtr));
    this.swigCPtr = cPtr;
  }

  ~AkTriangleProxy() => this.Dispose();

  public override void Dispose()
  {
    lock ((object) this)
    {
      if (this.swigCPtr != IntPtr.Zero)
      {
        if (this.swigCMemOwn)
        {
          this.swigCMemOwn = false;
          AkSoundEnginePINVOKE.CSharp_delete_AkTriangleProxy(this.swigCPtr);
        }
        this.swigCPtr = IntPtr.Zero;
      }
      GC.SuppressFinalize((object) this);
      base.Dispose();
    }
  }

  public void Clear() => AkSoundEnginePINVOKE.CSharp_AkTriangleProxy_Clear(this.swigCPtr);

  public void DeleteName() => AkSoundEnginePINVOKE.CSharp_AkTriangleProxy_DeleteName(this.swigCPtr);

  public static int GetSizeOf() => AkSoundEnginePINVOKE.CSharp_AkTriangleProxy_GetSizeOf();
}
