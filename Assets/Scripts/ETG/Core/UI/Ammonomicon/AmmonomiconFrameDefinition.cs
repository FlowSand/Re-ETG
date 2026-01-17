// Decompiled with JetBrains decompiler
// Type: AmmonomiconFrameDefinition
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;
using UnityEngine.Serialization;

#nullable disable

namespace ETG.Core.UI.Ammonomicon
{
    [Serializable]
    public class AmmonomiconFrameDefinition
    {
      [FormerlySerializedAs("AmmonomiconTopLayerImage")]
      public Texture2D AmmonomiconTopLayerTexture;
      [FormerlySerializedAs("AmmonomiconBottomLayerImage")]
      public Texture2D AmmonomiconBottomLayerTexture;
      public Texture2D AmmonomiconToppestLayerTexture;
      public float frameTime;
      public bool CurrentLeftVisible = true;
      public Vector3 CurrentLeftOffset;
      public Matrix4x4 CurrentLeftMatrix;
      public bool CurrentRightVisible = true;
      public Vector3 CurrentRightOffset;
      public Matrix4x4 CurrentRightMatrix;
      public bool ImpendingLeftVisible = true;
      public Vector3 ImpendingLeftOffset;
      public Matrix4x4 ImpendingLeftMatrix;
      public bool ImpendingRightVisible = true;
      public Vector3 ImpendingRightOffset;
      public Matrix4x4 ImpendingRightMatrix;
    }

}
