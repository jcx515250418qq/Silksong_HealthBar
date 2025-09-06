using UnityEngine;
using UnityEngine.Events;

public class HeroEnterSceneXResponder : MonoBehaviour
{
	public UnityEvent OnEnterSceneLeft;

	public UnityEvent OnEnterSceneRight;

	private HeroController hc;

	private void Awake()
	{
		hc = HeroController.instance;
		if (hc.isHeroInPosition)
		{
			DoSet();
		}
		else
		{
			hc.heroInPosition += OnHeroInPosition;
		}
	}

	private void OnHeroInPosition(bool forcedirect)
	{
		hc.heroInPosition -= OnHeroInPosition;
		DoSet();
	}

	public void DoSet()
	{
		Vector3 position = hc.transform.position;
		Vector3 position2 = base.transform.position;
		if (position.x < position2.x)
		{
			OnEnterSceneLeft.Invoke();
		}
		else
		{
			OnEnterSceneRight.Invoke();
		}
	}

	public void SetScaleSignX(float xSign)
	{
		Vector3 localScale = base.transform.localScale;
		localScale.x = Mathf.Abs(localScale.x) * xSign;
		base.transform.localScale = localScale;
	}
}
