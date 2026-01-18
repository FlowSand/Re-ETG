using UnityEngine;

#nullable disable

internal class dfRenderBatch
  {
    public Material Material;
    private dfList<dfRenderData> buffers = new dfList<dfRenderData>();

    public void Add(dfRenderData buffer)
    {
      if ((Object) this.Material == (Object) null && (Object) buffer.Material != (Object) null)
        this.Material = buffer.Material;
      this.buffers.Add(buffer);
    }

    public dfRenderData Combine()
    {
      dfRenderData dfRenderData1 = dfRenderData.Obtain();
      int count1 = this.buffers.Count;
      dfRenderData[] items1 = this.buffers.Items;
      if (count1 == 0)
        return dfRenderData1;
      dfRenderData1.Material = this.buffers[0].Material;
      int capacity = 0;
      for (int index = 0; index < count1; ++index)
        capacity = items1[index].Vertices.Count;
      dfRenderData1.EnsureCapacity(capacity);
      int[] items2 = dfRenderData1.Triangles.Items;
      for (int index1 = 0; index1 < count1; ++index1)
      {
        dfRenderData dfRenderData2 = items1[index1];
        int count2 = dfRenderData1.Vertices.Count;
        int count3 = dfRenderData1.Triangles.Count;
        int count4 = dfRenderData2.Triangles.Count;
        dfRenderData1.Vertices.AddRange(dfRenderData2.Vertices);
        dfRenderData1.Triangles.AddRange(dfRenderData2.Triangles);
        dfRenderData1.Colors.AddRange(dfRenderData2.Colors);
        dfRenderData1.UV.AddRange(dfRenderData2.UV);
        for (int index2 = count3; index2 < count3 + count4; ++index2)
          items2[index2] += count2;
      }
      return dfRenderData1;
    }
  }

