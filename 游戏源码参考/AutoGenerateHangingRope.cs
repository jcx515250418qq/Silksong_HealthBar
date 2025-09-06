using System;
using System.Linq;
using TeamCherry.Splines;
using UnityEngine;

[ExecuteInEditMode]
public class AutoGenerateHangingRope : MonoBehaviour
{
	[SerializeField]
	private LinearSpline template;

	[SerializeField]
	private Rigidbody2D pinnedControlPoint;

	[SerializeField]
	private Rigidbody2D hangingControlPoint;

	[Space]
	[SerializeField]
	private float controlPointDistance = 2f;

	[SerializeField]
	private int subdivisions;

	[SerializeField]
	private float autoTextureTiling;

	[Header("Optional")]
	[SerializeField]
	private GameObject midPointTemplate;

	[SerializeField]
	private BoxCollider2D scaleColliderToMatch;

	public bool HasGenerated { get; private set; }

	public event Action Generated;

	private void OnValidate()
	{
		if (subdivisions < 2)
		{
			subdivisions = 2;
		}
	}

	private void Awake()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		OnValidate();
		Transform transform = pinnedControlPoint.transform;
		Transform transform2 = hangingControlPoint.transform;
		Vector3 localPosition = transform.localPosition;
		Vector3 localPosition2 = transform2.localPosition;
		float num = Vector3.Distance(localPosition, localPosition2);
		if (num < 1.5f)
		{
			return;
		}
		UpdateTextureTiling(template, pinnedControlPoint.transform, hangingControlPoint.transform);
		float width = template.Width;
		SplineBase.TextureTilingMethods textureTilingMethod = template.TextureTilingMethod;
		float textureOffset = template.TextureOffset;
		bool flipTextureU = template.FlipTextureU;
		bool flipTextureV = template.FlipTextureV;
		float fpsLimit = template.FpsLimit;
		bool preventCulling = template.PreventCulling;
		SplineBase.TangentSources tangentSource = template.TangentSource;
		SplineBase.UpdateConditions updateCondition = template.UpdateCondition;
		bool reverseDirection = template.ReverseDirection;
		Color vertexColor = template.VertexColor;
		float num2 = template.TextureTiling;
		if (textureTilingMethod != SplineBase.TextureTilingMethods.Relative)
		{
			num2 *= 0.5f;
		}
		PinTransformToSpline[] array = (from pin in GetComponentsInChildren<PinTransformToSpline>(includeInactive: true)
			where pin.Spline == template
			select pin).ToArray();
		GameObject gameObject = template.gameObject;
		UnityEngine.Object.DestroyImmediate(template, allowDestroyingAssets: false);
		Rigidbody2D rigidbody2D;
		HingeJoint2D hingeJoint2D;
		if (hangingControlPoint.bodyType == RigidbodyType2D.Static && pinnedControlPoint.bodyType == RigidbodyType2D.Static)
		{
			rigidbody2D = null;
			hingeJoint2D = null;
			UnityEngine.Object.Destroy(hangingControlPoint);
			UnityEngine.Object.Destroy(pinnedControlPoint);
		}
		else
		{
			rigidbody2D = hangingControlPoint;
			hingeJoint2D = pinnedControlPoint.GetComponent<HingeJoint2D>();
		}
		int num3 = Mathf.Max(4, Mathf.RoundToInt(num / controlPointDistance));
		Transform[] array2 = new Transform[num3];
		array2[0] = transform;
		array2[num3 - 1] = transform2;
		for (int num4 = num3 - 2; num4 >= 0; num4--)
		{
			HingeJoint2D hingeJoint2D2;
			if (num4 != 0)
			{
				GameObject gameObject2;
				if ((bool)midPointTemplate)
				{
					gameObject2 = UnityEngine.Object.Instantiate(midPointTemplate);
					gameObject2.name = $"{midPointTemplate.name} (Control Point {num4})";
				}
				else
				{
					gameObject2 = new GameObject("Control Point " + num4);
				}
				array2[num4] = gameObject2.transform;
				if ((bool)rigidbody2D)
				{
					AddCopiedBody(gameObject2, rigidbody2D);
				}
				hingeJoint2D2 = (hingeJoint2D ? AddCopiedHinge(gameObject2, hingeJoint2D) : null);
				gameObject2.transform.SetParent(transform.parent);
				float t = (float)num4 / (float)(num3 - 1);
				Vector3 localPosition3 = Vector3.Lerp(localPosition, localPosition2, t);
				gameObject2.transform.localPosition = localPosition3;
			}
			else
			{
				hingeJoint2D2 = array2[num4].GetComponent<HingeJoint2D>();
			}
			if (!(hingeJoint2D2 == null))
			{
				Rigidbody2D rigidbody2D2 = (hingeJoint2D2.connectedBody = array2[num4 + 1].GetComponent<Rigidbody2D>());
				hingeJoint2D2.autoConfigureConnectedAnchor = false;
				hingeJoint2D2.connectedAnchor = hingeJoint2D2.transform.position - rigidbody2D2.transform.position;
			}
		}
		if ((bool)midPointTemplate)
		{
			midPointTemplate.gameObject.SetActive(value: false);
		}
		HermiteSpline hermiteSpline = gameObject.AddComponent<HermiteSpline>();
		hermiteSpline.Width = width;
		hermiteSpline.TextureTiling = num2;
		hermiteSpline.TextureTilingMethod = textureTilingMethod;
		hermiteSpline.TextureOffset = textureOffset;
		hermiteSpline.FlipTextureU = flipTextureU;
		hermiteSpline.FlipTextureV = flipTextureV;
		hermiteSpline.FpsLimit = fpsLimit;
		hermiteSpline.PreventCulling = preventCulling;
		hermiteSpline.TangentSource = tangentSource;
		hermiteSpline.UpdateCondition = updateCondition;
		hermiteSpline.ReverseDirection = reverseDirection;
		hermiteSpline.Subdivisions = subdivisions;
		hermiteSpline.ControlPoints = array2.ToList();
		hermiteSpline.NormaliseDistances = false;
		hermiteSpline.VertexColor = vertexColor;
		hermiteSpline.UpdateSpline(forceNewMesh: true);
		PinTransformToSpline[] array3 = array;
		for (int i = 0; i < array3.Length; i++)
		{
			array3[i].Spline = hermiteSpline;
		}
		HasGenerated = true;
		if (this.Generated != null)
		{
			this.Generated();
		}
		UnityEngine.Object.Destroy(this);
	}

	private void LateUpdate()
	{
		if (!Application.isPlaying && (bool)template && (bool)pinnedControlPoint && (bool)hangingControlPoint)
		{
			UpdateTextureTiling(template, pinnedControlPoint.transform, hangingControlPoint.transform);
		}
	}

	private static void AddCopiedBody(GameObject obj, Rigidbody2D sourceBody)
	{
		if (!obj.GetComponent<Rigidbody2D>())
		{
			Rigidbody2D rigidbody2D = obj.AddComponent<Rigidbody2D>();
			rigidbody2D.bodyType = sourceBody.bodyType;
			rigidbody2D.mass = sourceBody.mass;
			rigidbody2D.linearDamping = sourceBody.linearDamping;
			rigidbody2D.angularDamping = sourceBody.angularDamping;
			rigidbody2D.gravityScale = sourceBody.gravityScale;
			rigidbody2D.sleepMode = sourceBody.sleepMode;
		}
	}

	private static HingeJoint2D AddCopiedHinge(GameObject obj, HingeJoint2D sourceHinge)
	{
		HingeJoint2D component = obj.GetComponent<HingeJoint2D>();
		if ((bool)component)
		{
			return component;
		}
		HingeJoint2D hingeJoint2D = obj.AddComponent<HingeJoint2D>();
		hingeJoint2D.autoConfigureConnectedAnchor = true;
		hingeJoint2D.useLimits = sourceHinge.useLimits;
		hingeJoint2D.limits = sourceHinge.limits;
		return hingeJoint2D;
	}

	private void UpdateTextureTiling(SplineBase spline, Transform startPoint, Transform endPoint)
	{
		if ((bool)scaleColliderToMatch)
		{
			Vector3 position = scaleColliderToMatch.transform.position;
			Vector2 offset = scaleColliderToMatch.offset;
			Vector2 size = scaleColliderToMatch.size;
			Vector3 position2 = startPoint.position;
			Vector3 position3 = endPoint.position;
			scaleColliderToMatch.size = new Vector2(size.x, Mathf.Abs(position2.y - position3.y));
			float y = (position2.y + position3.y) / 2f - position.y;
			scaleColliderToMatch.offset = new Vector2(offset.x, y);
		}
		if (autoTextureTiling != 0f)
		{
			float num = Vector3.Distance(startPoint.localPosition, endPoint.localPosition);
			spline.TextureTiling = autoTextureTiling * (num / (float)subdivisions / controlPointDistance);
			if (spline.TextureTilingMethod != 0)
			{
				spline.TextureTilingMethod = SplineBase.TextureTilingMethods.Explicit;
			}
		}
	}
}
