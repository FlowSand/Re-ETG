// Decompiled with JetBrains decompiler
// Type: AkMIDIEvent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable

public class AkMIDIEvent : IDisposable
  {
    private IntPtr swigCPtr;
    protected bool swigCMemOwn;

    internal AkMIDIEvent(IntPtr cPtr, bool cMemoryOwn)
    {
      this.swigCMemOwn = cMemoryOwn;
      this.swigCPtr = cPtr;
    }

    public AkMIDIEvent()
      : this(AkSoundEnginePINVOKE.CSharp_new_AkMIDIEvent(), true)
    {
    }

    internal static IntPtr getCPtr(AkMIDIEvent obj) => obj == null ? IntPtr.Zero : obj.swigCPtr;

    internal virtual void setCPtr(IntPtr cPtr)
    {
      this.Dispose();
      this.swigCPtr = cPtr;
    }

    ~AkMIDIEvent() => this.Dispose();

    public virtual void Dispose()
    {
      lock ((object) this)
      {
        if (this.swigCPtr != IntPtr.Zero)
        {
          if (this.swigCMemOwn)
          {
            this.swigCMemOwn = false;
            AkSoundEnginePINVOKE.CSharp_delete_AkMIDIEvent(this.swigCPtr);
          }
          this.swigCPtr = IntPtr.Zero;
        }
        GC.SuppressFinalize((object) this);
      }
    }

    public byte byChan
    {
      set => AkSoundEnginePINVOKE.CSharp_AkMIDIEvent_byChan_set(this.swigCPtr, value);
      get => AkSoundEnginePINVOKE.CSharp_AkMIDIEvent_byChan_get(this.swigCPtr);
    }

    public AkMIDIEvent.tGen Gen
    {
      set
      {
        AkSoundEnginePINVOKE.CSharp_AkMIDIEvent_Gen_set(this.swigCPtr, AkMIDIEvent.tGen.getCPtr(value));
      }
      get
      {
        IntPtr cPtr = AkSoundEnginePINVOKE.CSharp_AkMIDIEvent_Gen_get(this.swigCPtr);
        return !(cPtr == IntPtr.Zero) ? new AkMIDIEvent.tGen(cPtr, false) : (AkMIDIEvent.tGen) null;
      }
    }

    public AkMIDIEvent.tCc Cc
    {
      set
      {
        AkSoundEnginePINVOKE.CSharp_AkMIDIEvent_Cc_set(this.swigCPtr, AkMIDIEvent.tCc.getCPtr(value));
      }
      get
      {
        IntPtr cPtr = AkSoundEnginePINVOKE.CSharp_AkMIDIEvent_Cc_get(this.swigCPtr);
        return !(cPtr == IntPtr.Zero) ? new AkMIDIEvent.tCc(cPtr, false) : (AkMIDIEvent.tCc) null;
      }
    }

    public AkMIDIEvent.tNoteOnOff NoteOnOff
    {
      set
      {
        AkSoundEnginePINVOKE.CSharp_AkMIDIEvent_NoteOnOff_set(this.swigCPtr, AkMIDIEvent.tNoteOnOff.getCPtr(value));
      }
      get
      {
        IntPtr cPtr = AkSoundEnginePINVOKE.CSharp_AkMIDIEvent_NoteOnOff_get(this.swigCPtr);
        return !(cPtr == IntPtr.Zero) ? new AkMIDIEvent.tNoteOnOff(cPtr, false) : (AkMIDIEvent.tNoteOnOff) null;
      }
    }

    public AkMIDIEvent.tPitchBend PitchBend
    {
      set
      {
        AkSoundEnginePINVOKE.CSharp_AkMIDIEvent_PitchBend_set(this.swigCPtr, AkMIDIEvent.tPitchBend.getCPtr(value));
      }
      get
      {
        IntPtr cPtr = AkSoundEnginePINVOKE.CSharp_AkMIDIEvent_PitchBend_get(this.swigCPtr);
        return !(cPtr == IntPtr.Zero) ? new AkMIDIEvent.tPitchBend(cPtr, false) : (AkMIDIEvent.tPitchBend) null;
      }
    }

    public AkMIDIEvent.tNoteAftertouch NoteAftertouch
    {
      set
      {
        AkSoundEnginePINVOKE.CSharp_AkMIDIEvent_NoteAftertouch_set(this.swigCPtr, AkMIDIEvent.tNoteAftertouch.getCPtr(value));
      }
      get
      {
        IntPtr cPtr = AkSoundEnginePINVOKE.CSharp_AkMIDIEvent_NoteAftertouch_get(this.swigCPtr);
        return !(cPtr == IntPtr.Zero) ? new AkMIDIEvent.tNoteAftertouch(cPtr, false) : (AkMIDIEvent.tNoteAftertouch) null;
      }
    }

    public AkMIDIEvent.tChanAftertouch ChanAftertouch
    {
      set
      {
        AkSoundEnginePINVOKE.CSharp_AkMIDIEvent_ChanAftertouch_set(this.swigCPtr, AkMIDIEvent.tChanAftertouch.getCPtr(value));
      }
      get
      {
        IntPtr cPtr = AkSoundEnginePINVOKE.CSharp_AkMIDIEvent_ChanAftertouch_get(this.swigCPtr);
        return !(cPtr == IntPtr.Zero) ? new AkMIDIEvent.tChanAftertouch(cPtr, false) : (AkMIDIEvent.tChanAftertouch) null;
      }
    }

    public AkMIDIEvent.tProgramChange ProgramChange
    {
      set
      {
        AkSoundEnginePINVOKE.CSharp_AkMIDIEvent_ProgramChange_set(this.swigCPtr, AkMIDIEvent.tProgramChange.getCPtr(value));
      }
      get
      {
        IntPtr cPtr = AkSoundEnginePINVOKE.CSharp_AkMIDIEvent_ProgramChange_get(this.swigCPtr);
        return !(cPtr == IntPtr.Zero) ? new AkMIDIEvent.tProgramChange(cPtr, false) : (AkMIDIEvent.tProgramChange) null;
      }
    }

    public AkMIDIEventTypes byType
    {
      set => AkSoundEnginePINVOKE.CSharp_AkMIDIEvent_byType_set(this.swigCPtr, (int) value);
      get => (AkMIDIEventTypes) AkSoundEnginePINVOKE.CSharp_AkMIDIEvent_byType_get(this.swigCPtr);
    }

    public byte byOnOffNote
    {
      set => AkSoundEnginePINVOKE.CSharp_AkMIDIEvent_byOnOffNote_set(this.swigCPtr, value);
      get => AkSoundEnginePINVOKE.CSharp_AkMIDIEvent_byOnOffNote_get(this.swigCPtr);
    }

    public byte byVelocity
    {
      set => AkSoundEnginePINVOKE.CSharp_AkMIDIEvent_byVelocity_set(this.swigCPtr, value);
      get => AkSoundEnginePINVOKE.CSharp_AkMIDIEvent_byVelocity_get(this.swigCPtr);
    }

    public AkMIDICcTypes byCc
    {
      set => AkSoundEnginePINVOKE.CSharp_AkMIDIEvent_byCc_set(this.swigCPtr, (int) value);
      get => (AkMIDICcTypes) AkSoundEnginePINVOKE.CSharp_AkMIDIEvent_byCc_get(this.swigCPtr);
    }

    public byte byCcValue
    {
      set => AkSoundEnginePINVOKE.CSharp_AkMIDIEvent_byCcValue_set(this.swigCPtr, value);
      get => AkSoundEnginePINVOKE.CSharp_AkMIDIEvent_byCcValue_get(this.swigCPtr);
    }

    public byte byValueLsb
    {
      set => AkSoundEnginePINVOKE.CSharp_AkMIDIEvent_byValueLsb_set(this.swigCPtr, value);
      get => AkSoundEnginePINVOKE.CSharp_AkMIDIEvent_byValueLsb_get(this.swigCPtr);
    }

    public byte byValueMsb
    {
      set => AkSoundEnginePINVOKE.CSharp_AkMIDIEvent_byValueMsb_set(this.swigCPtr, value);
      get => AkSoundEnginePINVOKE.CSharp_AkMIDIEvent_byValueMsb_get(this.swigCPtr);
    }

    public byte byAftertouchNote
    {
      set => AkSoundEnginePINVOKE.CSharp_AkMIDIEvent_byAftertouchNote_set(this.swigCPtr, value);
      get => AkSoundEnginePINVOKE.CSharp_AkMIDIEvent_byAftertouchNote_get(this.swigCPtr);
    }

    public byte byNoteAftertouchValue
    {
      set => AkSoundEnginePINVOKE.CSharp_AkMIDIEvent_byNoteAftertouchValue_set(this.swigCPtr, value);
      get => AkSoundEnginePINVOKE.CSharp_AkMIDIEvent_byNoteAftertouchValue_get(this.swigCPtr);
    }

    public byte byChanAftertouchValue
    {
      set => AkSoundEnginePINVOKE.CSharp_AkMIDIEvent_byChanAftertouchValue_set(this.swigCPtr, value);
      get => AkSoundEnginePINVOKE.CSharp_AkMIDIEvent_byChanAftertouchValue_get(this.swigCPtr);
    }

    public byte byProgramNum
    {
      set => AkSoundEnginePINVOKE.CSharp_AkMIDIEvent_byProgramNum_set(this.swigCPtr, value);
      get => AkSoundEnginePINVOKE.CSharp_AkMIDIEvent_byProgramNum_get(this.swigCPtr);
    }

    public class tGen : IDisposable
    {
      private IntPtr swigCPtr;
      protected bool swigCMemOwn;

      internal tGen(IntPtr cPtr, bool cMemoryOwn)
      {
        this.swigCMemOwn = cMemoryOwn;
        this.swigCPtr = cPtr;
      }

      public tGen()
        : this(AkSoundEnginePINVOKE.CSharp_new_AkMIDIEvent_tGen(), true)
      {
      }

      internal static IntPtr getCPtr(AkMIDIEvent.tGen obj)
      {
        return obj == null ? IntPtr.Zero : obj.swigCPtr;
      }

      internal virtual void setCPtr(IntPtr cPtr)
      {
        this.Dispose();
        this.swigCPtr = cPtr;
      }

      ~tGen() => this.Dispose();

      public virtual void Dispose()
      {
        lock ((object) this)
        {
          if (this.swigCPtr != IntPtr.Zero)
          {
            if (this.swigCMemOwn)
            {
              this.swigCMemOwn = false;
              AkSoundEnginePINVOKE.CSharp_delete_AkMIDIEvent_tGen(this.swigCPtr);
            }
            this.swigCPtr = IntPtr.Zero;
          }
          GC.SuppressFinalize((object) this);
        }
      }

      public byte byParam1
      {
        set => AkSoundEnginePINVOKE.CSharp_AkMIDIEvent_tGen_byParam1_set(this.swigCPtr, value);
        get => AkSoundEnginePINVOKE.CSharp_AkMIDIEvent_tGen_byParam1_get(this.swigCPtr);
      }

      public byte byParam2
      {
        set => AkSoundEnginePINVOKE.CSharp_AkMIDIEvent_tGen_byParam2_set(this.swigCPtr, value);
        get => AkSoundEnginePINVOKE.CSharp_AkMIDIEvent_tGen_byParam2_get(this.swigCPtr);
      }
    }

    public class tNoteOnOff : IDisposable
    {
      private IntPtr swigCPtr;
      protected bool swigCMemOwn;

      internal tNoteOnOff(IntPtr cPtr, bool cMemoryOwn)
      {
        this.swigCMemOwn = cMemoryOwn;
        this.swigCPtr = cPtr;
      }

      public tNoteOnOff()
        : this(AkSoundEnginePINVOKE.CSharp_new_AkMIDIEvent_tNoteOnOff(), true)
      {
      }

      internal static IntPtr getCPtr(AkMIDIEvent.tNoteOnOff obj)
      {
        return obj == null ? IntPtr.Zero : obj.swigCPtr;
      }

      internal virtual void setCPtr(IntPtr cPtr)
      {
        this.Dispose();
        this.swigCPtr = cPtr;
      }

      ~tNoteOnOff() => this.Dispose();

      public virtual void Dispose()
      {
        lock ((object) this)
        {
          if (this.swigCPtr != IntPtr.Zero)
          {
            if (this.swigCMemOwn)
            {
              this.swigCMemOwn = false;
              AkSoundEnginePINVOKE.CSharp_delete_AkMIDIEvent_tNoteOnOff(this.swigCPtr);
            }
            this.swigCPtr = IntPtr.Zero;
          }
          GC.SuppressFinalize((object) this);
        }
      }

      public byte byNote
      {
        set => AkSoundEnginePINVOKE.CSharp_AkMIDIEvent_tNoteOnOff_byNote_set(this.swigCPtr, value);
        get => AkSoundEnginePINVOKE.CSharp_AkMIDIEvent_tNoteOnOff_byNote_get(this.swigCPtr);
      }

      public byte byVelocity
      {
        set
        {
          AkSoundEnginePINVOKE.CSharp_AkMIDIEvent_tNoteOnOff_byVelocity_set(this.swigCPtr, value);
        }
        get => AkSoundEnginePINVOKE.CSharp_AkMIDIEvent_tNoteOnOff_byVelocity_get(this.swigCPtr);
      }
    }

    public class tCc : IDisposable
    {
      private IntPtr swigCPtr;
      protected bool swigCMemOwn;

      internal tCc(IntPtr cPtr, bool cMemoryOwn)
      {
        this.swigCMemOwn = cMemoryOwn;
        this.swigCPtr = cPtr;
      }

      public tCc()
        : this(AkSoundEnginePINVOKE.CSharp_new_AkMIDIEvent_tCc(), true)
      {
      }

      internal static IntPtr getCPtr(AkMIDIEvent.tCc obj) => obj == null ? IntPtr.Zero : obj.swigCPtr;

      internal virtual void setCPtr(IntPtr cPtr)
      {
        this.Dispose();
        this.swigCPtr = cPtr;
      }

      ~tCc() => this.Dispose();

      public virtual void Dispose()
      {
        lock ((object) this)
        {
          if (this.swigCPtr != IntPtr.Zero)
          {
            if (this.swigCMemOwn)
            {
              this.swigCMemOwn = false;
              AkSoundEnginePINVOKE.CSharp_delete_AkMIDIEvent_tCc(this.swigCPtr);
            }
            this.swigCPtr = IntPtr.Zero;
          }
          GC.SuppressFinalize((object) this);
        }
      }

      public byte byCc
      {
        set => AkSoundEnginePINVOKE.CSharp_AkMIDIEvent_tCc_byCc_set(this.swigCPtr, value);
        get => AkSoundEnginePINVOKE.CSharp_AkMIDIEvent_tCc_byCc_get(this.swigCPtr);
      }

      public byte byValue
      {
        set => AkSoundEnginePINVOKE.CSharp_AkMIDIEvent_tCc_byValue_set(this.swigCPtr, value);
        get => AkSoundEnginePINVOKE.CSharp_AkMIDIEvent_tCc_byValue_get(this.swigCPtr);
      }
    }

    public class tPitchBend : IDisposable
    {
      private IntPtr swigCPtr;
      protected bool swigCMemOwn;

      internal tPitchBend(IntPtr cPtr, bool cMemoryOwn)
      {
        this.swigCMemOwn = cMemoryOwn;
        this.swigCPtr = cPtr;
      }

      public tPitchBend()
        : this(AkSoundEnginePINVOKE.CSharp_new_AkMIDIEvent_tPitchBend(), true)
      {
      }

      internal static IntPtr getCPtr(AkMIDIEvent.tPitchBend obj)
      {
        return obj == null ? IntPtr.Zero : obj.swigCPtr;
      }

      internal virtual void setCPtr(IntPtr cPtr)
      {
        this.Dispose();
        this.swigCPtr = cPtr;
      }

      ~tPitchBend() => this.Dispose();

      public virtual void Dispose()
      {
        lock ((object) this)
        {
          if (this.swigCPtr != IntPtr.Zero)
          {
            if (this.swigCMemOwn)
            {
              this.swigCMemOwn = false;
              AkSoundEnginePINVOKE.CSharp_delete_AkMIDIEvent_tPitchBend(this.swigCPtr);
            }
            this.swigCPtr = IntPtr.Zero;
          }
          GC.SuppressFinalize((object) this);
        }
      }

      public byte byValueLsb
      {
        set
        {
          AkSoundEnginePINVOKE.CSharp_AkMIDIEvent_tPitchBend_byValueLsb_set(this.swigCPtr, value);
        }
        get => AkSoundEnginePINVOKE.CSharp_AkMIDIEvent_tPitchBend_byValueLsb_get(this.swigCPtr);
      }

      public byte byValueMsb
      {
        set
        {
          AkSoundEnginePINVOKE.CSharp_AkMIDIEvent_tPitchBend_byValueMsb_set(this.swigCPtr, value);
        }
        get => AkSoundEnginePINVOKE.CSharp_AkMIDIEvent_tPitchBend_byValueMsb_get(this.swigCPtr);
      }
    }

    public class tNoteAftertouch : IDisposable
    {
      private IntPtr swigCPtr;
      protected bool swigCMemOwn;

      internal tNoteAftertouch(IntPtr cPtr, bool cMemoryOwn)
      {
        this.swigCMemOwn = cMemoryOwn;
        this.swigCPtr = cPtr;
      }

      public tNoteAftertouch()
        : this(AkSoundEnginePINVOKE.CSharp_new_AkMIDIEvent_tNoteAftertouch(), true)
      {
      }

      internal static IntPtr getCPtr(AkMIDIEvent.tNoteAftertouch obj)
      {
        return obj == null ? IntPtr.Zero : obj.swigCPtr;
      }

      internal virtual void setCPtr(IntPtr cPtr)
      {
        this.Dispose();
        this.swigCPtr = cPtr;
      }

      ~tNoteAftertouch() => this.Dispose();

      public virtual void Dispose()
      {
        lock ((object) this)
        {
          if (this.swigCPtr != IntPtr.Zero)
          {
            if (this.swigCMemOwn)
            {
              this.swigCMemOwn = false;
              AkSoundEnginePINVOKE.CSharp_delete_AkMIDIEvent_tNoteAftertouch(this.swigCPtr);
            }
            this.swigCPtr = IntPtr.Zero;
          }
          GC.SuppressFinalize((object) this);
        }
      }

      public byte byNote
      {
        set
        {
          AkSoundEnginePINVOKE.CSharp_AkMIDIEvent_tNoteAftertouch_byNote_set(this.swigCPtr, value);
        }
        get => AkSoundEnginePINVOKE.CSharp_AkMIDIEvent_tNoteAftertouch_byNote_get(this.swigCPtr);
      }

      public byte byValue
      {
        set
        {
          AkSoundEnginePINVOKE.CSharp_AkMIDIEvent_tNoteAftertouch_byValue_set(this.swigCPtr, value);
        }
        get => AkSoundEnginePINVOKE.CSharp_AkMIDIEvent_tNoteAftertouch_byValue_get(this.swigCPtr);
      }
    }

    public class tChanAftertouch : IDisposable
    {
      private IntPtr swigCPtr;
      protected bool swigCMemOwn;

      internal tChanAftertouch(IntPtr cPtr, bool cMemoryOwn)
      {
        this.swigCMemOwn = cMemoryOwn;
        this.swigCPtr = cPtr;
      }

      public tChanAftertouch()
        : this(AkSoundEnginePINVOKE.CSharp_new_AkMIDIEvent_tChanAftertouch(), true)
      {
      }

      internal static IntPtr getCPtr(AkMIDIEvent.tChanAftertouch obj)
      {
        return obj == null ? IntPtr.Zero : obj.swigCPtr;
      }

      internal virtual void setCPtr(IntPtr cPtr)
      {
        this.Dispose();
        this.swigCPtr = cPtr;
      }

      ~tChanAftertouch() => this.Dispose();

      public virtual void Dispose()
      {
        lock ((object) this)
        {
          if (this.swigCPtr != IntPtr.Zero)
          {
            if (this.swigCMemOwn)
            {
              this.swigCMemOwn = false;
              AkSoundEnginePINVOKE.CSharp_delete_AkMIDIEvent_tChanAftertouch(this.swigCPtr);
            }
            this.swigCPtr = IntPtr.Zero;
          }
          GC.SuppressFinalize((object) this);
        }
      }

      public byte byValue
      {
        set
        {
          AkSoundEnginePINVOKE.CSharp_AkMIDIEvent_tChanAftertouch_byValue_set(this.swigCPtr, value);
        }
        get => AkSoundEnginePINVOKE.CSharp_AkMIDIEvent_tChanAftertouch_byValue_get(this.swigCPtr);
      }
    }

    public class tProgramChange : IDisposable
    {
      private IntPtr swigCPtr;
      protected bool swigCMemOwn;

      internal tProgramChange(IntPtr cPtr, bool cMemoryOwn)
      {
        this.swigCMemOwn = cMemoryOwn;
        this.swigCPtr = cPtr;
      }

      public tProgramChange()
        : this(AkSoundEnginePINVOKE.CSharp_new_AkMIDIEvent_tProgramChange(), true)
      {
      }

      internal static IntPtr getCPtr(AkMIDIEvent.tProgramChange obj)
      {
        return obj == null ? IntPtr.Zero : obj.swigCPtr;
      }

      internal virtual void setCPtr(IntPtr cPtr)
      {
        this.Dispose();
        this.swigCPtr = cPtr;
      }

      ~tProgramChange() => this.Dispose();

      public virtual void Dispose()
      {
        lock ((object) this)
        {
          if (this.swigCPtr != IntPtr.Zero)
          {
            if (this.swigCMemOwn)
            {
              this.swigCMemOwn = false;
              AkSoundEnginePINVOKE.CSharp_delete_AkMIDIEvent_tProgramChange(this.swigCPtr);
            }
            this.swigCPtr = IntPtr.Zero;
          }
          GC.SuppressFinalize((object) this);
        }
      }

      public byte byProgramNum
      {
        set
        {
          AkSoundEnginePINVOKE.CSharp_AkMIDIEvent_tProgramChange_byProgramNum_set(this.swigCPtr, value);
        }
        get => AkSoundEnginePINVOKE.CSharp_AkMIDIEvent_tProgramChange_byProgramNum_get(this.swigCPtr);
      }
    }
  }

