using UnityEngine;

public sealed class ParticleDetachReparent : MonoBehaviour
{
	[SerializeField]
	private ParticleSystem particleSystem;

	private Transform originalParent;

	private Vector3 localPosition;

	private Quaternion localRotation;

	private Vector3 localScale;

	private bool detached;

	private bool stopTriggered;

	private bool hasParent;

	private bool parentDestroyed;

	private void Awake()
	{
		if (particleSystem == null)
		{
			particleSystem = GetComponent<ParticleSystem>();
			if (particleSystem == null)
			{
				base.enabled = false;
				return;
			}
		}
		originalParent = base.transform.parent;
		hasParent = originalParent != null;
		if (originalParent == null)
		{
			base.enabled = false;
			return;
		}
		DestroyCallback.AddCallback(originalParent.gameObject, delegate
		{
			parentDestroyed = true;
			hasParent = false;
		});
	}

	private void OnValidate()
	{
		if (particleSystem == null)
		{
			particleSystem = GetComponent<ParticleSystem>();
		}
	}

	private void OnEnable()
	{
		if (base.transform.parent != null && !detached)
		{
			originalParent = base.transform.parent;
			localPosition = base.transform.localPosition;
			localRotation = base.transform.localRotation;
			localScale = base.transform.localScale;
			base.transform.parent = null;
			detached = true;
			stopTriggered = false;
		}
	}

	private void OnDisable()
	{
		if (detached)
		{
			detached = false;
			if ((bool)originalParent)
			{
				base.transform.parent = originalParent;
				base.transform.localPosition = localPosition;
				base.transform.localRotation = localRotation;
				base.transform.localScale = localScale;
			}
		}
	}

	private void LateUpdate()
	{
		if (!detached)
		{
			return;
		}
		if (hasParent && originalParent.gameObject.activeInHierarchy)
		{
			base.transform.position = originalParent.TransformPoint(localPosition);
			base.transform.rotation = originalParent.rotation * localRotation;
			return;
		}
		if (!stopTriggered)
		{
			particleSystem.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmitting);
			stopTriggered = true;
		}
		if (!particleSystem.IsAlive(withChildren: true))
		{
			detached = false;
			if (hasParent)
			{
				base.transform.parent = originalParent;
				base.transform.localPosition = localPosition;
				base.transform.localRotation = localRotation;
				base.transform.localScale = localScale;
			}
			else if (parentDestroyed)
			{
				Object.Destroy(base.gameObject);
			}
		}
	}
}
