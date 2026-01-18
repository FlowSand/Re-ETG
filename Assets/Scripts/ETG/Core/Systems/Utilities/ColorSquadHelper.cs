using Dungeonator;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

public class ColorSquadHelper : MonoBehaviour, IPlaceConfigurable
  {
    private RoomHandler m_room;

    [DebuggerHidden]
    private IEnumerator Start()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new ColorSquadHelper__Startc__Iterator0()
      {
        _this = this
      };
    }

    public void ConfigureOnPlacement(RoomHandler room) => this.m_room = room;
  }

