using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

namespace Kamgam.MeshExtractor
{
    public class TextureGenerator
    {
        public static Dictionary<Mesh, List<Material>> GenerateTexturesAndUpdateUVs(string path, Dictionary<Mesh, List<Material>> meshesAndMaterials, bool logFilePaths)
        {
            var results = new Dictionary<Mesh, List<Material>>();

            foreach (var kv in meshesAndMaterials)
            {
                var mesh = kv.Key;
                var materials = kv.Value;

                var newMaterials = new List<Material>();

                results.Add(mesh, newMaterials);

                if (materials == null || materials.Count == 0)
                    continue;

                var uvs = mesh.uv;
                var uv2s = mesh.uv2;
                var uv3s = mesh.uv3;
                var uv4s = mesh.uv4;
                var uv5s = mesh.uv5;
                var uv6s = mesh.uv6;
                var uv7s = mesh.uv7;
                var uv8s = mesh.uv8;
                var triangles = new List<int>();
                
                for (int submeshIndex = 0; submeshIndex < mesh.subMeshCount; submeshIndex++)
                {
                    if (materials.Count < submeshIndex)
                    {
                        newMaterials.Add(null);
                        continue;
                    }

                    var material = materials[submeshIndex];

                    if (material == null)
                    {
                        newMaterials.Add(null);
                        continue;
                    }

                    if (material.mainTexture == null)
                    {
                        newMaterials.Add(material);
                        continue;
                    }

                    var mainTexture = material.GetMainTexture() as Texture2D;

                    if (mainTexture == null)
                    {
                        newMaterials.Add(material);
                        continue;
                    }

                    triangles.Clear();
                    mesh.GetTriangles(triangles, submeshIndex);

                    // Get unique list of vertices used in submesh.
                    var vertexIndicesOfSubmesh = new List<int>();
                    int triCount = triangles.Count;
                    for (int i = 0; i < triCount; i++)
                    {
                        var vertexIndex = triangles[i];
                        if (!vertexIndicesOfSubmesh.Contains(vertexIndex))
                        {
                            vertexIndicesOfSubmesh.Add(vertexIndex);
                        }
                    }

                    // Find min/max UVs
                    Vector2 minUV = new Vector2(float.MaxValue, float.MaxValue);
                    Vector2 maxUV = new Vector2(float.MinValue, float.MinValue);

                    int vertextCount = vertexIndicesOfSubmesh.Count;
                    for (int i = 0; i < vertextCount; i++)
                    {
                        var vertexIndex = vertexIndicesOfSubmesh[i];
                        var uv = uvs[vertexIndex];
                        if (uv.x < minUV.x)
                        {
                            minUV.x = uv.x;
                        }

                        if (uv.y < minUV.y)
                        {
                            minUV.y = uv.y;
                        }

                        if (uv.x > maxUV.x)
                        {
                            maxUV.x = uv.x;
                        }

                        if (uv.y > maxUV.y)
                        {
                            maxUV.y = uv.y;
                        }
                    }

                    // Clamp to 0 to 1 range
                    var clampedMinUV = minUV;
                        clampedMinUV.x = Mathf.Clamp01(minUV.x);
                        clampedMinUV.y = Mathf.Clamp01(minUV.y);
                    var clampedMaxUV = maxUV;
                        clampedMaxUV.x = Mathf.Clamp01(maxUV.x);
                        clampedMaxUV.y = Mathf.Clamp01(maxUV.y);

                    // Skip if UV area inside 0 to 1 range ..
                    if (minUV.x >= 0f && minUV.y >= 0f && maxUV.x <= 1f && maxUV.y <= 1f)
                    {
                        // is close to 0.
                        if (clampedMaxUV.x - clampedMinUV.x < 0.0001f || clampedMaxUV.y - clampedMinUV.y < 0.0001f)
                        {
                            newMaterials.Add(material);
                            results[mesh] = newMaterials;
                            continue;
                        }
                    }

                    // Caculate updated UVs
                    float scaleX = 1f / (clampedMaxUV.x - clampedMinUV.x);
                    float scaleY = 1f / (clampedMaxUV.y - clampedMinUV.y);
                    float offsetX = clampedMinUV.x;
                    float offsetY = clampedMinUV.y;
                    // Handle UV outside of 0 to 1 range (x-axis)
                    if (minUV.x < 0f || minUV.x > 1f || maxUV.x < 0f || maxUV.x > 1f)
                    {
                        clampedMinUV.x = 0f;
                        clampedMaxUV.x = 1f;
                        offsetX = 0f;
                        scaleX = 1f;
                    }
                    // Handle UV outside of 0 to 1 range (y-axis)
                    if (minUV.y < 0f || minUV.y > 1f || maxUV.y < 0f || maxUV.y > 1f)
                    {
                        clampedMinUV.y = 0f;
                        clampedMaxUV.y = 1f;
                        offsetY = 0f;
                        scaleY = 1f;
                    }

                    for (int i = 0; i < vertextCount; i++)
                    {
                        var vertexIndex = vertexIndicesOfSubmesh[i];

                        transformUVs(uvs, uv2s, uv3s, uv4s, uv5s, uv6s, uv7s, uv8s, scaleX, scaleY, offsetX, offsetY, vertexIndex);
                    }

                    // Main texture (albedo)
                    string newMainTexturePath;
                    Texture newMainTexture;
                    createNewTexture(path, "-albedo", logFilePaths, mesh, submeshIndex, mainTexture, clampedMinUV, clampedMaxUV, out newMainTexture, out newMainTexturePath);

                    string materialPath = newMainTexturePath.Replace("-albedo", "");

                    // Normal map
                    var normalMap = material.GetNormalMap() as Texture2D;
                    Texture newNormalMap = null;
                    if (normalMap != null)
                    {
                        createNewTexture(path, "-normal", logFilePaths, mesh, submeshIndex, normalMap, clampedMinUV, clampedMaxUV, out newNormalMap, out _);
                    }

                    // Specular map
                    var specularMap = material.GetSpecularMap() as Texture2D;
                    Texture newSpecularMap = null;
                    if (specularMap != null)
                    {
                        createNewTexture(path, "-specular", logFilePaths, mesh, submeshIndex, specularMap, clampedMinUV, clampedMaxUV, out newSpecularMap, out _);
                    }

                    // Metallic map
                    var metallicMap = material.GetMetallicMap() as Texture2D;
                    Texture newMetallicMap = null;
                    if (metallicMap != null)
                    {
                        createNewTexture(path, "-metallic", logFilePaths, mesh, submeshIndex, metallicMap, clampedMinUV, clampedMaxUV, out newMetallicMap, out _);
                    }

                    // Occlusion map
                    var occlusionMap = material.GetOcclusionMap() as Texture2D;
                    Texture newOcclusionMap = null;
                    if (occlusionMap != null)
                    {
                        createNewTexture(path, "-occlusion", logFilePaths, mesh, submeshIndex, occlusionMap, clampedMinUV, clampedMaxUV, out newOcclusionMap, out _);
                    }

                    // Emission map
                    var emissionMap = material.GetOcclusionMap() as Texture2D;
                    Texture newEmissionMap = null;
                    if (emissionMap != null)
                    {
                        createNewTexture(path, "-emission", logFilePaths, mesh, submeshIndex, emissionMap, clampedMinUV, clampedMaxUV, out newEmissionMap, out _);
                    }

                    // create new material referencing the new texture(s)
                    var newMaterial = new Material(material);
                    newMaterial.SetMainTexture(newMainTexture);
                    newMaterial.SetNormalMap(newNormalMap);
                    newMaterial.SetSpecularMap(newSpecularMap);
                    newMaterial.SetMetallicMap(newMetallicMap);
                    newMaterial.SetOcclusionMap(newOcclusionMap);
                    newMaterial.SetEmissionMap(newEmissionMap);
                    saveMaterial(materialPath, newMaterial, logFilePaths);

                    newMaterials.Add(newMaterial);

                    // Clean up
                    triangles.Clear();
                }

                AssetDatabase.Refresh();

                // Apply uvs
                mesh.uv = uvs;
                mesh.uv2 = uv2s;
                mesh.uv3 = uv3s;
                mesh.uv4 = uv4s;
                mesh.uv5 = uv5s;
                mesh.uv6 = uv6s;
                mesh.uv7 = uv7s;
                mesh.uv8 = uv8s;

                // N2H: Support more than just the main (albedo) texture. This will require finding out
                // which shader is used on the material and then guess the property names to fetch the textures.
            }

            return results;
        }

        private static void createNewTexture(string meshPath, string suffix, bool logFilePaths, Mesh mesh, int submeshIndex, Texture2D mainTexture, Vector2 minUV, Vector2 maxUV, out Texture textureAsset, out string texturePath)
        {
            // Extract pixels from raw (uncompressed) texture
            (var pixelData, var size) = getRawPixelsFromNormalizedRect(mainTexture, minUV, maxUV);

            // Store in new texture
            var texture = new Texture2D(size.x, size.y, TextureFormat.RGB24, mipChain: false);
            texture.SetPixels(0, 0, size.x, size.y, pixelData);
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.Apply();

            // Save texture as PNG
            texturePath = Path.GetDirectoryName(meshPath) + "/" + Path.GetFileNameWithoutExtension(meshPath);
            if (mesh.subMeshCount > 1)
            {
                texturePath += " subMesh " + submeshIndex;
            }
            texturePath += suffix;
            texturePath = saveTextureAsPNG(texturePath, texture, logFilePaths);
            Object.DestroyImmediate(texture);

            // Make sure the texture is loaded as an asset.
            var newPath = "Assets/" + texturePath;
            AssetDatabase.ImportAsset(newPath);
            textureAsset = AssetDatabase.LoadAssetAtPath<Texture>(newPath);
            AssetDatabase.Refresh();

            // Apply texture importer settings similar to the original texture
            var originalPath = AssetDatabase.GetAssetPath(mainTexture);
            var originalImporter = (TextureImporter)AssetImporter.GetAtPath(originalPath);
            var newImporter = (TextureImporter)AssetImporter.GetAtPath(newPath);
            newImporter.maxTextureSize = originalImporter.maxTextureSize;
            newImporter.isReadable = originalImporter.isReadable;
            newImporter.crunchedCompression = originalImporter.crunchedCompression;
            newImporter.textureCompression = originalImporter.textureCompression;
            newImporter.compressionQuality = originalImporter.compressionQuality;
            newImporter.mipmapEnabled = originalImporter.mipmapEnabled;
            newImporter.mipmapFilter = originalImporter.mipmapFilter;
            newImporter.filterMode = originalImporter.filterMode;
            newImporter.textureType = originalImporter.textureType;
            newImporter.SaveAndReimport();
        }

        private static void transformUVs(Vector2[] uvs, Vector2[] uv2s, Vector2[] uv3s, Vector2[] uv4s, Vector2[] uv5s, Vector2[] uv6s, Vector2[] uv7s, Vector2[] uv8s, float scaleX, float scaleY, float offsetX, float offsetY, int vertexIndex)
        {
            uvs[vertexIndex].x -= offsetX;
            uvs[vertexIndex].y -= offsetY;
            uvs[vertexIndex].x *= scaleX;
            uvs[vertexIndex].y *= scaleY;

            if (uv2s.Length > 0)
            {
                uv2s[vertexIndex].x -= offsetX;
                uv2s[vertexIndex].y -= offsetY;
                uv2s[vertexIndex].x *= scaleX;
                uv2s[vertexIndex].y *= scaleY;
            }

            if (uv3s.Length > 0)
            {
                uv3s[vertexIndex].x -= offsetX;
                uv3s[vertexIndex].y -= offsetY;
                uv3s[vertexIndex].x *= scaleX;
                uv3s[vertexIndex].y *= scaleY;
            }

            if (uv4s.Length > 0)
            {
                uv4s[vertexIndex].x -= offsetX;
                uv4s[vertexIndex].y -= offsetY;
                uv4s[vertexIndex].x *= scaleX;
                uv4s[vertexIndex].y *= scaleY;
            }

            if (uv5s.Length > 0)
            {
                uv5s[vertexIndex].x -= offsetX;
                uv5s[vertexIndex].y -= offsetY;
                uv5s[vertexIndex].x *= scaleX;
                uv5s[vertexIndex].y *= scaleY;
            }

            if (uv6s.Length > 0)
            {
                uv6s[vertexIndex].x -= offsetX;
                uv6s[vertexIndex].y -= offsetY;
                uv6s[vertexIndex].x *= scaleX;
                uv6s[vertexIndex].y *= scaleY;
            }

            if (uv7s.Length > 0)
            {
                uv7s[vertexIndex].x -= offsetX;
                uv7s[vertexIndex].y -= offsetY;
                uv7s[vertexIndex].x *= scaleX;
                uv7s[vertexIndex].y *= scaleY;
            }

            if (uv8s.Length > 0)
            {
                uv8s[vertexIndex].x -= offsetX;
                uv8s[vertexIndex].y -= offsetY;
                uv8s[vertexIndex].x *= scaleX;
                uv8s[vertexIndex].y *= scaleY;
            }
        }

        private static (Color[], Vector2Int) getRawPixelsFromNormalizedRect(Texture2D originalTexture, Vector2 min, Vector2 max)
        {
            TextureImporter ti = null;

            var isReadable = originalTexture.isReadable;
            var isCrunchCompressed = false;
            var textureType = TextureImporterType.Default;
            var textureCompression = TextureImporterCompression.Uncompressed;
            var maxTextureSize = 16384;

            try
            {
                var path = AssetDatabase.GetAssetPath(originalTexture);
                ti = (TextureImporter)AssetImporter.GetAtPath(path);

                if (!isReadable)
                {
                    ti.isReadable = true;
                }

                isCrunchCompressed = ti.crunchedCompression;
                ti.crunchedCompression = false;

                textureCompression = ti.textureCompression;
                ti.textureCompression = TextureImporterCompression.Uncompressed;

                textureType = ti.textureType;
                ti.textureType = TextureImporterType.Default;

                maxTextureSize = ti.maxTextureSize;
                ti.maxTextureSize = 16384;

                ti.SaveAndReimport();

                int pMinX = Mathf.FloorToInt(min.x * originalTexture.width);
                int pMinY = Mathf.FloorToInt(min.y * originalTexture.height);
                int pMaxX = Mathf.CeilToInt(max.x * originalTexture.width);
                int pMaxY = Mathf.CeilToInt(max.y * originalTexture.height);
                int pWidth = pMaxX - pMinX;
                int pHeight = pMaxY - pMinY;

                Color[] pixelData = originalTexture.GetPixels(pMinX, pMinY, pWidth, pHeight);
                return (pixelData, new Vector2Int(pWidth, pHeight));
            }
            finally
            {
                // Revert
                if (ti != null)
                {
                    ti.crunchedCompression = isCrunchCompressed;
                    ti.textureCompression = textureCompression;
                    ti.maxTextureSize = maxTextureSize;
                    ti.textureType = textureType;
                    ti.isReadable = isReadable;
                    ti.SaveAndReimport();
                }
            }
        }

        static void saveMaterial(string path, Material material, bool logFilePaths)
        {
            // Remove "Assets/" from the start
            if (!path.StartsWith("Assets"))
            {
                path = "Assets/" + path;
            }

            // Create dir if necessary
            string dirPath = System.IO.Path.GetDirectoryName(path);
            if (!System.IO.Directory.Exists(dirPath))
            {
                System.IO.Directory.CreateDirectory(dirPath);
            }

            string fileName = System.IO.Path.GetFileNameWithoutExtension(path);
            string assetPath = dirPath + "/" + fileName + ".mat";

            AssetDatabase.CreateAsset(material, assetPath);

            if (logFilePaths)
                Logger.LogMessage("Material generated in <color=yellow>Assets/" + assetPath + "</color>");
        }

        static string saveTextureAsPNG(string path, Texture2D texture, bool logFilePaths)
        {
            // Remove "Assets/" from the start
            if (path.StartsWith("Assets/") || path.StartsWith("Assets\\"))
            {
                path = path.Substring(7);
            }

            // Create dir if necessary
            string dirPath = System.IO.Path.GetDirectoryName(Application.dataPath + "/" + path);
            if (!System.IO.Directory.Exists(dirPath))
            {
                System.IO.Directory.CreateDirectory(dirPath);
            }

            string fileName = System.IO.Path.GetFileNameWithoutExtension(path);

            byte[] bytes = ImageConversion.EncodeToPNG(texture);
            File.WriteAllBytes(dirPath + "/" + fileName + ".png", bytes);

            if (logFilePaths)
                Logger.LogMessage("Texture generated in <color=yellow>Assets/" + (Path.GetDirectoryName(path) + fileName + ".png") + "</color>");

            return path + ".png";
        }
    }
}
