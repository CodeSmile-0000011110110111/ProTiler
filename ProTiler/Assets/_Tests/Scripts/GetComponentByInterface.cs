// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using UnityEngine;

namespace _Tests.Scripts
{
	public interface IInterfaceTest {}

	[DisallowMultipleComponent]
	public class ComponentWithInterface : MonoBehaviour, IInterfaceTest {}

	[ExecuteAlways]
	public class GetComponentByInterface : MonoBehaviour
	{
		private void OnEnable() => TryGetByInterface();

		private void TryGetByInterface()
		{
			var com = gameObject.AddComponent<ComponentWithInterface>();
			Debug.Log("added interface component");

			var comByInterface = gameObject.GetComponent<IInterfaceTest>();
			Debug.Log("GetComponent by interface returned: " + comByInterface);

			DestroyImmediate(com);
		}
	}
}
