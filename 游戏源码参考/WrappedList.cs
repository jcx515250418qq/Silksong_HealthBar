using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

[Serializable]
[JsonObject(MemberSerialization.OptIn)]
public class WrappedList<T>
{
	[SerializeField]
	[JsonProperty]
	private List<T> list = new List<T>();

	public List<T> List => list;
}
