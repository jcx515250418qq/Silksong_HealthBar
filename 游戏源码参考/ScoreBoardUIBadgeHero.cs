using TeamCherry.SharedUtils;
using UnityEngine;

public class ScoreBoardUIBadgeHero : ScoreBoardUIBadgeBase
{
	[Space]
	[SerializeField]
	[PlayerDataField(typeof(bool), true)]
	private string playedBool;

	[SerializeField]
	[PlayerDataField(typeof(int), true)]
	private string scoreInt;

	public override int Score
	{
		get
		{
			if (!Application.isPlaying)
			{
				return 0;
			}
			if (string.IsNullOrEmpty(scoreInt))
			{
				return 0;
			}
			return PlayerData.instance.GetVariable<int>(scoreInt);
		}
	}

	protected override bool IsVisible
	{
		get
		{
			if (!Application.isPlaying)
			{
				return true;
			}
			return PlayerData.instance.GetVariable<bool>(playedBool);
		}
	}
}
