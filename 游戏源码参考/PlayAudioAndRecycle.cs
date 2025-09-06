using UnityEngine;

public class PlayAudioAndRecycle : MonoBehaviour
{
	[SerializeField]
	private AudioSource audioSource;

	private AudioSourceGamePause pauseHandler;

	private static readonly UniqueList<PlayAudioAndRecycle> activeList = new UniqueList<PlayAudioAndRecycle>();

	private int skipFrames;

	private float recycleTime;

	public bool KeepAliveThroughNextScene { get; set; }

	private void Awake()
	{
		pauseHandler = GetComponent<AudioSourceGamePause>();
	}

	private void OnEnable()
	{
		skipFrames = 2;
		if ((bool)audioSource.clip)
		{
			audioSource.Play();
		}
		activeList.Add(this);
	}

	private void OnDisable()
	{
		KeepAliveThroughNextScene = false;
		activeList.Remove(this);
	}

	private void Update()
	{
		if (skipFrames > 0)
		{
			skipFrames--;
		}
		else if (!audioSource.isPlaying && (!pauseHandler || !pauseHandler.IsPaused) && Time.realtimeSinceStartup > recycleTime)
		{
			Recycle();
		}
	}

	private void NextScene()
	{
		if (KeepAliveThroughNextScene)
		{
			KeepAliveThroughNextScene = false;
		}
		else
		{
			Recycle();
		}
	}

	public static void RecycleActiveRecyclers()
	{
		activeList.ReserveListUsage();
		foreach (PlayAudioAndRecycle item in activeList.List)
		{
			item.NextScene();
		}
		activeList.ReleaseListUsage();
	}

	private void Recycle()
	{
		base.gameObject.Recycle();
	}

	public void SetRecycleTime(float time)
	{
		recycleTime = time;
	}
}
