using System.Collections.Generic;
using HutongGames.PlayMaker;
using UnityEngine;

public abstract class PlayMakerCollectionProxy : MonoBehaviour
{
	public enum VariableEnum
	{
		GameObject = 0,
		Int = 1,
		Float = 2,
		String = 3,
		Bool = 4,
		Vector3 = 5,
		Rect = 6,
		Quaternion = 7,
		Color = 8,
		Material = 9,
		Texture = 10,
		Vector2 = 11,
		AudioClip = 12
	}

	public bool showEvents;

	public bool showContent;

	public bool TextureElementSmall;

	public bool condensedView;

	public bool liveUpdate;

	public string referenceName = "";

	public bool enablePlayMakerEvents;

	public string addEvent;

	public string setEvent;

	public string removeEvent;

	public int contentPreviewStartIndex;

	public int contentPreviewMaxRows = 10;

	public VariableEnum preFillType;

	public int preFillObjectTypeIndex;

	public int preFillCount;

	public List<string> preFillKeyList = new List<string>();

	public List<bool> preFillBoolList = new List<bool>();

	public List<Color> preFillColorList = new List<Color>();

	public List<float> preFillFloatList = new List<float>();

	public List<GameObject> preFillGameObjectList = new List<GameObject>();

	public List<int> preFillIntList = new List<int>();

	public List<Material> preFillMaterialList = new List<Material>();

	public List<Object> preFillObjectList = new List<Object>();

	public List<Quaternion> preFillQuaternionList = new List<Quaternion>();

	public List<Rect> preFillRectList = new List<Rect>();

	public List<string> preFillStringList = new List<string>();

	public List<Texture2D> preFillTextureList = new List<Texture2D>();

	public List<Vector2> preFillVector2List = new List<Vector2>();

	public List<Vector3> preFillVector3List = new List<Vector3>();

	public List<AudioClip> preFillAudioClipList = new List<AudioClip>();

	internal string getFsmVariableType(VariableType _type)
	{
		return _type.ToString();
	}

	internal void dispatchEvent(string anEvent, object value, string type)
	{
		if (enablePlayMakerEvents)
		{
			switch (type)
			{
			case "bool":
				Fsm.EventData.BoolData = (bool)value;
				break;
			case "color":
				Fsm.EventData.ColorData = (Color)value;
				break;
			case "float":
				Fsm.EventData.FloatData = (float)value;
				break;
			case "gameObject":
				Fsm.EventData.ObjectData = (GameObject)value;
				break;
			case "int":
				Fsm.EventData.IntData = (int)value;
				break;
			case "material":
				Fsm.EventData.MaterialData = (Material)value;
				break;
			case "object":
				Fsm.EventData.ObjectData = (Object)value;
				break;
			case "quaternion":
				Fsm.EventData.QuaternionData = (Quaternion)value;
				break;
			case "rect":
				Fsm.EventData.RectData = (Rect)value;
				break;
			case "string":
				Fsm.EventData.StringData = (string)value;
				break;
			case "texture":
				Fsm.EventData.TextureData = (Texture)value;
				break;
			case "vector2":
				Fsm.EventData.Vector3Data = (Vector3)value;
				break;
			case "vector3":
				Fsm.EventData.Vector3Data = (Vector3)value;
				break;
			}
			FsmEventTarget fsmEventTarget = new FsmEventTarget();
			fsmEventTarget.target = FsmEventTarget.EventTarget.BroadcastAll;
			List<Fsm> list = new List<Fsm>(Fsm.FsmList);
			if (list.Count > 0)
			{
				list[0].Event(fsmEventTarget, anEvent);
			}
		}
	}

	public void cleanPrefilledLists()
	{
		if (preFillKeyList.Count > preFillCount)
		{
			preFillKeyList.RemoveRange(preFillCount, preFillKeyList.Count - preFillCount);
		}
		if (preFillBoolList.Count > preFillCount)
		{
			preFillBoolList.RemoveRange(preFillCount, preFillBoolList.Count - preFillCount);
		}
		if (preFillColorList.Count > preFillCount)
		{
			preFillColorList.RemoveRange(preFillCount, preFillColorList.Count - preFillCount);
		}
		if (preFillFloatList.Count > preFillCount)
		{
			preFillFloatList.RemoveRange(preFillCount, preFillFloatList.Count - preFillCount);
		}
		if (preFillIntList.Count > preFillCount)
		{
			preFillIntList.RemoveRange(preFillCount, preFillIntList.Count - preFillCount);
		}
		if (preFillMaterialList.Count > preFillCount)
		{
			preFillMaterialList.RemoveRange(preFillCount, preFillMaterialList.Count - preFillCount);
		}
		if (preFillGameObjectList.Count > preFillCount)
		{
			preFillGameObjectList.RemoveRange(preFillCount, preFillGameObjectList.Count - preFillCount);
		}
		if (preFillObjectList.Count > preFillCount)
		{
			preFillObjectList.RemoveRange(preFillCount, preFillObjectList.Count - preFillCount);
		}
		if (preFillQuaternionList.Count > preFillCount)
		{
			preFillQuaternionList.RemoveRange(preFillCount, preFillQuaternionList.Count - preFillCount);
		}
		if (preFillRectList.Count > preFillCount)
		{
			preFillRectList.RemoveRange(preFillCount, preFillRectList.Count - preFillCount);
		}
		if (preFillStringList.Count > preFillCount)
		{
			preFillStringList.RemoveRange(preFillCount, preFillStringList.Count - preFillCount);
		}
		if (preFillTextureList.Count > preFillCount)
		{
			preFillTextureList.RemoveRange(preFillCount, preFillTextureList.Count - preFillCount);
		}
		if (preFillVector2List.Count > preFillCount)
		{
			preFillVector2List.RemoveRange(preFillCount, preFillVector2List.Count - preFillCount);
		}
		if (preFillVector3List.Count > preFillCount)
		{
			preFillVector3List.RemoveRange(preFillCount, preFillVector3List.Count - preFillCount);
		}
		if (preFillAudioClipList.Count > preFillCount)
		{
			preFillAudioClipList.RemoveRange(preFillCount, preFillAudioClipList.Count - preFillCount);
		}
	}
}
