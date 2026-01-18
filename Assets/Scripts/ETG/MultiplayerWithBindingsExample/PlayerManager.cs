using System;
using System.Collections.Generic;

using InControl;
using UnityEngine;

#nullable disable
namespace MultiplayerWithBindingsExample
{
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
        private PlayerActions keyboardListener;
        private PlayerActions joystickListener;

        private void OnEnable()
        {
            InputManager.OnDeviceDetached += new Action<InputDevice>(this.OnDeviceDetached);
            this.keyboardListener = PlayerActions.CreateWithKeyboardBindings();
            this.joystickListener = PlayerActions.CreateWithJoystickBindings();
        }

        private void OnDisable()
        {
            InputManager.OnDeviceDetached -= new Action<InputDevice>(this.OnDeviceDetached);
            this.joystickListener.Destroy();
            this.keyboardListener.Destroy();
        }

        private void Update()
        {
            if (this.JoinButtonWasPressedOnListener(this.joystickListener))
            {
                InputDevice activeDevice = InputManager.ActiveDevice;
                if (this.ThereIsNoPlayerUsingJoystick(activeDevice))
                    this.CreatePlayer(activeDevice);
            }
            if (!this.JoinButtonWasPressedOnListener(this.keyboardListener) || !this.ThereIsNoPlayerUsingKeyboard())
                return;
            this.CreatePlayer((InputDevice) null);
        }

        private bool JoinButtonWasPressedOnListener(PlayerActions actions)
        {
            return actions.Green.WasPressed || actions.Red.WasPressed || actions.Blue.WasPressed || actions.Yellow.WasPressed;
        }

        private Player FindPlayerUsingJoystick(InputDevice inputDevice)
        {
            int count = this.players.Count;
            for (int index = 0; index < count; ++index)
            {
                Player player = this.players[index];
                if (player.Actions.Device == inputDevice)
                    return player;
            }
            return (Player) null;
        }

        private bool ThereIsNoPlayerUsingJoystick(InputDevice inputDevice)
        {
            return (UnityEngine.Object) this.FindPlayerUsingJoystick(inputDevice) == (UnityEngine.Object) null;
        }

        private Player FindPlayerUsingKeyboard()
        {
            int count = this.players.Count;
            for (int index = 0; index < count; ++index)
            {
                Player player = this.players[index];
                if (player.Actions == this.keyboardListener)
                    return player;
            }
            return (Player) null;
        }

        private bool ThereIsNoPlayerUsingKeyboard()
        {
            return (UnityEngine.Object) this.FindPlayerUsingKeyboard() == (UnityEngine.Object) null;
        }

        private void OnDeviceDetached(InputDevice inputDevice)
        {
            Player playerUsingJoystick = this.FindPlayerUsingJoystick(inputDevice);
            if (!((UnityEngine.Object) playerUsingJoystick != (UnityEngine.Object) null))
                return;
            this.RemovePlayer(playerUsingJoystick);
        }

        private Player CreatePlayer(InputDevice inputDevice)
        {
            if (this.players.Count >= 4)
                return (Player) null;
            Vector3 playerPosition = this.playerPositions[0];
            this.playerPositions.RemoveAt(0);
            Player component = UnityEngine.Object.Instantiate<GameObject>(this.playerPrefab, playerPosition, Quaternion.identity).GetComponent<Player>();
            if (inputDevice == null)
            {
                component.Actions = this.keyboardListener;
            }
            else
            {
                PlayerActions joystickBindings = PlayerActions.CreateWithJoystickBindings();
                joystickBindings.Device = inputDevice;
                component.Actions = joystickBindings;
            }
            this.players.Add(component);
            return component;
        }

        private void RemovePlayer(Player player)
        {
            this.playerPositions.Insert(0, player.transform.position);
            this.players.Remove(player);
            player.Actions = (PlayerActions) null;
            UnityEngine.Object.Destroy((UnityEngine.Object) player.gameObject);
        }

        private void OnGUI()
        {
            float y1 = 10f;
            GUI.Label(new Rect(10f, y1, 300f, y1 + 22f), $"Active players: {(object) this.players.Count}/{(object) 4}");
            float y2 = y1 + 22f;
            if (this.players.Count >= 4)
                return;
            GUI.Label(new Rect(10f, y2, 300f, y2 + 22f), "Press a button or a/s/d/f key to join!");
            float num = y2 + 22f;
        }
    }
}
