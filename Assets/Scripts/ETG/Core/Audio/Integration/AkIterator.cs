// Decompiled with JetBrains decompiler
// Type: AkIterator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable

namespace ETG.Core.Audio.Integration
{
    public class AkIterator : IDisposable
    {
      private IntPtr swigCPtr;
      protected bool swigCMemOwn;

      internal AkIterator(IntPtr cPtr, bool cMemoryOwn)
      {
        this.swigCMemOwn = cMemoryOwn;
        this.swigCPtr = cPtr;
      }

      public AkIterator()
        : this(AkSoundEnginePINVOKE.CSharp_new_AkIterator(), true)
      {
      }

      internal static IntPtr getCPtr(AkIterator obj) => obj == null ? IntPtr.Zero : obj.swigCPtr;

      internal virtual void setCPtr(IntPtr cPtr)
      {
        this.Dispose();
        this.swigCPtr = cPtr;
      }

      ~AkIterator() => this.Dispose();

      public virtual void Dispose()
      {
        lock ((object) this)
        {
          if (this.swigCPtr != IntPtr.Zero)
          {
            if (this.swigCMemOwn)
            {
              this.swigCMemOwn = false;
              AkSoundEnginePINVOKE.CSharp_delete_AkIterator(this.swigCPtr);
            }
            this.swigCPtr = IntPtr.Zero;
          }
          GC.SuppressFinalize((object) this);
        }
      }

      public AkPlaylistItem pItem
      {
        set
        {
          AkSoundEnginePINVOKE.CSharp_AkIterator_pItem_set(this.swigCPtr, AkPlaylistItem.getCPtr(value));
        }
        get
        {
          IntPtr cPtr = AkSoundEnginePINVOKE.CSharp_AkIterator_pItem_get(this.swigCPtr);
          return !(cPtr == IntPtr.Zero) ? new AkPlaylistItem(cPtr, false) : (AkPlaylistItem) null;
        }
      }

      public AkIterator NextIter()
      {
        return new AkIterator(AkSoundEnginePINVOKE.CSharp_AkIterator_NextIter(this.swigCPtr), false);
      }

      public AkIterator PrevIter()
      {
        return new AkIterator(AkSoundEnginePINVOKE.CSharp_AkIterator_PrevIter(this.swigCPtr), false);
      }

      public AkPlaylistItem GetItem()
      {
        return new AkPlaylistItem(AkSoundEnginePINVOKE.CSharp_AkIterator_GetItem(this.swigCPtr), false);
      }

      public bool IsEqualTo(AkIterator in_rOp)
      {
        return AkSoundEnginePINVOKE.CSharp_AkIterator_IsEqualTo(this.swigCPtr, AkIterator.getCPtr(in_rOp));
      }

      public bool IsDifferentFrom(AkIterator in_rOp)
      {
        return AkSoundEnginePINVOKE.CSharp_AkIterator_IsDifferentFrom(this.swigCPtr, AkIterator.getCPtr(in_rOp));
      }
    }

}
