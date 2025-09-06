using UnityEngine;

public class ProjectileVelocityManager : MonoBehaviour
{
	[SerializeField]
	private DamageEnemies enemyDamager;

	[SerializeField]
	private Rigidbody2D body;

	[SerializeField]
	private float hitDamping;

	[SerializeField]
	private float hitDampDuration;

	[SerializeField]
	private AnimationCurve hitDampReturnCurve;

	private Coroutine dampRoutine;

	private float currentMultiplier;

	private Vector2 _desiredVelocity;

	public Vector2 DesiredVelocity
	{
		get
		{
			return _desiredVelocity;
		}
		set
		{
			_desiredVelocity = value;
			UpdateVelocity();
		}
	}

	private void Awake()
	{
		if ((bool)enemyDamager)
		{
			enemyDamager.DamagedEnemy += OnDamagedEnemy;
		}
	}

	private void OnEnable()
	{
		currentMultiplier = 1f;
	}

	private void OnDisable()
	{
		StopDampRoutine();
	}

	private void OnDestroy()
	{
		if ((bool)enemyDamager)
		{
			enemyDamager.DamagedEnemy -= OnDamagedEnemy;
		}
	}

	private void OnDamagedEnemy()
	{
		if (base.isActiveAndEnabled)
		{
			StopDampRoutine();
			dampRoutine = this.StartTimerRoutine(0f, hitDampDuration, delegate(float time)
			{
				currentMultiplier = Mathf.Lerp(hitDamping, 1f, hitDampReturnCurve.Evaluate(time));
				UpdateVelocity();
			}, null, delegate
			{
				dampRoutine = null;
			});
		}
	}

	private void StopDampRoutine()
	{
		if (dampRoutine != null)
		{
			StopCoroutine(dampRoutine);
			dampRoutine = null;
		}
	}

	private void UpdateVelocity()
	{
		body.linearVelocity = DesiredVelocity * currentMultiplier;
	}
}
