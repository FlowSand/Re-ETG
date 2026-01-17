// Decompiled with JetBrains decompiler
// Type: FullInspector.Internal.fiStackEnabled
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace FullInspector.Internal
{
  public class fiStackEnabled
  {
    private int _count;

    public void Push() => ++this._count;

    public void Pop()
    {
      --this._count;
      if (this._count >= 0)
        return;
      this._count = 0;
    }

    public bool Enabled => this._count > 0;
  }
}
