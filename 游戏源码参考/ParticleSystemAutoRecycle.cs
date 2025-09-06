using UnityEngine;

public class ParticleSystemAutoRecycle : MonoBehaviour
{
	private ParticleSystem[] ps;

	private bool activated;

	public void Start()
	{
		ps = GetComponents<ParticleSystem>();
		if (ps.Length == 0)
		{
			ps = GetComponentsInChildren<ParticleSystem>(includeInactive: true);
		}
	}

	private void OnDisable()
	{
		activated = false;
	}

	public void Update()
	{
		if (ps.Length == 0)
		{
			return;
		}
		ParticleSystem[] array;
		if (!activated)
		{
			array = ps;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].IsAlive())
				{
					activated = true;
					break;
				}
			}
			return;
		}
		bool flag = false;
		array = ps;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].IsAlive())
			{
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			RecycleSelf();
		}
	}

	public void RecycleSelf()
	{
		this.Recycle();
	}
}
