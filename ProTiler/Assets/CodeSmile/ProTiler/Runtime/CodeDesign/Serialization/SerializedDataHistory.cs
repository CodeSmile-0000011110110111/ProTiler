// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

namespace CodeSmile.ProTiler.Runtime.CodeDesign.Serialization
{
	/// <summary>
	/// This class retains a history of serialized data blobs (as byte[]) for Undo/Redo.
	/// We have to store these blobs together with the undo group (key) and
	/// some meta data like what this chunk is (type) or what to do with it when deserializing.
	/// </summary>
	public class SerializedChunkHistory
	{
		
	}
}
