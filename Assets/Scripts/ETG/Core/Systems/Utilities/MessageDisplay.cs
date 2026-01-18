using System;
using System.Collections.Generic;

using UnityEngine;

#nullable disable

[AddComponentMenu("Daikon Forge/Examples/Actionbar/Message Scroller")]
public class MessageDisplay : MonoBehaviour
    {
        private const float TIME_BEFORE_FADE = 3f;
        private const float FADE_TIME = 2f;
        private List<MessageDisplay.MessageInfo> messages = new List<MessageDisplay.MessageInfo>();
        private dfLabel lblTemplate;

        public void AddMessage(string text)
        {
            if ((UnityEngine.Object) this.lblTemplate == (UnityEngine.Object) null)
                return;
            for (int index = 0; index < this.messages.Count; ++index)
            {
                dfLabel label = this.messages[index].label;
                dfLabel dfLabel = label;
                dfLabel.RelativePosition = dfLabel.RelativePosition + new Vector3(0.0f, -label.Height);
            }
            GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.lblTemplate.gameObject);
            gameObject.transform.parent = this.transform;
            gameObject.transform.position = this.lblTemplate.transform.position;
            gameObject.name = "Message" + (object) this.messages.Count;
            dfLabel component = gameObject.GetComponent<dfLabel>();
            component.Text = text;
            component.IsVisible = true;
            this.messages.Add(new MessageDisplay.MessageInfo()
            {
                label = component,
                startTime = UnityEngine.Time.realtimeSinceStartup
            });
        }

        public void onSpellActivated(SpellDefinition spell) => this.AddMessage("You cast " + spell.Name);

        private void OnClick(dfControl sender, dfMouseEventArgs args)
        {
            this.AddMessage("New test message added to the list at " + (object) DateTime.Now);
            args.Use();
        }

        private void OnEnable()
        {
        }

        private void Start()
        {
            this.lblTemplate = this.GetComponentInChildren<dfLabel>();
            this.lblTemplate.IsVisible = false;
        }

        private void Update()
        {
            for (int index = this.messages.Count - 1; index >= 0; --index)
            {
                MessageDisplay.MessageInfo message = this.messages[index];
                float num1 = UnityEngine.Time.realtimeSinceStartup - message.startTime;
                if ((double) num1 >= 3.0)
                {
                    if ((double) num1 >= 5.0)
                    {
                        this.messages.RemoveAt(index);
                        UnityEngine.Object.Destroy((UnityEngine.Object) message.label.gameObject);
                    }
                    else
                    {
                        float num2 = (float) (1.0 - ((double) num1 - 3.0) / 2.0);
                        message.label.Opacity = num2;
                    }
                }
            }
        }

        private class MessageInfo
        {
            public dfLabel label;
            public float startTime;
        }
    }

