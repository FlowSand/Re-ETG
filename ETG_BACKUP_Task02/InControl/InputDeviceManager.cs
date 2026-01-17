// Decompiled with JetBrains decompiler
// Type: InControl.InputDeviceManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections.Generic;

#nullable disable
namespace InControl;

public abstract class InputDeviceManager
{
  protected List<InputDevice> devices = new List<InputDevice>();

  public abstract void Update(ulong updateTick, float deltaTime);

  public virtual void Destroy()
  {
  }
}
