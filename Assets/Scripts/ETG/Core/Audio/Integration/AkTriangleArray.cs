using System;
using System.Runtime.InteropServices;

#nullable disable

public class AkTriangleArray : IDisposable
  {
    private readonly int SIZE_OF_AKTRIANGLE = AkSoundEnginePINVOKE.CSharp_AkTriangleProxy_GetSizeOf();
    private IntPtr m_Buffer;
    private int m_Count;

    public AkTriangleArray(int count)
    {
      this.m_Count = count;
      this.m_Buffer = Marshal.AllocHGlobal(count * this.SIZE_OF_AKTRIANGLE);
      if (!(this.m_Buffer != IntPtr.Zero))
        return;
      for (int index = 0; index < count; ++index)
        AkSoundEnginePINVOKE.CSharp_AkTriangleProxy_Clear(this.GetObjectPtr(index));
    }

    public void Dispose()
    {
      if (!(this.m_Buffer != IntPtr.Zero))
        return;
      for (int index = 0; index < this.m_Count; ++index)
        AkSoundEnginePINVOKE.CSharp_AkTriangleProxy_DeleteName(this.GetObjectPtr(index));
      Marshal.FreeHGlobal(this.m_Buffer);
      this.m_Buffer = IntPtr.Zero;
      this.m_Count = 0;
    }

    ~AkTriangleArray() => this.Dispose();

    public void Reset() => this.m_Count = 0;

    public AkTriangle GetTriangle(int index)
    {
      return index >= this.m_Count ? (AkTriangle) null : new AkTriangle(this.GetObjectPtr(index), false);
    }

    public IntPtr GetBuffer() => this.m_Buffer;

    public int Count() => this.m_Count;

    private IntPtr GetObjectPtr(int index)
    {
      return (IntPtr) (this.m_Buffer.ToInt64() + (long) (this.SIZE_OF_AKTRIANGLE * index));
    }
  }

