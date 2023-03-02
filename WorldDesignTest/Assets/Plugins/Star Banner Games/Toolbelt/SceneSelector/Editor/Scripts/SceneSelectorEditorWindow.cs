using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;

namespace SBG.Toolbelt.Editor
{
	public class SceneSelectorEditorWindow : EditorWindow
	{
		[System.Serializable]
		private class SceneData
        {
			public List<CachedScene> VisibleScenes;
			public List<CachedScene> HiddenScenes;

			public SceneData()
            {
				VisibleScenes = new List<CachedScene>();
				HiddenScenes = new List<CachedScene>();
            }

			public void RefreshData()
            {
				ValidateAction<CachedScene> isSceneInvalid = (target) => !target.Refresh();

				VisibleScenes = THelper.RemoveCollectionElements(VisibleScenes, isSceneInvalid).ToList();
				HiddenScenes = THelper.RemoveCollectionElements(HiddenScenes, isSceneInvalid).ToList();
			}
        }

		private const string JSON_FILENAME = "SceneSelectorData.json";

		private SceneData Scenes;

		private ReorderableList _sceneList;
		private ReorderableList _hiddenSceneList;

		private Queue<int> _scenesToHide = new Queue<int>();
		private Queue<int> _scenesToShow = new Queue<int>();

		private Vector2 _scrollPos = Vector2.zero;
		private bool _isEditing = false;

		private GUIStyle _sceneNameStyle;
		private GUIStyle _hiddenNameStyle;
		private Texture2D _hoverTexture;

		private bool _stylesBuilt = false;
		private bool _showHiddenList = false;


		[MenuItem("Window/Star Banner Games/Toolbelt/Scene Selector")]
		public static void ShowWindow()
		{
			var window = GetWindow<SceneSelectorEditorWindow>("Scene Selector");
		}

        private void OnEnable()
        {
            UpdateScenes();
			EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
		}

        private void OnDisable()
        {
			EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
		}

		private void OnPlayModeStateChanged(PlayModeStateChange change)
        {
			if (change == PlayModeStateChange.EnteredEditMode)
            {
				UpdateScenes();
				_stylesBuilt = false;
				Repaint();
			}
		}

        private void OnGUI()
		{
			if (!_stylesBuilt) BuildStyles();

			//HEADER GUI
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Scenes", EditorStyles.largeLabel);

			if (GUILayout.Button("Refresh")) UpdateScenes();

			//EDIT OR APPLY BUTTON
			string editButtonName = _isEditing ? "Apply" : "Edit";
			if (GUILayout.Button(editButtonName))
			{
				_isEditing = !_isEditing;

				if (!_isEditing) SaveSceneData();
			}

			EditorGUILayout.EndHorizontal();

			if (Scenes.VisibleScenes == null) return;

			//SCENE LIST
			_scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, EditorStyles.helpBox);

			if (_isEditing) //EDIT MODE
            {
				if (_sceneList != null) _sceneList.DoLayoutList();

				if (GUILayout.Button("Toggle Hidden")) _showHiddenList = !_showHiddenList;
				if (_showHiddenList && _hiddenSceneList != null) _hiddenSceneList.DoLayoutList();

				ProcessRemoveQueue();
			}
            else //DEFAULT MODE
            {
				for (int i = 0; i < Scenes.VisibleScenes.Count; i++)
				{
					if (Scenes.VisibleScenes[i].Asset == null)
					{
						Scenes.VisibleScenes.RemoveAt(i);
						break;
					}

					EditorGUILayout.BeginHorizontal(EditorStyles.toolbarButton);

					if (GUILayout.Button(Scenes.VisibleScenes[i].DisplayName, GetNameStyle(Scenes.VisibleScenes[i])))
					{
						if (EditorApplication.isPlaying) return;
						string scenePath = AssetDatabase.GUIDToAssetPath(Scenes.VisibleScenes[i].SceneGUID);
						EditorSceneManager.OpenScene(scenePath);
						UpdateScenes();
						_stylesBuilt = false;
					}

					EditorGUILayout.EndHorizontal();
				}
			}

			EditorGUILayout.EndScrollView();
		}

		private GUIStyle GetNameStyle(CachedScene button)
        {
			float h, s, v;
			Color textColor = Color.black;

			Color.RGBToHSV(button.BackgroundColor, out h, out s, out v);

			if (v < 0.5f)
            {
				textColor = Color.white;
            }

			_sceneNameStyle.normal.textColor = textColor;
			_sceneNameStyle.normal.background = button.ColorTexture;

			return _sceneNameStyle;
		}

		private void BuildStyles()
        {
			_hoverTexture = new Texture2D(1, 1);
			_hoverTexture.SetPixel(0, 0, Color.black);
			_hoverTexture.Apply();

			_sceneNameStyle = new GUIStyle(EditorStyles.toolbarButton);
			_sceneNameStyle.hover.textColor = Color.white;
			_sceneNameStyle.hover.background = _hoverTexture;
			_sceneNameStyle.alignment = TextAnchor.MiddleLeft;

			_hiddenNameStyle = new GUIStyle(EditorStyles.label);
			_hiddenNameStyle.normal.textColor = Color.gray;
			_hiddenNameStyle.hover.textColor = Color.gray;
			_hiddenNameStyle.active.textColor = Color.gray;

			_stylesBuilt = true;
		}

        private void UpdateScenes()
        {
            LoadSceneData();

            Scenes.RefreshData();

            AddMissingScenes();

            _sceneList = new ReorderableList(Scenes.VisibleScenes, typeof(CachedScene), true, true, false, false);
            _sceneList.drawHeaderCallback += DrawSceneListHeader;
            _sceneList.drawElementCallback += DrawSceneListElement;

            _hiddenSceneList = new ReorderableList(Scenes.HiddenScenes, typeof(CachedScene), false, true, false, false);
            _hiddenSceneList.drawHeaderCallback += DrawHiddenListHeader;
            _hiddenSceneList.drawElementCallback += DrawHiddenListElement;

            SaveSceneData();
        }

        private void AddMissingScenes()
        {
            string[] guids = AssetDatabase.FindAssets(string.Format("t:Scene"));

            foreach (string guid in guids)
            {
                bool guidExists = false;

                foreach (CachedScene scene in Scenes.VisibleScenes)
                {
                    if (scene.SceneGUID.ToString() == guid)
                    {
                        guidExists = true; break;
                    }
                }

                if (guidExists) continue;

                foreach (CachedScene scene in Scenes.HiddenScenes)
                {
                    if (scene.SceneGUID == guid)
                    {
                        guidExists = true; break;
                    }
                }

                if (!guidExists)
                {
                    Scenes.VisibleScenes.Add(new CachedScene(guid));
                }
            }
        }

        private void DrawSceneListHeader(Rect rect)
		{
			EditorGUI.LabelField(rect, "Visible Scenes");
		}

		private void DrawSceneListElement(Rect rect, int index, bool isActive, bool isFocused)
        {
			Rect labelRect = new Rect(rect.x, rect.y, 150, EditorGUIUtility.singleLineHeight);
			Rect useAssetNameRect = new Rect(rect.x+160, rect.y, 120, EditorGUIUtility.singleLineHeight);
			Rect colorRect = new Rect(rect.x+rect.width-110, rect.y, 50, EditorGUIUtility.singleLineHeight);
			Rect hideRect = new Rect(rect.x+rect.width-50, rect.y, 50, EditorGUIUtility.singleLineHeight);


			CachedScene scene = Scenes.VisibleScenes[index];

			Color softCol = scene.BackgroundColor;
			softCol.a = 0.1f;
			EditorGUI.DrawRect(rect, softCol);

			if (scene.UseCustomName) scene.CustomName = EditorGUI.TextField(labelRect, scene.CustomName);
			else EditorGUI.LabelField(labelRect, scene.DisplayName);

			bool useCustomName = EditorGUI.ToggleLeft(useAssetNameRect, "Custom Name", scene.UseCustomName);
			
			if (useCustomName != scene.UseCustomName)
            {
				scene.UseCustomName = useCustomName;
				scene.Refresh();
            }

			scene.SetColor(EditorGUI.ColorField(colorRect, scene.BackgroundColor));

            if (GUI.Button(hideRect, "Hide"))
            {
				Scenes.HiddenScenes.Add(scene);
				_scenesToHide.Enqueue(index);
            }
        }

		private void DrawHiddenListHeader(Rect rect)
        {
			EditorGUI.LabelField(rect, "Hidden Scenes");
		}

		private void DrawHiddenListElement(Rect rect, int index, bool isActive, bool isFocused)
		{
			if (Scenes.HiddenScenes[index].Asset == null) return;

			Rect labelRect = new Rect(rect.x, rect.y, 400, EditorGUIUtility.singleLineHeight);
			Rect hideRect = new Rect(rect.x + rect.width - 50, rect.y, 50, EditorGUIUtility.singleLineHeight);

			EditorGUI.LabelField(labelRect, Scenes.HiddenScenes[index].DisplayName, _hiddenNameStyle);

			if (GUI.Button(hideRect, "Show"))
			{
				Scenes.VisibleScenes.Add(Scenes.HiddenScenes[index]);
				_scenesToShow.Enqueue(index);
			}
		}

		private void ProcessRemoveQueue()
        {
            while (_scenesToHide.Count > 0)
            {
				int index = _scenesToHide.Dequeue();

				Scenes.VisibleScenes.RemoveAt(index);
            }

			while (_scenesToShow.Count > 0)
			{
				int index = _scenesToShow.Dequeue();

				Scenes.HiddenScenes.RemoveAt(index);
			}
		}

		private void LoadSceneData()
        {
			string dataPath = GetSceneDataPath();

			if (!File.Exists(dataPath)) return;

			string data = File.ReadAllText(dataPath);

			if (string.IsNullOrEmpty(data))
            {
				Scenes = new SceneData();
				return;
            }

			Scenes = JsonUtility.FromJson<SceneData>(data);
		}

		private void SaveSceneData()
        {
			string filePath = GetSceneDataPath();

			if (string.IsNullOrEmpty(filePath)) return;

			string fileText = JsonUtility.ToJson(Scenes, true);

			File.WriteAllText(filePath, fileText);
		}

		private string GetSceneDataPath()
        {
			string[] files = Directory.GetFiles(Application.dataPath, JSON_FILENAME, SearchOption.AllDirectories);

			if (files.Length == 0)
			{
				Debug.LogError("SceneSelector is missing an important script! Try reimporting it.");
				return null;
			}

			return files[0].Replace("\\", "/");
		}
	}
}