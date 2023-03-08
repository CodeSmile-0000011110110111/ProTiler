// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using Unity.Mathematics;
using UnityEngine;

namespace CodeSmile.Tile
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(TileWorld))]
	public class TileWorldMeshRenderer : MonoBehaviour
	{
		[SerializeField] private bool m_PerformFrustumCulling;
		[SerializeField] private bool m_PerformRangeCulling;
		[SerializeField] private float m_MaxRangeToCameraPos;

		private TileWorld m_TileWorld;

		private void Start() => m_TileWorld = GetComponent<TileWorld>();

		private void OnRenderObject()
		{
			var camera = Camera.current;
			if (camera == null)
				return;

			var camPos = (float3)camera.transform.position;
			var layerPos = (float3)transform.position;
			var scale = new float3(1f, 1f, 1f);
			var maxRangeSqr = m_MaxRangeToCameraPos * m_MaxRangeToCameraPos;

			var tileLayer = m_TileWorld.ActiveLayer;
			var gridSize = tileLayer.Grid.Size;
			foreach (var coordTilePair in tileLayer)
			{
				var coord = coordTilePair.Key;
				var tile = coordTilePair.Value;
				if (tile == null)
					continue;

				var prefab = tileLayer.TileSet.GetPrefab(tile.TileSetIndex);
				if (prefab == null)
					continue;

				var mesh = prefab.GetComponent<MeshFilter>().sharedMesh;
				var materials = prefab.GetComponent<MeshRenderer>().sharedMaterials;

				
				var position = tileLayer.GetTileWorldPosition(coord) + layerPos;
				var matrix = Matrix4x4.TRS(position, quaternion.identity, scale);
				if (m_PerformRangeCulling)
				{
					var distanceToCameraSqr = math.lengthsq(position - camPos);
					if (distanceToCameraSqr > maxRangeSqr) 
						continue;
				}

				if (m_PerformFrustumCulling)
				{
					//var bounds = new Bounds(position, gridSize);
					//var frustumPlanes = GeometryUtility.CalculateFrustumPlanes(camera);
					//if (GeometryUtility.TestPlanesAABB(frustumPlanes, bounds) == false) continue;
				}

				for (var i = 0; i < materials.Length; i++)
				{
					materials[i].SetPass(0);
					Graphics.DrawMeshNow(mesh, matrix, i);
				}
			}
		}
	}
}