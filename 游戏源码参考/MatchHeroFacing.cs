using UnityEngine;

public class MatchHeroFacing : MonoBehaviour
{
	public bool reverse;

	private bool hasStarted;

	private void Start()
	{
		hasStarted = true;
		DoMatch();
	}

	private void OnEnable()
	{
		if (hasStarted)
		{
			DoMatch();
		}
	}

	private void DoMatch()
	{
		Transform transform = GameManager.instance.hero_ctrl.transform;
		if (!reverse)
		{
			if ((transform.localScale.x < 0f && base.transform.localScale.x > 0f) || (transform.localScale.x < 0f && base.transform.localScale.x < 0f))
			{
				base.transform.localScale = new Vector2(0f - base.transform.localScale.x, base.transform.localScale.y);
			}
		}
		else if ((transform.localScale.x < 0f && base.transform.localScale.x < 0f) || (transform.localScale.x > 0f && base.transform.localScale.x > 0f))
		{
			base.transform.localScale = new Vector2(0f - base.transform.localScale.x, base.transform.localScale.y);
		}
	}
}
