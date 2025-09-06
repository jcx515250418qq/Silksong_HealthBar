public interface IBreakableProjectile
{
	public struct HitInfo
	{
		public bool isWall;
	}

	void QueueBreak(HitInfo hitInfo);
}
