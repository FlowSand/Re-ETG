using System;

using UnityEngine;

#nullable disable
namespace FullInspector.Internal
{
    public static class fiCommentUtility
    {
        public static int GetCommentHeight(string comment, CommentType commentType)
        {
            int val2 = 38;
            if (commentType == CommentType.None)
                val2 = 17;
            return Math.Max((int) ((GUIStyle) "HelpBox").CalcHeight(new GUIContent(comment), (float) Screen.width), val2);
        }
    }
}
