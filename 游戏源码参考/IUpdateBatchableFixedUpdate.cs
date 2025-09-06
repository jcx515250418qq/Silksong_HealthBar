public interface IUpdateBatchableFixedUpdate
{
	bool ShouldUpdate { get; }

	void BatchedFixedUpdate();
}
