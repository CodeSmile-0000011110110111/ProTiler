// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using UnityEditor;
using UnityEditor.Toolbars;
using UnityEngine;

[EditorToolbarElement(id, typeof(SceneView))]
internal class ExampleCreateCubeToolbarButton : EditorToolbarButton, IAccessContainerWindow
{
	// This ID is used to populate toolbar elements.
	public const string id = "ExampleToolbar/Button";

	// IAccessContainerWindow provides a way for toolbar elements to access the `EditorWindow` in which they exist.
	// Here we use `containerWindow` to focus the camera on our newly instantiated objects after creation.
	public EditorWindow containerWindow { get; set; }

	// Because this is a VisualElement, it is appropriate to place initialization logic in the constructor.
	// In this method you can also register to any additional events as required. In this example there is a tooltip, an icon, and an action.
	public ExampleCreateCubeToolbarButton()
	{
		// A toolbar element can be either text, icon, or a combination of the two. Keep in mind that if a toolbar is
		// docked horizontally the text will be clipped, so usually it's a good idea to specify an icon.
		text = "Create Cube";
		icon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/CreateCubeIcon.png");
		Debug.Log($"icon is: {icon}");
		tooltip = "Instantiate a cube in the scene.";
		clicked += OnClick;
	}

	// This method will be invoked when the `Create Cube` button is clicked.
	private void OnClick()
	{
		var newObj = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
		newObj.gameObject.AddComponent<Rigidbody>();
		newObj.gameObject.AddComponent<SphereCollider>();

		// When writing editor tools don't forget to be a good citizen and implement Undo!
		Undo.RegisterCreatedObjectUndo(newObj.gameObject, "Create Cube");

		if (containerWindow is SceneView view)
			view.FrameSelected();
	}
}