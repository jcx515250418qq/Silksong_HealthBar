using System;
using System.Collections.Generic;
using UnityEngine;

public class ParticleEffectsScaleToCollider : MonoBehaviour
{
	[Serializable]
	private struct ParticleSystemInfo
	{
		public float EmissionRate;
	}

	[SerializeField]
	private ParticleSystem[] particles;

	[SerializeField]
	private ParticleEffectsLerpEmission lerpEmission;

	[SerializeField]
	private Vector2 referenceColliderSize = Vector2.one;

	[SerializeField]
	private bool useParent;

	private bool hasStarted;

	private Vector3 initialScale;

	private Dictionary<ParticleSystem, ParticleSystemInfo> particleInfos;

	private static List<Collider2D> colliders = new List<Collider2D>();

	private void OnValidate()
	{
		if (lerpEmission != null && particles != null && particles.Length != 0)
		{
			particles = null;
		}
	}

	private void Awake()
	{
		if (!lerpEmission)
		{
			particleInfos = new Dictionary<ParticleSystem, ParticleSystemInfo>();
		}
		initialScale = base.transform.localScale;
	}

	private void OnEnable()
	{
		if (!hasStarted)
		{
			return;
		}
		if (!lerpEmission)
		{
			ParticleSystem[] array = particles;
			foreach (ParticleSystem particleSystem in array)
			{
				ParticleSystem.EmissionModule emission = particleSystem.emission;
				particleInfos[particleSystem] = new ParticleSystemInfo
				{
					EmissionRate = emission.rateOverTimeMultiplier
				};
			}
		}
		if (!useParent)
		{
			return;
		}
		Transform parent = base.transform.parent;
		if (parent != null)
		{
			ParticleEffectsScaleTarget component = parent.GetComponent<ParticleEffectsScaleTarget>();
			if (component != null)
			{
				SetScaleToCollider(component.Target);
			}
			else
			{
				SetScaleToGameObject(base.transform.parent.gameObject);
			}
		}
	}

	private void Start()
	{
		hasStarted = true;
		OnEnable();
	}

	private void OnDisable()
	{
		if (!lerpEmission)
		{
			ParticleSystem[] array = particles;
			foreach (ParticleSystem particleSystem in array)
			{
				ParticleSystemInfo particleSystemInfo = particleInfos[particleSystem];
				ParticleSystem.EmissionModule emission = particleSystem.emission;
				emission.rateOverTimeMultiplier = particleSystemInfo.EmissionRate;
			}
		}
	}

	public void SetScaleToGameObject(GameObject obj)
	{
		Collider2D component = obj.GetComponent<Collider2D>();
		if ((bool)component)
		{
			SetScaleToCollider(component);
		}
		else
		{
			Debug.LogError("Could not find collider", this);
		}
	}

	public void SetScaleToCollider(Collider2D col)
	{
		base.transform.localScale = initialScale;
		Vector2 vector;
		if (col.isActiveAndEnabled)
		{
			vector = col.bounds.size;
		}
		else
		{
			Rigidbody2D attachedRigidbody = col.attachedRigidbody;
			bool flag = false;
			Bounds bounds = default(Bounds);
			if ((bool)attachedRigidbody)
			{
				colliders.Clear();
				int attachedColliders = attachedRigidbody.GetAttachedColliders(colliders);
				for (int i = 0; i < attachedColliders; i++)
				{
					Collider2D collider2D = colliders[i];
					if (collider2D.enabled && !collider2D.isTrigger)
					{
						if (!flag)
						{
							bounds = collider2D.bounds;
							flag = true;
						}
						else
						{
							bounds.Encapsulate(collider2D.bounds);
						}
					}
				}
				colliders.Clear();
			}
			vector = ((!flag) ? referenceColliderSize : ((Vector2)bounds.size));
		}
		float area = referenceColliderSize.GetArea();
		float num = vector.GetArea() / area;
		Vector2 original = vector.DivideElements(referenceColliderSize);
		Vector3 self = base.transform.InverseTransformVector(initialScale.MultiplyElements(original.ToVector3(1f)));
		base.transform.localScale = self.Abs();
		if ((bool)lerpEmission)
		{
			lerpEmission.TotalMultiplier = num;
			return;
		}
		ParticleSystem[] array = particles;
		foreach (ParticleSystem particleSystem in array)
		{
			ParticleSystemInfo particleSystemInfo = particleInfos[particleSystem];
			ParticleSystem.EmissionModule emission = particleSystem.emission;
			emission.rateOverTimeMultiplier = particleSystemInfo.EmissionRate * num;
		}
	}
}
