using HutongGames.PlayMaker;

public static class PlayMakerUtilsDotNetExtensions
{
	public static bool Contains(this VariableType[] target, VariableType vType)
	{
		if (target == null)
		{
			return false;
		}
		for (int i = 0; i < target.Length; i++)
		{
			if (target[i] == vType)
			{
				return true;
			}
		}
		return false;
	}
}
