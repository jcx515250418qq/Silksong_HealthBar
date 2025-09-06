using UnityEngine;

public class ScoreBoardUIBadge : ScoreBoardUIBadgeBase
{
	[Space]
	[SerializeField]
	[ModifiableProperty]
	[Conditional("IsUsingConstant", false, true, false)]
	private int score;

	[SerializeField]
	private string constantsInt;

	public override int Score
	{
		get
		{
			if (IsUsingConstant())
			{
				if (string.IsNullOrEmpty(constantsInt))
				{
					return 0;
				}
				return Constants.GetConstantValue<int>(constantsInt);
			}
			return score;
		}
	}

	private bool IsUsingConstant()
	{
		return !string.IsNullOrWhiteSpace(constantsInt);
	}
}
