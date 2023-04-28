// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler.Assets;
using UnityEngine;

public class TestRegisterLoad : MonoBehaviour
{
	public Tile3DAssetRegister register;

	private void Awake() =>
		Debug.LogError($"Awake Tile3DAssetRegister.Singleton ID: {Tile3DAssetRegister.Singleton.GetInstanceID()}");
}
