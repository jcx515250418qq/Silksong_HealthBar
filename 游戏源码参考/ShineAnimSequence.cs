using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class ShineAnimSequence : MonoBehaviour
{
	[Serializable]
	private class ShineObject
	{
		public SpriteRenderer renderer;

		public Sprite[] shineSprites;

		public float fps = 12f;

		private Sprite initialSprite;

		public void Setup()
		{
			if ((bool)renderer)
			{
				initialSprite = renderer.sprite;
			}
		}

		public IEnumerator ShineAnim()
		{
			if ((bool)renderer && shineSprites.Length != 0)
			{
				WaitForSeconds wait = new WaitForSeconds(1f / fps);
				Sprite[] array = shineSprites;
				foreach (Sprite sprite in array)
				{
					renderer.sprite = sprite;
					yield return wait;
				}
				ResetSprite();
			}
		}

		public void ResetSprite()
		{
			if ((bool)renderer)
			{
				renderer.sprite = initialSprite;
			}
		}
	}

	[SerializeField]
	private ShineObject[] shineObjects;

	[SerializeField]
	private float delayBetweenObjects;

	[SerializeField]
	private float minDelaySequence = 2f;

	[SerializeField]
	private float maxDelaySequence = 4f;

	[Space]
	public UnityEvent OnSequencePlay;

	private Coroutine shineRoutine;

	private void OnEnable()
	{
		StartShine();
	}

	private void OnDisable()
	{
		StopShine();
	}

	public void StartShine()
	{
		if (shineRoutine == null)
		{
			ShineObject[] array = shineObjects;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Setup();
			}
			shineRoutine = StartCoroutine(ShineSequence());
		}
	}

	public void StopShine()
	{
		if (shineRoutine != null)
		{
			StopCoroutine(shineRoutine);
			shineRoutine = null;
			ShineObject[] array = shineObjects;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].ResetSprite();
			}
		}
	}

	private IEnumerator ShineSequence()
	{
		while (true)
		{
			yield return new WaitForSeconds(UnityEngine.Random.Range(minDelaySequence, maxDelaySequence));
			OnSequencePlay.Invoke();
			ShineObject[] array = shineObjects;
			foreach (ShineObject shineObject in array)
			{
				if (shineObject.renderer.gameObject.activeInHierarchy)
				{
					StartCoroutine(shineObject.ShineAnim());
				}
				yield return new WaitForSeconds(delayBetweenObjects);
			}
		}
	}
}
