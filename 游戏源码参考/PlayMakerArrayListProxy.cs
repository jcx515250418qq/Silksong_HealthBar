using System.Collections;

public class PlayMakerArrayListProxy : PlayMakerCollectionProxy
{
	public ArrayList _arrayList;

	private ArrayList _snapShot;

	public ArrayList arrayList => _arrayList;

	public void Awake()
	{
		_arrayList = new ArrayList();
		PreFillArrayList();
		TakeSnapShot();
	}

	public bool isCollectionDefined()
	{
		return arrayList != null;
	}

	public void TakeSnapShot()
	{
		_snapShot = new ArrayList();
		_snapShot.AddRange(_arrayList);
	}

	public void RevertToSnapShot()
	{
		_arrayList = new ArrayList();
		_arrayList.AddRange(_snapShot);
	}

	public void Add(object value, string type, bool silent = false)
	{
		arrayList.Add(value);
		if (!silent)
		{
			dispatchEvent(addEvent, value, type);
		}
	}

	public int AddRange(ICollection collection, string type)
	{
		arrayList.AddRange(collection);
		return arrayList.Count;
	}

	public void InspectorEdit(int index)
	{
		dispatchEvent(setEvent, index, "int");
	}

	public void Set(int index, object value, string type)
	{
		arrayList[index] = value;
		dispatchEvent(setEvent, index, "int");
	}

	public bool Remove(object value, string type, bool silent = false)
	{
		if (arrayList.Contains(value))
		{
			arrayList.Remove(value);
			if (!silent)
			{
				dispatchEvent(removeEvent, value, type);
			}
			return true;
		}
		return false;
	}

	private void PreFillArrayList()
	{
		switch (preFillType)
		{
		case VariableEnum.Bool:
			arrayList.InsertRange(0, preFillBoolList);
			break;
		case VariableEnum.Color:
			arrayList.InsertRange(0, preFillColorList);
			break;
		case VariableEnum.Float:
			arrayList.InsertRange(0, preFillFloatList);
			break;
		case VariableEnum.GameObject:
			arrayList.InsertRange(0, preFillGameObjectList);
			break;
		case VariableEnum.Int:
			arrayList.InsertRange(0, preFillIntList);
			break;
		case VariableEnum.Material:
			arrayList.InsertRange(0, preFillMaterialList);
			break;
		case VariableEnum.Quaternion:
			arrayList.InsertRange(0, preFillQuaternionList);
			break;
		case VariableEnum.Rect:
			arrayList.InsertRange(0, preFillRectList);
			break;
		case VariableEnum.String:
			arrayList.InsertRange(0, preFillStringList);
			break;
		case VariableEnum.Texture:
			arrayList.InsertRange(0, preFillTextureList);
			break;
		case VariableEnum.Vector2:
			arrayList.InsertRange(0, preFillVector2List);
			break;
		case VariableEnum.Vector3:
			arrayList.InsertRange(0, preFillVector3List);
			break;
		case VariableEnum.AudioClip:
			arrayList.InsertRange(0, preFillAudioClipList);
			break;
		}
	}
}
