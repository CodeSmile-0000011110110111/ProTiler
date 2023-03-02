using UnityEditor;
using UnityEngine;

namespace SBG.Toolbelt.Editor
{
	[System.Serializable]
	public class CachedScene
	{
		private static float _randHue = 0;

		public SceneAsset Asset;
		public string SceneGUID;

		public string CustomName;
		public bool IsHidden;
		public bool UseCustomName;

		[SerializeField] private Color _color;

		public Texture2D ColorTexture { get; private set; }
		public Color BackgroundColor => _color;

		public string DisplayName
        {
            get
            {
				if (UseCustomName || Asset == null)
                {
					return CustomName;
                }
                else
                {
					return Asset.name;
                }
			}
        }


		public CachedScene(string guid)
		{
			SceneGUID = guid;
			string assetPath = AssetDatabase.GUIDToAssetPath(guid);

			if (string.IsNullOrEmpty(assetPath)) return;

			Asset = AssetDatabase.LoadAssetAtPath<SceneAsset>(assetPath);

			Color col = Color.HSVToRGB(_randHue, 0.5f, 1f);
			SetColor(col);

			_randHue += 0.1f;
			if (_randHue > 1) _randHue = 0;

			IsHidden = false;
			UseCustomName = false;

			if (Asset != null) CustomName = Asset.name;
		}

		public void SetColor(Color col)
        {
			_color = col;
			ColorTexture = new Texture2D(1, 1);
			ColorTexture.SetPixel(0, 0, col);
			ColorTexture.Apply();
		}

		/// <summary>
		/// Returns true if the Asset still exists
		/// </summary>
		public bool Refresh()
        {
			//Try to get Asset
			string assetPath = AssetDatabase.GUIDToAssetPath(SceneGUID);
			if (string.IsNullOrEmpty(assetPath)) return false;

			Asset = AssetDatabase.LoadAssetAtPath<SceneAsset>(assetPath);
			if (Asset == null) return false;

			//Set Display Parameters
			SetColor(_color);

			return true;
		}
	}
}