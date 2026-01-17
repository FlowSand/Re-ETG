// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.SendMessage
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HutongGames.PlayMaker.Tooltip("Sends a Message to a Game Object. See Unity docs for SendMessage.")]
[ActionCategory(ActionCategory.ScriptControl)]
public class SendMessage : FsmStateAction
{
  [RequiredField]
  [HutongGames.PlayMaker.Tooltip("GameObject that sends the message.")]
  public FsmOwnerDefault gameObject;
  [HutongGames.PlayMaker.Tooltip("Where to send the message.\nSee Unity docs.")]
  public SendMessage.MessageType delivery;
  [HutongGames.PlayMaker.Tooltip("Send options.\nSee Unity docs.")]
  public SendMessageOptions options;
  [RequiredField]
  public FunctionCall functionCall;

  public override void Reset()
  {
    this.gameObject = (FsmOwnerDefault) null;
    this.delivery = SendMessage.MessageType.SendMessage;
    this.options = SendMessageOptions.DontRequireReceiver;
    this.functionCall = (FunctionCall) null;
  }

  public override void OnEnter()
  {
    this.DoSendMessage();
    this.Finish();
  }

  private void DoSendMessage()
  {
    GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
    if ((Object) ownerDefaultTarget == (Object) null)
      return;
    object parameter = (object) null;
    string parameterType = this.functionCall.ParameterType;
    if (parameterType != null)
    {
      // ISSUE: reference to a compiler-generated field
      if (SendMessage.<>f__switch$map1 == null)
      {
        // ISSUE: reference to a compiler-generated field
        SendMessage.<>f__switch$map1 = new Dictionary<string, int>(16 /*0x10*/)
        {
          {
            "None",
            0
          },
          {
            "bool",
            1
          },
          {
            "int",
            2
          },
          {
            "float",
            3
          },
          {
            "string",
            4
          },
          {
            "Vector2",
            5
          },
          {
            "Vector3",
            6
          },
          {
            "Rect",
            7
          },
          {
            "GameObject",
            8
          },
          {
            "Material",
            9
          },
          {
            "Texture",
            10
          },
          {
            "Color",
            11
          },
          {
            "Quaternion",
            12
          },
          {
            "Object",
            13
          },
          {
            "Enum",
            14
          },
          {
            "Array",
            15
          }
        };
      }
      int num;
      // ISSUE: reference to a compiler-generated field
      if (SendMessage.<>f__switch$map1.TryGetValue(parameterType, out num))
      {
        switch (num)
        {
          case 1:
            parameter = (object) this.functionCall.BoolParameter.Value;
            break;
          case 2:
            parameter = (object) this.functionCall.IntParameter.Value;
            break;
          case 3:
            parameter = (object) this.functionCall.FloatParameter.Value;
            break;
          case 4:
            parameter = (object) this.functionCall.StringParameter.Value;
            break;
          case 5:
            parameter = (object) this.functionCall.Vector2Parameter.Value;
            break;
          case 6:
            parameter = (object) this.functionCall.Vector3Parameter.Value;
            break;
          case 7:
            parameter = (object) this.functionCall.RectParamater.Value;
            break;
          case 8:
            parameter = (object) this.functionCall.GameObjectParameter.Value;
            break;
          case 9:
            parameter = (object) this.functionCall.MaterialParameter.Value;
            break;
          case 10:
            parameter = (object) this.functionCall.TextureParameter.Value;
            break;
          case 11:
            parameter = (object) this.functionCall.ColorParameter.Value;
            break;
          case 12:
            parameter = (object) this.functionCall.QuaternionParameter.Value;
            break;
          case 13:
            parameter = (object) this.functionCall.ObjectParameter.Value;
            break;
          case 14:
            parameter = (object) this.functionCall.EnumParameter.Value;
            break;
          case 15:
            parameter = (object) this.functionCall.ArrayParameter.Values;
            break;
        }
      }
    }
    switch (this.delivery)
    {
      case SendMessage.MessageType.SendMessage:
        ownerDefaultTarget.SendMessage(this.functionCall.FunctionName, parameter, this.options);
        break;
      case SendMessage.MessageType.SendMessageUpwards:
        ownerDefaultTarget.SendMessageUpwards(this.functionCall.FunctionName, parameter, this.options);
        break;
      case SendMessage.MessageType.BroadcastMessage:
        ownerDefaultTarget.BroadcastMessage(this.functionCall.FunctionName, parameter, this.options);
        break;
    }
  }

  public enum MessageType
  {
    SendMessage,
    SendMessageUpwards,
    BroadcastMessage,
  }
}
