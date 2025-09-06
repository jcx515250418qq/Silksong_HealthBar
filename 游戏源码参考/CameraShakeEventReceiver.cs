using UnityEngine;

public class CameraShakeEventReceiver : EventBase
{
	[SerializeField]
	private CameraManagerReference cameraReference;

	[SerializeField]
	private float radius;

	[SerializeField]
	private Vector2 offset;

	[Space]
	[SerializeField]
	private CameraShakeWorldForceIntensities minIntensity = CameraShakeWorldForceIntensities.Medium;

	[SerializeField]
	private CameraShakeWorldForceIntensities maxIntensity = CameraShakeWorldForceIntensities.Intense;

	public override string InspectorInfo => $"{minIntensity.ToString()} - {maxIntensity.ToString()}";

	private void OnDrawGizmosSelected()
	{
		if (radius > 0f)
		{
			Gizmos.color = Color.cyan;
			Vector3 original = base.transform.TransformPoint(offset);
			float? z = 0f;
			Gizmos.DrawWireSphere(original.Where(null, null, z), radius);
		}
	}

	private void OnValidate()
	{
		if (minIntensity > maxIntensity)
		{
			maxIntensity = minIntensity;
		}
		if (radius < 0f)
		{
			radius = 0f;
		}
	}

	private void Start()
	{
		if ((bool)cameraReference)
		{
			cameraReference.CameraShakedWorldForce += OnCameraShaked;
		}
	}

	private void OnDestroy()
	{
		if ((bool)cameraReference)
		{
			cameraReference.CameraShakedWorldForce -= OnCameraShaked;
		}
	}

	private void OnCameraShaked(Vector2 cameraPosition, CameraShakeWorldForceIntensities intensity)
	{
		if (intensity >= minIntensity && intensity <= maxIntensity && (!(radius > 0f) || !(Vector2.SqrMagnitude((Vector2)base.transform.TransformPoint(offset) - cameraPosition) > radius * radius)))
		{
			CallReceivedEvent();
		}
	}
}
