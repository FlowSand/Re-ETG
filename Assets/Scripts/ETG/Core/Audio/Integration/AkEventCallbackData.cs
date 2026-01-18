// Decompiled with JetBrains decompiler
// Type: AkEventCallbackData
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable

public class AkEventCallbackData : ScriptableObject
  {
    public List<int> callbackFlags = new List<int>();
    public List<string> callbackFunc = new List<string>();
    public List<GameObject> callbackGameObj = new List<GameObject>();
    public int uFlags;
  }

