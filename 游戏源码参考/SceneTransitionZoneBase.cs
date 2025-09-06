using System.Collections;
using GlobalEnums;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public abstract class SceneTransitionZoneBase : TriggerEnterEvent
{
	[Space]
	[SerializeField]
	private Color fadeColor;

	[SerializeField]
	private float fadeDuration;

	[SerializeField]
	private float holdDuration;

	[SerializeField]
	private bool freezeCamera = true;

	[Space]
	[SerializeField]
	private AudioSource audioPrefab;

	[SerializeField]
	private AudioEvent transitionSound;

	[SerializeField]
	private AudioMixerSnapshot musicSnapshot;

	[SerializeField]
	private AudioMixerSnapshot atmosSnapshot;

	[SerializeField]
	private AudioMixerSnapshot actorSnapshot;

	[SerializeField]
	private float snapshotTransitionTime = 2f;

	private Coroutine respawnRoutine;

	protected abstract string TargetScene { get; }

	protected abstract string TargetGate { get; }

	protected virtual void OnPreTransition()
	{
	}

	protected override void Awake()
	{
		base.Awake();
		base.OnTriggerEntered += delegate(Collider2D col, GameObject _)
		{
			if (respawnRoutine == null)
			{
				HeroController component = col.GetComponent<HeroController>();
				if ((bool)component)
				{
					HeroControllerStates cState = component.cState;
					if (!cState.hazardDeath && !cState.hazardRespawning)
					{
						respawnRoutine = StartCoroutine(Respawn(component));
					}
				}
			}
		};
	}

	private IEnumerator Respawn(HeroController hc)
	{
		EventRegister.SendEvent(EventRegisterEvents.FsmCancel);
		ScenePreloader.SpawnPreloader(TargetScene, LoadSceneMode.Additive);
		AudioSource audioSource = transitionSound.SpawnAndPlayOneShot(audioPrefab, base.transform.position);
		if (audioSource != null)
		{
			PlayAudioAndRecycle component = audioSource.GetComponent<PlayAudioAndRecycle>();
			if ((bool)component)
			{
				component.KeepAliveThroughNextScene = true;
			}
		}
		if ((bool)musicSnapshot)
		{
			musicSnapshot.TransitionTo(snapshotTransitionTime);
		}
		if ((bool)atmosSnapshot)
		{
			atmosSnapshot.TransitionTo(snapshotTransitionTime);
		}
		if ((bool)actorSnapshot)
		{
			actorSnapshot.TransitionTo(snapshotTransitionTime);
		}
		hc.RelinquishControlNotVelocity();
		hc.StopAnimationControl();
		if (hc.cState.onGround)
		{
			hc.AffectedByGravity(gravityApplies: false);
		}
		hc.damageMode = DamageMode.NO_DAMAGE;
		if (freezeCamera)
		{
			GameCameras.instance.cameraController.FreezeInPlace(freezeTarget: true);
		}
		Color original = fadeColor;
		float? a = 0f;
		ScreenFaderUtils.Fade(original.Where(null, null, null, a), fadeColor, fadeDuration);
		float elapsedTime = 0f;
		Rigidbody2D body = hc.Body;
		float maxFallVelocity = 0f - hc.GetMaxFallVelocity();
		for (; elapsedTime <= fadeDuration; elapsedTime += Time.deltaTime)
		{
			Vector2 linearVelocity = body.linearVelocity;
			if (linearVelocity.y < maxFallVelocity)
			{
				body.linearVelocity = new Vector2(linearVelocity.x, maxFallVelocity);
			}
			yield return null;
		}
		hc.AffectedByGravity(gravityApplies: false);
		hc.Body.linearVelocity = Vector2.zero;
		hc.StartAnimationControl();
		yield return new WaitForSeconds(holdDuration - fadeDuration);
		OnPreTransition();
		GameManager.instance.BeginSceneTransition(new GameManager.SceneLoadInfo
		{
			PreventCameraFadeOut = true,
			EntryGateName = TargetGate,
			SceneName = TargetScene,
			Visualization = GameManager.SceneLoadVisualizations.Default
		});
	}
}
