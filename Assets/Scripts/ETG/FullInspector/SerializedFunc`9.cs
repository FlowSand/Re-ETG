// Decompiled with JetBrains decompiler
// Type: FullInspector.SerializedFunc`9
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using FullInspector.Modules.SerializableDelegates;

#nullable disable
namespace FullInspector
{
  public class SerializedFunc<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TResult> : 
    BaseSerializedFunc
  {
    public TResult Invoke(
      TParam1 param1,
      TParam2 param2,
      TParam3 param3,
      TParam4 param4,
      TParam5 param5,
      TParam6 param6,
      TParam7 param7,
      TParam8 param8)
    {
      return (TResult) this.DoInvoke((object) param1, (object) param2, (object) param3, (object) param4, (object) param5, (object) param6, (object) param7, (object) param8);
    }
  }
}
