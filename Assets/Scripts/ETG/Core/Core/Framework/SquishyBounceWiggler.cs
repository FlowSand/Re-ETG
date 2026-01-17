// Decompiled with JetBrains decompiler
// Type: SquishyBounceWiggler
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Core.Framework
{
    public class SquishyBounceWiggler : BraveBehaviour
    {
      private bool m_wiggleHold;
      protected tk2dBaseSprite m_sprite;
      protected IntVector2 m_spriteDimensions;

      public bool WiggleHold
      {
        get => this.m_wiggleHold;
        set
        {
          if (value && !this.m_wiggleHold)
          {
            if ((bool) (Object) this)
              this.gameObject.SetLayerRecursively(LayerMask.NameToLayer("FG_Critical"));
            this.ResetWiggle();
          }
          else if (!value && this.m_wiggleHold && (bool) (Object) this)
            this.gameObject.SetLayerRecursively(LayerMask.NameToLayer("Unpixelated"));
          this.m_wiggleHold = value;
        }
      }

      private void Awake() => this.m_sprite = this.GetComponent<tk2dBaseSprite>();

      private void Start()
      {
        if (!(bool) (Object) this.m_sprite)
          this.enabled = false;
        Bounds bounds = this.m_sprite.GetBounds();
        this.m_spriteDimensions = new IntVector2(Mathf.RoundToInt(bounds.size.x / (1f / 16f)), Mathf.RoundToInt(bounds.size.y / (1f / 16f)));
        this.transform.position = this.transform.position.Quantize(1f / 16f);
        if ((bool) (Object) this.specRigidbody)
          this.specRigidbody.Reinitialize();
        this.gameObject.SetLayerRecursively(LayerMask.NameToLayer("Unpixelated"));
        this.StartCoroutine(this.DoSquishyBounceWiggle());
      }

      protected override void OnDestroy() => base.OnDestroy();

      public void ResetWiggle()
      {
        if ((Object) this.m_sprite == (Object) null)
          return;
        MeshFilter component = this.GetComponent<MeshFilter>();
        Mesh mesh = component.mesh;
        Vector3[] vertices = mesh.vertices;
        Vector2[] uv = mesh.uv;
        Vector2 zero1 = Vector2.zero;
        Vector2 one1 = Vector2.one;
        Vector3 one2 = Vector3.one;
        Vector3 zero2 = Vector3.zero;
        this.SetClippedGeometry(this.m_sprite.GetCurrentSpriteDef(), vertices, uv, zero2, 0, one2, zero1, one1);
        Vector3[] normals = mesh.normals;
        Color[] colors = mesh.colors;
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.normals = normals;
        mesh.colors = colors;
        int[] indices = new int[6];
        tk2dSpriteGeomGen.SetClippedSpriteIndices(indices, 0, 0, this.m_sprite.GetCurrentSpriteDef());
        mesh.triangles = indices;
        mesh.RecalculateBounds();
        component.mesh = mesh;
      }

      [DebuggerHidden]
      private IEnumerator DoSquishyBounceWiggle()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new SquishyBounceWiggler.<DoSquishyBounceWiggle>c__Iterator0()
        {
          _this = this
        };
      }

      private void SetClippedGeometry(
        tk2dSpriteDefinition spriteDef,
        Vector3[] pos,
        Vector2[] uv,
        Vector3 translation,
        int offset,
        Vector3 scale,
        Vector2 clipBottomLeft,
        Vector2 clipTopRight)
      {
        Vector2 vector2_1 = clipBottomLeft;
        Vector2 vector2_2 = clipTopRight;
        Vector3 position0 = spriteDef.position0;
        Vector3 position3 = spriteDef.position3;
        Vector3 vector3_1 = new Vector3(Mathf.Lerp(position0.x, position3.x, vector2_1.x) * scale.x, Mathf.Lerp(position0.y, position3.y, vector2_1.y) * scale.y, position0.z * scale.z);
        Vector3 vector3_2 = new Vector3(Mathf.Lerp(position0.x, position3.x, vector2_2.x) * scale.x, Mathf.Lerp(position0.y, position3.y, vector2_2.y) * scale.y, position0.z * scale.z);
        pos[offset] = new Vector3(vector3_1.x, vector3_1.y, vector3_1.z) + translation;
        pos[offset + 1] = new Vector3(vector3_2.x, vector3_1.y, vector3_1.z) + translation;
        pos[offset + 2] = new Vector3(vector3_1.x, vector3_2.y, vector3_1.z) + translation;
        pos[offset + 3] = new Vector3(vector3_2.x, vector3_2.y, vector3_1.z) + translation;
        if (this.m_sprite.ShouldDoTilt)
        {
          for (int index = offset; index < offset + 4; ++index)
          {
            if (this.m_sprite.IsPerpendicular)
              pos[index].z -= pos[index].y;
            else
              pos[index].z += pos[index].y;
          }
        }
        if (spriteDef.flipped == tk2dSpriteDefinition.FlipMode.Tk2d)
        {
          Vector2 vector2_3 = new Vector2(Mathf.Lerp(spriteDef.uvs[0].x, spriteDef.uvs[3].x, vector2_1.y), Mathf.Lerp(spriteDef.uvs[0].y, spriteDef.uvs[3].y, vector2_1.x));
          Vector2 vector2_4 = new Vector2(Mathf.Lerp(spriteDef.uvs[0].x, spriteDef.uvs[3].x, vector2_2.y), Mathf.Lerp(spriteDef.uvs[0].y, spriteDef.uvs[3].y, vector2_2.x));
          uv[offset] = new Vector2(vector2_3.x, vector2_3.y);
          uv[offset + 1] = new Vector2(vector2_3.x, vector2_4.y);
          uv[offset + 2] = new Vector2(vector2_4.x, vector2_3.y);
          uv[offset + 3] = new Vector2(vector2_4.x, vector2_4.y);
        }
        else if (spriteDef.flipped == tk2dSpriteDefinition.FlipMode.TPackerCW)
        {
          Vector2 vector2_5 = new Vector2(Mathf.Lerp(spriteDef.uvs[0].x, spriteDef.uvs[3].x, vector2_1.y), Mathf.Lerp(spriteDef.uvs[0].y, spriteDef.uvs[3].y, vector2_1.x));
          Vector2 vector2_6 = new Vector2(Mathf.Lerp(spriteDef.uvs[0].x, spriteDef.uvs[3].x, vector2_2.y), Mathf.Lerp(spriteDef.uvs[0].y, spriteDef.uvs[3].y, vector2_2.x));
          uv[offset] = new Vector2(vector2_5.x, vector2_5.y);
          uv[offset + 2] = new Vector2(vector2_6.x, vector2_5.y);
          uv[offset + 1] = new Vector2(vector2_5.x, vector2_6.y);
          uv[offset + 3] = new Vector2(vector2_6.x, vector2_6.y);
        }
        else
        {
          Vector2 vector2_7 = new Vector2(Mathf.Lerp(spriteDef.uvs[0].x, spriteDef.uvs[3].x, vector2_1.x), Mathf.Lerp(spriteDef.uvs[0].y, spriteDef.uvs[3].y, vector2_1.y));
          Vector2 vector2_8 = new Vector2(Mathf.Lerp(spriteDef.uvs[0].x, spriteDef.uvs[3].x, vector2_2.x), Mathf.Lerp(spriteDef.uvs[0].y, spriteDef.uvs[3].y, vector2_2.y));
          uv[offset] = new Vector2(vector2_7.x, vector2_7.y);
          uv[offset + 1] = new Vector2(vector2_8.x, vector2_7.y);
          uv[offset + 2] = new Vector2(vector2_7.x, vector2_8.y);
          uv[offset + 3] = new Vector2(vector2_8.x, vector2_8.y);
        }
      }
    }

}
