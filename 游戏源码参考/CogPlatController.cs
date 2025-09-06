using UnityEngine;
using UnityEngine.Events;

public class CogPlatController : MonoBehaviour
{
	[Header("Structure")]
	[SerializeField]
	private CogPlatArm[] arms;

	[Header("Parameters")]
	[SerializeField]
	private TimedTicker mainTicker;

	[SerializeField]
	private TimedTicker armTicker;

	[SerializeField]
	private float armRotationDuration;

	[Space]
	public UnityEvent OnTick;

	public UnityEvent OnArmTick;

	public UnityEvent OnArmRotationEnd;

	private bool doRotateArms;

	private float rotateTimeElapsed;

	private bool isRotating;

	private void Start()
	{
		if ((bool)mainTicker)
		{
			mainTicker.ReceivedEvent += TickMain;
		}
		if ((bool)armTicker)
		{
			armTicker.ReceivedEvent += TickArms;
		}
	}

	private void OnDestroy()
	{
		if ((bool)mainTicker)
		{
			mainTicker.ReceivedEvent -= TickMain;
		}
		if ((bool)armTicker)
		{
			armTicker.ReceivedEvent -= TickArms;
		}
	}

	private void Update()
	{
		if (doRotateArms)
		{
			float num = Mathf.Min(armRotationDuration, armTicker.TickDelay);
			float num2 = rotateTimeElapsed / num;
			if (num2 >= 1f)
			{
				num2 = 1f;
				doRotateArms = false;
				EndRotation();
			}
			else
			{
				UpdateRotation(num2);
			}
			rotateTimeElapsed += Time.deltaTime;
		}
	}

	private void TickMain()
	{
		if (base.isActiveAndEnabled)
		{
			OnTick.Invoke();
		}
	}

	private void TickArms()
	{
		if (base.isActiveAndEnabled)
		{
			OnArmTick.Invoke();
			doRotateArms = true;
			rotateTimeElapsed = 0f;
			StartRotation();
		}
	}

	private void StartRotation()
	{
		if (isRotating)
		{
			EndRotation();
		}
		isRotating = true;
		CogPlatArm[] array = arms;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].StartRotation();
		}
	}

	private void UpdateRotation(float time)
	{
		CogPlatArm[] array = arms;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].UpdateRotation(time);
		}
	}

	private void EndRotation()
	{
		OnArmRotationEnd.Invoke();
		CogPlatArm[] array = arms;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].EndRotation();
		}
		isRotating = false;
	}
}
