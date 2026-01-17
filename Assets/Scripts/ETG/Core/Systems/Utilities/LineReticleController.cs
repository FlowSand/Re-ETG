// Decompiled with JetBrains decompiler
// Type: LineReticleController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    public class LineReticleController : MonoBehaviour
    {
      public float MinLength;
      public float Speed;
      private LineReticleController.State m_state = LineReticleController.State.Static;
      private tk2dSlicedSprite m_slicedSprite;
      private float m_currentLength;
      private float m_maxLength;

      public void Awake() => this.m_slicedSprite = this.GetComponent<tk2dSlicedSprite>();

      public void Init(Vector3 pos, Quaternion rotation, float maxLength)
      {
        this.m_slicedSprite.transform.position = pos;
        this.m_slicedSprite.transform.localRotation = rotation;
        this.m_currentLength = this.MinLength;
        this.m_maxLength = maxLength;
        this.m_state = LineReticleController.State.Growing;
        this.m_slicedSprite.dimensions = new Vector2(this.m_currentLength * 16f, 5f);
        this.m_slicedSprite.UpdateZDepth();
      }

      public void Update()
      {
        if (this.m_state == LineReticleController.State.Growing)
        {
          this.m_currentLength = Mathf.Min(this.m_currentLength + this.Speed * BraveTime.DeltaTime, this.m_maxLength);
          this.m_slicedSprite.dimensions = new Vector2(this.m_currentLength * 16f, 5f);
          this.m_slicedSprite.UpdateZDepth();
          if ((double) this.m_currentLength < (double) this.m_maxLength)
            return;
          this.m_state = LineReticleController.State.Static;
        }
        else
        {
          if (this.m_state != LineReticleController.State.Shrinking)
            return;
          float currentLength = this.m_currentLength;
          this.m_currentLength = Mathf.Max(this.m_currentLength - this.Speed * BraveTime.DeltaTime, this.MinLength);
          this.transform.position += this.transform.rotation * new Vector3(currentLength - this.m_currentLength, 0.0f, 0.0f);
          this.m_slicedSprite.dimensions = new Vector2(this.m_currentLength * 16f, 5f);
          this.m_slicedSprite.UpdateZDepth();
          if ((double) this.m_currentLength > (double) this.MinLength)
            return;
          this.m_state = LineReticleController.State.Static;
          SpawnManager.Despawn(this.gameObject);
        }
      }

      public void Cleanup() => this.m_state = LineReticleController.State.Shrinking;

      public enum State
      {
        Growing,
        Static,
        Shrinking,
      }
    }

}
