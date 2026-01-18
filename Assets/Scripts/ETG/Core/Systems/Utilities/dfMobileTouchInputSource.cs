using System.Collections.Generic;

using UnityEngine;

#nullable disable

public class dfMobileTouchInputSource : IDFTouchInputSource
    {
        private static dfMobileTouchInputSource instance;
        private List<dfTouchInfo> activeTouches = new List<dfTouchInfo>();

        public static dfMobileTouchInputSource Instance
        {
            get
            {
                if (dfMobileTouchInputSource.instance == null)
                    dfMobileTouchInputSource.instance = new dfMobileTouchInputSource();
                return dfMobileTouchInputSource.instance;
            }
        }

        public int TouchCount => Input.touchCount;

        public IList<dfTouchInfo> Touches => (IList<dfTouchInfo>) this.activeTouches;

        public dfTouchInfo GetTouch(int index) => (dfTouchInfo) Input.GetTouch(index);

        public void Update()
        {
            this.activeTouches.Clear();
            for (int index = 0; index < this.TouchCount; ++index)
                this.activeTouches.Add(this.GetTouch(index));
        }
    }

