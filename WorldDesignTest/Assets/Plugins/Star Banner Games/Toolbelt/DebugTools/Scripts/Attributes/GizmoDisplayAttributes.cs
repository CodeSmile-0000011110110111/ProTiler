using System;

namespace SBG.Toolbelt.DebugTools
{
	/// <summary>
	/// Base Class for Gizmo Attributes
	/// </summary>
	public class GizmoAttribute : Attribute { }

	/// <summary>
	/// Overwrites the Color of attached Gizmo Components
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public class GizmoColor : GizmoAttribute { }

	/// <summary>
	/// Overwrites the Origin (Transform) of attached Gizmo Components
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public class GizmoOrigin : GizmoAttribute { }

	/// <summary>
	/// Overwrites the Origin Offset (Vector3) of attached Gizmo Components.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public class GizmoOffset : GizmoAttribute { }

	/// <summary>
	/// Overwrites the Rotation Offset (Vector3) of the attached Gizmo Components
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public class GizmoRotationOffset : GizmoAttribute { }

	/// <summary>
	/// Overwrites the Size (Vector2/Vector3) of the attached Gizmo Components
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public class GizmoSize : GizmoAttribute { }

	/// <summary>
	/// Overwrites the Radius (Float) of the attached Gizmo Components
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public class GizmoRadius : GizmoAttribute { }

	/// <summary>
	/// Overwrites the Segment Count (int) of the attached CircleGizmo Components
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public class GizmoSegments : GizmoAttribute { }

	/// <summary>
	/// Overwrites the Target (Transform) of the attached LineGizmo Components
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public class GizmoTarget : GizmoAttribute { }

	/// <summary>
	/// Overwrites the Target Offset (Vector3) of the attached LineGizmo Components
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public class GizmoTargetOffset : GizmoAttribute { }
}