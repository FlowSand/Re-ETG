using UnityEngine;

#nullable disable

public class HelpBoxAttribute : PropertyAttribute
    {
        public string Message;

        public HelpBoxAttribute(string message) => this.Message = message;
    }

