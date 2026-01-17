// Decompiled with JetBrains decompiler
// Type: FullInspector.fiValueNullSerializer`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using FullInspector.Internal;

#nullable disable
namespace FullInspector
{
  public abstract class fiValueNullSerializer<T> : fiValueProxyEditor, fiIValueProxyAPI
  {
    public T Value;

    object fiIValueProxyAPI.Value
    {
      get => (object) this.Value;
      set => this.Value = (T) value;
    }

    void fiIValueProxyAPI.SaveState()
    {
    }

    void fiIValueProxyAPI.LoadState()
    {
    }
  }
}
