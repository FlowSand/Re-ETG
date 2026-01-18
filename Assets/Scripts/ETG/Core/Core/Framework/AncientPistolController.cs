using Dungeonator;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

#nullable disable

public class AncientPistolController : BraveBehaviour, IPlaceConfigurable
  {
    [NonSerialized]
    public List<RoomHandler> RoomSequence;
    public List<bool> RoomSequenceEnemies;

    public void ConfigureOnPlacement(RoomHandler room)
    {
      this.StartCoroutine(this.HandleDelayedInitialization(room));
    }

    [DebuggerHidden]
    private IEnumerator HandleDelayedInitialization(RoomHandler room)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new AncientPistolController__HandleDelayedInitializationc__Iterator0()
      {
        room = room,
        _this = this
      };
    }

    protected override void OnDestroy() => base.OnDestroy();
  }

