using UnityEngine;
using UnityEngine.Audio;

public abstract class SnapshotMarker : MonoBehaviour
{
	[SerializeField]
	private float maxIntensityRadius;

	[SerializeField]
	private float cutoffRadius;

	[SerializeField]
	private AnimationCurve blendCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	[Space]
	[SerializeField]
	private AudioMixerSnapshot snapshot;

	[SerializeField]
	private float transitionTime = 0.2f;

	private bool hasStarted;

	public AudioMixerSnapshot Snapshot => snapshot;

	public float TransitionTime => transitionTime;

	private void OnDrawGizmosSelected()
	{
		Vector3 position = base.transform.position;
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(position, maxIntensityRadius);
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(position, cutoffRadius);
		if (Application.isPlaying)
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireSphere(position, Mathf.Lerp(cutoffRadius, maxIntensityRadius, GetBlendAmountRaw()));
			Gizmos.color = Color.cyan;
			Gizmos.DrawWireSphere(position, Mathf.Lerp(cutoffRadius, maxIntensityRadius, GetBlendAmount()));
		}
	}

	private void OnEnable()
	{
		if (hasStarted)
		{
			AddMarker();
		}
	}

	private void Start()
	{
		hasStarted = true;
		AddMarker();
	}

	private void OnDisable()
	{
		RemoveMarker();
	}

	public float GetBlendAmount()
	{
		float blendAmountRaw = GetBlendAmountRaw();
		return blendCurve.Evaluate(blendAmountRaw);
	}

	private float GetBlendAmountRaw()
	{
		Vector2 vector = (Vector2)GameCameras.instance.mainCamera.transform.position - (Vector2)base.transform.position;
		float num = maxIntensityRadius * maxIntensityRadius;
		float num2 = cutoffRadius * cutoffRadius - num;
		float num3 = (vector.sqrMagnitude - num) / num2;
		return Mathf.Clamp01(1f - num3);
	}

	protected abstract void AddMarker();

	protected abstract void RemoveMarker();
}
