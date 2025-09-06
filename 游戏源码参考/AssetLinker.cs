using System;
using UnityEngine;

[Serializable]
public class AssetLinker<T> : IAssetLinker where T : UnityEngine.Object
{
	[SerializeField]
	private string guid;

	[NonSerialized]
	private T asset;

	public T Asset
	{
		get
		{
			return asset;
		}
		set
		{
			asset = value;
		}
	}

	public UnityEngine.Object GetAsset()
	{
		return Asset;
	}

	public void SetAsset(UnityEngine.Object asset)
	{
		Asset = asset as T;
	}

	public Type GetAssetType()
	{
		return typeof(T);
	}

	public static implicit operator T(AssetLinker<T> assetLinker)
	{
		if (assetLinker != null)
		{
			return assetLinker.Asset;
		}
		return null;
	}

	public static implicit operator AssetLinker<T>(T asset)
	{
		return new AssetLinker<T>
		{
			Asset = asset
		};
	}
}
