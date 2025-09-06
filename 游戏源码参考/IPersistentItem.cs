public interface IPersistentItem
{
	string GetId();

	string GetSceneName();

	string GetValueTypeName();

	bool GetIsSemiPersistent();
}
