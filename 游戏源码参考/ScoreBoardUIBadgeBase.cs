using TMProOld;
using UnityEngine;

public abstract class ScoreBoardUIBadgeBase : MonoBehaviour
{
	[SerializeField]
	private TMP_Text scoreText;

	public abstract int Score { get; }

	protected virtual bool IsVisible => true;

	private void OnValidate()
	{
		Refresh();
		if (!Application.isPlaying)
		{
			ScoreBoardUI componentInParent = GetComponentInParent<ScoreBoardUI>();
			if ((bool)componentInParent)
			{
				componentInParent.Refresh();
			}
		}
	}

	private void OnEnable()
	{
		Refresh();
	}

	private void Refresh()
	{
		int score = Score;
		if ((bool)scoreText)
		{
			scoreText.text = score.ToString();
		}
	}

	public void Evaluate()
	{
		base.gameObject.SetActive(IsVisible);
	}
}
