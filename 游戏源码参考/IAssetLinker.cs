using System;
using UnityEngine;

public interface IAssetLinker
{
	UnityEngine.Object GetAsset();

	void SetAsset(UnityEngine.Object asset);

	Type GetAssetType();
}
