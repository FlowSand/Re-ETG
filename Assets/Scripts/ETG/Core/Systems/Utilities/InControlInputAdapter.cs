// Decompiled with JetBrains decompiler
// Type: InControlInputAdapter
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using InControl;
using UnityEngine;

#nullable disable

public class InControlInputAdapter : MonoBehaviour
  {
    private static int m_skipInputForRestOfFrame;
    public static bool CurrentlyUsingAllDevices;

    private void OnEnable()
    {
    }

    public static bool SkipInputForRestOfFrame
    {
      get => InControlInputAdapter.m_skipInputForRestOfFrame > 0;
      set
      {
        InControlInputAdapter.m_skipInputForRestOfFrame = !value ? 0 : Mathf.Max(5, InControlInputAdapter.m_skipInputForRestOfFrame);
      }
    }

    private void Update()
    {
      if (GameManager.Instance.IsLoadingLevel)
        return;
      bool inputForRestOfFrame = InControlInputAdapter.SkipInputForRestOfFrame;
      this.ProcessPrimaryPlayerInput(ref inputForRestOfFrame);
      if (GameManager.Instance.IsPaused && GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER)
        this.ProcessSecondaryPlayerInput(ref inputForRestOfFrame);
      if (!InControlInputAdapter.CurrentlyUsingAllDevices)
        return;
      for (int index = 0; index < InputManager.Devices.Count; ++index)
        this.ProcessRawDeviceInput(InputManager.Devices[index], ref inputForRestOfFrame);
    }

    private void ProcessRawDeviceInput(InputDevice device, ref bool didProcessInput)
    {
      dfControl activeControl = dfGUIManager.ActiveControl;
      if ((Object) activeControl == (Object) null || !activeControl.transform.IsChildOf(this.transform))
        return;
      if (!device.LeftStickUp.IsPressed)
        InControlInputAdapter.HandleControl((OneAxisInputControl) device.DPadUp, activeControl, KeyCode.UpArrow, ref didProcessInput, true);
      if (!device.LeftStickRight.IsPressed)
        InControlInputAdapter.HandleControl((OneAxisInputControl) device.DPadRight, activeControl, KeyCode.RightArrow, ref didProcessInput, true);
      if (!device.LeftStickDown.IsPressed)
        InControlInputAdapter.HandleControl((OneAxisInputControl) device.DPadDown, activeControl, KeyCode.DownArrow, ref didProcessInput, true);
      if (!device.LeftStickLeft.IsPressed)
        InControlInputAdapter.HandleControl((OneAxisInputControl) device.DPadLeft, activeControl, KeyCode.LeftArrow, ref didProcessInput, true);
      if (!device.DPadUp.IsPressed)
        InControlInputAdapter.HandleControlAsDpad((OneAxisInputControl) device.LeftStickUp, activeControl, KeyCode.UpArrow, ref didProcessInput, true);
      if (!device.DPadRight.IsPressed)
        InControlInputAdapter.HandleControlAsDpad((OneAxisInputControl) device.LeftStickRight, activeControl, KeyCode.RightArrow, ref didProcessInput, true);
      if (!device.DPadDown.IsPressed)
        InControlInputAdapter.HandleControlAsDpad((OneAxisInputControl) device.LeftStickDown, activeControl, KeyCode.DownArrow, ref didProcessInput, true);
      if (!device.DPadLeft.IsPressed)
        InControlInputAdapter.HandleControlAsDpad((OneAxisInputControl) device.LeftStickLeft, activeControl, KeyCode.LeftArrow, ref didProcessInput, true);
      switch (GungeonActions.LocalizedMenuSelectAction)
      {
        case InputControlType.Action1:
          InControlInputAdapter.HandleControl((OneAxisInputControl) device.Action1, activeControl, KeyCode.Return, ref didProcessInput);
          break;
        case InputControlType.Action2:
          InControlInputAdapter.HandleControl((OneAxisInputControl) device.Action2, activeControl, KeyCode.Return, ref didProcessInput);
          break;
      }
      switch (GungeonActions.LocalizedMenuCancelAction)
      {
        case InputControlType.Action1:
          InControlInputAdapter.HandleControl((OneAxisInputControl) device.Action1, activeControl, KeyCode.Escape, ref didProcessInput);
          break;
        case InputControlType.Action2:
          InControlInputAdapter.HandleControl((OneAxisInputControl) device.Action2, activeControl, KeyCode.Escape, ref didProcessInput);
          break;
      }
    }

    private void ProcessPrimaryPlayerInput(ref bool didProcessInput)
    {
      if ((Object) BraveInput.PrimaryPlayerInstance == (Object) null)
        return;
      GungeonActions activeActions = BraveInput.PrimaryPlayerInstance.ActiveActions;
      dfControl activeControl = dfGUIManager.ActiveControl;
      if ((Object) activeControl == (Object) null || !activeControl.transform.IsChildOf(this.transform))
        return;
      InControlInputAdapter.HandleControlAsDpad((OneAxisInputControl) activeActions.SelectUp, activeControl, KeyCode.UpArrow, ref didProcessInput, true);
      InControlInputAdapter.HandleControlAsDpad((OneAxisInputControl) activeActions.SelectDown, activeControl, KeyCode.DownArrow, ref didProcessInput, true);
      InControlInputAdapter.HandleControlAsDpad((OneAxisInputControl) activeActions.SelectLeft, activeControl, KeyCode.LeftArrow, ref didProcessInput, true);
      InControlInputAdapter.HandleControlAsDpad((OneAxisInputControl) activeActions.SelectRight, activeControl, KeyCode.RightArrow, ref didProcessInput, true);
      InControlInputAdapter.HandleControl((OneAxisInputControl) activeActions.MenuSelectAction, activeControl, KeyCode.Return, ref didProcessInput);
      InControlInputAdapter.HandleControl((OneAxisInputControl) activeActions.CancelAction, activeControl, KeyCode.Escape, ref didProcessInput);
      if (!Input.GetKeyUp(KeyCode.Return))
        return;
      activeControl.OnKeyUp(new dfKeyEventArgs(activeControl, KeyCode.Return, false, false, false));
      didProcessInput = true;
    }

    private void ProcessSecondaryPlayerInput(ref bool didProcessInput)
    {
      GungeonActions activeActions = BraveInput.SecondaryPlayerInstance.ActiveActions;
      if (activeActions == null || activeActions.ForceDisable)
        return;
      dfControl activeControl = dfGUIManager.ActiveControl;
      if ((Object) activeControl == (Object) null || !activeControl.transform.IsChildOf(this.transform))
        return;
      InControlInputAdapter.HandleControlAsDpad((OneAxisInputControl) activeActions.SelectUp, activeControl, KeyCode.UpArrow, ref didProcessInput, true);
      InControlInputAdapter.HandleControlAsDpad((OneAxisInputControl) activeActions.SelectDown, activeControl, KeyCode.DownArrow, ref didProcessInput, true);
      InControlInputAdapter.HandleControlAsDpad((OneAxisInputControl) activeActions.SelectLeft, activeControl, KeyCode.LeftArrow, ref didProcessInput, true);
      InControlInputAdapter.HandleControlAsDpad((OneAxisInputControl) activeActions.SelectRight, activeControl, KeyCode.RightArrow, ref didProcessInput, true);
      InControlInputAdapter.HandleControl((OneAxisInputControl) activeActions.MenuSelectAction, activeControl, KeyCode.Return, ref didProcessInput);
      InControlInputAdapter.HandleControl((OneAxisInputControl) activeActions.CancelAction, activeControl, KeyCode.Escape, ref didProcessInput);
    }

    public void LateUpdate()
    {
      InControlInputAdapter.m_skipInputForRestOfFrame = Mathf.Max(InControlInputAdapter.m_skipInputForRestOfFrame - 1, 0);
    }

    private static void HandleControl(
      OneAxisInputControl control,
      dfControl target,
      KeyCode keyCode,
      ref bool didProcessInput,
      bool repeating = false)
    {
      if ((!repeating ? (control.WasPressed ? 1 : 0) : (control.WasPressedRepeating ? 1 : 0)) != 0)
      {
        if (didProcessInput)
          return;
        target.OnKeyDown(new dfKeyEventArgs(target, keyCode, false, false, false));
        didProcessInput = true;
      }
      else
      {
        if (!control.WasReleased || didProcessInput)
          return;
        target.OnKeyUp(new dfKeyEventArgs(target, keyCode, false, false, false));
        didProcessInput = true;
      }
    }

    private static void HandleControlAsDpad(
      OneAxisInputControl control,
      dfControl target,
      KeyCode keyCode,
      ref bool didProcessInput,
      bool repeating = false)
    {
      if ((!repeating ? (control.WasPressedAsDpad ? 1 : 0) : (control.WasPressedAsDpadRepeating ? 1 : 0)) != 0)
      {
        if (didProcessInput)
          return;
        target.OnKeyDown(new dfKeyEventArgs(target, keyCode, false, false, false));
        didProcessInput = true;
      }
      else
      {
        if (!control.WasReleased || didProcessInput)
          return;
        target.OnKeyUp(new dfKeyEventArgs(target, keyCode, false, false, false));
        didProcessInput = true;
      }
    }
  }

