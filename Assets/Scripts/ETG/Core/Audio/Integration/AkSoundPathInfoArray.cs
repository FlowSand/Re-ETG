using System;
using System.Runtime.InteropServices;

#nullable disable

public class AkSoundPathInfoArray : IDisposable
    {
        private readonly int SIZE_OF_STRUCTURE = AkSoundEnginePINVOKE.CSharp_AkSoundPathInfoProxy_GetSizeOf();
        private IntPtr m_Buffer;
        private int m_Count;

        public AkSoundPathInfoArray(int count)
        {
            this.m_Count = count;
            this.m_Buffer = Marshal.AllocHGlobal(count * this.SIZE_OF_STRUCTURE);
        }

        public void Dispose()
        {
            if (!(this.m_Buffer != IntPtr.Zero))
                return;
            Marshal.FreeHGlobal(this.m_Buffer);
            this.m_Buffer = IntPtr.Zero;
            this.m_Count = 0;
        }

        ~AkSoundPathInfoArray() => this.Dispose();

        public void Reset() => this.m_Count = 0;

        public AkSoundPathInfoProxy GetSoundPathInfo(int index)
        {
            return index >= this.m_Count ? (AkSoundPathInfoProxy) null : new AkSoundPathInfoProxy(this.GetObjectPtr(index), false);
        }

        public IntPtr GetBuffer() => this.m_Buffer;

        public int Count() => this.m_Count;

        private IntPtr GetObjectPtr(int index)
        {
            return (IntPtr) (this.m_Buffer.ToInt64() + (long) (this.SIZE_OF_STRUCTURE * index));
        }
    }

