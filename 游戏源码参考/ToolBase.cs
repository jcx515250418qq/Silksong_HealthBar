public abstract class ToolBase : QuestTargetCounter
{
	public override bool CanGetMultipleAtOnce => false;

	public override bool IsUnique => true;

	public abstract bool IsEquipped { get; }
}
