using UnityEngine;

public class RecycleAfter2dtkAnimation : MonoBehaviour
{
	public tk2dSpriteAnimator spriteAnimator;

	public bool randomiseRotation;

	private float timer;

	private void OnEnable()
	{
		timer = 0f;
		if (spriteAnimator == null)
		{
			spriteAnimator = GetComponent<tk2dSpriteAnimator>();
		}
		if (randomiseRotation)
		{
			base.transform.eulerAngles = new Vector3(base.transform.rotation.x, base.transform.rotation.y, Random.Range(0, 360));
		}
		if ((bool)spriteAnimator && (spriteAnimator.CurrentClip != null || spriteAnimator.DefaultClip != null))
		{
			spriteAnimator.PlayFromFrame(0);
		}
	}

	private void Update()
	{
		if (timer > 0.1f)
		{
			timer -= Time.deltaTime;
		}
		else if (!spriteAnimator.Playing)
		{
			base.gameObject.Recycle();
		}
	}
}
