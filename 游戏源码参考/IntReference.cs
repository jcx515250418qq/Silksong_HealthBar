using UnityEngine;
using UnityEngine.Serialization;

public class IntReference : ScriptableObject
{
	[SerializeField]
	[FormerlySerializedAs("cost")]
	private int value;

	public int Value => value;
}
