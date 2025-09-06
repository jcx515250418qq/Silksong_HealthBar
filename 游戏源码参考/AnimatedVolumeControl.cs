using UnityEngine;

public sealed class AnimatedVolumeControl : MonoBehaviour
{
	[SerializeField]
	private AudioSource audioSource;

	[SerializeField]
	[Range(0f, 1f)]
	private float volume = 1f;

	[SerializeField]
	private float maxChangeRate = 5f;

	private float currentValue;

	private VolumeModifier volumeModifier;

	private void Awake()
	{
		VolumeBlendController component = base.gameObject.GetComponent<VolumeBlendController>();
		if ((bool)component)
		{
			volumeModifier = component.GetModifier("AnimatedVolumeControl");
		}
	}

	private void OnValidate()
	{
		if (audioSource == null)
		{
			audioSource = GetComponent<AudioSource>();
		}
		if (!Application.isPlaying && (bool)audioSource)
		{
			audioSource.volume = volume;
		}
	}

	private void OnEnable()
	{
		if ((bool)audioSource)
		{
			currentValue = audioSource.volume;
		}
	}

	private void LateUpdate()
	{
		if (!Mathf.Approximately(currentValue, volume))
		{
			currentValue = Mathf.MoveTowards(currentValue, volume, maxChangeRate * Time.deltaTime);
			SetVolume(currentValue);
		}
		else
		{
			base.enabled = false;
		}
	}

	private void OnDidApplyAnimationProperties()
	{
		if (!Mathf.Approximately(currentValue, volume))
		{
			base.enabled = true;
		}
		else
		{
			SetVolume(volume);
		}
	}

	public void SetTargetAlpha(float targetAlpha)
	{
		volume = Mathf.Clamp01(targetAlpha);
		if (!Mathf.Approximately(currentValue, volume))
		{
			base.enabled = true;
		}
	}

	private void SetVolume(float alpha)
	{
		if (volumeModifier != null)
		{
			volumeModifier.Volume = (currentValue = alpha);
		}
		else if ((bool)audioSource)
		{
			audioSource.volume = (currentValue = alpha);
		}
		else
		{
			base.enabled = false;
		}
	}
}
