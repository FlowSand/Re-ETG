// Decompiled with JetBrains decompiler
// Type: CreepyEyeController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    [ExecuteInEditMode]
    public class CreepyEyeController : MonoBehaviour
    {
      public const float c_TimeBeforeWarp = 15f;
      public float MaxPupilRadius = 2.5f;
      public CreepyEyeLayer[] layers;
      public Transform poopil;
      public tk2dSprite baseSprite;
      private RoomHandler m_parentRoom;
      private bool m_alreadyWarpingAutomatically;

      private void Start()
      {
        this.m_parentRoom = this.transform.position.GetAbsoluteRoom();
        this.m_parentRoom.Entered += new RoomHandler.OnEnteredEventHandler(this.HandlePlayerEntered);
      }

      private void HandlePlayerEntered(PlayerController p)
      {
        if (this.m_alreadyWarpingAutomatically)
          return;
        this.StartCoroutine(this.HandleAutowarpOut());
      }

      [DebuggerHidden]
      private IEnumerator HandleAutowarpOut()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new CreepyEyeController.<HandleAutowarpOut>c__Iterator0()
        {
          $this = this
        };
      }

      private void LateUpdate()
      {
        if (Application.isPlaying)
        {
          Vector2 vector2 = GameManager.Instance.PrimaryPlayer.CenterPosition - this.transform.position.XY();
          this.poopil.transform.localPosition = (Vector3) (Mathf.Lerp(0.0f, this.MaxPupilRadius, vector2.magnitude / 7f) * vector2.normalized);
        }
        float x1 = this.baseSprite.GetBounds().extents.x;
        float x2 = this.poopil.GetComponent<tk2dSprite>().GetBounds().extents.x;
        for (int index = 0; index < this.layers.Length; ++index)
        {
          if ((Object) this.layers[index].sprite == (Object) null)
            this.layers[index].sprite = this.layers[index].xform.GetComponent<tk2dSprite>();
          float num1 = Mathf.Pow((float) (1.0 - (double) this.layers[index].sprite.GetBounds().extents.x / (double) x1), Mathf.Lerp(0.75f, 1f, 1f - (float) index / ((float) this.layers.Length - 1f)));
          float num2 = this.poopil.localPosition.magnitude / (x1 - x2);
          this.layers[index].xform.localPosition = this.poopil.localPosition * num1 + this.poopil.localPosition.normalized * x2 * num2 * num1;
          this.layers[index].xform.localPosition = this.layers[index].xform.localPosition.Quantize(1f / 16f);
          this.layers[index].sprite.HeightOffGround = (float) ((double) index * 0.10000000149011612 + 0.10000000149011612);
          this.layers[index].sprite.UpdateZDepth();
        }
        this.poopil.GetComponent<tk2dSprite>().HeightOffGround = 1f;
        this.poopil.GetComponent<tk2dSprite>().UpdateZDepth();
      }
    }

}
