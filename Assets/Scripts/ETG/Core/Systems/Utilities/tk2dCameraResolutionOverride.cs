using System;

using UnityEngine;

#nullable disable

[Serializable]
public class tk2dCameraResolutionOverride
    {
        public string name;
        public tk2dCameraResolutionOverride.MatchByType matchBy;
        public int width;
        public int height;
        public float aspectRatioNumerator = 4f;
        public float aspectRatioDenominator = 3f;
        public float scale = 1f;
        public Vector2 offsetPixels = new Vector2(0.0f, 0.0f);
        public tk2dCameraResolutionOverride.AutoScaleMode autoScaleMode;
        public tk2dCameraResolutionOverride.FitMode fitMode;

        public bool Match(int pixelWidth, int pixelHeight)
        {
            switch (this.matchBy)
            {
                case tk2dCameraResolutionOverride.MatchByType.Resolution:
                    return pixelWidth == this.width && pixelHeight == this.height;
                case tk2dCameraResolutionOverride.MatchByType.AspectRatio:
                    return Mathf.Approximately((float) pixelWidth * this.aspectRatioDenominator / this.aspectRatioNumerator, (float) pixelHeight);
                case tk2dCameraResolutionOverride.MatchByType.Wildcard:
                    return true;
                default:
                    return false;
            }
        }

        public void Upgrade(int version)
        {
            if (version != 0)
                return;
            this.matchBy = this.width == -1 && this.height == -1 || this.width == 0 && this.height == 0 ? tk2dCameraResolutionOverride.MatchByType.Wildcard : tk2dCameraResolutionOverride.MatchByType.Resolution;
        }

        public static tk2dCameraResolutionOverride DefaultOverride
        {
            get
            {
                return new tk2dCameraResolutionOverride()
                {
                    name = "Override",
                    matchBy = tk2dCameraResolutionOverride.MatchByType.Wildcard,
                    autoScaleMode = tk2dCameraResolutionOverride.AutoScaleMode.FitVisible,
                    fitMode = tk2dCameraResolutionOverride.FitMode.Center
                };
            }
        }

        public enum MatchByType
        {
            Resolution,
            AspectRatio,
            Wildcard,
        }

        public enum AutoScaleMode
        {
            None,
            FitWidth,
            FitHeight,
            FitVisible,
            StretchToFit,
            ClosestMultipleOfTwo,
            PixelPerfect,
            Fill,
        }

        public enum FitMode
        {
            Constant,
            Center,
        }
    }

