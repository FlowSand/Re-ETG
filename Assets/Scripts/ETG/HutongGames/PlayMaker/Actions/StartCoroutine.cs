// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.StartCoroutine
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[ActionCategory(ActionCategory.ScriptControl)]
[HutongGames.PlayMaker.Tooltip("Start a Coroutine in a Behaviour on a Game Object. See Unity StartCoroutine docs.")]
public class StartCoroutine : FsmStateAction
{
  [RequiredField]
  [HutongGames.PlayMaker.Tooltip("The game object that owns the Behaviour.")]
  public FsmOwnerDefault gameObject;
  [HutongGames.PlayMaker.Tooltip("The Behaviour that contains the method to start as a coroutine.")]
  [UIHint(UIHint.Behaviour)]
  [RequiredField]
  public FsmString behaviour;
  [UIHint(UIHint.Coroutine)]
  [HutongGames.PlayMaker.Tooltip("The name of the coroutine method.")]
  [RequiredField]
  public FunctionCall functionCall;
  [HutongGames.PlayMaker.Tooltip("Stop the coroutine when the state is exited.")]
  public bool stopOnExit;
  private MonoBehaviour component;

  public override void Reset()
  {
    this.gameObject = (FsmOwnerDefault) null;
    this.behaviour = (FsmString) null;
    this.functionCall = (FunctionCall) null;
    this.stopOnExit = false;
  }

  public override void OnEnter()
  {
    this.DoStartCoroutine();
    this.Finish();
  }

  private void DoStartCoroutine()
  {
    GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
    if ((Object) ownerDefaultTarget == (Object) null)
      return;
    this.component = ownerDefaultTarget.GetComponent(ReflectionUtils.GetGlobalType(this.behaviour.Value)) as MonoBehaviour;
    if ((Object) this.component == (Object) null)
    {
      this.LogWarning($"StartCoroutine: {ownerDefaultTarget.name} missing behaviour: {this.behaviour.Value}");
    }
    else
    {
      string parameterType = this.functionCall.ParameterType;
      if (parameterType == null)
        return;
      // ISSUE: reference to a compiler-generated field
      if (HutongGames.PlayMaker.Actions.StartCoroutine.\u003C\u003Ef__switch\u0024map2 == null)
      {
        // ISSUE: reference to a compiler-generated field
        HutongGames.PlayMaker.Actions.StartCoroutine.\u003C\u003Ef__switch\u0024map2 = new Dictionary<string, int>(13)
        {
          {
            "None",
            0
          },
          {
            "int",
            1
          },
          {
            "float",
            2
          },
          {
            "string",
            3
          },
          {
            "bool",
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
            "Quaternion",
            11
          },
          {
            "Object",
            12
          }
        };
      }
      int num;
      // ISSUE: reference to a compiler-generated field
      if (!HutongGames.PlayMaker.Actions.StartCoroutine.\u003C\u003Ef__switch\u0024map2.TryGetValue(parameterType, out num))
        return;
      switch (num)
      {
        case 0:
          this.component.StartCoroutine(this.functionCall.FunctionName);
          break;
        case 1:
          this.component.StartCoroutine(this.functionCall.FunctionName, (object) this.functionCall.IntParameter.Value);
          break;
        case 2:
          this.component.StartCoroutine(this.functionCall.FunctionName, (object) this.functionCall.FloatParameter.Value);
          break;
        case 3:
          this.component.StartCoroutine(this.functionCall.FunctionName, (object) this.functionCall.StringParameter.Value);
          break;
        case 4:
          this.component.StartCoroutine(this.functionCall.FunctionName, (object) this.functionCall.BoolParameter.Value);
          break;
        case 5:
          this.component.StartCoroutine(this.functionCall.FunctionName, (object) this.functionCall.Vector2Parameter.Value);
          break;
        case 6:
          this.component.StartCoroutine(this.functionCall.FunctionName, (object) this.functionCall.Vector3Parameter.Value);
          break;
        case 7:
          this.component.StartCoroutine(this.functionCall.FunctionName, (object) this.functionCall.RectParamater.Value);
          break;
        case 8:
          this.component.StartCoroutine(this.functionCall.FunctionName, (object) this.functionCall.GameObjectParameter.Value);
          break;
        case 9:
          this.component.StartCoroutine(this.functionCall.FunctionName, (object) this.functionCall.MaterialParameter.Value);
          break;
        case 10:
          this.component.StartCoroutine(this.functionCall.FunctionName, (object) this.functionCall.TextureParameter.Value);
          break;
        case 11:
          this.component.StartCoroutine(this.functionCall.FunctionName, (object) this.functionCall.QuaternionParameter.Value);
          break;
        case 12:
          this.component.StartCoroutine(this.functionCall.FunctionName, (object) this.functionCall.ObjectParameter.Value);
          break;
      }
    }
  }

  public override void OnExit()
  {
    if ((Object) this.component == (Object) null || !this.stopOnExit)
      return;
    this.component.StopCoroutine(this.functionCall.FunctionName);
  }
}
