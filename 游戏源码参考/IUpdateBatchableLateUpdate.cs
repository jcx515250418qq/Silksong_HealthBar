public interface IUpdateBatchableLateUpdate
{
	bool ShouldUpdate { get; }

	void BatchedLateUpdate();
}
