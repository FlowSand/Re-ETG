using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Gets info on the last event that caused a state change. See also Set Event Data action.")]
  [ActionCategory(ActionCategory.StateMachine)]
  public class GetEventInfo : FsmStateAction
  {
    [UIHint(UIHint.Variable)]
    public FsmGameObject sentByGameObject;
    [UIHint(UIHint.Variable)]
    public FsmString fsmName;
    [UIHint(UIHint.Variable)]
    public FsmBool getBoolData;
    [UIHint(UIHint.Variable)]
    public FsmInt getIntData;
    [UIHint(UIHint.Variable)]
    public FsmFloat getFloatData;
    [UIHint(UIHint.Variable)]
    public FsmVector2 getVector2Data;
    [UIHint(UIHint.Variable)]
    public FsmVector3 getVector3Data;
    [UIHint(UIHint.Variable)]
    public FsmString getStringData;
    [UIHint(UIHint.Variable)]
    public FsmGameObject getGameObjectData;
    [UIHint(UIHint.Variable)]
    public FsmRect getRectData;
    [UIHint(UIHint.Variable)]
    public FsmQuaternion getQuaternionData;
    [UIHint(UIHint.Variable)]
    public FsmMaterial getMaterialData;
    [UIHint(UIHint.Variable)]
    public FsmTexture getTextureData;
    [UIHint(UIHint.Variable)]
    public FsmColor getColorData;
    [UIHint(UIHint.Variable)]
    public FsmObject getObjectData;

    public override void Reset()
    {
      this.sentByGameObject = (FsmGameObject) null;
      this.fsmName = (FsmString) null;
      this.getBoolData = (FsmBool) null;
      this.getIntData = (FsmInt) null;
      this.getFloatData = (FsmFloat) null;
      this.getVector2Data = (FsmVector2) null;
      this.getVector3Data = (FsmVector3) null;
      this.getStringData = (FsmString) null;
      this.getGameObjectData = (FsmGameObject) null;
      this.getRectData = (FsmRect) null;
      this.getQuaternionData = (FsmQuaternion) null;
      this.getMaterialData = (FsmMaterial) null;
      this.getTextureData = (FsmTexture) null;
      this.getColorData = (FsmColor) null;
      this.getObjectData = (FsmObject) null;
    }

    public override void OnEnter()
    {
      if (Fsm.EventData.SentByFsm != null)
      {
        this.sentByGameObject.Value = Fsm.EventData.SentByFsm.GameObject;
        this.fsmName.Value = Fsm.EventData.SentByFsm.Name;
      }
      else
      {
        this.sentByGameObject.Value = (GameObject) null;
        this.fsmName.Value = string.Empty;
      }
      this.getBoolData.Value = Fsm.EventData.BoolData;
      this.getIntData.Value = Fsm.EventData.IntData;
      this.getFloatData.Value = Fsm.EventData.FloatData;
      this.getVector2Data.Value = Fsm.EventData.Vector2Data;
      this.getVector3Data.Value = Fsm.EventData.Vector3Data;
      this.getStringData.Value = Fsm.EventData.StringData;
      this.getGameObjectData.Value = Fsm.EventData.GameObjectData;
      this.getRectData.Value = Fsm.EventData.RectData;
      this.getQuaternionData.Value = Fsm.EventData.QuaternionData;
      this.getMaterialData.Value = Fsm.EventData.MaterialData;
      this.getTextureData.Value = Fsm.EventData.TextureData;
      this.getColorData.Value = Fsm.EventData.ColorData;
      this.getObjectData.Value = Fsm.EventData.ObjectData;
      this.Finish();
    }
  }
}
