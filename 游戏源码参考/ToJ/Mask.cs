using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ToJ
{
	[ExecuteInEditMode]
	[AddComponentMenu("Alpha Mask")]
	public class Mask : MonoBehaviour
	{
		public enum MappingAxis
		{
			X = 0,
			Y = 1,
			Z = 2
		}

		private bool shaderErrorLogged;

		[SerializeField]
		private MappingAxis _maskMappingWorldAxis = MappingAxis.Z;

		[SerializeField]
		private bool _invertAxis;

		[SerializeField]
		private bool _clampAlphaHorizontally;

		[SerializeField]
		private bool _clampAlphaVertically;

		[SerializeField]
		private float _clampingBorder = 0.01f;

		[SerializeField]
		private bool _useMaskAlphaChannel;

		private Shader _maskedSpriteWorldCoordsShader;

		private Shader _maskedUnlitWorldCoordsShader;

		public MappingAxis maskMappingWorldAxis
		{
			get
			{
				return _maskMappingWorldAxis;
			}
			set
			{
				ChangeMappingAxis(value, _maskMappingWorldAxis, _invertAxis);
				_maskMappingWorldAxis = value;
			}
		}

		public bool invertAxis
		{
			get
			{
				return _invertAxis;
			}
			set
			{
				ChangeMappingAxis(_maskMappingWorldAxis, _maskMappingWorldAxis, value);
				_invertAxis = value;
			}
		}

		public bool clampAlphaHorizontally
		{
			get
			{
				return _clampAlphaHorizontally;
			}
			set
			{
				SetMaskBoolValueInMaterials("_ClampHoriz", value);
				_clampAlphaHorizontally = value;
			}
		}

		public bool clampAlphaVertically
		{
			get
			{
				return _clampAlphaVertically;
			}
			set
			{
				SetMaskBoolValueInMaterials("_ClampVert", value);
				_clampAlphaVertically = value;
			}
		}

		public float clampingBorder
		{
			get
			{
				return _clampingBorder;
			}
			set
			{
				SetMaskFloatValueInMaterials("_ClampBorder", value);
				_clampingBorder = value;
			}
		}

		public bool useMaskAlphaChannel
		{
			get
			{
				return _useMaskAlphaChannel;
			}
			set
			{
				SetMaskBoolValueInMaterials("_UseAlphaChannel", value);
				_useMaskAlphaChannel = value;
			}
		}

		private void Start()
		{
			_maskedSpriteWorldCoordsShader = Shader.Find("Alpha Masked/Sprites Alpha Masked - World Coords");
			_maskedUnlitWorldCoordsShader = Shader.Find("Alpha Masked/Unlit Alpha Masked - World Coords");
			MeshRenderer component = GetComponent<MeshRenderer>();
			GetComponent<MeshFilter>();
			if (Application.isPlaying && component != null)
			{
				component.enabled = false;
			}
		}

		private void Update()
		{
			if (_maskedSpriteWorldCoordsShader == null)
			{
				_maskedSpriteWorldCoordsShader = Shader.Find("Alpha Masked/Sprites Alpha Masked - World Coords");
			}
			if (_maskedUnlitWorldCoordsShader == null)
			{
				_maskedUnlitWorldCoordsShader = Shader.Find("Alpha Masked/Unlit Alpha Masked - World Coords");
			}
			if (_maskedSpriteWorldCoordsShader == null || _maskedUnlitWorldCoordsShader == null)
			{
				if (!shaderErrorLogged)
				{
					Debug.LogError("Shaders necessary for masking don't seem to be present in the project.");
				}
			}
			else
			{
				if (!base.transform.hasChanged)
				{
					return;
				}
				base.transform.hasChanged = false;
				if (maskMappingWorldAxis == MappingAxis.X && (Mathf.Abs(Mathf.DeltaAngle(base.transform.eulerAngles.x, 0f)) > 0.01f || Mathf.Abs(Mathf.DeltaAngle(base.transform.eulerAngles.y, invertAxis ? (-90) : 90)) > 0.01f))
				{
					base.transform.eulerAngles = new Vector3(0f, invertAxis ? 270 : 90, base.transform.eulerAngles.z);
				}
				else if (maskMappingWorldAxis == MappingAxis.Y && (Mathf.Abs(Mathf.DeltaAngle(base.transform.eulerAngles.x, invertAxis ? (-90) : 90)) > 0.01f || Mathf.Abs(Mathf.DeltaAngle(base.transform.eulerAngles.z, 0f)) > 0.01f))
				{
					base.transform.eulerAngles = new Vector3(invertAxis ? (-90) : 90, base.transform.eulerAngles.y, 0f);
				}
				else if (maskMappingWorldAxis == MappingAxis.Z && (Mathf.Abs(Mathf.DeltaAngle(base.transform.eulerAngles.x, 0f)) > 0.01f || Mathf.Abs(Mathf.DeltaAngle(base.transform.eulerAngles.y, invertAxis ? (-180) : 0)) > 0.01f))
				{
					base.transform.eulerAngles = new Vector3(0f, invertAxis ? (-180) : 0, base.transform.eulerAngles.z);
				}
				if (!(base.transform.parent != null))
				{
					return;
				}
				Renderer[] componentsInChildren = base.transform.parent.gameObject.GetComponentsInChildren<Renderer>();
				Graphic[] componentsInChildren2 = base.transform.parent.gameObject.GetComponentsInChildren<Graphic>();
				List<Material> list = new List<Material>();
				Dictionary<Material, Graphic> dictionary = new Dictionary<Material, Graphic>();
				Renderer[] array = componentsInChildren;
				foreach (Renderer renderer in array)
				{
					if (!(renderer.gameObject != base.gameObject))
					{
						continue;
					}
					Material[] sharedMaterials = renderer.sharedMaterials;
					foreach (Material item in sharedMaterials)
					{
						if (!list.Contains(item))
						{
							list.Add(item);
						}
					}
				}
				Graphic[] array2 = componentsInChildren2;
				foreach (Graphic graphic in array2)
				{
					if (graphic.gameObject != base.gameObject && !list.Contains(graphic.material))
					{
						list.Add(graphic.material);
						Canvas canvas = graphic.canvas;
						if (canvas.renderMode == RenderMode.ScreenSpaceOverlay || (canvas.renderMode == RenderMode.ScreenSpaceCamera && canvas.worldCamera == null))
						{
							dictionary.Add(list[list.Count - 1], graphic);
						}
					}
				}
				foreach (Material item2 in list)
				{
					if (item2.shader.ToString() == _maskedSpriteWorldCoordsShader.ToString() && item2.shader.GetInstanceID() != _maskedSpriteWorldCoordsShader.GetInstanceID())
					{
						_maskedSpriteWorldCoordsShader = null;
					}
					if (item2.shader.ToString() == _maskedUnlitWorldCoordsShader.ToString() && item2.shader.GetInstanceID() != _maskedUnlitWorldCoordsShader.GetInstanceID())
					{
						_maskedUnlitWorldCoordsShader = null;
					}
					if (!(item2.shader == _maskedSpriteWorldCoordsShader) && !(item2.shader == _maskedUnlitWorldCoordsShader))
					{
						continue;
					}
					item2.DisableKeyword("_SCREEN_SPACE_UI");
					Vector2 value = new Vector2(1f / base.transform.lossyScale.x, 1f / base.transform.lossyScale.y);
					Vector2 vector = Vector2.zero;
					float num = 0f;
					int num2 = 1;
					if (maskMappingWorldAxis == MappingAxis.X)
					{
						num2 = (invertAxis ? 1 : (-1));
						vector = new Vector2(0f - base.transform.position.z, 0f - base.transform.position.y);
						num = (float)num2 * base.transform.eulerAngles.z;
					}
					else if (maskMappingWorldAxis == MappingAxis.Y)
					{
						vector = new Vector2(0f - base.transform.position.x, 0f - base.transform.position.z);
						num = 0f - base.transform.eulerAngles.y;
					}
					else if (maskMappingWorldAxis == MappingAxis.Z)
					{
						num2 = ((!invertAxis) ? 1 : (-1));
						vector = new Vector2(0f - base.transform.position.x, 0f - base.transform.position.y);
						num = (float)num2 * base.transform.eulerAngles.z;
					}
					RectTransform component = GetComponent<RectTransform>();
					if (component != null)
					{
						Rect rect = component.rect;
						vector += (Vector2)(base.transform.right * (component.pivot.x - 0.5f) * rect.width * base.transform.lossyScale.x + base.transform.up * (component.pivot.y - 0.5f) * rect.height * base.transform.lossyScale.y);
						value.x /= rect.width;
						value.y /= rect.height;
					}
					if (dictionary.ContainsKey(item2))
					{
						vector = dictionary[item2].transform.InverseTransformVector(vector);
						switch (maskMappingWorldAxis)
						{
						case MappingAxis.X:
							vector.x *= dictionary[item2].transform.lossyScale.z;
							vector.y *= dictionary[item2].transform.lossyScale.y;
							break;
						case MappingAxis.Y:
							vector.x *= dictionary[item2].transform.lossyScale.x;
							vector.y *= dictionary[item2].transform.lossyScale.z;
							break;
						case MappingAxis.Z:
							vector.x *= dictionary[item2].transform.lossyScale.x;
							vector.y *= dictionary[item2].transform.lossyScale.y;
							break;
						}
						Canvas canvas2 = dictionary[item2].canvas;
						vector /= canvas2.scaleFactor;
						vector = RotateVector(vector, dictionary[item2].transform.eulerAngles);
						vector += canvas2.GetComponent<RectTransform>().sizeDelta * 0.5f;
						value *= canvas2.scaleFactor;
						item2.EnableKeyword("_SCREEN_SPACE_UI");
					}
					Vector2 mainTextureScale = base.gameObject.GetComponent<Renderer>().sharedMaterial.mainTextureScale;
					value.x *= mainTextureScale.x;
					value.y *= mainTextureScale.y;
					value.x *= num2;
					Vector2 vector2 = vector;
					float num3 = Mathf.Sin((0f - num) * (MathF.PI / 180f));
					float num4 = Mathf.Cos((0f - num) * (MathF.PI / 180f));
					vector.x = (num4 * vector2.x - num3 * vector2.y) * value.x + 0.5f * mainTextureScale.x;
					vector.y = (num3 * vector2.x + num4 * vector2.y) * value.y + 0.5f * mainTextureScale.y;
					vector += base.gameObject.GetComponent<Renderer>().sharedMaterial.mainTextureOffset;
					item2.SetTextureOffset("_AlphaTex", vector);
					item2.SetTextureScale("_AlphaTex", value);
					item2.SetFloat("_MaskRotation", num * (MathF.PI / 180f));
				}
			}
		}

		private Vector3 RotateVector(Vector3 point, Vector3 angles)
		{
			return Quaternion.Euler(angles) * point;
		}

		private void SetMaskMappingAxisInMaterials(MappingAxis mappingAxis)
		{
			if (base.transform.parent == null)
			{
				return;
			}
			Renderer[] componentsInChildren = base.transform.parent.gameObject.GetComponentsInChildren<Renderer>();
			Graphic[] componentsInChildren2 = base.transform.parent.gameObject.GetComponentsInChildren<Graphic>();
			List<Material> list = new List<Material>();
			Renderer[] array = componentsInChildren;
			foreach (Renderer renderer in array)
			{
				if (!(renderer.gameObject != base.gameObject))
				{
					continue;
				}
				Material[] sharedMaterials = renderer.sharedMaterials;
				foreach (Material material in sharedMaterials)
				{
					if (!list.Contains(material))
					{
						list.Add(material);
						SetMaskMappingAxisInMaterial(mappingAxis, material);
					}
				}
			}
			Graphic[] array2 = componentsInChildren2;
			foreach (Graphic graphic in array2)
			{
				if (graphic.gameObject != base.gameObject && !list.Contains(graphic.material))
				{
					list.Add(graphic.material);
					SetMaskMappingAxisInMaterial(mappingAxis, graphic.material);
				}
			}
		}

		public void SetMaskMappingAxisInMaterial(MappingAxis mappingAxis, Material material)
		{
			if (material.shader == _maskedSpriteWorldCoordsShader || material.shader == _maskedUnlitWorldCoordsShader)
			{
				switch (mappingAxis)
				{
				case MappingAxis.X:
					material.SetFloat("_Axis", 0f);
					material.EnableKeyword("_AXIS_X");
					material.DisableKeyword("_AXIS_Y");
					material.DisableKeyword("_AXIS_Z");
					break;
				case MappingAxis.Y:
					material.SetFloat("_Axis", 1f);
					material.DisableKeyword("_AXIS_X");
					material.EnableKeyword("_AXIS_Y");
					material.DisableKeyword("_AXIS_Z");
					break;
				case MappingAxis.Z:
					material.SetFloat("_Axis", 2f);
					material.DisableKeyword("_AXIS_X");
					material.DisableKeyword("_AXIS_Y");
					material.EnableKeyword("_AXIS_Z");
					break;
				}
			}
		}

		private void SetMaskFloatValueInMaterials(string variable, float value)
		{
			if (base.transform.parent == null)
			{
				return;
			}
			Renderer[] componentsInChildren = base.transform.parent.gameObject.GetComponentsInChildren<Renderer>();
			Graphic[] componentsInChildren2 = base.transform.parent.gameObject.GetComponentsInChildren<Graphic>();
			List<Material> list = new List<Material>();
			Renderer[] array = componentsInChildren;
			foreach (Renderer renderer in array)
			{
				if (!(renderer.gameObject != base.gameObject))
				{
					continue;
				}
				Material[] sharedMaterials = renderer.sharedMaterials;
				foreach (Material material in sharedMaterials)
				{
					if (!list.Contains(material))
					{
						list.Add(material);
						material.SetFloat(variable, value);
					}
				}
			}
			Graphic[] array2 = componentsInChildren2;
			foreach (Graphic graphic in array2)
			{
				if (graphic.gameObject != base.gameObject && !list.Contains(graphic.material))
				{
					list.Add(graphic.material);
					graphic.material.SetFloat(variable, value);
				}
			}
		}

		private void SetMaskBoolValueInMaterials(string variable, bool value)
		{
			if (base.transform.parent == null)
			{
				return;
			}
			Renderer[] componentsInChildren = base.transform.parent.gameObject.GetComponentsInChildren<Renderer>();
			Graphic[] componentsInChildren2 = base.transform.parent.gameObject.GetComponentsInChildren<Graphic>();
			List<Material> list = new List<Material>();
			Renderer[] array = componentsInChildren;
			foreach (Renderer renderer in array)
			{
				if (!(renderer.gameObject != base.gameObject))
				{
					continue;
				}
				Material[] sharedMaterials = renderer.sharedMaterials;
				foreach (Material material in sharedMaterials)
				{
					if (!list.Contains(material))
					{
						list.Add(material);
						SetMaskBoolValueInMaterial(variable, value, material);
					}
				}
			}
			Graphic[] array2 = componentsInChildren2;
			foreach (Graphic graphic in array2)
			{
				if (graphic.gameObject != base.gameObject && !list.Contains(graphic.material))
				{
					list.Add(graphic.material);
					SetMaskBoolValueInMaterial(variable, value, graphic.material);
				}
			}
		}

		public void SetMaskBoolValueInMaterial(string variable, bool value, Material material)
		{
			if (material.shader == _maskedSpriteWorldCoordsShader || material.shader == _maskedUnlitWorldCoordsShader)
			{
				material.SetFloat(variable, value ? 1 : 0);
				if (value)
				{
					material.EnableKeyword(variable.ToUpper() + "_ON");
				}
				else
				{
					material.DisableKeyword(variable.ToUpper() + "_ON");
				}
			}
		}

		private void CreateAndAssignQuad(Mesh mesh, float minX = -0.5f, float maxX = 0.5f, float minY = -0.5f, float maxY = 0.5f)
		{
			mesh.vertices = new Vector3[4]
			{
				new Vector3(minX, minY, 0f),
				new Vector3(maxX, minY, 0f),
				new Vector3(minX, maxY, 0f),
				new Vector3(maxX, maxY, 0f)
			};
			mesh.triangles = new int[6] { 0, 2, 1, 2, 3, 1 };
			mesh.normals = new Vector3[4]
			{
				-Vector3.forward,
				-Vector3.forward,
				-Vector3.forward,
				-Vector3.forward
			};
			mesh.uv = new Vector2[4]
			{
				new Vector2(0f, 0f),
				new Vector2(1f, 0f),
				new Vector2(0f, 1f),
				new Vector2(1f, 1f)
			};
		}

		public void SetMaskRendererActive(bool value)
		{
			if (GetComponent<Renderer>() != null)
			{
				if (value)
				{
					GetComponent<Renderer>().enabled = true;
				}
				else
				{
					GetComponent<Renderer>().enabled = false;
				}
			}
		}

		private void ChangeMappingAxis(MappingAxis currMaskMappingWorldAxis, MappingAxis prevMaskMappingWorldAxis, bool currInvertAxis)
		{
			switch (currMaskMappingWorldAxis)
			{
			case MappingAxis.X:
				if (prevMaskMappingWorldAxis == MappingAxis.Y)
				{
					base.transform.eulerAngles = new Vector3(0f, currInvertAxis ? (-90) : 90, base.transform.eulerAngles.y);
				}
				else
				{
					base.transform.eulerAngles = new Vector3(0f, currInvertAxis ? (-90) : 90, base.transform.eulerAngles.z);
				}
				break;
			case MappingAxis.Y:
				if (prevMaskMappingWorldAxis == MappingAxis.Y)
				{
					base.transform.eulerAngles = new Vector3(currInvertAxis ? (-90) : 90, base.transform.eulerAngles.y, 0f);
				}
				else
				{
					base.transform.eulerAngles = new Vector3(currInvertAxis ? (-90) : 90, base.transform.eulerAngles.z, 0f);
				}
				break;
			case MappingAxis.Z:
				if (prevMaskMappingWorldAxis == MappingAxis.Y)
				{
					base.transform.eulerAngles = new Vector3(0f, currInvertAxis ? (-180) : 0, base.transform.eulerAngles.y);
				}
				else
				{
					base.transform.eulerAngles = new Vector3(0f, currInvertAxis ? (-180) : 0, base.transform.eulerAngles.z);
				}
				break;
			}
			SetMaskMappingAxisInMaterials(currMaskMappingWorldAxis);
		}
	}
}
