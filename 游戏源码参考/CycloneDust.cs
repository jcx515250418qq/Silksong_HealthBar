using UnityEngine;

public class CycloneDust : MonoBehaviour
{
	public float dustY;

	public ParticleSystem particle;

	private Transform parent;

	private bool playing;

	private void Start()
	{
		parent = base.transform.parent;
	}

	private void OnEnable()
	{
		playing = false;
		particle.Stop();
	}

	private void Update()
	{
		if (parent.position.y < dustY)
		{
			if (!playing)
			{
				particle.Play();
				playing = true;
			}
		}
		else if (playing)
		{
			particle.Stop();
			playing = false;
		}
	}
}
