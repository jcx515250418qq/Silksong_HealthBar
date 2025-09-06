using UnityEngine;

public class OverrideNeedolinLoop : MonoBehaviour
{
	[SerializeField]
	private AudioSource syncToSource;

	[SerializeField]
	private AudioClip needolinClip;

	[SerializeField]
	private TrackTriggerObjects heroRange;

	[SerializeField]
	private bool dontLoop;

	[SerializeField]
	private bool reverseSyncDirection;

	private AudioSource lastSource;

	private static OverrideNeedolinLoop _current;

	public AudioClip NeedolinClip
	{
		get
		{
			return needolinClip;
		}
		set
		{
			needolinClip = value;
		}
	}

	public bool DoSync { get; set; } = true;

	public static bool IsOverridden
	{
		get
		{
			if ((bool)_current)
			{
				if (!_current.reverseSyncDirection && (bool)_current.syncToSource)
				{
					return _current.syncToSource.isPlaying;
				}
				return true;
			}
			return false;
		}
	}

	private void Awake()
	{
		heroRange.InsideStateChanged += OnHeroRangeStateChanged;
	}

	private void OnEnable()
	{
		OnHeroRangeStateChanged(heroRange.IsInside);
	}

	private void OnDisable()
	{
		OnHeroRangeStateChanged(heroRange.IsInside);
	}

	private void OnDestroy()
	{
		heroRange.InsideStateChanged -= OnHeroRangeStateChanged;
		if (_current == this)
		{
			_current = null;
		}
	}

	private void OnHeroRangeStateChanged(bool isInside)
	{
		if (!base.isActiveAndEnabled)
		{
			isInside = false;
		}
		if (isInside)
		{
			_current = this;
		}
		else if (_current == this)
		{
			_current = null;
		}
	}

	public float GetTimeLeft()
	{
		float length = lastSource.clip.length;
		float num = length - lastSource.time;
		if (dontLoop)
		{
			return num;
		}
		float num2 = length * 0.5f;
		if (num > num2)
		{
			num -= num2;
		}
		return num;
	}

	public static void StartSyncedAudio(AudioSource targetSource, AudioClip defaultClip)
	{
		targetSource.volume = 1f;
		targetSource.loop = true;
		if (IsOverridden)
		{
			_current.lastSource = targetSource;
			bool flag = false;
			if (targetSource.clip != null && (bool)_current.syncToSource && _current.syncToSource.clip != null)
			{
				flag = targetSource.clip.frequency != _current.syncToSource.clip.frequency;
			}
			if (!targetSource.isPlaying || targetSource.clip != _current.NeedolinClip)
			{
				targetSource.clip = _current.NeedolinClip;
				targetSource.loop = !_current.dontLoop;
				targetSource.Play();
			}
			if (_current.reverseSyncDirection)
			{
				if ((bool)_current.syncToSource)
				{
					if (!flag)
					{
						_current.syncToSource.timeSamples = targetSource.timeSamples;
					}
					else
					{
						_current.syncToSource.time = targetSource.time;
					}
					if (!_current.syncToSource.isPlaying)
					{
						_current.syncToSource.volume = 0f;
						_current.syncToSource.Play();
					}
				}
			}
			else if (_current.DoSync && (bool)_current.syncToSource)
			{
				if (!flag)
				{
					targetSource.timeSamples = _current.syncToSource.timeSamples;
				}
				else
				{
					targetSource.time = _current.syncToSource.time;
				}
			}
		}
		else if (!targetSource.isPlaying || targetSource.clip != defaultClip)
		{
			targetSource.clip = defaultClip;
			targetSource.Play();
		}
	}

	public void SetReverseSync()
	{
		reverseSyncDirection = true;
	}
}
