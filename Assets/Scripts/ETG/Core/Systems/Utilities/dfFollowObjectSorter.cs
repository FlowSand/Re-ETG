// Decompiled with JetBrains decompiler
// Type: dfFollowObjectSorter
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable

public class dfFollowObjectSorter : MonoBehaviour
  {
    private static dfFollowObjectSorter _instance;
    private static dfList<dfFollowObjectSorter.FollowSortRecord> list = new dfList<dfFollowObjectSorter.FollowSortRecord>();

    public static dfFollowObjectSorter Instance
    {
      get
      {
        lock ((object) typeof (dfFollowObjectSorter))
        {
          if ((UnityEngine.Object) dfFollowObjectSorter._instance == (UnityEngine.Object) null && Application.isPlaying)
          {
            dfFollowObjectSorter._instance = new GameObject("Follow Object Sorter").AddComponent<dfFollowObjectSorter>();
            dfFollowObjectSorter.list.Clear();
          }
          return dfFollowObjectSorter._instance;
        }
      }
    }

    public static void Register(dfFollowObject follow)
    {
      if (!Application.isPlaying)
        return;
      dfFollowObjectSorter.Instance.register(follow);
    }

    public static void Unregister(dfFollowObject follow)
    {
      for (int index = 0; index < dfFollowObjectSorter.list.Count; ++index)
      {
        if ((UnityEngine.Object) dfFollowObjectSorter.list[index].follow == (UnityEngine.Object) follow)
        {
          dfFollowObjectSorter.list.RemoveAt(index);
          break;
        }
      }
    }

    public void Update()
    {
      int num = int.MaxValue;
      for (int index = 0; index < dfFollowObjectSorter.list.Count; ++index)
      {
        dfFollowObjectSorter.FollowSortRecord followSortRecord = dfFollowObjectSorter.list[index];
        if ((bool) (UnityEngine.Object) followSortRecord.follow.attach)
        {
          followSortRecord.distance = this.getDistance(followSortRecord.follow);
          if (followSortRecord.control.ZOrder < num)
            num = followSortRecord.control.ZOrder;
        }
      }
      dfFollowObjectSorter.list.Sort();
      for (int index = 0; index < dfFollowObjectSorter.list.Count; ++index)
        dfFollowObjectSorter.list[index].control.ZOrder = num++;
    }

    private void register(dfFollowObject follow)
    {
      for (int index = 0; index < dfFollowObjectSorter.list.Count; ++index)
      {
        if ((UnityEngine.Object) dfFollowObjectSorter.list[index].follow == (UnityEngine.Object) follow)
          return;
      }
      dfFollowObjectSorter.list.Add(new dfFollowObjectSorter.FollowSortRecord(follow));
    }

    private float getDistance(dfFollowObject follow)
    {
      return (follow.mainCamera.transform.position - follow.attach.transform.position).sqrMagnitude;
    }

    private class FollowSortRecord : IComparable<dfFollowObjectSorter.FollowSortRecord>
    {
      public float distance;
      public dfFollowObject follow;
      public dfControl control;

      public FollowSortRecord(dfFollowObject follow)
      {
        this.follow = follow;
        this.control = follow.GetComponent<dfControl>();
      }

      public int CompareTo(dfFollowObjectSorter.FollowSortRecord other)
      {
        return other.distance.CompareTo(this.distance);
      }
    }
  }

