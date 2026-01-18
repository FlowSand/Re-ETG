using System.Collections.Generic;

using UnityEngine;

#nullable disable

[AddComponentMenu("Wwise/AkBank")]
[ExecuteInEditMode]
public class AkBank : AkUnityEventHandler
    {
        public string bankName = string.Empty;
        public bool decodeBank;
        public bool loadAsynchronous;
        public bool saveDecodedBank;
        public List<int> unloadTriggerList = new List<int>()
        {
            -358577003
        };

        protected override void Awake()
        {
            base.Awake();
            this.RegisterTriggers(this.unloadTriggerList, new AkTriggerBase.Trigger(this.UnloadBank));
            if (!this.unloadTriggerList.Contains(1151176110))
                return;
            this.UnloadBank((GameObject) null);
        }

        protected override void Start()
        {
            base.Start();
            if (!this.unloadTriggerList.Contains(1281810935))
                return;
            this.UnloadBank((GameObject) null);
        }

        public override void HandleEvent(GameObject in_gameObject)
        {
            if (!this.loadAsynchronous)
                AkBankManager.LoadBank(this.bankName, this.decodeBank, this.saveDecodedBank);
            else
                AkBankManager.LoadBankAsync(this.bankName);
        }

        public void UnloadBank(GameObject in_gameObject) => AkBankManager.UnloadBank(this.bankName);

        protected override void OnDestroy()
        {
            base.OnDestroy();
            this.UnregisterTriggers(this.unloadTriggerList, new AkTriggerBase.Trigger(this.UnloadBank));
            if (!this.unloadTriggerList.Contains(-358577003))
                return;
            this.UnloadBank((GameObject) null);
        }
    }

