// Decompiled with JetBrains decompiler
// Type: FullInspector.SerializedFunc`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using FullInspector.Modules.SerializableDelegates;

#nullable disable
namespace FullInspector;

public class SerializedFunc<TResult> : BaseSerializedFunc
{
  public TResult Invoke() => (TResult) this.DoInvoke((object[]) null);
}
