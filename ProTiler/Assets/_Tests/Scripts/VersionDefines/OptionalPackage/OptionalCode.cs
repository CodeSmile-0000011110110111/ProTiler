#define USE_OPTIONAL_PACKAGE

using System.Collections;
using System.Collections.Generic;
using Unity.Serialization.Json;
using UnityEngine;

namespace OptionalAssembly
{
	public struct TestSerializedData
	{
		public int value;
		public bool flag;
		public string text;
		public double floating;
	}

	public class OptionalCode
	{
		public string GetOptionalString()
		{
			#if UNITY_SERIALIZATION_3_1_0_AVAILABLE
			return "optional installed, serialization is v3.1.0 or higher, here's some json: " +
			       JsonSerialization.ToJson(new TestSerializedData());
			#else
			return "optional installed, serialization is not installed or older than v3.1.0";
			#endif
		}
	}
}
