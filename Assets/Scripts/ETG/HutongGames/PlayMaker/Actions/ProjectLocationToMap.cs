using System;

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Device)]
    [HutongGames.PlayMaker.Tooltip("Projects the location found with Get Location Info to a 2d map using common projections.")]
    public class ProjectLocationToMap : FsmStateAction
    {
        [HutongGames.PlayMaker.Tooltip("Location vector in degrees longitude and latitude. Typically returned by the Get Location Info action.")]
        public FsmVector3 GPSLocation;
        [HutongGames.PlayMaker.Tooltip("The projection used by the map.")]
        public ProjectLocationToMap.MapProjection mapProjection;
        [HasFloatSlider(-180f, 180f)]
        [ActionSection("Map Region")]
        public FsmFloat minLongitude;
        [HasFloatSlider(-180f, 180f)]
        public FsmFloat maxLongitude;
        [HasFloatSlider(-90f, 90f)]
        public FsmFloat minLatitude;
        [HasFloatSlider(-90f, 90f)]
        public FsmFloat maxLatitude;
        [ActionSection("Screen Region")]
        public FsmFloat minX;
        public FsmFloat minY;
        public FsmFloat width;
        public FsmFloat height;
        [ActionSection("Projection")]
        [HutongGames.PlayMaker.Tooltip("Store the projected X coordinate in a Float Variable. Use this to display a marker on the map.")]
        [UIHint(UIHint.Variable)]
        public FsmFloat projectedX;
        [HutongGames.PlayMaker.Tooltip("Store the projected Y coordinate in a Float Variable. Use this to display a marker on the map.")]
        [UIHint(UIHint.Variable)]
        public FsmFloat projectedY;
        [HutongGames.PlayMaker.Tooltip("If true all coordinates in this action are normalized (0-1); otherwise coordinates are in pixels.")]
        public FsmBool normalized;
        public bool everyFrame;
        private float x;
        private float y;

        public override void Reset()
        {
            FsmVector3 fsmVector3 = new FsmVector3();
            fsmVector3.UseVariable = true;
            this.GPSLocation = fsmVector3;
            this.mapProjection = ProjectLocationToMap.MapProjection.EquidistantCylindrical;
            this.minLongitude = (FsmFloat) -180f;
            this.maxLongitude = (FsmFloat) 180f;
            this.minLatitude = (FsmFloat) -90f;
            this.maxLatitude = (FsmFloat) 90f;
            this.minX = (FsmFloat) 0.0f;
            this.minY = (FsmFloat) 0.0f;
            this.width = (FsmFloat) 1f;
            this.height = (FsmFloat) 1f;
            this.normalized = (FsmBool) true;
            this.projectedX = (FsmFloat) null;
            this.projectedY = (FsmFloat) null;
            this.everyFrame = false;
        }

        public override void OnEnter()
        {
            if (this.GPSLocation.IsNone)
            {
                this.Finish();
            }
            else
            {
                this.DoProjectGPSLocation();
                if (this.everyFrame)
                    return;
                this.Finish();
            }
        }

        public override void OnUpdate() => this.DoProjectGPSLocation();

        private void DoProjectGPSLocation()
        {
            this.x = Mathf.Clamp(this.GPSLocation.Value.x, this.minLongitude.Value, this.maxLongitude.Value);
            this.y = Mathf.Clamp(this.GPSLocation.Value.y, this.minLatitude.Value, this.maxLatitude.Value);
            switch (this.mapProjection)
            {
                case ProjectLocationToMap.MapProjection.EquidistantCylindrical:
                    this.DoEquidistantCylindrical();
                    break;
                case ProjectLocationToMap.MapProjection.Mercator:
                    this.DoMercatorProjection();
                    break;
            }
            this.x *= this.width.Value;
            this.y *= this.height.Value;
            this.projectedX.Value = !this.normalized.Value ? this.minX.Value + this.x * (float) Screen.width : this.minX.Value + this.x;
            this.projectedY.Value = !this.normalized.Value ? this.minY.Value + this.y * (float) Screen.height : this.minY.Value + this.y;
        }

        private void DoEquidistantCylindrical()
        {
            this.x = (float) (((double) this.x - (double) this.minLongitude.Value) / ((double) this.maxLongitude.Value - (double) this.minLongitude.Value));
            this.y = (float) (((double) this.y - (double) this.minLatitude.Value) / ((double) this.maxLatitude.Value - (double) this.minLatitude.Value));
        }

        private void DoMercatorProjection()
        {
            this.x = (float) (((double) this.x - (double) this.minLongitude.Value) / ((double) this.maxLongitude.Value - (double) this.minLongitude.Value));
            float mercator1 = ProjectLocationToMap.LatitudeToMercator(this.minLatitude.Value);
            float mercator2 = ProjectLocationToMap.LatitudeToMercator(this.maxLatitude.Value);
            this.y = (float) (((double) ProjectLocationToMap.LatitudeToMercator(this.GPSLocation.Value.y) - (double) mercator1) / ((double) mercator2 - (double) mercator1));
        }

        private static float LatitudeToMercator(float latitudeInDegrees)
        {
            return Mathf.Log(Mathf.Tan((float) ((double) ((float) Math.PI / 180f * Mathf.Clamp(latitudeInDegrees, -85f, 85f)) / 2.0 + 0.78539818525314331)));
        }

        public enum MapProjection
        {
            EquidistantCylindrical,
            Mercator,
        }
    }
}
