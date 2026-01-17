// Decompiled with JetBrains decompiler
// Type: FullInspector.BackupService.fiSerializedMember
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable
namespace FullInspector.BackupService
{
  [Serializable]
  public class fiSerializedMember
  {
    public string Name;
    public string Value;
    public fiEnableRestore ShouldRestore = new fiEnableRestore()
    {
      Enabled = true
    };
  }
}
