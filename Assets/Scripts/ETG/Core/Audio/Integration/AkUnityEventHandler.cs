using System.Collections.Generic;
using UnityEngine;

#nullable disable

  public abstract class AkUnityEventHandler : MonoBehaviour
  {
    public const int AWAKE_TRIGGER_ID = 1151176110;
    public const int START_TRIGGER_ID = 1281810935;
    public const int DESTROY_TRIGGER_ID = -358577003;
    public const int MAX_NB_TRIGGERS = 32 /*0x20*/;
    public static Dictionary<uint, string> triggerTypes = AkTriggerBase.GetAllDerivedTypes();
    private bool didDestroy;
    public List<int> triggerList = new List<int>()
    {
      1281810935
    };
    public bool useOtherObject;

    public abstract void HandleEvent(GameObject in_gameObject);

    protected virtual void Awake()
    {
      this.RegisterTriggers(this.triggerList, new AkTriggerBase.Trigger(this.HandleEvent));
      if (!this.triggerList.Contains(1151176110))
        return;
      this.HandleEvent((GameObject) null);
    }

    protected virtual void Start()
    {
      if (!this.triggerList.Contains(1281810935))
        return;
      this.HandleEvent((GameObject) null);
    }

    protected virtual void OnDestroy()
    {
      if (this.didDestroy)
        return;
      this.DoDestroy();
    }

    public void DoDestroy()
    {
      this.UnregisterTriggers(this.triggerList, new AkTriggerBase.Trigger(this.HandleEvent));
      this.didDestroy = true;
      if (!this.triggerList.Contains(-358577003))
        return;
      this.HandleEvent((GameObject) null);
    }

    protected void RegisterTriggers(List<int> in_triggerList, AkTriggerBase.Trigger in_delegate)
    {
      foreach (uint inTrigger in in_triggerList)
      {
        string empty = string.Empty;
        if (AkUnityEventHandler.triggerTypes.TryGetValue(inTrigger, out empty) && !(empty == "Awake") && !(empty == "Start") && !(empty == "Destroy"))
        {
          AkTriggerBase akTriggerBase = (AkTriggerBase) this.GetComponent(System.Type.GetType(empty));
          if ((UnityEngine.Object) akTriggerBase == (UnityEngine.Object) null)
            akTriggerBase = (AkTriggerBase) this.gameObject.AddComponent(System.Type.GetType(empty));
          akTriggerBase.triggerDelegate += in_delegate;
        }
      }
    }

    protected void UnregisterTriggers(List<int> in_triggerList, AkTriggerBase.Trigger in_delegate)
    {
      foreach (uint inTrigger in in_triggerList)
      {
        string empty = string.Empty;
        if (AkUnityEventHandler.triggerTypes.TryGetValue(inTrigger, out empty) && !(empty == "Awake") && !(empty == "Start") && !(empty == "Destroy"))
        {
          AkTriggerBase component = (AkTriggerBase) this.GetComponent(System.Type.GetType(empty));
          if ((UnityEngine.Object) component != (UnityEngine.Object) null)
          {
            component.triggerDelegate -= in_delegate;
            if (component.triggerDelegate == null)
              UnityEngine.Object.Destroy((UnityEngine.Object) component);
          }
        }
      }
    }
  }

