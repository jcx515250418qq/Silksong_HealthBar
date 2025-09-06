using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class CogCylinderPuzzleCog : MonoBehaviour
{
	[Header("Structure")]
	[SerializeField]
	private MeshRenderer wiggleScroller;

	[SerializeField]
	private Vector2 wiggleOffset;

	[SerializeField]
	private AnimationCurve wiggleCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	[Space]
	[SerializeField]
	private MeshRenderer fleurScroller;

	[SerializeField]
	private Color fluerScrollerVertexColor;

	[SerializeField]
	private float hitSpinDuration;

	[SerializeField]
	private AnimationCurve hitSpinCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	[SerializeField]
	[Range(0f, 1f)]
	private float impactPoint;

	[SerializeField]
	private PlayMakerFSM leverFsm;

	[Space]
	public UnityEvent OnRotateStart;

	public UnityEvent OnRotateImpact;

	[Space]
	[SerializeField]
	private float sectionCountX;

	[SerializeField]
	private float sectionCountY;

	[SerializeField]
	private float currentSectionY;

	[Header("Target Config")]
	[SerializeField]
	private int targetSectionX;

	[Header("Active Gameplay")]
	[SerializeField]
	private float currentSectionX;

	private MaterialPropertyBlock fleurBlock;

	private MaterialPropertyBlock wiggleBlock;

	private Vector4 wiggleSt;

	private Coroutine animateRoutine;

	private float nextSectionX;

	private static readonly int _texStProp = Shader.PropertyToID("_MainTex_ST");

	public bool IsInTargetPos
	{
		get
		{
			if (animateRoutine != null)
			{
				return false;
			}
			return currentSectionX.IsWithinTolerance(0.1f, targetSectionX);
		}
	}

	public event Action RotateFinished;

	private void OnValidate()
	{
		if (!Application.isPlaying && (bool)fleurScroller)
		{
			SetInitialBlock();
		}
	}

	private void Awake()
	{
		SetInitialBlock();
		Material sharedMaterial = wiggleScroller.sharedMaterial;
		wiggleSt = sharedMaterial.GetVector(_texStProp);
		wiggleBlock = new MaterialPropertyBlock();
		wiggleScroller.GetPropertyBlock(wiggleBlock);
		Mesh mesh = fleurScroller.GetComponent<MeshFilter>().mesh;
		Color[] array = new Color[mesh.vertexCount];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = fluerScrollerVertexColor;
		}
		mesh.colors = array;
	}

	private void SetInitialBlock()
	{
		if (fleurBlock == null)
		{
			fleurBlock = new MaterialPropertyBlock();
		}
		fleurBlock.Clear();
		fleurScroller.GetPropertyBlock(fleurBlock);
		UpdateScrollerPos();
	}

	private void UpdateScrollerPos()
	{
		float x = 1f / sectionCountX;
		float num = 1f / sectionCountY;
		float z = currentSectionX / sectionCountX;
		float w = currentSectionY / sectionCountY * -1f - num;
		fleurBlock.SetVector(_texStProp, new Vector4(x, num, z, w));
		fleurScroller.SetPropertyBlock(fleurBlock);
	}

	private IEnumerator AnimateScrollerPos()
	{
		float lastSectionX = currentSectionX;
		Vector2 lastWiggleOffset = new Vector2(wiggleSt.z, wiggleSt.w);
		Vector2 nextWiggleOffset = lastWiggleOffset + wiggleOffset;
		bool hasImpacted = false;
		OnRotateStart.Invoke();
		float elapsed = 0f;
		while (elapsed < hitSpinDuration)
		{
			float t = elapsed / hitSpinDuration;
			float t2 = hitSpinCurve.Evaluate(t);
			currentSectionX = Mathf.LerpUnclamped(lastSectionX, nextSectionX, t2);
			UpdateScrollerPos();
			float t3 = wiggleCurve.Evaluate(t);
			Vector2 vector = Vector2.LerpUnclamped(lastWiggleOffset, nextWiggleOffset, t3);
			wiggleSt.z = vector.x;
			wiggleSt.w = vector.y;
			wiggleBlock.SetVector(_texStProp, wiggleSt);
			wiggleScroller.SetPropertyBlock(wiggleBlock);
			yield return null;
			elapsed += Time.deltaTime;
			if (!hasImpacted && t >= impactPoint)
			{
				hasImpacted = true;
				OnRotateImpact.Invoke();
			}
		}
		currentSectionX = nextSectionX % sectionCountX;
		UpdateScrollerPos();
		animateRoutine = null;
		if (this.RotateFinished != null)
		{
			this.RotateFinished();
		}
	}

	public void HitMove()
	{
		if (animateRoutine == null)
		{
			nextSectionX = currentSectionX + 1f;
		}
		else
		{
			StopCoroutine(animateRoutine);
			nextSectionX += 1f;
		}
		animateRoutine = StartCoroutine(AnimateScrollerPos());
	}

	public void SetComplete()
	{
		if ((bool)leverFsm && leverFsm.isActiveAndEnabled)
		{
			leverFsm.SendEvent("RETRACT");
		}
		currentSectionX = targetSectionX;
		UpdateScrollerPos();
	}
}
