// Decompiled with JetBrains decompiler
// Type: AkBankManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;

#nullable disable

namespace ETG.Core.Audio.Integration
{
    public static class AkBankManager
    {
      private static readonly Dictionary<string, AkBankManager.BankHandle> m_BankHandles = new Dictionary<string, AkBankManager.BankHandle>();
      private static readonly List<AkBankManager.BankHandle> BanksToUnload = new List<AkBankManager.BankHandle>();
      private static readonly Mutex m_Mutex = new Mutex();

      internal static void DoUnloadBanks()
      {
        int count = AkBankManager.BanksToUnload.Count;
        for (int index = 0; index < count; ++index)
          AkBankManager.BanksToUnload[index].UnloadBank();
        AkBankManager.BanksToUnload.Clear();
      }

      internal static void Reset()
      {
        AkBankManager.m_BankHandles.Clear();
        AkBankManager.BanksToUnload.Clear();
      }

      public static void LoadBank(string name, bool decodeBank, bool saveDecodedBank)
      {
        AkBankManager.m_Mutex.WaitOne();
        AkBankManager.BankHandle bankHandle1 = (AkBankManager.BankHandle) null;
        if (!AkBankManager.m_BankHandles.TryGetValue(name, out bankHandle1))
        {
          AkBankManager.BankHandle bankHandle2 = !decodeBank ? new AkBankManager.BankHandle(name) : (AkBankManager.BankHandle) new AkBankManager.DecodableBankHandle(name, saveDecodedBank);
          AkBankManager.m_BankHandles.Add(name, bankHandle2);
          AkBankManager.m_Mutex.ReleaseMutex();
          bankHandle2.LoadBank();
        }
        else
        {
          bankHandle1.IncRef();
          AkBankManager.m_Mutex.ReleaseMutex();
        }
      }

      public static void LoadBankAsync(string name, AkCallbackManager.BankCallback callback = null)
      {
        AkBankManager.m_Mutex.WaitOne();
        AkBankManager.BankHandle bankHandle = (AkBankManager.BankHandle) null;
        if (!AkBankManager.m_BankHandles.TryGetValue(name, out bankHandle))
        {
          AkBankManager.AsyncBankHandle asyncBankHandle = new AkBankManager.AsyncBankHandle(name, callback);
          AkBankManager.m_BankHandles.Add(name, (AkBankManager.BankHandle) asyncBankHandle);
          AkBankManager.m_Mutex.ReleaseMutex();
          asyncBankHandle.LoadBank();
        }
        else
        {
          bankHandle.IncRef();
          AkBankManager.m_Mutex.ReleaseMutex();
        }
      }

      public static void UnloadBank(string name)
      {
        AkBankManager.m_Mutex.WaitOne();
        AkBankManager.BankHandle bankHandle = (AkBankManager.BankHandle) null;
        if (AkBankManager.m_BankHandles.TryGetValue(name, out bankHandle))
        {
          bankHandle.DecRef();
          if (bankHandle.RefCount == 0)
            AkBankManager.m_BankHandles.Remove(name);
        }
        AkBankManager.m_Mutex.ReleaseMutex();
      }

      private class BankHandle
      {
        protected readonly string bankName;
        protected uint m_BankID;

        public BankHandle(string name) => this.bankName = name;

        public int RefCount { get; private set; }

        public virtual AKRESULT DoLoadBank()
        {
          return AkSoundEngine.LoadBank(this.bankName, -1, out this.m_BankID);
        }

        public void LoadBank()
        {
          if (this.RefCount == 0)
          {
            if (AkBankManager.BanksToUnload.Contains(this))
              AkBankManager.BanksToUnload.Remove(this);
            else
              this.LogLoadResult(this.DoLoadBank());
          }
          this.IncRef();
        }

        public virtual void UnloadBank()
        {
          int num = (int) AkSoundEngine.UnloadBank(this.m_BankID, IntPtr.Zero, (AkCallbackManager.BankCallback) null, (object) null);
        }

        public void IncRef() => ++this.RefCount;

        public void DecRef()
        {
          --this.RefCount;
          if (this.RefCount != 0)
            return;
          AkBankManager.BanksToUnload.Add(this);
        }

        protected void LogLoadResult(AKRESULT result)
        {
          if (result == AKRESULT.AK_Success || !AkSoundEngine.IsInitialized())
            return;
          Debug.LogWarning((object) $"WwiseUnity: Bank {this.bankName} failed to load ({(object) result})");
        }
      }

      private class AsyncBankHandle : AkBankManager.BankHandle
      {
        private readonly AkCallbackManager.BankCallback bankCallback;

        public AsyncBankHandle(string name, AkCallbackManager.BankCallback callback)
          : base(name)
        {
          this.bankCallback = callback;
        }

        private static void GlobalBankCallback(
          uint in_bankID,
          IntPtr in_pInMemoryBankPtr,
          AKRESULT in_eLoadResult,
          uint in_memPoolId,
          object in_Cookie)
        {
          AkBankManager.m_Mutex.WaitOne();
          AkBankManager.AsyncBankHandle asyncBankHandle = (AkBankManager.AsyncBankHandle) in_Cookie;
          AkCallbackManager.BankCallback bankCallback = asyncBankHandle.bankCallback;
          if (in_eLoadResult != AKRESULT.AK_Success)
          {
            asyncBankHandle.LogLoadResult(in_eLoadResult);
            AkBankManager.m_BankHandles.Remove(asyncBankHandle.bankName);
          }
          AkBankManager.m_Mutex.ReleaseMutex();
          if (bankCallback == null)
            return;
          bankCallback(in_bankID, in_pInMemoryBankPtr, in_eLoadResult, in_memPoolId, (object) null);
        }

        public override AKRESULT DoLoadBank()
        {
          string bankName = this.bankName;
          // ISSUE: reference to a compiler-generated field
          if (AkBankManager.AsyncBankHandle._f__mg_cache0 == null)
          {
            // ISSUE: reference to a compiler-generated field
            AkBankManager.AsyncBankHandle._f__mg_cache0 = new AkCallbackManager.BankCallback(AkBankManager.AsyncBankHandle.GlobalBankCallback);
          }
          // ISSUE: reference to a compiler-generated field
          AkCallbackManager.BankCallback fMgCache0 = AkBankManager.AsyncBankHandle._f__mg_cache0;
          ref uint local = ref this.m_BankID;
          return AkSoundEngine.LoadBank(bankName, fMgCache0, (object) this, -1, out local);
        }
      }

      private class DecodableBankHandle : AkBankManager.BankHandle
      {
        private readonly bool decodeBank = true;
        private readonly string decodedBankPath;
        private readonly bool saveDecodedBank;

        public DecodableBankHandle(string name, bool save)
          : base(name)
        {
          this.saveDecodedBank = save;
          string path2 = this.bankName + ".bnk";
          string currentLanguage = AkInitializer.GetCurrentLanguage();
          this.decodedBankPath = Path.Combine(AkSoundEngineController.GetDecodedBankFullPath(), currentLanguage);
          string path = Path.Combine(this.decodedBankPath, path2);
          bool flag = File.Exists(path);
          if (!flag)
          {
            this.decodedBankPath = AkSoundEngineController.GetDecodedBankFullPath();
            path = Path.Combine(this.decodedBankPath, path2);
            flag = File.Exists(path);
          }
          if (!flag)
            return;
          try
          {
            this.decodeBank = File.GetLastWriteTime(path) <= File.GetLastWriteTime(Path.Combine(AkBasePathGetter.GetSoundbankBasePath(), path2));
          }
          catch
          {
          }
        }

        public override AKRESULT DoLoadBank()
        {
          if (this.decodeBank)
            return AkSoundEngine.LoadAndDecodeBank(this.bankName, this.saveDecodedBank, out this.m_BankID);
          AKRESULT akresult = AKRESULT.AK_Success;
          if (!string.IsNullOrEmpty(this.decodedBankPath))
            akresult = AkSoundEngine.SetBasePath(this.decodedBankPath);
          if (akresult == AKRESULT.AK_Success)
          {
            akresult = AkSoundEngine.LoadBank(this.bankName, -1, out this.m_BankID);
            if (!string.IsNullOrEmpty(this.decodedBankPath))
            {
              int num = (int) AkSoundEngine.SetBasePath(AkBasePathGetter.GetSoundbankBasePath());
            }
          }
          return akresult;
        }

        public override void UnloadBank()
        {
          if (this.decodeBank && !this.saveDecodedBank)
          {
            int num = (int) AkSoundEngine.PrepareBank(AkPreparationType.Preparation_Unload, this.m_BankID);
          }
          else
            base.UnloadBank();
        }
      }
    }

}
