// Decompiled with JetBrains decompiler
// Type: BraveGoodInspectorSettings
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using FullInspector;

#nullable disable

public class BraveGoodInspectorSettings : fiSettingsProcessor
  {
    public void Process()
    {
      fiSettings.SerializeAutoProperties = false;
      fiSettings.RootDirectory = "Assets/Libraries/FullInspector2/";
    }
  }

