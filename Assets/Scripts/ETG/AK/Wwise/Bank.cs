// Decompiled with JetBrains decompiler
// Type: AK.Wwise.Bank
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable
namespace AK.Wwise;

[Serializable]
public class Bank : BaseType
{
  public string name;

  public void Load(bool decodeBank = false, bool saveDecodedBank = false)
  {
    if (!this.IsValid())
      return;
    AkBankManager.LoadBank(this.name, decodeBank, saveDecodedBank);
  }

  public void LoadAsync(AkCallbackManager.BankCallback callback = null)
  {
    if (!this.IsValid())
      return;
    AkBankManager.LoadBankAsync(this.name, callback);
  }

  public void Unload()
  {
    if (!this.IsValid())
      return;
    AkBankManager.UnloadBank(this.name);
  }

  public override bool IsValid() => this.name.Length != 0 || base.IsValid();
}
