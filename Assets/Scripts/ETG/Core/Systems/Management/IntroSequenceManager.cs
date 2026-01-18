// Decompiled with JetBrains decompiler
// Type: IntroSequenceManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable

public class IntroSequenceManager : MonoBehaviour
  {
    public List<IntroSequenceElement> elements;
    public dfControl postVideoPanel;
    public string nextSceneName;
    private AsyncOperation async;

    [DebuggerHidden]
    private IEnumerator Start()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new IntroSequenceManager__Startc__Iterator0()
      {
        _this = this
      };
    }

    private void Update() => BraveCameraUtility.MaintainCameraAspect(Camera.main);

    [DebuggerHidden]
    private IEnumerator HandleElement(int index)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new IntroSequenceManager__HandleElementc__Iterator1()
      {
        index = index,
        _this = this
      };
    }
  }

