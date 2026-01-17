// Decompiled with JetBrains decompiler
// Type: FullInspector.Internal.fiStackValue`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections.Generic;

#nullable disable
namespace FullInspector.Internal;

public class fiStackValue<T>
{
  private readonly Stack<T> _stack = new Stack<T>();

  public void Push(T value) => this._stack.Push(value);

  public T Pop() => this._stack.Count > 0 ? this._stack.Pop() : default (T);

  public T Value
  {
    get => this._stack.Peek();
    set
    {
      this.Pop();
      this.Push(value);
    }
  }
}
