using System.Collections.Generic;
using UnityEngine;

public sealed class VolumeBlendController : MonoBehaviour
{
	[Range(0f, 1f)]
	[SerializeField]
	private float baseVolume = 1f;

	[SerializeField]
	private AudioSource audioSource;

	private const string FSM_SHARED_KEY = "FSM_SHARED_KEY";

	private Dictionary<string, VolumeModifier> modifiers = new Dictionary<string, VolumeModifier>(4);

	private float cachedProduct = 1f;

	private bool hasAudioSource;

	private float initialVolume = 1f;

	private bool hasInitialized;

	public float InitialVolume
	{
		get
		{
			Init();
			return initialVolume;
		}
	}

	public float FSMSharedVolume
	{
		get
		{
			return GetSharedFSMModifier().Volume;
		}
		set
		{
			GetSharedFSMModifier().Volume = value;
		}
	}

	public void Init()
	{
		if (hasInitialized)
		{
			return;
		}
		hasInitialized = true;
		hasAudioSource = audioSource;
		if (!hasAudioSource)
		{
			audioSource = GetComponent<AudioSource>();
			if (audioSource == null)
			{
				return;
			}
			initialVolume = audioSource.volume;
			hasAudioSource = true;
		}
		UpdateFinalVolume();
	}

	private void Awake()
	{
		Init();
	}

	private void OnValidate()
	{
		if (audioSource == null)
		{
			audioSource = GetComponent<AudioSource>();
		}
	}

	public void AddOrUpdateModifier(string key, float value)
	{
		if (modifiers.TryGetValue(key, out var value2))
		{
			value2.Volume = value;
			return;
		}
		value2 = new VolumeModifier(value);
		value2.OnValueChanged += OnModifierChanged;
		modifiers.Add(key, value2);
		RecalculateCachedProduct();
	}

	public void RemoveModifier(string key)
	{
		if (modifiers.TryGetValue(key, out var value))
		{
			value.OnValueChanged -= OnModifierChanged;
			modifiers.Remove(key);
			value.SetInvalid();
			RecalculateCachedProduct();
		}
	}

	private void OnModifierChanged()
	{
		RecalculateCachedProduct();
	}

	private void RecalculateCachedProduct()
	{
		cachedProduct = 1f;
		foreach (VolumeModifier value in modifiers.Values)
		{
			cachedProduct *= value.Volume;
		}
		UpdateFinalVolume();
	}

	private void UpdateFinalVolume()
	{
		if (hasAudioSource)
		{
			audioSource.volume = baseVolume * cachedProduct;
		}
	}

	public VolumeModifier GetModifier(string key)
	{
		if (!modifiers.TryGetValue(key, out var value))
		{
			value = (modifiers[key] = new VolumeModifier(1f));
			value.OnValueChanged += OnModifierChanged;
		}
		return value;
	}

	public VolumeModifier GetSharedFSMModifier()
	{
		return GetModifier("FSM_SHARED_KEY");
	}

	public void SetSharedFSMModifier(float volume)
	{
		AddOrUpdateModifier("FSM_SHARED_KEY", volume);
	}
}
