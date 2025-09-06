using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Device)]
	[Tooltip("Projects the location found with Get Location Info to a 2d map using common projections.")]
	public class ProjectLocationToMap : FsmStateAction
	{
		public enum MapProjection
		{
			EquidistantCylindrical = 0,
			Mercator = 1
		}

		[Tooltip("Location vector in degrees longitude and latitude. Typically returned by the {{Get Location Info}} action.")]
		public FsmVector3 GPSLocation;

		[Tooltip("The projection used by the map.")]
		public MapProjection mapProjection;

		[ActionSection("Map Region")]
		[HasFloatSlider(-180f, 180f)]
		[Tooltip("The minimum Longitude shown on the map.")]
		public FsmFloat minLongitude;

		[HasFloatSlider(-180f, 180f)]
		[Tooltip("The maximum Longitude show on the map.")]
		public FsmFloat maxLongitude;

		[HasFloatSlider(-90f, 90f)]
		[Tooltip("The minimum Latitude shown on the map.")]
		public FsmFloat minLatitude;

		[HasFloatSlider(-90f, 90f)]
		[Tooltip("The maximum Latitude shown on the map.")]
		public FsmFloat maxLatitude;

		[ActionSection("Screen Region")]
		[Tooltip("The screen coordinate of the left edge of the map image.")]
		public FsmFloat minX;

		[Tooltip("The screen coordinate of the top edge of the map image.")]
		public FsmFloat minY;

		[Tooltip("The width of the map image in screen coordinates.")]
		public FsmFloat width;

		[Tooltip("The height of the map in screen coordinates.")]
		public FsmFloat height;

		[ActionSection("Projection")]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the projected X coordinate in a Float Variable. Use this to display a marker on the map.")]
		public FsmFloat projectedX;

		[UIHint(UIHint.Variable)]
		[Tooltip("Store the projected Y coordinate in a Float Variable. Use this to display a marker on the map.")]
		public FsmFloat projectedY;

		[Tooltip("If true all coordinates in this action are normalized (0-1); otherwise coordinates are in pixels.")]
		public FsmBool normalized;

		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		private float x;

		private float y;

		public override void Reset()
		{
			GPSLocation = new FsmVector3
			{
				UseVariable = true
			};
			mapProjection = MapProjection.EquidistantCylindrical;
			minLongitude = -180f;
			maxLongitude = 180f;
			minLatitude = -90f;
			maxLatitude = 90f;
			minX = 0f;
			minY = 0f;
			width = 1f;
			height = 1f;
			normalized = true;
			projectedX = null;
			projectedY = null;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			if (GPSLocation.IsNone)
			{
				Finish();
				return;
			}
			DoProjectGPSLocation();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoProjectGPSLocation();
		}

		private void DoProjectGPSLocation()
		{
			x = Mathf.Clamp(GPSLocation.Value.x, minLongitude.Value, maxLongitude.Value);
			y = Mathf.Clamp(GPSLocation.Value.y, minLatitude.Value, maxLatitude.Value);
			switch (mapProjection)
			{
			case MapProjection.EquidistantCylindrical:
				DoEquidistantCylindrical();
				break;
			case MapProjection.Mercator:
				DoMercatorProjection();
				break;
			}
			x *= width.Value;
			y *= height.Value;
			projectedX.Value = (normalized.Value ? (minX.Value + x) : (minX.Value + x * (float)Screen.width));
			projectedY.Value = (normalized.Value ? (minY.Value + y) : (minY.Value + y * (float)Screen.height));
		}

		private void DoEquidistantCylindrical()
		{
			x = (x - minLongitude.Value) / (maxLongitude.Value - minLongitude.Value);
			y = (y - minLatitude.Value) / (maxLatitude.Value - minLatitude.Value);
		}

		private void DoMercatorProjection()
		{
			x = (x - minLongitude.Value) / (maxLongitude.Value - minLongitude.Value);
			float num = LatitudeToMercator(minLatitude.Value);
			float num2 = LatitudeToMercator(maxLatitude.Value);
			y = (LatitudeToMercator(GPSLocation.Value.y) - num) / (num2 - num);
		}

		private static float LatitudeToMercator(float latitudeInDegrees)
		{
			float num = Mathf.Clamp(latitudeInDegrees, -85f, 85f);
			num = MathF.PI / 180f * num;
			return Mathf.Log(Mathf.Tan(num / 2f + MathF.PI / 4f));
		}
	}
}
