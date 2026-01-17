// Decompiled with JetBrains decompiler
// Type: MultiplayerBasicExample.PlayerManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using InControl;
using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace MultiplayerBasicExample;

public class PlayerManager : MonoBehaviour
{
  public GameObject playerPrefab;
  private const int maxPlayers = 4;
  private List<Vector3> playerPositions = new List<Vector3>()
  {
    new Vector3(-1f, 1f, -10f),
    new Vector3(1f, 1f, -10f),
    new Vector3(-1f, -1f, -10f),
    new Vector3(1f, -1f, -10f)
  };
  private List<Player> players = new List<Player>(4);

  private void Start()
  {
    InputManager.OnDeviceDetached += new Action<InputDevice>(this.OnDeviceDetached);
  }

  private void Update()
  {
    InputDevice activeDevice = InputManager.ActiveDevice;
    if (!this.JoinButtonWasPressedOnDevice(activeDevice) || !this.ThereIsNoPlayerUsingDevice(activeDevice))
      return;
    this.CreatePlayer(activeDevice);
  }

  private bool JoinButtonWasPressedOnDevice(InputDevice inputDevice)
  {
    return inputDevice.Action1.WasPressed || inputDevice.Action2.WasPressed || inputDevice.Action3.WasPressed || inputDevice.Action4.WasPressed;
  }

  private Player FindPlayerUsingDevice(InputDevice inputDevice)
  {
    int count = this.players.Count;
    for (int index = 0; index < count; ++index)
    {
      Player player = this.players[index];
      if (player.Device == inputDevice)
        return player;
    }
    return (Player) null;
  }

  private bool ThereIsNoPlayerUsingDevice(InputDevice inputDevice)
  {
    return (UnityEngine.Object) this.FindPlayerUsingDevice(inputDevice) == (UnityEngine.Object) null;
  }

  private void OnDeviceDetached(InputDevice inputDevice)
  {
    Player playerUsingDevice = this.FindPlayerUsingDevice(inputDevice);
    if (!((UnityEngine.Object) playerUsingDevice != (UnityEngine.Object) null))
      return;
    this.RemovePlayer(playerUsingDevice);
  }

  private Player CreatePlayer(InputDevice inputDevice)
  {
    if (this.players.Count >= 4)
      return (Player) null;
    Vector3 playerPosition = this.playerPositions[0];
    this.playerPositions.RemoveAt(0);
    Player component = UnityEngine.Object.Instantiate<GameObject>(this.playerPrefab, playerPosition, Quaternion.identity).GetComponent<Player>();
    component.Device = inputDevice;
    this.players.Add(component);
    return component;
  }

  private void RemovePlayer(Player player)
  {
    this.playerPositions.Insert(0, player.transform.position);
    this.players.Remove(player);
    player.Device = (InputDevice) null;
    UnityEngine.Object.Destroy((UnityEngine.Object) player.gameObject);
  }

  private void OnGUI()
  {
    float y1 = 10f;
    GUI.Label(new Rect(10f, y1, 300f, y1 + 22f), $"Active players: {(object) this.players.Count}/{(object) 4}");
    float y2 = y1 + 22f;
    if (this.players.Count >= 4)
      return;
    GUI.Label(new Rect(10f, y2, 300f, y2 + 22f), "Press a button to join!");
    float num = y2 + 22f;
  }
}
