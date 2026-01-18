using System;

#nullable disable

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field)]
public class dfHelpAttribute : Attribute
    {
        public dfHelpAttribute(string url) => this.HelpURL = url;

        public string HelpURL { get; private set; }
    }

