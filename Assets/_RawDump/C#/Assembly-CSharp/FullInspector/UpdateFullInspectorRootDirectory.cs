// Decompiled with JetBrains decompiler
// Type: FullInspector.UpdateFullInspectorRootDirectory
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace FullInspector;

public class UpdateFullInspectorRootDirectory : fiSettingsProcessor
{
  public void Process() => fiSettings.RootDirectory = "Assets/Libraries/FullInspector2/";
}
