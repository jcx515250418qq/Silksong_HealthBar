using UnityEngine;

public sealed class ReparentOnParticleStop : MonoBehaviour
{
	[SerializeField]
	private ParticleSystem particleSystem;

	[SerializeField]
	private Transform parent;

	private void Awake()
	{
		if (particleSystem == null)
		{
			particleSystem = GetComponent<ParticleSystem>();
			if (particleSystem == null)
			{
				return;
			}
		}
		if (parent == null)
		{
			parent = base.transform.parent;
		}
		ParticleSystem.MainModule main = particleSystem.main;
		main.stopAction = ParticleSystemStopAction.Callback;
	}

	private void OnValidate()
	{
		if (parent == null)
		{
			parent = base.transform.parent;
		}
		if (particleSystem == null)
		{
			particleSystem = GetComponent<ParticleSystem>();
		}
	}

	private void OnParticleSystemStopped()
	{
		if (parent != null)
		{
			base.transform.SetParent(parent);
		}
	}
}
