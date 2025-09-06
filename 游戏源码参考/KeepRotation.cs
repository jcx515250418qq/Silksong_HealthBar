using TeamCherry.SharedUtils;
using UnityEngine;

public class KeepRotation : MonoBehaviour
{
	[SerializeField]
	private float angle;

	[SerializeField]
	private bool worldSpace;

	[SerializeField]
	private bool useParentAngle;

	[SerializeField]
	private bool forceEveryFrame;

	[SerializeField]
	private bool keepWorldScale;

	private bool hasTransform;

	private Transform tf;

	private Vector3 rotation;

	private Rigidbody2D parentBody;

	private bool hasParentBody;

	private Vector3 originalScale;

	private bool hasParent;

	private bool started;

	private void Awake()
	{
		originalScale = base.transform.lossyScale;
	}

	private void Start()
	{
		tf = base.transform;
		hasTransform = true;
		if (!useParentAngle)
		{
			rotation = new Vector3(0f, 0f, angle);
			UpdateScale();
		}
		parentBody = GetComponentInParent<Rigidbody2D>();
		hasParentBody = parentBody != null;
		UpdateParent();
		started = true;
		ComponentSingleton<KeepRotationCallbackHooks>.Instance.OnLateUpdate += OnLateUpdate;
	}

	private void OnEnable()
	{
		if (started)
		{
			ComponentSingleton<KeepRotationCallbackHooks>.Instance.OnLateUpdate += OnLateUpdate;
		}
		if (useParentAngle)
		{
			angle = base.transform.parent.transform.localEulerAngles.z;
			rotation = new Vector3(0f, 0f, angle);
			UpdateScale();
		}
	}

	private void OnDisable()
	{
		ComponentSingleton<KeepRotationCallbackHooks>.Instance.OnLateUpdate -= OnLateUpdate;
	}

	private void OnLateUpdate()
	{
		if ((!hasParentBody || parentBody.IsAwake() || forceEveryFrame) && hasTransform)
		{
			if (worldSpace)
			{
				tf.eulerAngles = rotation;
			}
			else
			{
				tf.localEulerAngles = rotation;
			}
			UpdateScale();
		}
	}

	private void UpdateScale()
	{
		if (keepWorldScale && !hasParent)
		{
			Vector3 lossyScale = tf.parent.lossyScale;
			tf.localScale = new Vector3(originalScale.x / lossyScale.x, originalScale.y / lossyScale.y, originalScale.z / lossyScale.z);
		}
	}

	private void UpdateParent()
	{
		if (hasTransform)
		{
			hasParent = tf.parent != null;
		}
	}

	private void OnTransformParentChanged()
	{
		UpdateParent();
	}
}
