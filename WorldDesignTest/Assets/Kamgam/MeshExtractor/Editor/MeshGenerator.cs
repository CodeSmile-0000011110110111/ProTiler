using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Unity.Collections;
using System.Linq;

namespace Kamgam.MeshExtractor
{
    public class MeshGenerator
    {
        /// <summary>
        /// Takes the selected triangles and generates a new mesh from them.
        /// </summary>
        /// <param name="pivotGlobal"></param>
        /// <param name="selectedTriangles"></param>
        /// <param name="filePath">File path relative to the Assets directory. You don not have to include "Assets/".</param>
        /// <param name="replaceOldFiles"></param>
        /// <param name="preserveSubMeshes"></param>
        /// <param name="combineSubMeshesBasedOnMaterials"></param>
        /// <param name="combineMeshes"></param>
        /// <param name="saveAsObj"></param>
        /// <param name="extractTextures"></param>
        /// <param name="createPrefab"></param>
        /// <param name="showProgress"></param>
        public static void GenerateMesh(
            Vector3 pivotGlobal, HashSet<SelectedTriangle> selectedTriangles,
            string filePath, bool replaceOldFiles,
            bool preserveSubMeshes = true, bool combineSubMeshesBasedOnMaterials = true, bool combineMeshes = true,
            bool saveAsObj = false, bool extractTextures = false, bool createPrefab = true, bool showProgress = true)
        {
            // A list linking new meshes to selected triangles.
            var newMeshToTriMap = new List<(Mesh, SelectedTriangle)>();

            // Notice that the vertices stored in the SelectedTriangles are NOT used. Instead the vertices
            // are directly copied form the source mesh (or baked mesh if skinned).
            var selectedTrisPerMesh = new Dictionary<Mesh, List<SelectedTriangle>>();
            var vertices = new Dictionary<Mesh, List<Vector3>>();
            var triangles = new Dictionary<Mesh, List<int[]>>();
            var normals = new Dictionary<Mesh, List<Vector3>>();
            var uvs = new Dictionary<Mesh, List<Vector2>>();
            var uv2s = new Dictionary<Mesh, List<Vector2>>();
            var uv3s = new Dictionary<Mesh, List<Vector2>>();
            var uv4s = new Dictionary<Mesh, List<Vector2>>();
            var uv5s = new Dictionary<Mesh, List<Vector2>>();
            var uv6s = new Dictionary<Mesh, List<Vector2>>();
            var uv7s = new Dictionary<Mesh, List<Vector2>>();
            var uv8s = new Dictionary<Mesh, List<Vector2>>();
            var colors = new Dictionary<Mesh, List<Color>>();
            var tangents = new Dictionary<Mesh, List<Vector4>>();
            var materials = new Dictionary<Mesh, List<Material>>();
            var bindPoses = new Dictionary<Mesh, List<Matrix4x4>>();

            foreach (var tri in selectedTriangles)
            {
                if (!vertices.ContainsKey(tri.Mesh))
                {
                    // Create new triangle list for this mesh
                    selectedTrisPerMesh.Add(tri.Mesh, new List<SelectedTriangle>());

                    // Vertices
                    // Convert vertices to world space and match the desired pivot.
                    tri.Mesh.GetVertices(getOrCreateMeshData(tri.Mesh, vertices));
                    int numOfVertices = vertices[tri.Mesh].Count;
                    for (int i = 0; i < numOfVertices; i++)
                    {
                        vertices[tri.Mesh][i] = tri.Transform.TransformPoint(vertices[tri.Mesh][i]) - pivotGlobal;
                    }

                    // Triangles
                    var tris = new List<int[]>();
                    for (int i = 0; i < tri.Mesh.subMeshCount; i++)
                    {
                        tris.Add(tri.Mesh.GetTriangles(i));
                    }
                    triangles.Add(tri.Mesh, tris);

                    // Normals
                    // Convert normals to world space.
                    tri.Mesh.GetNormals(getOrCreateMeshData(tri.Mesh, normals));
                    int numOfNormals = normals[tri.Mesh].Count;
                    for (int i = 0; i < numOfNormals; i++)
                    {
                        normals[tri.Mesh][i] = tri.Transform.TransformDirection(normals[tri.Mesh][i]);
                    }

                    // UVs
                    tri.Mesh.GetUVs(0, getOrCreateMeshData(tri.Mesh, uvs));
                    tri.Mesh.GetUVs(1, getOrCreateMeshData(tri.Mesh, uv2s));
                    tri.Mesh.GetUVs(2, getOrCreateMeshData(tri.Mesh, uv3s));
                    tri.Mesh.GetUVs(3, getOrCreateMeshData(tri.Mesh, uv4s));
                    tri.Mesh.GetUVs(4, getOrCreateMeshData(tri.Mesh, uv5s));
                    tri.Mesh.GetUVs(5, getOrCreateMeshData(tri.Mesh, uv6s));
                    tri.Mesh.GetUVs(6, getOrCreateMeshData(tri.Mesh, uv7s));
                    tri.Mesh.GetUVs(7, getOrCreateMeshData(tri.Mesh, uv8s));

                    // Colors
                    tri.Mesh.GetColors(getOrCreateMeshData(tri.Mesh, colors));

                    // Tangents
                    tri.Mesh.GetTangents(getOrCreateMeshData(tri.Mesh, tangents));

                    // Materials
                    var meshRenderer = tri.Transform.GetComponent<MeshRenderer>();
                    if (meshRenderer != null)
                    {
                        meshRenderer.GetSharedMaterials(getOrCreateMeshData(tri.Mesh, materials));
                    }
                    else
                    {
                        var skinnedMeshRenderer = tri.Component as SkinnedMeshRenderer;
                        if (skinnedMeshRenderer != null)
                        {
                            // tri.Mesh is already baked
                            skinnedMeshRenderer.GetSharedMaterials(getOrCreateMeshData(tri.Mesh, materials));
                        }
                        else
                        {
                            // init empty if no renderer is found (no materials)
                            getOrCreateMeshData(tri.Mesh, materials);
                        }
                    }
                }

                // Add triangle to mesh list
                selectedTrisPerMesh[tri.Mesh].Add(tri);
            }

            if (showProgress)
                EditorUtility.DisplayProgressBar("Extracting Mesh", "Gathering and sorting " + selectedTriangles.Count + " triangles " + (selectedTriangles.Count > 10000 ? "(this may take a while)" : "") + " ..", 0.2f);

            // new mesh to new (possibly merged) sub mesh materials list
            var materialsForSubMeshes = new Dictionary<Mesh, List<Material>>();

            foreach (var kv in selectedTrisPerMesh)
            {
                var mesh = kv.Key;
                var selectedTris = kv.Value;

                // gather old vertices and sort them
                var newVertexIndices = new List<int>();
                foreach (var tri in selectedTris)
                {
                    int a = tri.TriangleIndices[0];
                    if (!newVertexIndices.Contains(a))
                    {
                        newVertexIndices.Add(a);
                    }

                    int b = tri.TriangleIndices[1];
                    if (!newVertexIndices.Contains(b))
                    {
                        newVertexIndices.Add(b);
                    }

                    int c = tri.TriangleIndices[2];
                    if (!newVertexIndices.Contains(c))
                    {
                        newVertexIndices.Add(c);
                    }
                }
                newVertexIndices.Sort();
                var oldToNewVertexMap = new Dictionary<int, int>();
                for (int i = 0; i < newVertexIndices.Count; i++)
                {
                    oldToNewVertexMap.Add(newVertexIndices[i], i);
                }

                // New vertices and per vertex infos
                var newVertices = new List<Vector3>();
                var newNormals = new List<Vector3>();
                var newUVs = new List<Vector2>();
                var newUV2s = new List<Vector2>();
                var newUV3s = new List<Vector2>();
                var newUV4s = new List<Vector2>();
                var newUV5s = new List<Vector2>();
                var newUV6s = new List<Vector2>();
                var newUV7s = new List<Vector2>();
                var newUV8s = new List<Vector2>();
                var newColors = new List<Color>();
                var newTangents = new List<Vector4>();
                for (int i = 0; i < newVertexIndices.Count; i++)
                {
                    int vertexIndex = newVertexIndices[i];
                    newVertices.Add(vertices[mesh][vertexIndex]);
                    newNormals.Add(normals[mesh][vertexIndex]);
                    newUVs.Add(uvs[mesh][vertexIndex]);
                    if (uv2s[mesh].Count > 0) newUV2s.Add(uv2s[mesh][vertexIndex]);
                    if (uv3s[mesh].Count > 0) newUV3s.Add(uv3s[mesh][vertexIndex]);
                    if (uv4s[mesh].Count > 0) newUV4s.Add(uv4s[mesh][vertexIndex]);
                    if (uv5s[mesh].Count > 0) newUV5s.Add(uv5s[mesh][vertexIndex]);
                    if (uv6s[mesh].Count > 0) newUV6s.Add(uv6s[mesh][vertexIndex]);
                    if (uv7s[mesh].Count > 0) newUV7s.Add(uv7s[mesh][vertexIndex]);
                    if (uv8s[mesh].Count > 0) newUV8s.Add(uv8s[mesh][vertexIndex]);
                    if (colors[mesh].Count > 0) newColors.Add(colors[mesh][vertexIndex]);
                    if (tangents[mesh].Count > 0) newTangents.Add(tangents[mesh][vertexIndex]);
                }

                var newMesh = new Mesh();

                // new tris (and sub meshes)
                if (!preserveSubMeshes)
                {
                    var newTris = new List<int>();
                    foreach (var tri in selectedTris)
                    {
                        int a = oldToNewVertexMap[tri.TriangleIndices[0]];
                        int b = oldToNewVertexMap[tri.TriangleIndices[1]];
                        int c = oldToNewVertexMap[tri.TriangleIndices[2]];

                        newTris.Add(a);
                        newTris.Add(b);
                        newTris.Add(c);
                    }

                    newMesh.SetVertices(newVertices);
                    newMesh.SetNormals(newNormals);
                    newMesh.SetTriangles(newTris, 0);
                    newMesh.SetUVs(0, newUVs);
                    if (newUV2s.Count > 0) newMesh.SetUVs(1, newUV2s);
                    if (newUV3s.Count > 0) newMesh.SetUVs(2, newUV3s);
                    if (newUV4s.Count > 0) newMesh.SetUVs(3, newUV4s);
                    if (newUV5s.Count > 0) newMesh.SetUVs(4, newUV5s);
                    if (newUV6s.Count > 0) newMesh.SetUVs(5, newUV6s);
                    if (newUV7s.Count > 0) newMesh.SetUVs(6, newUV7s);
                    if (newUV8s.Count > 0) newMesh.SetUVs(7, newUV8s);
                    if (newColors.Count > 0) newMesh.SetColors(newColors);
                    newMesh.SetTangents(newTangents);

                    newMesh.RecalculateBounds();
                }
                else
                {
                    // NOTICE: We are still only working on the tris within each mesh. Merging
                    // sub meshes across meshes is done later in the mesh combine step (see below).

                    var trisPerSubMesh = new Dictionary<int, List<SelectedTriangle>>();
                    // Merge by material only if the flag is set AND if there are materials.
                    if (combineSubMeshesBasedOnMaterials && materials[mesh] != null && materials[mesh].Count > 0)
                    {
                        // SubMeshMap: index 0 = the material for subMesh 0, 1 = the material for subMesh 1, ...
                        var subMeshToMaterialMap = new List<Material>();

                        // Group selected tris into sub meshes based on MATERIAL within one mesh.
                        foreach (var tri in selectedTris)
                        {
                            // Get the material matching the current tri.
                            // It may happen that no material is assigned. In that case
                            // we sort the tri into the NULL material sub mesh.
                            Material mat = null;
                            if (materials[tri.Mesh] != null && materials[tri.Mesh].Count > tri.SubMeshIndex && materials[tri.Mesh][tri.SubMeshIndex] != null)
                            {
                                mat = materials[tri.Mesh][tri.SubMeshIndex];
                            }
                            if(!subMeshToMaterialMap.Contains(mat))
                            {
                                subMeshToMaterialMap.Add(mat);
                            }
                            // Convert material to sub mesh index.
                            int index = subMeshToMaterialMap.IndexOf(mat);
                            // Insert tris per material
                            if (!trisPerSubMesh.ContainsKey(index))
                            {
                                trisPerSubMesh.Add(index, new List<SelectedTriangle>());
                            }
                            trisPerSubMesh[index].Add(tri);
                        }

                        // Save the materials per sub mesh info for merging multiple meshes.
                        materialsForSubMeshes.Add(newMesh, subMeshToMaterialMap);
                    }
                    else
                    {
                        // Group selected tris into sub meshes based on INDEX within one mesh.
                        foreach (var tri in selectedTris)
                        {
                            if (!trisPerSubMesh.ContainsKey(tri.SubMeshIndex))
                            {
                                trisPerSubMesh.Add(tri.SubMeshIndex, new List<SelectedTriangle>());
                            }
                            trisPerSubMesh[tri.SubMeshIndex].Add(tri);
                        }

                        // Save the materials per sub mesh info for merging multiple meshes.
                        materialsForSubMeshes.Add(newMesh, new List<Material>(materials[mesh]));
                    }

                    // Start mesh
                    newMesh.SetVertices(newVertices);
                    newMesh.SetNormals(newNormals);
                    newMesh.SetUVs(0, newUVs);
                    if (newUV2s.Count > 0) newMesh.SetUVs(1, newUV2s);
                    if (newUV3s.Count > 0) newMesh.SetUVs(2, newUV3s);
                    if (newUV4s.Count > 0) newMesh.SetUVs(3, newUV4s);
                    if (newUV5s.Count > 0) newMesh.SetUVs(4, newUV5s);
                    if (newUV6s.Count > 0) newMesh.SetUVs(5, newUV6s);
                    if (newUV7s.Count > 0) newMesh.SetUVs(6, newUV7s);
                    if (newUV8s.Count > 0) newMesh.SetUVs(7, newUV8s);
                    if (newColors.Count > 0) newMesh.SetColors(newColors);
                    newMesh.SetTangents(newTangents);

                    // Add sub mesh tris
                    newMesh.subMeshCount = trisPerSubMesh.Count;
                    int subMeshIndex = 0;
                    foreach (var subKV in trisPerSubMesh)
                    {
                        var tris = subKV.Value;
                        var newTris = new List<int>();
                        foreach (var tri in tris)
                        {
                            int a = oldToNewVertexMap[tri.TriangleIndices[0]];
                            int b = oldToNewVertexMap[tri.TriangleIndices[1]];
                            int c = oldToNewVertexMap[tri.TriangleIndices[2]];

                            newTris.Add(a);
                            newTris.Add(b);
                            newTris.Add(c);
                        }

                        newMesh.SetTriangles(newTris, subMeshIndex);
                        subMeshIndex++;
                    }

                    // finalize mesh
                    newMesh.RecalculateTangents();
                    newMesh.RecalculateBounds();
                }

                // Add a tuple which links the new mesh to the old mesh infos via a single SelectedTriangle.
                newMeshToTriMap.Add((newMesh, selectedTris[0]));
            }

            if (showProgress)
                EditorUtility.DisplayProgressBar("Extracting Mesh", "Merging tris and materials ..", 0.7f);

            if (combineMeshes && newMeshToTriMap.Count > 1)
            {
                // Combine meshes of multiple objects into one.
                var combinedMesh = new Mesh();

                // Calc sub mesh number
                int maxSubMeshCount = 0;
                if (combineSubMeshesBasedOnMaterials)
                {
                    maxSubMeshCount = materialsForSubMeshes.SelectMany(kv => kv.Value).Distinct().Count();
                }
                else
                {
                    for (int m = 0; m < newMeshToTriMap.Count; m++)
                    {
                        var newMesh = newMeshToTriMap[m].Item1;
                        maxSubMeshCount = Mathf.Max(newMesh.subMeshCount, maxSubMeshCount);
                    }
                }
                combinedMesh.subMeshCount = maxSubMeshCount;

                // Find materials
                // SubMeshMap: index 0 = the material for subMesh 0, 1 = the material for subMesh 1, ...
                var subMeshToMaterialMap = new List<Material>();
                for (int m = 0; m < newMeshToTriMap.Count; m++)
                {
                    var newMesh = newMeshToTriMap[m].Item1;
                    var oldMesh = newMeshToTriMap[m].Item2.Mesh;

                    for (int s = 0; s < maxSubMeshCount; s++)
                    {
                        // Not all meshes may have that many sub meshes.
                        if (s >= newMesh.subMeshCount)
                            continue;
                        
                        // Find the material for the current sub mesh
                        Material mat = null;
                        if (materialsForSubMeshes[newMesh] != null && materialsForSubMeshes[newMesh].Count > s)
                        {
                            mat = materialsForSubMeshes[newMesh][s];
                        }

                        // Add the material to the materials for sub meshes list.
                        if (combineSubMeshesBasedOnMaterials)
                        {
                            // Ensure each material is only used once.
                            if (!subMeshToMaterialMap.Contains(mat))
                            {
                                subMeshToMaterialMap.Add(mat);
                            }
                        }
                        else
                        {
                            // Simply use the first material found.
                            // If the current material is null then try to replace it with the current one (if the current ons is not null).
                            if(subMeshToMaterialMap.Count <= s)
                            {
                                subMeshToMaterialMap.Add(mat);
                            }
                            else if (subMeshToMaterialMap[s] == null && mat != null)
                            {
                                subMeshToMaterialMap[s] = mat;
                            }
                        }
                    }
                }

                // Combined vertices and per vertex infos
                var combinedVertices = new List<Vector3>();
                var combinedNormals = new List<Vector3>();
                var combinedUVs = new List<Vector2>();
                var combinedUV2s = new List<Vector2>();
                var combinedUV3s = new List<Vector2>();
                var combinedUV4s = new List<Vector2>();
                var combinedUV5s = new List<Vector2>();
                var combinedUV6s = new List<Vector2>();
                var combinedUV7s = new List<Vector2>();
                var combinedUV8s = new List<Vector2>();
                var combinedColors = new List<Color>();
                var combinedTangents = new List<Vector4>();
                var combinedMaterials = new List<Material>(subMeshToMaterialMap);

                var tmpVector2 = new List<Vector2>();
                var tmpVector3 = new List<Vector3>();
                var tmpVector4 = new List<Vector4>();
                var tmpColor = new List<Color>();
                var tmpInt = new List<int>();

                var offsets = new List<int>();

                for (int m = 0; m < newMeshToTriMap.Count; m++)
                {
                    var newMesh = newMeshToTriMap[m].Item1;
                    var oldMesh = newMeshToTriMap[m].Item2.Mesh;

                    offsets.Add(combinedVertices.Count);

                    // Vertices
                    tmpVector3.Clear();
                    newMesh.GetVertices(tmpVector3);
                    combinedVertices.AddRange(tmpVector3);

                    // Normals
                    tmpVector3.Clear();
                    newMesh.GetNormals(tmpVector3);
                    combinedNormals.AddRange(tmpVector3);

                    // UV0s
                    tmpVector2.Clear();
                    newMesh.GetUVs(0, tmpVector2);
                    combinedUVs.AddRange(tmpVector2);

                    // UV2
                    tmpVector2.Clear();
                    newMesh.GetUVs(1, tmpVector2);
                    combinedUV2s.AddRange(tmpVector2);

                    // UV3
                    tmpVector2.Clear();
                    newMesh.GetUVs(2, tmpVector2);
                    combinedUV3s.AddRange(tmpVector2);

                    // UV4
                    tmpVector2.Clear();
                    newMesh.GetUVs(3, tmpVector2);
                    combinedUV4s.AddRange(tmpVector2);

                    // UV5
                    tmpVector2.Clear();
                    newMesh.GetUVs(4, tmpVector2);
                    combinedUV5s.AddRange(tmpVector2);

                    // UV6
                    tmpVector2.Clear();
                    newMesh.GetUVs(5, tmpVector2);
                    combinedUV6s.AddRange(tmpVector2);

                    // UV7
                    tmpVector2.Clear();
                    newMesh.GetUVs(6, tmpVector2);
                    combinedUV7s.AddRange(tmpVector2);

                    // UV8
                    tmpVector2.Clear();
                    newMesh.GetUVs(7, tmpVector2);
                    combinedUV8s.AddRange(tmpVector2);

                    // Colors
                    tmpColor.Clear();
                    newMesh.GetColors(tmpColor);
                    combinedColors.AddRange(tmpColor);

                    // Tangents
                    tmpVector4.Clear();
                    newMesh.GetTangents(tmpVector4);
                    combinedTangents.AddRange(tmpVector4);
                }

                combinedMesh.SetVertices(combinedVertices);
                combinedMesh.SetNormals(combinedNormals);
                combinedMesh.SetUVs(0, combinedUVs);
                if (combinedUV2s.Count > 0) combinedMesh.SetUVs(1, combinedUV2s);
                if (combinedUV3s.Count > 0) combinedMesh.SetUVs(2, combinedUV3s);
                if (combinedUV4s.Count > 0) combinedMesh.SetUVs(3, combinedUV4s);
                if (combinedUV5s.Count > 0) combinedMesh.SetUVs(4, combinedUV5s);
                if (combinedUV6s.Count > 0) combinedMesh.SetUVs(5, combinedUV6s);
                if (combinedUV7s.Count > 0) combinedMesh.SetUVs(6, combinedUV7s);
                if (combinedUV8s.Count > 0) combinedMesh.SetUVs(7, combinedUV8s);
                if (combinedColors.Count > 0) combinedMesh.SetColors(combinedColors);
                combinedMesh.SetTangents(combinedTangents);

                // s is the final sub mesh index
                for (int s = 0; s < maxSubMeshCount; s++)
                {
                    tmpInt.Clear();

                    for (int m = 0; m < newMeshToTriMap.Count; m++)
                    {
                        int vertexOffset = offsets[m];
                        var newMesh = newMeshToTriMap[m].Item1;
                        var oldMesh = newMeshToTriMap[m].Item2.Mesh;

                        if (combineSubMeshesBasedOnMaterials)
                        {
                            // Find the material for the current sub mesh
                            Material material = subMeshToMaterialMap[s];

                            // Add all the sub meshes with that material to the current final submesh.
                            for (int i = 0; i < newMesh.subMeshCount; i++)
                            {
                                // Convert material to sub mesh index.
                                var subMeshMaterial = materialsForSubMeshes[newMesh][i];
                                if (material == subMeshMaterial)
                                {
                                    var tris = newMesh.GetTriangles(i);
                                    for (int t = 0; t < tris.Length; t++)
                                    {
                                        tris[t] += vertexOffset;
                                    }
                                    tmpInt.AddRange(tris);
                                }
                            }
                        }
                        else
                        {
                            // Not all meshes may have that many sub meshes.
                            if (s >= newMesh.subMeshCount)
                                continue;

                            // Simply append to the sub mesh at index s.
                            var tris = newMesh.GetTriangles(s);
                            for (int t = 0; t < tris.Length; t++)
                            {
                                tris[t] += vertexOffset;
                            }
                            tmpInt.AddRange(tris);
                        }
                    }
                    // Set sub mesh
                    combinedMesh.SetTriangles(tmpInt.ToArray(), s);
                }

                // Finalize mesh
                combinedMesh.RecalculateBounds();

                if (showProgress)
                    EditorUtility.DisplayProgressBar("Extracting Mesh", "Saving meshes ..", 0.9f);

                var path = deleteFileAndGetPath(replaceOldFiles, "Assets/" + filePath + ".asset");

                var combinedMeshAndMaterials = new Dictionary<Mesh, List<Material>>();
                combinedMeshAndMaterials[combinedMesh] = combinedMaterials;

                if (extractTextures)
                {
                    combinedMeshAndMaterials = TextureGenerator.GenerateTexturesAndUpdateUVs(path, combinedMeshAndMaterials, MeshExtractorSettings.GetOrCreateSettings().LogFilePaths);
                }

                Dictionary<string, string> objFiles = null;
                if (saveAsObj) 
                {
                    // It does not matter that the path ends with ".asset" because the SaveMeshAsObj() ignores the file extension anyways.
                    objFiles = ObjExporter.SaveMeshAsObj(path, "Mesh", combinedMesh, materials: combinedMeshAndMaterials[combinedMesh], MeshExtractorSettings.GetOrCreateSettings().LogFilePaths);
                }
                else
                {
                    AssetExporter.SaveMeshAsAsset(combinedMesh, path, MeshExtractorSettings.GetOrCreateSettings().LogFilePaths);
                    ProjectWindowUtil.ShowCreatedAsset(combinedMesh);  // Forces an update on the project view window.
                }

                if (createPrefab)
                {
                    createPrefabAsset(filePath, replaceOldFiles, saveAsObj, combinedMesh, combinedMeshAndMaterials[combinedMesh], path, objFiles);
                }

                AssetDatabase.Refresh();
            }
            else
            {
                // If meshes should not be combined then create one asset for each mesh.

                if (showProgress)
                    EditorUtility.DisplayProgressBar("Extracting Mesh", "Saving meshes ..", 0.9f);

                int index = 0;
                Dictionary<string, string> objFiles = null;
                foreach (var m in newMeshToTriMap)
                {
                    var newMesh = m.Item1;
                    string path;
                    if(newMeshToTriMap.Count > 1)
                    {
                        index++;
                        path = deleteFileAndGetPath(replaceOldFiles, "Assets/" + filePath + " part " + index + ".asset");
                    }
                    else
                    {
                        path = deleteFileAndGetPath(replaceOldFiles, "Assets/" + filePath + ".asset");
                    }

                    var meshAndMaterials = new Dictionary<Mesh, List<Material>>();
                    meshAndMaterials[newMesh] = materialsForSubMeshes[newMesh];

                    if (extractTextures)
                    {
                        meshAndMaterials = TextureGenerator.GenerateTexturesAndUpdateUVs(path, meshAndMaterials, MeshExtractorSettings.GetOrCreateSettings().LogFilePaths);
                    }

                    if (saveAsObj)
                    {
                        // It does not matter that the path ends with ".asset" because the SaveMeshAsObj() ignores the file extension anyways.
                        objFiles = ObjExporter.SaveMeshAsObj(path, "Mesh", newMesh, meshAndMaterials[newMesh], MeshExtractorSettings.GetOrCreateSettings().LogFilePaths);
                    }
                    else
                    {
                        AssetExporter.SaveMeshAsAsset(newMesh, path, MeshExtractorSettings.GetOrCreateSettings().LogFilePaths);
                        ProjectWindowUtil.ShowCreatedAsset(newMesh); // Forces an update on the project view window.
                    }

                    if (createPrefab)
                    {
                        createPrefabAsset(filePath, replaceOldFiles, saveAsObj, newMesh, meshAndMaterials[newMesh], path, objFiles);
                    }
                }

                AssetDatabase.Refresh();
            }
        }

        private static void createPrefabAsset(string filePath, bool replaceOldFiles, bool saveAsObj, Mesh combinedMesh, List<Material> combinedMaterials, string path, Dictionary<string, string> objFiles)
        {
            var prefabPath = path.Replace(".asset", ".prefab");
            prefabPath = deleteFileAndGetPath(replaceOldFiles, prefabPath);

            var name = System.IO.Path.GetFileNameWithoutExtension(filePath);
            var prefabGO = new GameObject(name);
            var meshFilter = prefabGO.AddComponent<MeshFilter>();
            if (saveAsObj)
            {
                // Load the model and extract the mesh
                string modelFileName = objFiles.Keys.FirstOrDefault(k => k.EndsWith(".obj"));
                var modelPath = System.IO.Path.GetDirectoryName(path) + "/" + modelFileName;
                var model = AssetDatabase.LoadAssetAtPath<GameObject>(modelPath);
                if (model != null)
                {
                    var modelMeshFilter = model.GetComponentInChildren<MeshFilter>(includeInactive: true);
                    if (modelMeshFilter != null)
                    {
                        meshFilter.sharedMesh = modelMeshFilter.sharedMesh;
                    }
                }
            }
            else
            {
                meshFilter.sharedMesh = combinedMesh;
            }
            var meshRenderer = prefabGO.AddComponent<MeshRenderer>();
            meshRenderer.sharedMaterials = combinedMaterials.ToArray();

            // Save the transform's GameObject as a prefab asset.
            var prefabAsset = PrefabUtility.SaveAsPrefabAsset(prefabGO, prefabPath);
            GameObject.DestroyImmediate(prefabGO);
            ProjectWindowUtil.ShowCreatedAsset(prefabAsset);
        }

        private static string deleteFileAndGetPath(bool replaceOldMesh, string filePath)
        {
            // Create dirs if necessary
            string dirPath = System.IO.Path.GetDirectoryName(Application.dataPath + "/../" + filePath);
            if (!System.IO.Directory.Exists(dirPath))
            {
                System.IO.Directory.CreateDirectory(dirPath);
            }

            if (replaceOldMesh && System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
                if (System.IO.File.Exists(filePath + ".meta"))
                {
                    System.IO.File.Delete(filePath + ".meta");
                }
                return filePath;
            }
            else
            {
                return AssetDatabase.GenerateUniqueAssetPath(filePath);
            }
        }

        static T getOrCreateMeshData<T>(Mesh mesh, Dictionary<Mesh, T> data) where T : new()
        {
            if (!data.ContainsKey(mesh))
            {
                data.Add(mesh, new T());
            }
            return data[mesh];
        }
    }
}
