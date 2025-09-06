using System.Collections;
using UnityEngine;

public class StateTransitionAnimator : MonoBehaviour
{
	[SerializeField]
	private SpriteRenderer[] spriteRenderers;

	[SerializeField]
	private Sprite[] sprites;

	[SerializeField]
	private float framerate = 18f;

	[Space]
	[SerializeField]
	private Transform[] transforms;

	[SerializeField]
	private Vector3 localOffset;

	[SerializeField]
	private float moveTime;

	[SerializeField]
	private AudioEvent sitOnAudio;

	[SerializeField]
	private AudioEvent getOffAudio;

	private bool state;

	private Vector3[] initialPositions;

	private Coroutine spriteRoutine;

	private Coroutine transformsRoutine;

	private void SetSprite(Sprite sprite)
	{
		SpriteRenderer[] array = spriteRenderers;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].sprite = sprite;
		}
	}

	private void OnEnable()
	{
		if (transforms.Length != 0)
		{
			initialPositions = new Vector3[transforms.Length];
			for (int i = 0; i < initialPositions.Length; i++)
			{
				initialPositions[i] = transforms[i].localPosition;
			}
			SetTransformsOffset(Vector3.zero);
		}
		if (sprites.Length != 0)
		{
			SetSprite(sprites[0]);
		}
	}

	private void OnDisable()
	{
		StopAllCoroutines();
	}

	private IEnumerator AnimateSpritesSit()
	{
		WaitForSeconds wait = new WaitForSeconds(1f / framerate);
		Sprite[] array = sprites;
		foreach (Sprite sprite in array)
		{
			SetSprite(sprite);
			yield return wait;
		}
	}

	private IEnumerator AnimateSpritesGetOff()
	{
		WaitForSeconds wait = new WaitForSeconds(1f / framerate);
		for (int i = sprites.Length - 1; i >= 0; i--)
		{
			SetSprite(sprites[i]);
			yield return wait;
		}
	}

	private IEnumerator AnimateTransformsSit()
	{
		for (float elapsed = 0f; elapsed < moveTime; elapsed += Time.deltaTime)
		{
			SetTransformsOffset(localOffset * (elapsed / moveTime));
			yield return null;
		}
		SetTransformsOffset(localOffset);
	}

	private IEnumerator AnimateTransformsGetOff()
	{
		for (float elapsed = 0f; elapsed < moveTime; elapsed += Time.deltaTime)
		{
			SetTransformsOffset(localOffset * (1f - elapsed / moveTime));
			yield return null;
		}
		SetTransformsOffset(Vector3.zero);
	}

	private void SetTransformsOffset(Vector3 offset)
	{
		for (int i = 0; i < transforms.Length; i++)
		{
			transforms[i].localPosition = initialPositions[i] + offset;
		}
	}

	public void SetState(bool value, bool isInstant)
	{
		if (!base.isActiveAndEnabled || state == value)
		{
			return;
		}
		state = value;
		if (spriteRoutine != null)
		{
			StopCoroutine(spriteRoutine);
		}
		if (sprites.Length != 0)
		{
			if (isInstant)
			{
				SetSprite(value ? sprites[^1] : sprites[0]);
			}
			else
			{
				spriteRoutine = StartCoroutine(value ? AnimateSpritesSit() : AnimateSpritesGetOff());
			}
		}
		if (transformsRoutine != null)
		{
			StopCoroutine(transformsRoutine);
		}
		if (transforms.Length != 0)
		{
			if (isInstant)
			{
				SetTransformsOffset(value ? localOffset : Vector3.zero);
			}
			else
			{
				transformsRoutine = StartCoroutine(value ? AnimateTransformsSit() : AnimateTransformsGetOff());
			}
		}
		if (!isInstant)
		{
			if (value)
			{
				sitOnAudio.SpawnAndPlayOneShot(base.transform.position);
			}
			else
			{
				getOffAudio.SpawnAndPlayOneShot(base.transform.position);
			}
		}
	}
}
