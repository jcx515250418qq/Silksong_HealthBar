using System.Collections;
using HutongGames;
using HutongGames.PlayMaker;
using UnityEngine;

public class TickedPendulum : MonoBehaviour
{
	private enum TickState
	{
		None = 0,
		OnStart = 1,
		OnEnabled = 2
	}

	[SerializeField]
	private EventRegister tickRegister;

	[SerializeField]
	private float swingTime = 1f;

	[SerializeField]
	private Transform spikeRotate;

	[SerializeField]
	private AudioSource audioPrefab;

	[SerializeField]
	private RandomAudioClipTable outSound;

	[SerializeField]
	private RandomAudioClipTable inSound;

	[Space]
	[SerializeField]
	private bool playSound = true;

	[Space]
	[SerializeField]
	private TickState tickState;

	private bool ticking;

	private bool tickQueued;

	private float angleStart;

	private float angleEnd;

	private float initialParentX;

	private double tickTime;

	private Transform mainCamera;

	private bool started;

	private Coroutine tickRoutine;

	private void Awake()
	{
		PlayMakerFSM component = GetComponent<PlayMakerFSM>();
		if ((bool)component)
		{
			FsmBool fsmBool = component.FsmVariables.FindFsmBool("Dont Play Sound");
			if (fsmBool != null)
			{
				playSound = !fsmBool.Value;
				Object.Destroy(component);
			}
		}
		tickRegister.ReceivedEvent += Ticked;
		angleStart = base.transform.eulerAngles.z;
		angleEnd = Helper.GetReflectedAngle(angleStart, reflectHorizontal: true, reflectVertical: false, disallowNegative: true);
		initialParentX = spikeRotate.parent.position.x;
		UpdateSpikeRotation();
		ResetPosition();
	}

	private void Start()
	{
		mainCamera = GameCameras.instance.mainCamera.transform;
		started = true;
		if (tickState == TickState.OnStart)
		{
			AutoTick();
		}
		StartTick();
	}

	private void OnEnable()
	{
		if (tickState == TickState.OnEnabled)
		{
			AutoTick();
		}
		if (started)
		{
			if (ticking)
			{
				tickQueued = true;
			}
			ResetPosition();
			StartTick();
		}
	}

	private void ResetPosition()
	{
		Vector3 eulerAngles = base.transform.eulerAngles;
		eulerAngles.z = angleStart;
		base.transform.eulerAngles = eulerAngles;
	}

	private void UpdateSpikeRotation()
	{
		float num = spikeRotate.position.x - initialParentX;
		num *= 2f;
		spikeRotate.SetLocalRotation2D(num);
	}

	private void AutoTick()
	{
		if (!ticking)
		{
			Ticked();
			tickTime = Time.timeAsDouble + (double)(swingTime * 0.4f);
		}
	}

	private void Ticked()
	{
		if (!(Time.timeAsDouble < tickTime))
		{
			tickQueued = true;
		}
	}

	private void PlaySound(bool isIn)
	{
		if (playSound)
		{
			Vector3 position = mainCamera.position;
			float num = Vector3.Distance(position, base.transform.position);
			float num2 = Vector3.Distance(position, spikeRotate.position);
			if (!(num > 45f) || !(num2 > 45f))
			{
				(isIn ? inSound : outSound).SpawnAndPlayOneShot(audioPrefab, base.transform.position);
			}
		}
	}

	private void StartTick()
	{
		if (tickRoutine != null)
		{
			StopCoroutine(tickRoutine);
		}
		tickRoutine = StartCoroutine(TickRoutine());
	}

	private IEnumerator TickRoutine()
	{
		while (true)
		{
			if (!tickQueued)
			{
				yield return null;
				continue;
			}
			tickQueued = false;
			ticking = true;
			PlaySound(outSound);
			float startRotation = base.transform.eulerAngles.z;
			float elapsed;
			for (elapsed = 0f; elapsed < swingTime; elapsed += Time.deltaTime)
			{
				if (tickQueued)
				{
					break;
				}
				float rotation = EasingFunction.EaseInOutQuad(startRotation, angleEnd, elapsed / swingTime);
				base.transform.SetRotation2D(rotation);
				UpdateSpikeRotation();
				yield return null;
			}
			ticking = false;
			while (!tickQueued)
			{
				yield return null;
			}
			tickQueued = false;
			ticking = true;
			PlaySound(outSound);
			elapsed = base.transform.eulerAngles.z;
			for (startRotation = 0f; startRotation < swingTime; startRotation += Time.deltaTime)
			{
				if (tickQueued)
				{
					break;
				}
				float rotation2 = EasingFunction.EaseInOutQuad(elapsed, angleStart, startRotation / swingTime);
				base.transform.SetRotation2D(rotation2);
				UpdateSpikeRotation();
				yield return null;
			}
			ticking = false;
		}
	}
}
