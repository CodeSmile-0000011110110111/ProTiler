// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler.CodeDesign.v4.GridMap;

namespace CodeSmile.ProTiler.CodeDesign.v4.DataMaps
{
	public abstract class DataMapBase
	{
		protected IDataMapStream m_Stream;
		public DataMapBase() {}

		public DataMapBase(IDataMapStream stream) => m_Stream = stream;

		// coord to chunk key
		// hashmap of modified (unsaved) chunks
		// possibly: hashmap of loaded chunks together with access timestamp

		// create instance of undo/redo system (editor and runtime edit-mode)
		public virtual void Serialize(IBinaryWriter writer)
		{
			//writer.Add(..);
		}

		public virtual DataMapBase Deserialize(IBinaryReader reader, byte userDataVersion) =>
			// deserialize base class fields first
			//baseField = reader.ReadNext<Byte>();
			this;
	}
}
