using UnityEngine;

public class ParticleSystemAutoDestroy : MonoBehaviour
{
	private ParticleSystem ps;

	private bool activated;

	public void Start()
	{
		ps = GetComponent<ParticleSystem>();
	}

	public void Update()
	{
		if ((bool)ps)
		{
			if (ps.IsAlive())
			{
				activated = true;
			}
			if (!ps.IsAlive() && activated)
			{
				Object.Destroy(base.gameObject);
			}
		}
	}
}
