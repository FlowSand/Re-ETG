// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.SetEventData
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[Tooltip("Sets Event Data before sending an event. Get the Event Data, along with sender information, using Get Event Info action.")]
[ActionCategory(ActionCategory.StateMachine)]
public class SetEventData : FsmStateAction
{
  public FsmGameObject setGameObjectData;
  public FsmInt setIntData;
  public FsmFloat setFloatData;
  public FsmString setStringData;
  public FsmBool setBoolData;
  public FsmVector2 setVector2Data;
  public FsmVector3 setVector3Data;
  public FsmRect setRectData;
  public FsmQuaternion setQuaternionData;
  public FsmColor setColorData;
  public FsmMaterial setMaterialData;
  public FsmTexture setTextureData;
  public FsmObject setObjectData;

  public override void Reset()
  {
    FsmGameObject fsmGameObject = new FsmGameObject();
    fsmGameObject.UseVariable = true;
    this.setGameObjectData = fsmGameObject;
    FsmInt fsmInt = new FsmInt();
    fsmInt.UseVariable = true;
    this.setIntData = fsmInt;
    FsmFloat fsmFloat = new FsmFloat();
    fsmFloat.UseVariable = true;
    this.setFloatData = fsmFloat;
    FsmString fsmString = new FsmString();
    fsmString.UseVariable = true;
    this.setStringData = fsmString;
    FsmBool fsmBool = new FsmBool();
    fsmBool.UseVariable = true;
    this.setBoolData = fsmBool;
    FsmVector2 fsmVector2 = new FsmVector2();
    fsmVector2.UseVariable = true;
    this.setVector2Data = fsmVector2;
    FsmVector3 fsmVector3 = new FsmVector3();
    fsmVector3.UseVariable = true;
    this.setVector3Data = fsmVector3;
    FsmRect fsmRect = new FsmRect();
    fsmRect.UseVariable = true;
    this.setRectData = fsmRect;
    FsmQuaternion fsmQuaternion = new FsmQuaternion();
    fsmQuaternion.UseVariable = true;
    this.setQuaternionData = fsmQuaternion;
    FsmColor fsmColor = new FsmColor();
    fsmColor.UseVariable = true;
    this.setColorData = fsmColor;
    FsmMaterial fsmMaterial = new FsmMaterial();
    fsmMaterial.UseVariable = true;
    this.setMaterialData = fsmMaterial;
    FsmTexture fsmTexture = new FsmTexture();
    fsmTexture.UseVariable = true;
    this.setTextureData = fsmTexture;
    FsmObject fsmObject = new FsmObject();
    fsmObject.UseVariable = true;
    this.setObjectData = fsmObject;
  }

  public override void OnEnter()
  {
    Fsm.EventData.BoolData = this.setBoolData.Value;
    Fsm.EventData.IntData = this.setIntData.Value;
    Fsm.EventData.FloatData = this.setFloatData.Value;
    Fsm.EventData.Vector2Data = this.setVector2Data.Value;
    Fsm.EventData.Vector3Data = this.setVector3Data.Value;
    Fsm.EventData.StringData = this.setStringData.Value;
    Fsm.EventData.GameObjectData = this.setGameObjectData.Value;
    Fsm.EventData.RectData = this.setRectData.Value;
    Fsm.EventData.QuaternionData = this.setQuaternionData.Value;
    Fsm.EventData.ColorData = this.setColorData.Value;
    Fsm.EventData.MaterialData = this.setMaterialData.Value;
    Fsm.EventData.TextureData = this.setTextureData.Value;
    Fsm.EventData.ObjectData = this.setObjectData.Value;
    this.Finish();
  }
}
