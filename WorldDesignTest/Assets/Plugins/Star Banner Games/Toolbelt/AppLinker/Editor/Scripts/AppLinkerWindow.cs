using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SBG.Toolbelt.AppLinker.Editor
{
	/// <summary>
	/// Editor Window to change your AppLink Settings
	/// </summary>
	public class AppLinkerWindow : EditorWindow
	{
		#region Wrapper Class to Convert List to JSON
		[Serializable]
		private class AppListWrapper
        {
			public List<LinkedApp> AppList;
			public AppListWrapper(List<LinkedApp> apps) { AppList = apps; }
        }
        #endregion

		//Cached App Links
        [SerializeField] private List<LinkedApp> _linkedApps = new List<LinkedApp>();

		private Vector2 _scroll; //Scrollbar Value of Applist
		private bool _changesApplied = true;

		private GUIContent[] assetMenuOptions = new GUIContent[]
		{
			new GUIContent("Default Link", "Adds a Shortcut in the toolbar and to the Asset Context Menu"),
			new GUIContent("Toolbar Link", "Adds a Shortcut only in the toolbar. For programs that cannot open files.")
		};


		[MenuItem("Window/Star Banner Games/Toolbelt/App Linker Settings")]
		public static void ShowWindow() => GetWindow<AppLinkerWindow>("App Linker Settings");

		private void OnEnable()
        {
			LoadCurrentApps();
        }

		private void OnDisable()
        {
			//Before closing the window, display a save dialog if changes weren't applied
			if (!_changesApplied)
            {
				bool applyChanges = EditorUtility.DisplayDialog("Unsaved Changes", "You have not applied all of your changes? Would you like to apply them before closing?", "Apply", "Discard");

				if (applyChanges) ApplyChanges();
			}
        }

		private void OnGUI()
		{
			//HEADER
			EditorGUILayout.LabelField("Linked Apps", EditorStyles.boldLabel);

			if (_linkedApps.Count == 0) EditorGUILayout.LabelField("None");

			_scroll = EditorGUILayout.BeginScrollView(_scroll);

			//APP LIST
            foreach (LinkedApp app in _linkedApps)
            {
				EditorGUILayout.BeginHorizontal(EditorStyles.toolbarButton);

				//EDIT APP NAME
				if (!app.IsEditingName)
				{
					EditorGUILayout.LabelField(new GUIContent(app.Name, "App Name"), GUILayout.MaxWidth(90));
				}
				else
				{
					string newName = EditorGUILayout.DelayedTextField(app.Name, GUILayout.MaxWidth(90));

					if (newName != app.Name) //Detect & Apply Name Changes
                    {
						app.IsEditingName = false;
						Undo.RecordObject(this, "Changed App Name");
						_changesApplied = false;
						app.Name = newName;
                    }
				}

				//BUTTON TO START/STOP NAME EDITING
				if (GUILayout.Button(app.IsEditingName ? "Stop" : "Edit", GUILayout.Width(40)))
				{
					app.IsEditingName = !app.IsEditingName;
				}

				//APP ASSET MENU OPTION
				int hasAssetMenuVal = EditorGUILayout.Popup(app.HasAssetMenu ? 0 : 1, assetMenuOptions, GUILayout.Width(90));
				bool hasAssetMenu = hasAssetMenuVal == 0;

				if(app.HasAssetMenu != hasAssetMenu) //Detect & Apply Asset Menu Changes
                {
					Undo.RecordObject(this, "Changed Asset Menu Option");
					_changesApplied = false;
					app.HasAssetMenu = hasAssetMenu;
				}

				//APP PATH
				EditorGUILayout.LabelField(new GUIContent(app.Path, app.Path).text, GUILayout.MinWidth(50));

				//CHANGE APP PATH
				if (GUILayout.Button(new GUIContent("...", "Change Path"), GUILayout.MaxWidth(25)))
                {
					string path = GetAppPath();

					if (!string.IsNullOrEmpty(path)) //Apply new App Path
                    {
						Undo.RecordObject(this, "Changed App Path");
						_changesApplied = false;
						app.Path = path;
                    }
                }

				//REMOVE APP
				if (GUILayout.Button(new GUIContent("-", "Remove App"), GUILayout.MaxWidth(25)))
				{
					if (EditorUtility.DisplayDialog("Remove App", $"You are about to remove {app.Name} from the linked applications. Are you sure?", "Yes", "No"))
                    {
						Undo.RecordObject(this, "Removed App");
						_changesApplied = false;
						_linkedApps.Remove(app);
						EditorGUILayout.EndHorizontal();
						EditorGUILayout.EndScrollView();
						return;
					}
				}

				EditorGUILayout.EndHorizontal();
            }

			EditorGUILayout.EndScrollView();

			EditorGUILayout.Separator();

			//ADD NEW APP
			if (GUILayout.Button("Add Application", GUILayout.Height(30)))
			{
				string path = GetAppPath();

				if (string.IsNullOrEmpty(path)) return;

				string[] seperatedPath = path.Split(new string[] { "\\", "/", ".exe" }, StringSplitOptions.RemoveEmptyEntries);

				string appName = seperatedPath[seperatedPath.Length - 1];

				Undo.RecordObject(this, "Added App");
				_changesApplied = false;
				_linkedApps.Add(new LinkedApp(appName, path));
			}

			//APPLY CHANGES
			if (GUILayout.Button("Apply", GUILayout.Height(30)))
            {
				ApplyChanges();
            }
		}

		private string GetAppPath() => EditorUtility.OpenFilePanel("Application Path", "", "exe");

		private void ApplyChanges()
        {
			_changesApplied = true; //Needs to be set BEFORE "SaveAssets" and "ImportAsset" is called

			//Get Path to CS script with Context Menus
			string appLinksPath = GetFilePath("AppLinks.cs");
			if (string.IsNullOrEmpty(appLinksPath)) return;

			//Update File Content
			string appLinkContent = File.ReadAllText(appLinksPath);
			appLinkContent = ClearContextMenus(appLinkContent);
			appLinkContent = InsertContextMenus(appLinkContent);

			File.WriteAllText(appLinksPath, appLinkContent);
			SaveCurrentApps();

			//Save Assets & Recompile
			AssetDatabase.SaveAssets();
			Debug.Log("App Links applied successfully!");

			string relativePath = appLinksPath.Replace(Application.dataPath, "Assets");
			AssetDatabase.ImportAsset(relativePath);
		}

		private string GetFilePath(string filename)
        {
			string[] files = Directory.GetFiles(Application.dataPath, filename, SearchOption.AllDirectories);

			if (files.Length == 0)
			{
				Debug.LogError("App Linker is missing an important script! Try reimporting it.");
				return null;
			}

			return files[0].Replace("\\", "/");
		}

		private string ClearContextMenus(string fileContent)
        {
			string regionString = "#region LINKS";

			int startIndex = fileContent.IndexOf(regionString) + regionString.Length;
			int endIndex = fileContent.IndexOf("#endregion");

			fileContent = fileContent.Remove(startIndex, endIndex - startIndex);
			fileContent = fileContent.Insert(startIndex, "\r\n\r\n\t\t");

			return fileContent;
		}

		private string InsertContextMenus(string fileContent)
        {
			//GET TEMPLATE FILE
			string templatePath = GetFilePath("AppLinkTemplate.txt");
			if (string.IsNullOrEmpty(templatePath)) return fileContent;

			string templateContent = File.ReadAllText(templatePath);

			//USE TEMPLATE TO INSERT ALL APP MENUS
			foreach (LinkedApp app in _linkedApps)
			{
				//Turn the app name into a string that can be used in code without errors
				string safeName = GetSafeAppName(app.Name);

				string modifiedTemplate = templateContent.Replace("#DP_NAME#", app.Name) //Insert Display Name into Context Menu Strings
														 .Replace("#NAME#", safeName)	 //Insert Safe Name into Function names
														 .Replace("#PATH#", app.Path);	 //Insert App Path into Code

				//If the App doesnt have an Asset Menu,
				//cut out the Asset Menu Chunk out of the Template Code
				if (app.HasAssetMenu == false)
                {
					int cutoff = modifiedTemplate.IndexOf("ASSET_MENU");
					modifiedTemplate = modifiedTemplate.Substring(0, cutoff);
					modifiedTemplate += $"END: {safeName}\r\n\t\t";
                }

				//Apply Content
				fileContent = fileContent.Replace("#region LINKS", modifiedTemplate);
			}

			return fileContent;
		}

		private string GetSafeAppName(string appName)
        {
			ValidateAction<char> removeCondition = (character) =>
			{
				if (!char.IsLetterOrDigit(character) && character != '_')
				{
					return true;
				}

				return false;
			};

			char[] newChars = THelper.RemoveCollectionElements(appName, removeCondition).ToArray();
			return "App_" + new string(newChars);
		}

		private void LoadCurrentApps()
        {
			string fileContent = File.ReadAllText(GetFilePath("AppLinks.json"));

			AppListWrapper newApps = JsonUtility.FromJson<AppListWrapper>(fileContent);

			if (newApps == null || newApps.AppList == null)
            {
				_linkedApps = new List<LinkedApp>();
			}
            else
            {
				_linkedApps = newApps.AppList;
			}
		}

		private void SaveCurrentApps()
        {
			string path = GetFilePath("AppLinks.json");

			AppListWrapper apps = new AppListWrapper(_linkedApps);
			string fileContent = JsonUtility.ToJson(apps, true);
			File.WriteAllText(path, fileContent);
		}
	}
}