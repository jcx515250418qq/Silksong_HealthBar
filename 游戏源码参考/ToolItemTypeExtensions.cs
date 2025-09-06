public static class ToolItemTypeExtensions
{
	public static bool IsAttackType(this ToolItemType type)
	{
		if (type != 0)
		{
			return type == ToolItemType.Skill;
		}
		return true;
	}
}
