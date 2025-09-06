using UnityEngine;

public class ParticleSystemAutoDeactivate : MonoBehaviour
{
	private ParticleSystem[] particleSystems;

	private bool activated;

	private bool subscribed;

	public void Start()
	{
		ParticleSystem component = GetComponent<ParticleSystem>();
		if ((bool)component)
		{
			particleSystems = new ParticleSystem[1] { component };
		}
		else
		{
			particleSystems = GetComponentsInChildren<ParticleSystem>();
		}
	}

	public void Update()
	{
		bool flag = false;
		ParticleSystem[] array = particleSystems;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].IsAlive())
			{
				activated = true;
				flag = true;
				break;
			}
		}
		if (!flag && activated)
		{
			base.gameObject.SetActive(value: false);
		}
	}
}
