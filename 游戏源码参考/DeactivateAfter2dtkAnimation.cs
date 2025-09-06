using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DeactivateAfter2dtkAnimation : MonoBehaviour
{
	[SerializeField]
	private List<tk2dSpriteAnimator> animators;

	[SerializeField]
	private bool stayInPlace;

	[SerializeField]
	private bool matchParentScale;

	[SerializeField]
	private bool randomlyFlipXScale;

	[SerializeField]
	private bool randomlyFlipYScale;

	[SerializeField]
	private bool keepScaleSign;

	[SerializeField]
	private Transform myParent;

	[SerializeField]
	private bool deactivateMeshRendererInstead;

	[SerializeField]
	[HideInInspector]
	[Obsolete("Old serialized field, use \"animators\" instead.")]
	private tk2dSpriteAnimator spriteAnimator;

	private float timer;

	private Vector3 worldPos;

	private Vector3 startPos;

	private float startRotation;

	private float worldRotation;

	private float parentXScale;

	private bool queuedStayInPlaceUpdate;

	private Vector3 lossyScaleSign;

	private Vector3 localSign;

	private void OnValidate()
	{
		if ((bool)spriteAnimator)
		{
			animators.Add(spriteAnimator);
			spriteAnimator = null;
		}
	}

	private void Awake()
	{
		OnValidate();
		Transform transform = base.transform;
		startPos = transform.localPosition;
		startRotation = transform.localEulerAngles.z;
	}

	private void OnEnable()
	{
		timer = 0f;
		if (animators.Count <= 0)
		{
			tk2dSpriteAnimator component = GetComponent<tk2dSpriteAnimator>();
			if ((bool)component)
			{
				animators.Add(component);
			}
		}
		foreach (tk2dSpriteAnimator animator in animators)
		{
			animator.PlayFromFrame(0);
		}
		Transform transform = base.transform;
		if (stayInPlace)
		{
			transform.localPosition = startPos;
			transform.localEulerAngles = new Vector3(0f, 0f, startRotation);
			queuedStayInPlaceUpdate = true;
		}
		bool flag = randomlyFlipXScale && (float)UnityEngine.Random.Range(1, 100) > 50f;
		bool flag2 = randomlyFlipYScale && (float)UnityEngine.Random.Range(1, 100) > 50f;
		if (flag || flag2)
		{
			transform.FlipLocalScale(flag, flag2);
		}
		if (keepScaleSign)
		{
			lossyScaleSign = transform.lossyScale.GetSign();
			localSign = transform.localScale.GetSign();
		}
	}

	private void OnDisable()
	{
		if (keepScaleSign)
		{
			localSign = localSign.GetSign();
			Transform obj = base.transform;
			Vector3 sign = obj.localScale.GetSign();
			Vector3 localScale = obj.localScale;
			if (sign.x != localSign.x)
			{
				localScale.x *= -1f;
			}
			if (sign.y != localSign.y)
			{
				localScale.y *= -1f;
			}
			if (sign.z != localSign.z)
			{
				localScale.z *= -1f;
			}
			obj.localScale = localScale;
		}
	}

	private void Update()
	{
		Transform transform = base.transform;
		if (queuedStayInPlaceUpdate)
		{
			queuedStayInPlaceUpdate = false;
			worldPos = transform.position;
			worldRotation = transform.eulerAngles.z;
		}
		if (timer > 0.1f)
		{
			timer -= Time.deltaTime;
		}
		else if (animators.All((tk2dSpriteAnimator anim) => !anim.Playing))
		{
			if (!deactivateMeshRendererInstead)
			{
				base.gameObject.SetActive(value: false);
			}
			else
			{
				base.gameObject.GetComponent<MeshRenderer>().enabled = false;
			}
			return;
		}
		if (stayInPlace)
		{
			transform.position = worldPos;
			transform.eulerAngles = new Vector3(0f, 0f, worldRotation);
		}
		if (matchParentScale)
		{
			parentXScale = myParent.localScale.x;
			if ((parentXScale < 0f && transform.lossyScale.x > 0f) || (parentXScale > 0f && transform.lossyScale.x < 0f))
			{
				transform.FlipLocalScale(x: true);
			}
		}
		if (keepScaleSign)
		{
			Vector3 sign = transform.lossyScale.GetSign();
			Vector3 localScale = transform.localScale;
			if (sign.x != lossyScaleSign.x)
			{
				localScale.x *= -1f;
			}
			if (sign.y != lossyScaleSign.y)
			{
				localScale.y *= -1f;
			}
			if (sign.z != lossyScaleSign.z)
			{
				localScale.z *= -1f;
			}
			transform.localScale = localScale;
		}
	}
}
