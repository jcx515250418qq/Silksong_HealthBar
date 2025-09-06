using UnityEngine;

public class GroundSplat : MonoBehaviour
{
	private static readonly int _splatAnim = Animator.StringToHash("Splat");

	[SerializeField]
	private CollisionEnterEvent collisionDetector;

	[SerializeField]
	private BloodSpawner.GeneralConfig blood;

	[SerializeField]
	private Animator animator;

	[SerializeField]
	private AudioEventRandom splatSound;

	private bool isActive;

	private void Awake()
	{
		if ((bool)collisionDetector)
		{
			collisionDetector.CollisionEnteredDirectional += OnCollisionEnteredDirectional;
		}
	}

	private void OnEnable()
	{
		isActive = true;
	}

	private void OnCollisionEnteredDirectional(CollisionEnterEvent.Direction direction, Collision2D collision)
	{
		if (isActive)
		{
			Vector3 position = base.transform.position;
			BloodSpawner.SpawnBlood(blood, position);
			if ((bool)animator)
			{
				animator.Play(_splatAnim, 0, 0f);
			}
			splatSound.SpawnAndPlayOneShot(position);
		}
	}

	public void Stop()
	{
		isActive = false;
	}
}
