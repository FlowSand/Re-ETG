// Decompiled with JetBrains decompiler
// Type: PunchoutDroppedItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable

public class PunchoutDroppedItem : MonoBehaviour
  {
    private static int[] s_randomIndices;
    private static int s_indicesIndex;
    public List<Vector2> targetOffsets;
    public float airTime;
    public float airHeight;
    public float airTime2;
    public float airHeight2;
    public float motionStartPercent = 0.75f;
    public float motionMultiplier = 4f;
    public float gravityMultiplier = 6f;
    private Vector2 m_offset;
    private Vector2 m_startPosition;
    private Vector2 m_targetPosition;

    public void Init(bool isLeft)
    {
      if (PunchoutDroppedItem.s_randomIndices == null || PunchoutDroppedItem.s_randomIndices.Length != this.targetOffsets.Count)
      {
        PunchoutDroppedItem.s_randomIndices = new int[this.targetOffsets.Count];
        for (int index = 0; index < PunchoutDroppedItem.s_randomIndices.Length; ++index)
          PunchoutDroppedItem.s_randomIndices[index] = index;
        BraveUtility.RandomizeArray<int>(PunchoutDroppedItem.s_randomIndices);
        PunchoutDroppedItem.s_indicesIndex = 0;
      }
      this.m_offset = this.transform.position.XY() - this.GetComponent<tk2dBaseSprite>().WorldBottomCenter;
      this.m_startPosition = (Vector2) this.transform.position;
      this.m_targetPosition = this.m_startPosition + this.targetOffsets[PunchoutDroppedItem.s_randomIndices[PunchoutDroppedItem.s_indicesIndex]].Scale(!isLeft ? 1f : -1f, 1f);
      PunchoutDroppedItem.s_indicesIndex = (PunchoutDroppedItem.s_indicesIndex + 1) % PunchoutDroppedItem.s_randomIndices.Length;
      this.StartCoroutine(this.MoveCR());
      int num = (int) AkSoundEngine.PostEvent("Play_OBJ_coin_spill_01", this.gameObject);
    }

    [DebuggerHidden]
    private IEnumerator MoveCR()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new PunchoutDroppedItem__MoveCRc__Iterator0()
      {
        _this = this
      };
    }
  }

