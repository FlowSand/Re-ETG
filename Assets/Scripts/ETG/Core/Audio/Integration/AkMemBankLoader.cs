// Decompiled with JetBrains decompiler
// Type: AkMemBankLoader
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

#nullable disable

namespace ETG.Core.Audio.Integration
{
    public class AkMemBankLoader : MonoBehaviour
    {
      private const int WaitMs = 50;
      private const long AK_BANK_PLATFORM_DATA_ALIGNMENT = 16 /*0x10*/;
      private const long AK_BANK_PLATFORM_DATA_ALIGNMENT_MASK = 15;
      public string bankName = string.Empty;
      public bool isLocalizedBank;
      private string m_bankPath;
      [HideInInspector]
      public uint ms_bankID;
      private IntPtr ms_pInMemoryBankPtr = IntPtr.Zero;
      private GCHandle ms_pinnedArray;
      private WWW ms_www;

      private void Start()
      {
        if (this.isLocalizedBank)
          this.LoadLocalizedBank(this.bankName);
        else
          this.LoadNonLocalizedBank(this.bankName);
      }

      public void LoadNonLocalizedBank(string in_bankFilename)
      {
        this.DoLoadBank("file://" + Path.Combine(AkBasePathGetter.GetPlatformBasePath(), in_bankFilename));
      }

      public void LoadLocalizedBank(string in_bankFilename)
      {
        this.DoLoadBank("file://" + Path.Combine(Path.Combine(AkBasePathGetter.GetPlatformBasePath(), AkInitializer.GetCurrentLanguage()), in_bankFilename));
      }

      [DebuggerHidden]
      private IEnumerator LoadFile()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new AkMemBankLoader.<LoadFile>c__Iterator0()
        {
          $this = this
        };
      }

      private void DoLoadBank(string in_bankPath)
      {
        this.m_bankPath = in_bankPath;
        this.StartCoroutine(this.LoadFile());
      }

      private void OnDestroy()
      {
        if (!(this.ms_pInMemoryBankPtr != IntPtr.Zero) || AkSoundEngine.UnloadBank(this.ms_bankID, this.ms_pInMemoryBankPtr) != AKRESULT.AK_Success)
          return;
        this.ms_pinnedArray.Free();
      }
    }

}
