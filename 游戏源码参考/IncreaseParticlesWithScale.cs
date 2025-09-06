using UnityEngine;

public class IncreaseParticlesWithScale : MonoBehaviour
{
	private void OnDrawGizmos()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Color gray = Color.gray;
		float? a = 0.5f;
		Gizmos.color = gray.Where(null, null, null, a);
		Gizmos.DrawLine(Vector3.left, Vector3.right);
	}

	private void Start()
	{
		Transform obj = base.transform;
		ParticleSystem component = GetComponent<ParticleSystem>();
		float x = obj.lossyScale.x;
		ParticleSystem.MainModule main = component.main;
		main.maxParticles = Mathf.CeilToInt((float)main.maxParticles * x);
		ParticleSystem.EmissionModule emission = component.emission;
		emission.rateOverTimeMultiplier *= x;
	}
}
