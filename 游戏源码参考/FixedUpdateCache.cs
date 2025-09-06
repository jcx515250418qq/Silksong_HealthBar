public class FixedUpdateCache
{
	private int lastUpdate = -1;

	public bool ShouldUpdate()
	{
		int fixedUpdateCycle = CustomPlayerLoop.FixedUpdateCycle;
		bool result = lastUpdate != fixedUpdateCycle;
		lastUpdate = fixedUpdateCycle;
		return result;
	}
}
