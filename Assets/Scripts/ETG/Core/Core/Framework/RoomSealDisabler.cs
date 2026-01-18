// Decompiled with JetBrains decompiler
// Type: RoomSealDisabler
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable

public class RoomSealDisabler : BraveBehaviour
  {
    public bool MatchRoomState = true;

    private void Start()
    {
      this.transform.position.GetAbsoluteRoom().OnSealChanged += new Action<bool>(this.HandleSealStateChanged);
      this.HandleSealStateChanged(false);
    }

    private void HandleSealStateChanged(bool isSealed)
    {
      if (this.MatchRoomState)
      {
        if ((bool) (UnityEngine.Object) this.specRigidbody)
          this.specRigidbody.enabled = isSealed;
        this.gameObject.SetActive(isSealed);
      }
      else
      {
        if ((bool) (UnityEngine.Object) this.specRigidbody)
          this.specRigidbody.enabled = !isSealed;
        this.gameObject.SetActive(!isSealed);
      }
    }
  }

