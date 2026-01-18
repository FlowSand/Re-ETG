using System;
using UnityEngine;

#nullable disable

public class dfMarkupBoxTexture : dfMarkupBox
  {
    public dfMarkupBoxTexture(
      dfMarkupElement element,
      dfMarkupDisplayType display,
      dfMarkupStyle style) : base(element, display, style)
    {
    }

    private static int[] TRIANGLE_INDICES = new int[6]
    {
      0,
      1,
      2,
      0,
      2,
      3
    };
    private dfRenderData renderData = new dfRenderData();
    private Material material;

    public Texture Texture { get; set; }

    internal void LoadTexture(Texture texture)
    {
      this.Texture = !((UnityEngine.Object) texture == (UnityEngine.Object) null) ? texture : throw new InvalidOperationException();
      this.Size = new Vector2((float) texture.width, (float) texture.height);
      this.Baseline = (int) this.Size.y;
    }

    protected override dfRenderData OnRebuildRenderData()
    {
      this.renderData.Clear();
      this.ensureMaterial();
      this.renderData.Material = this.material;
      this.renderData.Material.mainTexture = this.Texture;
      Vector3 zero = Vector3.zero;
      Vector3 vector3_1 = zero + Vector3.right * this.Size.x;
      Vector3 vector3_2 = vector3_1 + Vector3.down * this.Size.y;
      Vector3 vector3_3 = zero + Vector3.down * this.Size.y;
      this.renderData.Vertices.Add(zero);
      this.renderData.Vertices.Add(vector3_1);
      this.renderData.Vertices.Add(vector3_2);
      this.renderData.Vertices.Add(vector3_3);
      this.renderData.Triangles.AddRange(dfMarkupBoxTexture.TRIANGLE_INDICES);
      this.renderData.UV.Add(new Vector2(0.0f, 1f));
      this.renderData.UV.Add(new Vector2(1f, 1f));
      this.renderData.UV.Add(new Vector2(1f, 0.0f));
      this.renderData.UV.Add(new Vector2(0.0f, 0.0f));
      Color color = this.Style.Color;
      this.renderData.Colors.Add((Color32) color);
      this.renderData.Colors.Add((Color32) color);
      this.renderData.Colors.Add((Color32) color);
      this.renderData.Colors.Add((Color32) color);
      return this.renderData;
    }

    private void ensureMaterial()
    {
      if ((UnityEngine.Object) this.material != (UnityEngine.Object) null || (UnityEngine.Object) this.Texture == (UnityEngine.Object) null)
        return;
      Shader shader = Shader.Find("Daikon Forge/Default UI Shader");
      if ((UnityEngine.Object) shader == (UnityEngine.Object) null)
      {
        Debug.LogError((object) "Failed to find default shader");
      }
      else
      {
        Material material = new Material(shader);
        material.name = "Default Texture Shader";
        material.hideFlags = HideFlags.DontSave;
        material.mainTexture = this.Texture;
        this.material = material;
      }
    }

    private static void addTriangleIndices(dfList<Vector3> verts, dfList<int> triangles)
    {
      int count = verts.Count;
      foreach (int triangleIndex in dfMarkupBoxTexture.TRIANGLE_INDICES)
        triangles.Add(count + triangleIndex);
    }
  }

