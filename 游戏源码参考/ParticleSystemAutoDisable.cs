using System;
using UnityEngine;

public class ParticleSystemAutoDisable : MonoBehaviour
{
	private enum DisableBehaviours
	{
		Recycle = 0,
		Disable = 1
	}

	[SerializeField]
	private DisableBehaviours disableBehaviours;

	[SerializeField]
	private bool disableOnSceneLoad = true;

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

	private void OnEnable()
	{
		activated = false;
		if (disableOnSceneLoad)
		{
			GameManager.instance.NextSceneWillActivate += Recycle;
		}
	}

	private void OnDisable()
	{
		if (disableOnSceneLoad && (bool)GameManager.UnsafeInstance)
		{
			GameManager.UnsafeInstance.NextSceneWillActivate -= Recycle;
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
			Recycle();
		}
	}

	private void Recycle()
	{
		switch (disableBehaviours)
		{
		case DisableBehaviours.Recycle:
			base.gameObject.Recycle();
			break;
		case DisableBehaviours.Disable:
			base.gameObject.SetActive(value: false);
			break;
		default:
			throw new NotImplementedException();
		}
	}
}
