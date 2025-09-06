public interface IUpdateBatchableUpdate
{
	bool ShouldUpdate { get; }

	void BatchedUpdate();
}
