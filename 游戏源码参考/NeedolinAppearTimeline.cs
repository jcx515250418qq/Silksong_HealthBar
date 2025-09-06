using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

public class NeedolinAppearTimeline : MonoBehaviour
{
	[SerializeField]
	private PlayableDirector director;

	[SerializeField]
	private float maxTime;

	[SerializeField]
	private float minTime;

	[SerializeField]
	private AnimationCurve curve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	[SerializeField]
	private float appearSpeed;

	[SerializeField]
	private float appearAcceleration;

	[SerializeField]
	private float pauseTime;

	[SerializeField]
	private float disappearSpeed;

	[SerializeField]
	private float disappearAcceleration;

	[Space]
	public UnityEvent OnFinish;

	[Space]
	public UnityEvent OnDisappeared;

	private bool isAppearing;

	private float currentTime;

	private float currentSpeed;

	private float pauseTimeLeft;

	private bool isFinished;

	private void Update()
	{
		if (isFinished)
		{
			return;
		}
		if (pauseTimeLeft > 0f)
		{
			pauseTimeLeft -= Time.deltaTime;
			return;
		}
		if (isAppearing)
		{
			currentSpeed += appearAcceleration * Time.deltaTime;
			if (currentSpeed > appearSpeed)
			{
				currentSpeed = appearSpeed;
			}
			currentTime += currentSpeed * Time.deltaTime;
			if (currentTime > maxTime)
			{
				currentTime = maxTime;
			}
		}
		else
		{
			currentSpeed += disappearAcceleration * Time.deltaTime;
			if (currentSpeed > disappearSpeed)
			{
				currentSpeed = disappearSpeed;
			}
			bool num = currentTime > Mathf.Epsilon;
			currentTime -= currentSpeed * Time.deltaTime;
			if (currentTime < 0f)
			{
				currentTime = 0f;
			}
			if (num && currentTime <= Mathf.Epsilon)
			{
				OnDisappeared.Invoke();
			}
		}
		float time = Mathf.Clamp01(currentTime / maxTime);
		time = curve.Evaluate(time);
		director.time = maxTime * time;
		director.Evaluate();
		if (time >= 1f - Mathf.Epsilon)
		{
			isFinished = true;
			OnFinish.Invoke();
		}
	}

	public void StartAppear()
	{
		isAppearing = true;
		currentSpeed = ((currentTime > minTime) ? 0f : appearSpeed);
		pauseTimeLeft = 0f;
	}

	public void CancelAppear()
	{
		isAppearing = false;
		isFinished = false;
		if (currentTime > minTime)
		{
			currentSpeed = 0f;
			pauseTimeLeft = pauseTime;
		}
		else
		{
			currentSpeed = disappearSpeed;
			pauseTimeLeft = 0f;
		}
	}
}
