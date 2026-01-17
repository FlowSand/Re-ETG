// Decompiled with JetBrains decompiler
// Type: FullInspector.SharedInstance`2
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace FullInspector;

public class SharedInstance<TInstance, TSerializer> : BaseScriptableObject<TSerializer> where TSerializer : BaseSerializer
{
  public TInstance Instance;
}
