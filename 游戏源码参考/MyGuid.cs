using System;
using UnityEngine;

[ExecuteInEditMode]
public class MyGuid : MonoBehaviour
{
	[SerializeField]
	private string guidAsString;

	private Guid _guid;

	public Guid guid
	{
		get
		{
			if (_guid == Guid.Empty && !string.IsNullOrEmpty(guidAsString))
			{
				_guid = new Guid(guidAsString);
			}
			return _guid;
		}
	}

	public void Generate()
	{
		_guid = Guid.NewGuid();
		guidAsString = guid.ToString();
	}

	public string GetGuid()
	{
		return guidAsString;
	}
}
