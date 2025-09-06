using System.Collections;
using TeamCherry.SharedUtils;
using UnityEngine;

public class SandCentipede : MonoBehaviour
{
	[SerializeField]
	private Animator animator;

	[SerializeField]
	private SpriteRenderer sprite;

	[SerializeField]
	private string[] animations;

	[SerializeField]
	private MinMaxFloat waitTime;

	[SerializeField]
	private Vector2 rangePos;

	private Vector2 minPos;

	private Vector2 maxPos;

	private int[] animationIds;

	private Coroutine animRoutine;

	private bool hasEverWarmedUp;

	private static readonly Vector2 BOUNDS_BUFFER = new Vector2(3f, 5f);

	private void Awake()
	{
		animationIds = new int[animations.Length];
		for (int i = 0; i < animationIds.Length; i++)
		{
			animationIds[i] = Animator.StringToHash(animations[i]);
		}
		minPos = base.transform.position;
		maxPos = base.transform.TransformPoint(rangePos);
		hasEverWarmedUp = false;
	}

	private void OnEnable()
	{
		animator.enabled = false;
		sprite.enabled = false;
		animRoutine = StartCoroutine(Anim());
	}

	private void OnDisable()
	{
		StopCoroutine(animRoutine);
	}

	private IEnumerator Anim()
	{
		if (!hasEverWarmedUp)
		{
			animator.enabled = true;
			AnimatorCullingMode previous = animator.cullingMode;
			if (previous != 0)
			{
				animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
			}
			yield return null;
			if (previous != 0)
			{
				animator.cullingMode = previous;
			}
			animator.enabled = false;
			hasEverWarmedUp = true;
		}
		while (true)
		{
			float initialWaitTimeLeft = waitTime.GetRandomValue();
			while (initialWaitTimeLeft > 0f)
			{
				initialWaitTimeLeft -= Time.deltaTime;
				yield return null;
			}
			Vector2 position = Vector2.Lerp(minPos, maxPos, Random.Range(0f, 1f));
			if (CameraInfoCache.IsWithinBounds(position, BOUNDS_BUFFER))
			{
				base.transform.SetPosition2D(position);
				animator.enabled = true;
				sprite.enabled = true;
				if (Random.Range(0, 2) == 0)
				{
					base.transform.FlipLocalScale(x: true);
				}
				animator.Play(animationIds.GetRandomElement());
				yield return null;
				float waitTimeLeft = animator.GetCurrentAnimatorStateInfo(0).length;
				while (waitTimeLeft > 0f)
				{
					waitTimeLeft -= Time.deltaTime;
					yield return null;
				}
				animator.enabled = false;
				sprite.enabled = false;
			}
		}
	}
}
