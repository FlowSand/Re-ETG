// Decompiled with JetBrains decompiler
// Type: InControl.IInputControl
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace InControl;

public interface IInputControl
{
  bool HasChanged { get; }

  bool IsPressed { get; }

  bool WasPressed { get; }

  bool WasReleased { get; }

  void ClearInputState();
}
