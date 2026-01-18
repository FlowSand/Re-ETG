using System;
using System.IO;

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Application)]
    [HutongGames.PlayMaker.Tooltip("Saves a Screenshot. NOTE: Does nothing in Web Player. On Android, the resulting screenshot is available some time later.")]
    public class TakeScreenshot : FsmStateAction
    {
        [HutongGames.PlayMaker.Tooltip("Where to save the screenshot.")]
        public TakeScreenshot.Destination destination;
        [HutongGames.PlayMaker.Tooltip("Path used with Custom Path Destination option.")]
        public FsmString customPath;
        [RequiredField]
        public FsmString filename;
        [HutongGames.PlayMaker.Tooltip("Add an auto-incremented number to the filename.")]
        public FsmBool autoNumber;
        [HutongGames.PlayMaker.Tooltip("Factor by which to increase resolution.")]
        public FsmInt superSize;
        [HutongGames.PlayMaker.Tooltip("Log saved file info in Unity console.")]
        public FsmBool debugLog;
        private int screenshotCount;

        public override void Reset()
        {
            this.destination = TakeScreenshot.Destination.MyPictures;
            this.filename = (FsmString) string.Empty;
            this.autoNumber = (FsmBool) null;
            this.superSize = (FsmInt) null;
            this.debugLog = (FsmBool) null;
        }

        public override void OnEnter()
        {
            if (string.IsNullOrEmpty(this.filename.Value))
                return;
            string str1;
            switch (this.destination)
            {
                case TakeScreenshot.Destination.MyPictures:
                    str1 = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                    break;
                case TakeScreenshot.Destination.PersistentDataPath:
                    str1 = Application.persistentDataPath;
                    break;
                case TakeScreenshot.Destination.CustomPath:
                    str1 = this.customPath.Value;
                    break;
                default:
                    str1 = string.Empty;
                    break;
            }
            string str2 = str1.Replace("\\", "/") + "/";
            string str3 = $"{str2}{this.filename.Value}.png";
            object[] objArray;
            if (this.autoNumber.Value)
            {
                for (; File.Exists(str3); str3 = string.Concat(objArray))
                {
                    ++this.screenshotCount;
                    objArray = new object[4]
                    {
                        (object) str2,
                        (object) this.filename.Value,
                        (object) this.screenshotCount,
                        (object) ".png"
                    };
                }
            }
            if (this.debugLog.Value)
                Debug.Log((object) ("TakeScreenshot: " + str3));
            ScreenCapture.CaptureScreenshot(str3, this.superSize.Value);
            this.Finish();
        }

        public enum Destination
        {
            MyPictures,
            PersistentDataPath,
            CustomPath,
        }
    }
}
