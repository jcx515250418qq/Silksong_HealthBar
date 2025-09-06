using UnityEngine;

public class DeactivateOnParentScaleFlip : MonoBehaviour
{
	[SerializeField]
	private Transform parent;

	private float initialXSign;

	private HeroController hc;

	private void Awake()
	{
		if (!parent)
		{
			hc = GetComponentInParent<HeroController>(includeInactive: true);
			if ((bool)hc)
			{
				hc.FlippedSprite += Disable;
			}
		}
	}

	private void OnEnable()
	{
		if ((bool)parent)
		{
			initialXSign = Mathf.Sign(parent.transform.localScale.x);
		}
	}

	private void OnDestroy()
	{
		if ((bool)hc)
		{
			hc.FlippedSprite -= Disable;
			hc = null;
		}
	}

	private void LateUpdate()
	{
		if ((bool)parent && !Mathf.Approximately(Mathf.Sign(parent.transform.localScale.x), initialXSign))
		{
			Disable();
		}
	}

	private void Disable()
	{
		base.gameObject.SetActive(value: false);
	}
}
