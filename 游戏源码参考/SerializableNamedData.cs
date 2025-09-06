using System;

[Serializable]
public abstract class SerializableNamedData<T>
{
	public string Name;

	public T Data;
}
