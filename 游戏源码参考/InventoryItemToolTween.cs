using System;
using System.Collections;
using UnityEngine;

public class InventoryItemToolTween : MonoBehaviour
{
	[SerializeField]
	private SpriteRenderer spriteRenderer;

	[SerializeField]
	private Animator animator;

	[SerializeField]
	private ParticleSystem trailParticles;

	[SerializeField]
	private SpriteRenderer[] toolColoured;

	[Space]
	[SerializeField]
	private AnimationCurve moveCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

	[SerializeField]
	private float moveDuration = 0.1f;

	[SerializeField]
	private CameraShakeTarget placeShake;

	[SerializeField]
	private CameraShakeTarget returnShake;

	[Space]
	[SerializeField]
	private AudioSource audioSourcePrefab;

	[SerializeField]
	private AudioEvent placeSound;

	[SerializeField]
	private AudioEvent returnSound;

	[SerializeField]
	private AudioEvent placeSkillSound;

	[SerializeField]
	private AudioEvent returnSkillSound;

	private Coroutine moveRoutine;

	private Action queuedTweenEnd;

	private InventoryItemToolManager manager;

	private void Awake()
	{
		manager = GetComponentInParent<InventoryItemToolManager>();
	}

	public void DoPlace(Vector2 fromPosition, Vector2 toPosition, ToolItem tool, Action onTweenEnd)
	{
		if (tool.Type == ToolItemType.Skill)
		{
			placeSkillSound.SpawnAndPlayOneShot(audioSourcePrefab, base.transform.position);
		}
		else
		{
			placeSound.SpawnAndPlayOneShot(audioSourcePrefab, base.transform.position);
		}
		DoMove(MoveRoutine(fromPosition, toPosition, (tool.Type == ToolItemType.Skill) ? "Place Skill" : "Place"), tool, onTweenEnd, placeShake);
	}

	public void DoReturn(Vector2 fromPosition, Vector2 toPosition, ToolItem tool, Action onTweenEnd)
	{
		if (tool.Type == ToolItemType.Skill)
		{
			returnSkillSound.SpawnAndPlayOneShot(audioSourcePrefab, base.transform.position);
		}
		else
		{
			returnSound.SpawnAndPlayOneShot(audioSourcePrefab, base.transform.position);
		}
		returnShake.DoShake(this);
		DoMove(MoveRoutine(fromPosition, toPosition, null), tool, onTweenEnd, null);
	}

	private void DoMove(IEnumerator routine, ToolItem tool, Action onTweenEnd, CameraShakeTarget endShake)
	{
		base.gameObject.SetActive(value: true);
		Color toolTypeColor = manager.GetToolTypeColor(tool.Type);
		if ((bool)spriteRenderer)
		{
			spriteRenderer.enabled = true;
			spriteRenderer.sprite = tool.InventorySpriteBase;
		}
		if ((bool)trailParticles)
		{
			ParticleSystem.MainModule main = trailParticles.main;
			main.startColor = toolTypeColor;
			trailParticles.Play(withChildren: true);
		}
		SpriteRenderer[] array = toolColoured;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].color = toolTypeColor;
		}
		manager.IsActionsBlocked = true;
		CancelRoutines();
		queuedTweenEnd = delegate
		{
			endShake?.DoShake(this);
			manager.IsActionsBlocked = false;
			onTweenEnd();
		};
		moveRoutine = StartCoroutine(routine);
	}

	private void CancelRoutines()
	{
		if (moveRoutine != null)
		{
			StopCoroutine(moveRoutine);
		}
		if (queuedTweenEnd != null)
		{
			queuedTweenEnd();
			queuedTweenEnd = null;
		}
	}

	public void Cancel()
	{
		CancelRoutines();
		base.gameObject.SetActive(value: false);
	}

	private IEnumerator MoveRoutine(Vector2 fromPosition, Vector2 toPosition, string endAnimationState)
	{
		if ((bool)animator)
		{
			animator.Play("Idle");
		}
		for (float elapsed = 0f; elapsed < moveDuration; elapsed += Time.unscaledDeltaTime)
		{
			SetPosition(elapsed / moveDuration);
			yield return null;
		}
		SetPosition(1f);
		if (queuedTweenEnd != null)
		{
			queuedTweenEnd();
			queuedTweenEnd = null;
		}
		if ((bool)animator && !string.IsNullOrEmpty(endAnimationState))
		{
			animator.Play(endAnimationState);
			yield return null;
			yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
		}
		if ((bool)spriteRenderer)
		{
			spriteRenderer.enabled = false;
		}
		if ((bool)trailParticles)
		{
			trailParticles.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmitting);
			while (trailParticles.IsAlive(withChildren: true))
			{
				yield return null;
			}
		}
		moveRoutine = null;
		base.gameObject.SetActive(value: false);
		void SetPosition(float time)
		{
			base.transform.SetPosition2D(Vector2.Lerp(fromPosition, toPosition, moveCurve.Evaluate(time)));
		}
	}
}
