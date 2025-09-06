public class PersistentItemData<T>
{
	public string ID;

	public string SceneName;

	public bool IsSemiPersistent;

	public T Value;

	public SceneData.PersistentMutatorTypes Mutator;
}
