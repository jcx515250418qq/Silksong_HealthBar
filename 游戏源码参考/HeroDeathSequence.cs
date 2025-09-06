using UnityEngine;

public sealed class HeroDeathSequence : MonoBehaviour
{
	[Tooltip("Amount of time to wait before beginning transition to next scene.")]
	[SerializeField]
	private float deathWait = 4.1f;

	public float DeathWait => deathWait;
}
