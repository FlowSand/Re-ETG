using UnityEngine;

#nullable disable

[AddComponentMenu("Daikon Forge/Examples/Rich Text/Rich Text Events")]
public class rtSampleEvents : MonoBehaviour
    {
        public void OnLinkClicked(dfRichTextLabel sender, dfMarkupTagAnchor tag)
        {
            string href = tag.HRef;
            if (!href.ToLowerInvariant().StartsWith("http:"))
                return;
            Application.OpenURL(href);
        }
    }

