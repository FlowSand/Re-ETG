// Decompiled with JetBrains decompiler
// Type: FullInspector.tkControlEditor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace FullInspector;

public class tkControlEditor
{
  public bool Debug;
  public tkIControl Control;
  public object Context;

  public tkControlEditor(tkIControl control)
    : this(false, control)
  {
  }

  public tkControlEditor(bool debug, tkIControl control)
  {
    this.Debug = debug;
    this.Control = control;
    int nextId = 0;
    control.InitializeId(ref nextId);
  }
}
