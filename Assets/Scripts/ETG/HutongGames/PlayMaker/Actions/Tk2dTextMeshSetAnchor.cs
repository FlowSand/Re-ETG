// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.Tk2dTextMeshSetAnchor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[ActionCategory("2D Toolkit/TextMesh")]
[HutongGames.PlayMaker.Tooltip("Set the anchor of a TextMesh. \nChanges will not be updated if commit is OFF. This is so you can change multiple parameters without reconstructing the mesh repeatedly.\n Use tk2dtextMeshCommit or set commit to true on your last change for that mesh. \nNOTE: The Game Object must have a tk2dTextMesh attached.")]
public class Tk2dTextMeshSetAnchor : FsmStateAction
{
  [CheckForComponent(typeof (tk2dTextMesh))]
  [RequiredField]
  [HutongGames.PlayMaker.Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dTextMesh component attached.")]
  public FsmOwnerDefault gameObject;
  [HutongGames.PlayMaker.Tooltip("The anchor")]
  public TextAnchor textAnchor;
  [UIHint(UIHint.FsmString)]
  [HutongGames.PlayMaker.Tooltip("The anchor as a string (text Anchor setting will be ignore if set). \npossible values ( case insensitive): LowerLeft,LowerCenter,LowerRight,MiddleLeft,MiddleCenter,MiddleRight,UpperLeft,UpperCenter or UpperRight ")]
  public FsmString OrTextAnchorString;
  [UIHint(UIHint.FsmBool)]
  [HutongGames.PlayMaker.Tooltip("Commit changes")]
  public FsmBool commit;
  private tk2dTextMesh _textMesh;

  private void _getTextMesh()
  {
    GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
    if ((Object) ownerDefaultTarget == (Object) null)
      return;
    this._textMesh = ownerDefaultTarget.GetComponent<tk2dTextMesh>();
  }

  public override void Reset()
  {
    this.gameObject = (FsmOwnerDefault) null;
    this.textAnchor = TextAnchor.LowerLeft;
    this.OrTextAnchorString = (FsmString) string.Empty;
    this.commit = (FsmBool) true;
  }

  public override void OnEnter()
  {
    this._getTextMesh();
    this.DoSetAnchor();
    this.Finish();
  }

  private void DoSetAnchor()
  {
    if ((Object) this._textMesh == (Object) null)
    {
      this.LogWarning("Missing tk2dTextMesh component: " + this._textMesh.gameObject.name);
    }
    else
    {
      bool flag = false;
      TextAnchor textAnchor = this.textAnchor;
      if (this.OrTextAnchorString.Value != string.Empty)
      {
        bool isValid = false;
        TextAnchor anchorFromString = this.getTextAnchorFromString(this.OrTextAnchorString.Value, out isValid);
        if (isValid)
          textAnchor = anchorFromString;
      }
      if (this._textMesh.anchor != textAnchor)
      {
        this._textMesh.anchor = textAnchor;
        flag = true;
      }
      if (!this.commit.Value || !flag)
        return;
      this._textMesh.Commit();
    }
  }

  public override string ErrorCheck()
  {
    if (this.OrTextAnchorString.Value != string.Empty)
    {
      bool isValid = false;
      int anchorFromString = (int) this.getTextAnchorFromString(this.OrTextAnchorString.Value, out isValid);
      if (!isValid)
        return $"Text Anchor string '{this.OrTextAnchorString.Value}' is not valid. Use (case insensitive): LowerLeft,LowerCenter,LowerRight,MiddleLeft,MiddleCenter,MiddleRight,UpperLeft,UpperCenter or UpperRight";
    }
    return (string) null;
  }

  private TextAnchor getTextAnchorFromString(string textAnchorString, out bool isValid)
  {
    isValid = true;
    string lower = textAnchorString.ToLower();
    if (lower != null)
    {
      // ISSUE: reference to a compiler-generated field
      if (Tk2dTextMeshSetAnchor._f__switch_map3 == null)
      {
        // ISSUE: reference to a compiler-generated field
        Tk2dTextMeshSetAnchor._f__switch_map3 = new Dictionary<string, int>(9)
        {
          {
            "lowerleft",
            0
          },
          {
            "lowercenter",
            1
          },
          {
            "lowerright",
            2
          },
          {
            "middleleft",
            3
          },
          {
            "middlecenter",
            4
          },
          {
            "middleright",
            5
          },
          {
            "upperleft",
            6
          },
          {
            "uppercenter",
            7
          },
          {
            "upperright",
            8
          }
        };
      }
      int num;
      // ISSUE: reference to a compiler-generated field
      if (Tk2dTextMeshSetAnchor._f__switch_map3.TryGetValue(lower, out num))
      {
        switch (num)
        {
          case 0:
            return TextAnchor.LowerLeft;
          case 1:
            return TextAnchor.LowerCenter;
          case 2:
            return TextAnchor.LowerRight;
          case 3:
            return TextAnchor.MiddleLeft;
          case 4:
            return TextAnchor.MiddleCenter;
          case 5:
            return TextAnchor.MiddleRight;
          case 6:
            return TextAnchor.UpperLeft;
          case 7:
            return TextAnchor.UpperCenter;
          case 8:
            return TextAnchor.UpperRight;
        }
      }
    }
    isValid = false;
    return TextAnchor.LowerLeft;
  }
}
