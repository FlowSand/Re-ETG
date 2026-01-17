// Decompiled with JetBrains decompiler
// Type: BraveInput
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using InControl;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    public class BraveInput : MonoBehaviour
    {
      public static bool AllowPausedRumble = false;
      public BraveInput.AutoAim autoAimMode;
      public bool showCursor;
      public MagnetAngles magnetAngles;
      public float controllerAutoAimDegrees = 15f;
      public float controllerSuperAutoAimDegrees = 25f;
      public float controllerFakeSemiAutoCooldown = 0.25f;
      [BetterList]
      public BraveInput.BufferedInput[] PressActions;
      [BetterList]
      public BraveInput.BufferedInput[] HoldActions;
      private GungeonActions m_activeGungeonActions;
      [NonSerialized]
      private int m_playerID;
      private PooledLinkedList<BraveInput.PressAction> m_pressActions = new PooledLinkedList<BraveInput.PressAction>();
      private PooledLinkedList<BraveInput.HoldAction> m_holdActions = new PooledLinkedList<BraveInput.HoldAction>();
      private List<BraveInput.TimedVibration> m_currentVibrations = new List<BraveInput.TimedVibration>();
      private float m_sustainedLargeVibration;
      private float m_sustainedSmallVibration;
      private static Dictionary<int, BraveInput> m_instances = new Dictionary<int, BraveInput>();

      private static void DoStartupAssignmentOfControllers(int lastActiveDeviceIndex = -1)
      {
        if (GameManager.PreventGameManagerExistence || GameManager.Instance.CurrentGameType == GameManager.GameType.SINGLE_PLAYER)
        {
          GameManager.Options.PlayerIDtoDeviceIndexMap.Clear();
        }
        else
        {
          if (GameManager.Instance.CurrentGameType != GameManager.GameType.COOP_2_PLAYER)
            return;
          GameManager.Options.PlayerIDtoDeviceIndexMap.Clear();
          if (Application.platform == RuntimePlatform.PS4)
          {
            GameManager.Options.PlayerIDtoDeviceIndexMap.Add(0, 0);
            GameManager.Options.PlayerIDtoDeviceIndexMap.Add(1, 1);
          }
          else if (InputManager.Devices.Count == 1)
          {
            if (lastActiveDeviceIndex != 0)
            {
              GameManager.Options.PlayerIDtoDeviceIndexMap.Add(1, 0);
              GameManager.Options.PlayerIDtoDeviceIndexMap.Add(0, 1);
            }
            else
            {
              GameManager.Options.PlayerIDtoDeviceIndexMap.Add(0, 0);
              GameManager.Options.PlayerIDtoDeviceIndexMap.Add(1, 1);
            }
          }
          else if (InputManager.Devices.Count == 2)
          {
            if (lastActiveDeviceIndex >= 1)
            {
              GameManager.Options.PlayerIDtoDeviceIndexMap.Add(0, lastActiveDeviceIndex);
              GameManager.Options.PlayerIDtoDeviceIndexMap.Add(1, 0);
            }
            else
            {
              GameManager.Options.PlayerIDtoDeviceIndexMap.Add(0, 0);
              GameManager.Options.PlayerIDtoDeviceIndexMap.Add(1, 1);
            }
          }
          else if (lastActiveDeviceIndex >= 1)
          {
            GameManager.Options.PlayerIDtoDeviceIndexMap.Add(0, lastActiveDeviceIndex);
            GameManager.Options.PlayerIDtoDeviceIndexMap.Add(1, -1);
            GameManager.Instance.StartCoroutine(BraveInput.AssignPlayerTwoToNextActiveDevice());
          }
          else
          {
            GameManager.Options.PlayerIDtoDeviceIndexMap.Add(0, 0);
            GameManager.Options.PlayerIDtoDeviceIndexMap.Add(1, 1);
          }
        }
      }

      [DebuggerHidden]
      private static IEnumerator AssignPlayerTwoToNextActiveDevice()
      {
        // ISSUE: object of a compiler-generated type is created
        // ISSUE: variable of a compiler-generated type
        BraveInput.<AssignPlayerTwoToNextActiveDevice>c__Iterator0 nextActiveDevice = new BraveInput.<AssignPlayerTwoToNextActiveDevice>c__Iterator0();
        return (IEnumerator) nextActiveDevice;
      }

      public static void ReassignAllControllers(InputDevice overrideLastActiveDevice = null)
      {
        UnityEngine.Debug.LogWarning((object) "Reassigning all controllers.");
        InputDevice inputDevice = overrideLastActiveDevice ?? InputManager.ActiveDevice;
        int lastActiveDeviceIndex = -1;
        for (int index = 0; index < InputManager.Devices.Count; ++index)
        {
          if (InputManager.Devices[index] == inputDevice)
            lastActiveDeviceIndex = index;
        }
        for (int key = 0; key < BraveInput.m_instances.Count; ++key)
        {
          if (BraveInput.m_instances[key].m_activeGungeonActions != null)
            BraveInput.m_instances[key].m_activeGungeonActions.Destroy();
          BraveInput.m_instances[key].m_activeGungeonActions = (GungeonActions) null;
        }
        BraveInput.DoStartupAssignmentOfControllers(lastActiveDeviceIndex);
        for (int key = 0; key < BraveInput.m_instances.Count; ++key)
        {
          if (BraveInput.m_instances[key].m_activeGungeonActions == null)
          {
            BraveInput.m_instances[key].m_activeGungeonActions = new GungeonActions();
            BraveInput.m_instances[key].AssignActionsDevice();
            BraveInput.m_instances[key].m_activeGungeonActions.InitializeDefaults();
            if ((UnityEngine.Object) GameManager.Instance.PrimaryPlayer == (UnityEngine.Object) null && BraveInput.m_instances[key].m_playerID == 0 || BraveInput.m_instances[key].m_playerID == GameManager.Instance.PrimaryPlayer.PlayerIDX)
              BraveInput.TryLoadBindings(0, BraveInput.m_instances[key].ActiveActions);
            else
              BraveInput.TryLoadBindings(1, BraveInput.m_instances[key].ActiveActions);
          }
          BraveInput.m_instances[key].AssignActionsDevice();
        }
        for (int key = 0; key < BraveInput.m_instances.Count; ++key)
        {
          if (GameManager.Instance.AllPlayers.Length > 1)
          {
            if (BraveInput.m_instances[key].m_activeGungeonActions.Device == null)
              BraveInput.m_instances[key].m_activeGungeonActions.IgnoreBindingsOfType(BindingSourceType.DeviceBindingSource);
            else if (BraveInput.m_instances[key].m_playerID == GameManager.Instance.PrimaryPlayer.PlayerIDX)
            {
              if (BraveInput.GetInstanceForPlayer(GameManager.Instance.SecondaryPlayer.PlayerIDX).m_activeGungeonActions.Device == null)
              {
                BraveInput.m_instances[key].m_activeGungeonActions.IgnoreBindingsOfType(BindingSourceType.KeyBindingSource);
                BraveInput.m_instances[key].m_activeGungeonActions.IgnoreBindingsOfType(BindingSourceType.MouseBindingSource);
              }
            }
            else
            {
              BraveInput.m_instances[key].m_activeGungeonActions.IgnoreBindingsOfType(BindingSourceType.KeyBindingSource);
              BraveInput.m_instances[key].m_activeGungeonActions.IgnoreBindingsOfType(BindingSourceType.MouseBindingSource);
            }
          }
        }
      }

      public static void ForceLoadBindingInfoFromOptions()
      {
        if (GameManager.Options == null)
          return;
        for (int key = 0; key < BraveInput.m_instances.Count; ++key)
        {
          if (GameManager.PreventGameManagerExistence || (UnityEngine.Object) GameManager.Instance.PrimaryPlayer == (UnityEngine.Object) null)
          {
            if (BraveInput.m_instances[key].m_playerID == 0)
              BraveInput.TryLoadBindings(0, BraveInput.m_instances[key].ActiveActions);
          }
          else if (BraveInput.m_instances[key].m_playerID == GameManager.Instance.PrimaryPlayer.PlayerIDX)
            BraveInput.TryLoadBindings(0, BraveInput.m_instances[key].ActiveActions);
          else
            BraveInput.TryLoadBindings(1, BraveInput.m_instances[key].ActiveActions);
        }
      }

      public static void SavePlayerlessBindingsToOptions()
      {
        if (GameManager.Options == null || (UnityEngine.Object) GameManager.Instance.PrimaryPlayer != (UnityEngine.Object) null || (UnityEngine.Object) BraveInput.PlayerlessInstance == (UnityEngine.Object) null)
          return;
        GameManager.Options.playerOneBindingDataV2 = BraveInput.PlayerlessInstance.ActiveActions.Save();
      }

      public static void SaveBindingInfoToOptions()
      {
        if (GameManager.Options == null || (UnityEngine.Object) GameManager.Instance.PrimaryPlayer == (UnityEngine.Object) null)
          return;
        UnityEngine.Debug.Log((object) "Saving Binding Info To Options");
        for (int key = 0; key < BraveInput.m_instances.Count; ++key)
        {
          if (BraveInput.m_instances[key].m_playerID == GameManager.Instance.PrimaryPlayer.PlayerIDX)
            GameManager.Options.playerOneBindingDataV2 = BraveInput.m_instances[key].ActiveActions.Save();
          else
            GameManager.Options.playerTwoBindingDataV2 = BraveInput.m_instances[key].ActiveActions.Save();
        }
      }

      public static void OnLanguageChanged()
      {
        for (int key = 0; key < BraveInput.m_instances.Count; ++key)
        {
          if ((bool) (UnityEngine.Object) BraveInput.m_instances[key] && BraveInput.m_instances[key].ActiveActions != null)
            BraveInput.m_instances[key].ActiveActions.ReinitializeMenuDefaults();
        }
      }

      public static void ResetBindingsToDefaults()
      {
        GameManager.Options.playerOneBindingData = string.Empty;
        GameManager.Options.playerOneBindingDataV2 = string.Empty;
        GameManager.Options.playerTwoBindingData = string.Empty;
        GameManager.Options.playerTwoBindingDataV2 = string.Empty;
        BraveInput.DoStartupAssignmentOfControllers();
        for (int key = 0; key < BraveInput.m_instances.Count; ++key)
        {
          if (BraveInput.m_instances[key].m_activeGungeonActions != null)
            BraveInput.m_instances[key].m_activeGungeonActions.Destroy();
          BraveInput.m_instances[key].m_activeGungeonActions = (GungeonActions) null;
          BraveInput.m_instances[key].CheckForActionInitialization();
        }
        BraveInput.SaveBindingInfoToOptions();
      }

      public static int GetDeviceIndex(InputDevice device)
      {
        int deviceIndex = -1;
        for (int index = 0; index < InputManager.Devices.Count; ++index)
        {
          if (InputManager.Devices[index] == device)
            deviceIndex = index;
        }
        return deviceIndex;
      }

      public static XInputDevice GetXInputDeviceInSlot(int xInputSlot)
      {
        for (int index = 0; index < InputManager.Devices.Count; ++index)
        {
          if (InputManager.Devices[index] is XInputDevice)
          {
            XInputDevice device = InputManager.Devices[index] as XInputDevice;
            if (device.DeviceIndex == xInputSlot)
              return device;
          }
        }
        return (XInputDevice) null;
      }

      public static void ReassignPlayerPort(int playerID, int portNum)
      {
        GameManager.Options.PlayerIDtoDeviceIndexMap.Remove(playerID);
        GameManager.Options.PlayerIDtoDeviceIndexMap.Add(playerID, portNum);
        for (int key = 0; key < BraveInput.m_instances.Count; ++key)
        {
          if (BraveInput.m_instances[key].m_activeGungeonActions != null)
            BraveInput.m_instances[key].m_activeGungeonActions.Destroy();
          BraveInput.m_instances[key].m_activeGungeonActions = (GungeonActions) null;
        }
        InControlInputAdapter.SkipInputForRestOfFrame = true;
      }

      public static BraveInput PlayerlessInstance
      {
        get
        {
          return BraveInput.m_instances == null || BraveInput.m_instances.Count < 1 ? (BraveInput) null : BraveInput.m_instances[0];
        }
      }

      public static BraveInput PrimaryPlayerInstance
      {
        get
        {
          if (BraveInput.m_instances == null || BraveInput.m_instances.Count < 1)
            return (BraveInput) null;
          return (UnityEngine.Object) GameManager.Instance.PrimaryPlayer == (UnityEngine.Object) null ? BraveInput.m_instances[0] : BraveInput.GetInstanceForPlayer(GameManager.Instance.PrimaryPlayer.PlayerIDX);
        }
      }

      public static BraveInput SecondaryPlayerInstance
      {
        get
        {
          if (BraveInput.m_instances == null || BraveInput.m_instances.Count < 2)
            return (BraveInput) null;
          if (GameManager.Instance.CurrentGameType == GameManager.GameType.SINGLE_PLAYER)
            return (BraveInput) null;
          return (UnityEngine.Object) GameManager.Instance.SecondaryPlayer == (UnityEngine.Object) null ? (BraveInput) null : BraveInput.GetInstanceForPlayer(GameManager.Instance.SecondaryPlayer.PlayerIDX);
        }
      }

      public static bool HasInstanceForPlayer(int id)
      {
        return BraveInput.m_instances.ContainsKey(id) && (UnityEngine.Object) BraveInput.m_instances[id] != (UnityEngine.Object) null && (bool) (UnityEngine.Object) BraveInput.m_instances[id];
      }

      public static BraveInput GetInstanceForPlayer(int id)
      {
        if (BraveInput.m_instances.ContainsKey(id) && ((UnityEngine.Object) BraveInput.m_instances[id] == (UnityEngine.Object) null || !(bool) (UnityEngine.Object) BraveInput.m_instances[id]))
          BraveInput.m_instances.Remove(id);
        if (!BraveInput.m_instances.ContainsKey(id))
        {
          if (BraveInput.m_instances.ContainsKey(0))
          {
            BraveInput component = UnityEngine.Object.Instantiate<GameObject>(BraveInput.m_instances[0].gameObject).GetComponent<BraveInput>();
            component.m_playerID = id;
            BraveInput.m_instances.Add(id, component);
          }
          else
            UnityEngine.Debug.LogError((object) $"Player {(object) id} is attempting to get a BraveInput instance, and player 0's doesn't exist.");
        }
        return !BraveInput.m_instances.ContainsKey(id) ? (BraveInput) null : BraveInput.m_instances[id];
      }

      public static BraveInput.AutoAim AutoAimMode
      {
        get => BraveInput.m_instances[0].autoAimMode;
        set => BraveInput.m_instances[0].autoAimMode = value;
      }

      public static bool ShowCursor
      {
        get => BraveInput.m_instances[0].showCursor;
        set => BraveInput.m_instances[0].showCursor = value;
      }

      public static MagnetAngles MagnetAngles => BraveInput.m_instances[0].magnetAngles;

      public static float ControllerAutoAimDegrees
      {
        get
        {
          float controllerAutoAimDegrees = BraveInput.m_instances[0].controllerAutoAimDegrees;
          if (GameManager.Options != null)
            controllerAutoAimDegrees *= GameManager.Options.controllerAimAssistMultiplier;
          return controllerAutoAimDegrees;
        }
      }

      public static float ControllerSuperAutoAimDegrees
      {
        get
        {
          float superAutoAimDegrees = BraveInput.m_instances[0].controllerSuperAutoAimDegrees;
          if (GameManager.Options != null)
            superAutoAimDegrees *= GameManager.Options.controllerAimAssistMultiplier;
          return superAutoAimDegrees;
        }
      }

      public static float ControllerFakeSemiAutoCooldown
      {
        get => BraveInput.m_instances[0].controllerFakeSemiAutoCooldown;
      }

      public GungeonActions ActiveActions => this.m_activeGungeonActions;

      public void Awake()
      {
        if (BraveInput.m_instances.ContainsKey(0))
          return;
        BraveInput.m_instances.Add(0, this);
      }

      public void OnDestroy()
      {
        if (this.m_activeGungeonActions != null)
        {
          this.m_activeGungeonActions.Destroy();
          this.m_activeGungeonActions = (GungeonActions) null;
        }
        if (!BraveInput.m_instances.ContainsValue(this))
          return;
        BraveInput.m_instances.Remove(this.m_playerID);
      }

      private void AssignActionsDevice()
      {
        if (GameManager.PreventGameManagerExistence || GameManager.Instance.AllPlayers.Length < 2)
        {
          this.m_activeGungeonActions.Device = InputManager.ActiveDevice;
        }
        else
        {
          this.m_activeGungeonActions.Device = InputManager.GetActiveDeviceForPlayer(this.m_playerID);
          if (this.m_playerID == 0 || this.m_activeGungeonActions.Device != InputManager.GetActiveDeviceForPlayer(0))
            return;
          this.m_activeGungeonActions.ForceDisable = true;
        }
      }

      private static void TryLoadBindings(int playerNum, GungeonActions actions)
      {
        string data1;
        string data2;
        switch (playerNum)
        {
          case 0:
            data1 = GameManager.Options.playerOneBindingData;
            data2 = GameManager.Options.playerOneBindingDataV2;
            break;
          case 1:
            data1 = GameManager.Options.playerTwoBindingData;
            data2 = GameManager.Options.playerTwoBindingDataV2;
            break;
          default:
            return;
        }
        if (!string.IsNullOrEmpty(data2))
          actions.Load(data2);
        else if (!string.IsNullOrEmpty(data1))
          actions.Load(data1, true);
        actions.PostProcessAdditionalBlankControls(playerNum);
      }

      public void CheckForActionInitialization()
      {
        if (this.m_activeGungeonActions == null)
        {
          this.m_activeGungeonActions = new GungeonActions();
          this.AssignActionsDevice();
          this.m_activeGungeonActions.InitializeDefaults();
          if (GameManager.PreventGameManagerExistence || (UnityEngine.Object) GameManager.Instance.PrimaryPlayer == (UnityEngine.Object) null && this.m_playerID == 0 || this.m_playerID == GameManager.Instance.PrimaryPlayer.PlayerIDX)
            BraveInput.TryLoadBindings(0, this.ActiveActions);
          else
            BraveInput.TryLoadBindings(1, this.ActiveActions);
          if (!GameManager.PreventGameManagerExistence && GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER)
          {
            if (this.m_playerID == 0 && BraveInput.GetInstanceForPlayer(GameManager.Instance.SecondaryPlayer.PlayerIDX).m_activeGungeonActions == null)
              BraveInput.GetInstanceForPlayer(GameManager.Instance.SecondaryPlayer.PlayerIDX).CheckForActionInitialization();
            if (this.m_activeGungeonActions.Device == null)
              this.m_activeGungeonActions.IgnoreBindingsOfType(BindingSourceType.DeviceBindingSource);
            else if (this.m_playerID == GameManager.Instance.PrimaryPlayer.PlayerIDX)
            {
              if (BraveInput.GetInstanceForPlayer(GameManager.Instance.SecondaryPlayer.PlayerIDX).m_activeGungeonActions.Device == null)
              {
                this.m_activeGungeonActions.IgnoreBindingsOfType(BindingSourceType.KeyBindingSource);
                this.m_activeGungeonActions.IgnoreBindingsOfType(BindingSourceType.MouseBindingSource);
              }
            }
            else
            {
              this.m_activeGungeonActions.IgnoreBindingsOfType(BindingSourceType.KeyBindingSource);
              this.m_activeGungeonActions.IgnoreBindingsOfType(BindingSourceType.MouseBindingSource);
            }
          }
        }
        this.AssignActionsDevice();
      }

      public void Update()
      {
        if (GameManager.Options.PlayerIDtoDeviceIndexMap == null || GameManager.Options.PlayerIDtoDeviceIndexMap.Count == 0)
          BraveInput.DoStartupAssignmentOfControllers();
        this.CheckForActionInitialization();
        LinkedListNode<BraveInput.PressAction> node1 = this.m_pressActions.First;
        while (node1 != null)
        {
          BraveInput.PressAction pressAction = node1.Value;
          pressAction.Timer += GameManager.INVARIANT_DELTA_TIME;
          if ((double) pressAction.Timer >= (double) pressAction.Buffer)
          {
            LinkedListNode<BraveInput.PressAction> next = node1.Next;
            BraveInput.PressAction.Pool.Free(ref pressAction);
            this.m_pressActions.Remove(node1, true);
            node1 = next;
          }
          else
            node1 = node1.Next;
        }
        for (int index = 0; index < this.PressActions.Length; ++index)
        {
          if (this.m_activeGungeonActions.GetActionFromType(this.PressActions[index].Control).WasPressed)
          {
            BraveInput.PressAction pressAction = BraveInput.PressAction.Pool.Allocate();
            pressAction.SetAll(this.PressActions[index]);
            this.m_pressActions.AddLast(pressAction);
          }
        }
        LinkedListNode<BraveInput.HoldAction> linkedListNode = this.m_holdActions.First;
        while (linkedListNode != null)
        {
          BraveInput.HoldAction holdAction1 = linkedListNode.Value;
          holdAction1.DownTimer += GameManager.INVARIANT_DELTA_TIME;
          if (!holdAction1.Held)
            holdAction1.UpTimer += GameManager.INVARIANT_DELTA_TIME;
          else if (!this.m_activeGungeonActions.GetActionFromType(holdAction1.Control).IsPressed)
            holdAction1.Held = false;
          LinkedListNode<BraveInput.HoldAction> node2 = linkedListNode;
          linkedListNode = linkedListNode.Next;
          if (!holdAction1.Held)
          {
            if (holdAction1.ConsumedDown && holdAction1.ConsumedUp)
            {
              BraveInput.HoldAction holdAction2 = node2.Value;
              this.m_holdActions.Remove(node2, true);
              BraveInput.HoldAction.Pool.Free(ref holdAction2);
            }
            else if (!holdAction1.ConsumedDown && (double) holdAction1.UpTimer >= (double) holdAction1.Buffer)
            {
              BraveInput.HoldAction holdAction3 = node2.Value;
              this.m_holdActions.Remove(node2, true);
              BraveInput.HoldAction.Pool.Free(ref holdAction3);
            }
          }
        }
        for (int index = 0; index < this.HoldActions.Length; ++index)
        {
          if (this.m_activeGungeonActions.GetActionFromType(this.HoldActions[index].Control).WasPressed)
          {
            BraveInput.HoldAction holdAction = BraveInput.HoldAction.Pool.Allocate();
            holdAction.SetAll(this.HoldActions[index]);
            this.m_holdActions.AddLast(holdAction);
          }
        }
      }

      public void LateUpdate()
      {
        if (!GameManager.Options.RumbleEnabled || GameManager.Instance.IsLoadingLevel)
        {
          this.SetVibration(0.0f, 0.0f);
          this.m_currentVibrations.Clear();
        }
        else if (GameManager.Instance.IsPaused && !BraveInput.AllowPausedRumble)
        {
          this.SetVibration(0.0f, 0.0f);
        }
        else
        {
          float b1 = 0.0f;
          float b2 = 0.0f;
          float a = Vibration.ConvertFromShakeMagnitude(GameManager.Instance.MainCameraController.ScreenShakeVibration);
          float b3 = Mathf.Max(a, b1);
          float b4 = Mathf.Max(a, b2);
          float num1 = Mathf.Max(this.m_sustainedLargeVibration, b3);
          float num2 = Mathf.Max(this.m_sustainedSmallVibration, b4);
          for (int index = this.m_currentVibrations.Count - 1; index >= 0; --index)
          {
            BraveInput.TimedVibration currentVibration = this.m_currentVibrations[index];
            num1 = Mathf.Max(currentVibration.largeMotor, num1);
            num2 = Mathf.Max(currentVibration.smallMotor, num2);
            if (GameManager.Instance.IsPaused && BraveInput.AllowPausedRumble)
              currentVibration.timer -= GameManager.INVARIANT_DELTA_TIME;
            else
              currentVibration.timer -= BraveTime.DeltaTime;
            if ((double) currentVibration.timer < 0.0)
              this.m_currentVibrations.RemoveAt(index);
          }
          this.SetVibration(num1, num2);
        }
        GameManager.Instance.MainCameraController.MarkScreenShakeVibrationDirty();
        this.m_sustainedLargeVibration = 0.0f;
        this.m_sustainedSmallVibration = 0.0f;
      }

      public BindingSourceType GetLastInputType()
      {
        return this.m_activeGungeonActions == null ? BindingSourceType.None : this.m_activeGungeonActions.LastInputType;
      }

      public bool IsKeyboardAndMouse(bool includeNone = false)
      {
        return this.m_activeGungeonActions == null || includeNone && this.m_activeGungeonActions.LastInputType == BindingSourceType.None || this.m_activeGungeonActions.LastInputType == BindingSourceType.KeyBindingSource || this.m_activeGungeonActions.LastInputType == BindingSourceType.MouseBindingSource;
      }

      public bool HasMouse()
      {
        return this.m_activeGungeonActions == null || this.m_activeGungeonActions.LastInputType == BindingSourceType.KeyBindingSource || this.m_activeGungeonActions.LastInputType == BindingSourceType.MouseBindingSource;
      }

      public Vector2 MousePosition => Input.mousePosition.XY();

      public static void FlushAll()
      {
        for (int key = 0; key < BraveInput.m_instances.Count; ++key)
          BraveInput.m_instances[key].Flush();
      }

      public void Flush()
      {
        while (this.m_pressActions.Count > 0)
        {
          BraveInput.PressAction pressAction = this.m_pressActions.First.Value;
          BraveInput.PressAction.Pool.Free(ref pressAction);
          this.m_pressActions.RemoveFirst();
        }
        while (this.m_holdActions.Count > 0)
        {
          BraveInput.HoldAction holdAction = this.m_holdActions.First.Value;
          BraveInput.HoldAction.Pool.Free(ref holdAction);
          this.m_holdActions.RemoveFirst();
        }
      }

      public void DoVibration(Vibration.Time time, Vibration.Strength strength)
      {
        this.m_currentVibrations.Add(new BraveInput.TimedVibration(Vibration.ConvertTime(time), Vibration.ConvertStrength(strength)));
      }

      public void DoVibration(float time, Vibration.Strength strength)
      {
        this.m_currentVibrations.Add(new BraveInput.TimedVibration(time, Vibration.ConvertStrength(strength)));
      }

      public void DoVibration(
        Vibration.Time time,
        Vibration.Strength largeMotor,
        Vibration.Strength smallMotor)
      {
        this.m_currentVibrations.Add(new BraveInput.TimedVibration(Vibration.ConvertTime(time), Vibration.ConvertStrength(largeMotor), Vibration.ConvertStrength(smallMotor)));
      }

      public void DoScreenShakeVibration(float time, float magnitude)
      {
        this.m_currentVibrations.Add(new BraveInput.TimedVibration(time, Vibration.ConvertFromShakeMagnitude(magnitude)));
      }

      public void DoSustainedVibration(Vibration.Strength strength)
      {
        this.m_sustainedLargeVibration = Mathf.Max(this.m_sustainedLargeVibration, Vibration.ConvertStrength(strength));
      }

      public void DoSustainedVibration(Vibration.Strength largeMotor, Vibration.Strength smallMotor)
      {
        this.m_sustainedLargeVibration = Mathf.Max(this.m_sustainedLargeVibration, Vibration.ConvertStrength(largeMotor));
        this.m_sustainedSmallVibration = Mathf.Max(this.m_sustainedSmallVibration, Vibration.ConvertStrength(smallMotor));
      }

      public static void DoVibrationForAllPlayers(Vibration.Time time, Vibration.Strength strength)
      {
        for (int key = 0; key < BraveInput.m_instances.Count; ++key)
        {
          if ((UnityEngine.Object) BraveInput.m_instances[key] != (UnityEngine.Object) null)
            BraveInput.m_instances[key].DoVibration(time, strength);
        }
      }

      public static void DoVibrationForAllPlayers(
        Vibration.Time time,
        Vibration.Strength largeMotor,
        Vibration.Strength smallMotor)
      {
        for (int key = 0; key < BraveInput.m_instances.Count; ++key)
        {
          if ((UnityEngine.Object) BraveInput.m_instances[key] != (UnityEngine.Object) null)
            BraveInput.m_instances[key].DoVibration(time, largeMotor, smallMotor);
        }
      }

      public static void DoSustainedScreenShakeVibration(float magnitude)
      {
        for (int key = 0; key < BraveInput.m_instances.Count; ++key)
        {
          if ((UnityEngine.Object) BraveInput.m_instances[key] != (UnityEngine.Object) null)
          {
            BraveInput.m_instances[key].m_sustainedLargeVibration = Mathf.Max(BraveInput.m_instances[key].m_sustainedLargeVibration, Vibration.ConvertFromShakeMagnitude(magnitude));
            BraveInput.m_instances[key].m_sustainedSmallVibration = Mathf.Max(BraveInput.m_instances[key].m_sustainedSmallVibration, Vibration.ConvertFromShakeMagnitude(magnitude));
          }
        }
      }

      private void SetVibration(float largeMotor, float smallMotor)
      {
        if (this.m_activeGungeonActions == null || this.m_activeGungeonActions.Device == null)
          return;
        this.m_activeGungeonActions.Device.Vibrate(largeMotor, smallMotor);
      }

      private bool CheckBufferedActionsForControlType(
        BraveInput.BufferedInput[] bufferedInputs,
        GungeonActions.GungeonActionType controlType)
      {
        for (int index = 0; index < bufferedInputs.Length; ++index)
        {
          if (bufferedInputs[index].Control == controlType)
            return true;
        }
        return false;
      }

      private bool CheckPressActionsForControlType(GungeonActions.GungeonActionType controlType)
      {
        for (LinkedListNode<BraveInput.PressAction> linkedListNode = this.m_pressActions.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
        {
          if (linkedListNode.Value.Control == controlType)
            return true;
        }
        return false;
      }

      private BraveInput.PressAction GetPressActionForControlType(
        GungeonActions.GungeonActionType controlType)
      {
        for (LinkedListNode<BraveInput.PressAction> linkedListNode = this.m_pressActions.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
        {
          if (linkedListNode.Value.Control == controlType)
            return linkedListNode.Value;
        }
        return (BraveInput.PressAction) null;
      }

      private BraveInput.HoldAction GetHoldActionForControlType(
        GungeonActions.GungeonActionType controlType)
      {
        for (LinkedListNode<BraveInput.HoldAction> linkedListNode = this.m_holdActions.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
        {
          if (linkedListNode.Value.Control == controlType)
            return linkedListNode.Value;
        }
        return (BraveInput.HoldAction) null;
      }

      public bool GetButtonDown(GungeonActions.GungeonActionType controlType)
      {
        if (this.CheckBufferedActionsForControlType(this.PressActions, controlType))
          return this.CheckPressActionsForControlType(controlType);
        if (this.CheckBufferedActionsForControlType(this.HoldActions, controlType))
        {
          BraveInput.HoldAction actionForControlType = this.GetHoldActionForControlType(controlType);
          return actionForControlType != null && !actionForControlType.ConsumedDown;
        }
        UnityEngine.Debug.LogError((object) $"BraveInput.GetButtonDown(): {controlType} isn't registered with the BraveInput object");
        return false;
      }

      public void ConsumeButtonDown(GungeonActions.GungeonActionType controlType)
      {
        if (this.CheckBufferedActionsForControlType(this.PressActions, controlType))
        {
          BraveInput.PressAction actionForControlType = this.GetPressActionForControlType(controlType);
          if (actionForControlType != null)
          {
            this.m_pressActions.Remove(actionForControlType);
            BraveInput.PressAction.Pool.Free(ref actionForControlType);
          }
          else
            UnityEngine.Debug.LogError((object) $"BraveInput.ConsumeButtonDown(): No action for {controlType.ToString()} was found");
        }
        else if (this.CheckBufferedActionsForControlType(this.HoldActions, controlType))
        {
          BraveInput.HoldAction actionForControlType = this.GetHoldActionForControlType(controlType);
          if (actionForControlType != null)
          {
            actionForControlType.ConsumedDown = true;
          }
          else
          {
            if (MemoryTester.HasInstance)
              return;
            UnityEngine.Debug.LogError((object) $"BraveInput.ConsumeButtonDown(): No action for {controlType.ToString()} was found");
          }
        }
        else
          UnityEngine.Debug.LogError((object) $"BraveInput.ConsumeButtonDown(): {controlType.ToString()} isn't registered with the BraveInput object");
      }

      public bool GetButton(GungeonActions.GungeonActionType controlType)
      {
        if (this.CheckBufferedActionsForControlType(this.HoldActions, controlType))
        {
          BraveInput.HoldAction actionForControlType = this.GetHoldActionForControlType(controlType);
          return actionForControlType != null && actionForControlType.ConsumedDown && actionForControlType.Held;
        }
        if (!MemoryTester.HasInstance)
          UnityEngine.Debug.LogError((object) $"BraveInput.GetButtonDown(): {controlType.ToString()} isn't a registered hold action with the BraveInput object");
        return false;
      }

      public bool GetButtonUp(GungeonActions.GungeonActionType controlType)
      {
        if (this.CheckBufferedActionsForControlType(this.HoldActions, controlType))
        {
          BraveInput.HoldAction actionForControlType = this.GetHoldActionForControlType(controlType);
          return actionForControlType != null && !actionForControlType.Held && actionForControlType.ConsumedDown && !actionForControlType.ConsumedUp;
        }
        if (!MemoryTester.HasInstance)
          UnityEngine.Debug.LogError((object) $"BraveInput.GetButtonDown(): {controlType.ToString()} isn't a registered hold action with the BraveInput object");
        return false;
      }

      public void ConsumeButtonUp(GungeonActions.GungeonActionType controlType)
      {
        if (this.CheckBufferedActionsForControlType(this.HoldActions, controlType))
        {
          BraveInput.HoldAction actionForControlType = this.GetHoldActionForControlType(controlType);
          if (actionForControlType != null)
          {
            actionForControlType.ConsumedUp = true;
          }
          else
          {
            if (MemoryTester.HasInstance)
              return;
            UnityEngine.Debug.LogError((object) $"BraveInput.ConsumeButtonUp(): No action for {controlType.ToString()} was found");
          }
        }
        else
          UnityEngine.Debug.LogError((object) $"BraveInput.ConsumeButtonUp(): {controlType.ToString()} isn't registered with the BraveInput object");
      }

      public static void ConsumeAllAcrossInstances(GungeonActions.GungeonActionType controlType)
      {
        for (int key = 0; key < BraveInput.m_instances.Count; ++key)
          BraveInput.m_instances[key].ConsumeAll(controlType);
      }

      public void ConsumeAll(GungeonActions.GungeonActionType controlType)
      {
        LinkedListNode<BraveInput.PressAction> linkedListNode1 = this.m_pressActions.First;
        while (linkedListNode1 != null)
        {
          LinkedListNode<BraveInput.PressAction> node = linkedListNode1;
          linkedListNode1 = linkedListNode1.Next;
          if (node.Value.Control == controlType)
          {
            BraveInput.PressAction pressAction = node.Value;
            this.m_pressActions.Remove(node, true);
            BraveInput.PressAction.Pool.Free(ref pressAction);
          }
        }
        LinkedListNode<BraveInput.HoldAction> linkedListNode2 = this.m_holdActions.First;
        while (linkedListNode2 != null)
        {
          LinkedListNode<BraveInput.HoldAction> node = linkedListNode2;
          linkedListNode2 = linkedListNode2.Next;
          if (node.Value.Control == controlType && !node.Value.ConsumedDown)
          {
            BraveInput.HoldAction holdAction = node.Value;
            this.m_holdActions.Remove(node, true);
            BraveInput.HoldAction.Pool.Free(ref holdAction);
          }
        }
      }

      public static GameOptions.ControllerSymbology PlayerOneCurrentSymbology
      {
        get => BraveInput.GetCurrentSymbology(0);
      }

      public static GameOptions.ControllerSymbology PlayerTwoCurrentSymbology
      {
        get => BraveInput.GetCurrentSymbology(1);
      }

      public bool MenuInteractPressed
      {
        get
        {
          if (this.ActiveActions == null)
            return false;
          return this.ActiveActions.InteractAction.WasPressed || this.ActiveActions.MenuSelectAction.WasPressed;
        }
      }

      public bool WasAdvanceDialoguePressed(out bool suppressThisClick)
      {
        suppressThisClick = false;
        if (this.MenuInteractPressed)
          return true;
        if (!this.IsKeyboardAndMouse())
          return false;
        if (!Input.GetMouseButtonDown(0))
          return Input.GetKeyDown(KeyCode.Return);
        suppressThisClick = true;
        return true;
      }

      public bool WasAdvanceDialoguePressed()
      {
        if (this.MenuInteractPressed)
          return true;
        if (!this.IsKeyboardAndMouse())
          return false;
        return Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Return);
      }

      public static GameOptions.ControllerSymbology GetCurrentSymbology(int id)
      {
        GameOptions.ControllerSymbology currentSymbology = id != 0 ? GameManager.Options.PlayerTwoPreferredSymbology : GameManager.Options.PlayerOnePreferredSymbology;
        if (currentSymbology == GameOptions.ControllerSymbology.AutoDetect)
        {
          BraveInput instanceForPlayer = BraveInput.GetInstanceForPlayer(id);
          if ((UnityEngine.Object) instanceForPlayer != (UnityEngine.Object) null && !instanceForPlayer.IsKeyboardAndMouse())
          {
            InputDevice device = instanceForPlayer.ActiveActions.Device;
            if (device != null)
              currentSymbology = device.ControllerSymbology;
          }
        }
        if (currentSymbology == GameOptions.ControllerSymbology.AutoDetect)
          currentSymbology = GameOptions.ControllerSymbology.Xbox;
        return currentSymbology;
      }

      public static bool WasSelectPressed(InputDevice device = null)
      {
        if (device == null)
          device = InputManager.ActiveDevice;
        return device.Action1.WasPressed || GameManager.HasInstance && GameManager.Options.allowUnknownControllers && BraveInput.GetInstanceForPlayer(0).ActiveActions.MenuSelectAction.WasPressed;
      }

      public static bool WasCancelPressed(InputDevice device = null)
      {
        if (device == null)
          device = InputManager.ActiveDevice;
        return device.Action2.WasPressed || GameManager.HasInstance && GameManager.Options.allowUnknownControllers && BraveInput.GetInstanceForPlayer(0).ActiveActions.CancelAction.WasPressed;
      }

      public enum AutoAim
      {
        AutoAim,
        SuperAutoAim,
      }

      [Serializable]
      public class BufferedInput
      {
        public GungeonActions.GungeonActionType Control;
        public float BufferTime = 0.3f;
      }

      public class PressAction
      {
        public float Timer;
        private BraveInput.BufferedInput m_bufferedInput;
        public static ObjectPool<BraveInput.PressAction> Pool;

        private PressAction()
        {
        }

        public float Buffer => this.m_bufferedInput.BufferTime;

        public GungeonActions.GungeonActionType Control => this.m_bufferedInput.Control;

        public void SetAll(BraveInput.BufferedInput bufferedInput)
        {
          this.m_bufferedInput = bufferedInput;
          this.Timer = 0.0f;
        }

        public static void Cleanup(BraveInput.PressAction pressAction)
        {
          pressAction.m_bufferedInput = (BraveInput.BufferedInput) null;
        }

        static PressAction()
        {
          ObjectPool<BraveInput.PressAction>.Factory factory = (ObjectPool<BraveInput.PressAction>.Factory) (() => new BraveInput.PressAction());
          // ISSUE: reference to a compiler-generated field
          if (BraveInput.PressAction.<>f__mg$cache0 == null)
          {
            // ISSUE: reference to a compiler-generated field
            BraveInput.PressAction.<>f__mg$cache0 = new ObjectPool<BraveInput.PressAction>.Cleanup(BraveInput.PressAction.Cleanup);
          }
          // ISSUE: reference to a compiler-generated field
          ObjectPool<BraveInput.PressAction>.Cleanup fMgCache0 = BraveInput.PressAction.<>f__mg$cache0;
          BraveInput.PressAction.Pool = new ObjectPool<BraveInput.PressAction>(factory, 10, fMgCache0);
        }
      }

      public class HoldAction
      {
        public float DownTimer;
        public float UpTimer;
        public bool Held = true;
        public bool ConsumedDown;
        public bool ConsumedUp;
        private BraveInput.BufferedInput m_bufferedInput;
        public static ObjectPool<BraveInput.HoldAction> Pool;

        private HoldAction()
        {
        }

        public float Buffer => this.m_bufferedInput.BufferTime;

        public GungeonActions.GungeonActionType Control => this.m_bufferedInput.Control;

        public void SetAll(BraveInput.BufferedInput bufferedInput)
        {
          this.m_bufferedInput = bufferedInput;
          this.DownTimer = 0.0f;
          this.UpTimer = 0.0f;
          this.Held = true;
          this.ConsumedDown = false;
          this.ConsumedUp = false;
        }

        public static void Cleanup(BraveInput.HoldAction holdAction)
        {
          holdAction.m_bufferedInput = (BraveInput.BufferedInput) null;
        }

        static HoldAction()
        {
          ObjectPool<BraveInput.HoldAction>.Factory factory = (ObjectPool<BraveInput.HoldAction>.Factory) (() => new BraveInput.HoldAction());
          // ISSUE: reference to a compiler-generated field
          if (BraveInput.HoldAction.<>f__mg$cache0 == null)
          {
            // ISSUE: reference to a compiler-generated field
            BraveInput.HoldAction.<>f__mg$cache0 = new ObjectPool<BraveInput.HoldAction>.Cleanup(BraveInput.HoldAction.Cleanup);
          }
          // ISSUE: reference to a compiler-generated field
          ObjectPool<BraveInput.HoldAction>.Cleanup fMgCache0 = BraveInput.HoldAction.<>f__mg$cache0;
          BraveInput.HoldAction.Pool = new ObjectPool<BraveInput.HoldAction>(factory, 10, fMgCache0);
        }
      }

      private class TimedVibration
      {
        public float timer;
        public float largeMotor;
        public float smallMotor;

        public TimedVibration(float timer, float intensity)
        {
          this.timer = timer;
          this.largeMotor = intensity;
          this.smallMotor = intensity;
        }

        public TimedVibration(float timer, float largeMotor, float smallMotor)
        {
          this.timer = timer;
          this.largeMotor = largeMotor;
          this.smallMotor = smallMotor;
        }
      }
    }

}
