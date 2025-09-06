using TeamCherry.SharedUtils;
using UnityEngine;

public class SetParticleScale : MonoBehaviour
{
	public bool grandParent;

	public bool greatGrandParent;

	[SerializeField]
	private bool doesNotMove;

	private float parentXScale;

	private float selfXScale;

	private Vector3 scaleVector;

	private bool unparented;

	private bool hasParent;

	private bool hasParentBody;

	private GameObject parent;

	private Rigidbody2D parentBody;

	private bool updated;

	private bool started;

	private void Start()
	{
		if (grandParent)
		{
			if (base.transform.parent != null && base.transform.parent.parent != null)
			{
				parent = base.transform.parent.gameObject.transform.parent.gameObject;
			}
		}
		else if (greatGrandParent)
		{
			if (base.transform.parent != null && base.transform.parent.parent != null && base.transform.parent.parent.parent != null)
			{
				parent = base.transform.parent.gameObject.transform.parent.gameObject.transform.parent.gameObject;
			}
		}
		else if (base.transform.parent != null)
		{
			parent = base.transform.parent.gameObject;
		}
		hasParent = parent != null;
		parentBody = GetComponentInParent<Rigidbody2D>();
		hasParentBody = parentBody != null;
		started = true;
		ComponentSingleton<SetParticleScaleCallbackHooks>.Instance.OnUpdate += OnUpdate;
	}

	private void OnEnable()
	{
		if (started)
		{
			ComponentSingleton<SetParticleScaleCallbackHooks>.Instance.OnUpdate += OnUpdate;
		}
		updated = false;
	}

	private void OnDisable()
	{
		ComponentSingleton<SetParticleScaleCallbackHooks>.Instance.OnUpdate -= OnUpdate;
	}

	private void OnUpdate()
	{
		if (updated || (hasParentBody && !parentBody.IsAwake()))
		{
			return;
		}
		if (hasParent && !unparented)
		{
			parentXScale = parent.transform.localScale.x;
			selfXScale = base.transform.localScale.x;
			if ((parentXScale < 0f && selfXScale > 0f) || (parentXScale > 0f && selfXScale < 0f))
			{
				scaleVector.Set(0f - base.transform.localScale.x, base.transform.localScale.y, base.transform.localScale.z);
				base.transform.localScale = scaleVector;
			}
		}
		else
		{
			unparented = true;
			selfXScale = base.transform.localScale.x;
			if (selfXScale < 0f)
			{
				scaleVector.Set(0f - base.transform.localScale.x, base.transform.localScale.y, base.transform.localScale.z);
				base.transform.localScale = scaleVector;
			}
		}
		if (doesNotMove)
		{
			updated = true;
		}
	}

	private void OnTransformParentChanged()
	{
		hasParent = parentBody != null;
		updated = false;
	}
}
