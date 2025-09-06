using UnityEngine;

[CreateAssetMenu(menuName = "Profiles/Frost Speed")]
public class FrostSpeedProfile : ScriptableObject
{
	[SerializeField]
	private float frostSpeed;

	public float FrostSpeed => frostSpeed;
}
