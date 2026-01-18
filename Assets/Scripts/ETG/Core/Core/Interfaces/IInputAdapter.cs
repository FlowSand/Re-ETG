// Decompiled with JetBrains decompiler
// Type: IInputAdapter
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

public interface IInputAdapter
  {
    bool GetKeyDown(KeyCode key);

    bool GetKeyUp(KeyCode key);

    float GetAxis(string axisName);

    Vector2 GetMousePosition();

    bool GetMouseButton(int button);

    bool GetMouseButtonDown(int button);

    bool GetMouseButtonUp(int button);
  }

