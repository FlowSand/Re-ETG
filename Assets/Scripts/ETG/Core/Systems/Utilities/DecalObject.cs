using Dungeonator;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

#nullable disable

public class DecalObject : EphemeralObject
  {
    private static Dictionary<RoomHandler, List<DecalObject>> m_roomMap = new Dictionary<RoomHandler, List<DecalObject>>();
    public bool IsRoomLimited;
    [ShowInInspectorIf("IsRoomLimited", false)]
    public int MaxNumberInRoom = 5;
    private RoomHandler m_parent;

    public static void ClearPerLevelData() => DecalObject.m_roomMap.Clear();

    public override void Start()
    {
      base.Start();
      if (!this.IsRoomLimited)
        return;
      this.m_parent = this.transform.position.GetAbsoluteRoom();
      if (!DecalObject.m_roomMap.ContainsKey(this.m_parent))
        DecalObject.m_roomMap.Add(this.m_parent, new List<DecalObject>());
      DecalObject.m_roomMap[this.m_parent].Add(this);
      if (DecalObject.m_roomMap[this.m_parent].Count <= this.MaxNumberInRoom)
        return;
      DecalObject decal = DecalObject.m_roomMap[this.m_parent][0];
      DecalObject.m_roomMap[this.m_parent].RemoveAt(0);
      decal.StartCoroutine(decal.FadeAndDestroy(decal));
    }

    [DebuggerHidden]
    public IEnumerator FadeAndDestroy(DecalObject decal)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new DecalObject__FadeAndDestroyc__Iterator0()
      {
        decal = decal
      };
    }

    protected override void OnDestroy()
    {
      base.OnDestroy();
      if (!this.IsRoomLimited || !DecalObject.m_roomMap.ContainsKey(this.m_parent))
        return;
      DecalObject.m_roomMap[this.m_parent].Remove(this);
    }
  }

