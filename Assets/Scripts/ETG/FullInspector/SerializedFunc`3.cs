// Decompiled with JetBrains decompiler
// Type: FullInspector.SerializedFunc`3
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using FullInspector.Modules.SerializableDelegates;

#nullable disable
namespace FullInspector
{
  public class SerializedFunc<TParam1, TParam2, TResult> : BaseSerializedFunc
  {
    public TResult Invoke(TParam1 param1, TParam2 param2)
    {
      return (TResult) this.DoInvoke((object) param1, (object) param2);
    }
  }
}
