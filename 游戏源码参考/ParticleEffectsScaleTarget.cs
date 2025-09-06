using UnityEngine;

public sealed class ParticleEffectsScaleTarget : MonoBehaviour
{
	[SerializeField]
	private Collider2D target;

	private bool init;

	public Collider2D Target
	{
		get
		{
			Init();
			return target;
		}
	}

	public bool HasTarget { get; private set; }

	private void OnValidate()
	{
		if (target == null)
		{
			target = GetComponent<Collider2D>();
		}
	}

	private void Init()
	{
		if (!init)
		{
			init = true;
			HasTarget = target != null;
			if (!HasTarget)
			{
				target = GetComponent<Collider2D>();
				HasTarget = target != null;
			}
		}
	}
}
