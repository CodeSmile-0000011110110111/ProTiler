using UnityEngine;

namespace Kamgam.MeshExtractor
{
    public static class MaterialPropertyExtensions
    {
        public static bool HasTextureProperty(this Material material, string propertyName)
        {
            if (material == null || material.shader == null)
                return false;

            int count = material.shader.GetPropertyCount();
            for (int i = 0; i < count; i++)
            {
                string name = material.shader.GetPropertyName(i);
                if (name == propertyName)
                {
                    var type = material.shader.GetPropertyType(i);
                    if (type == UnityEngine.Rendering.ShaderPropertyType.Texture)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Tries each property name in order and returns the result for the first property that is found.
        /// </summary>
        /// <param name="material"></param>
        /// <param name="propertyNames"></param>
        /// <returns></returns>
        public static Texture GetTextureOrNull(this Material material, params string[] propertyNames)
        {
            if (material == null || material.shader == null)
                return null;

            for (int i = 0; i < propertyNames.Length; i++)
            {
                if (material.HasTextureProperty(propertyNames[i]))
                {
                    return material.GetTexture(propertyNames[i]);
                }
            }

            return null;
        }

        /// <summary>
        /// Tries each property name in order and sets the first property that is found.
        /// </summary>
        /// <param name="material"></param>
        /// <param name="texture"></param>
        /// <param name="propertyNames"></param>
        public static void SetTexture(this Material material, Texture texture, params string[] propertyNames)
        {
            if (material == null || material.shader == null || texture == null)
                return;

            for (int i = 0; i < propertyNames.Length; i++)
            {
                if (material.HasTextureProperty(propertyNames[i]))
                {
                    material.SetTexture(propertyNames[i], texture);
                    return;
                }
            }
        }


        public static Texture GetMainTexture(this Material material)
        {
            if (material == null || material.shader == null)
                return null;

            Texture result = material.mainTexture;
            
            if (result == null)
            {
                // Add custom shader propery names at the end of this list (don't forget to add them to Set... too).
                result = material.GetTextureOrNull("_BaseMap", "_MainTex", "_AlbedoMap", "_AlbedoTex", "_Main", "_Albedo");
            }

            return result;
        }

        public static void SetMainTexture(this Material material, Texture texture)
        {
            if (material == null || material.shader == null || texture == null)
                return;

            material.SetTexture(texture, "_BaseMap", "_MainTex", "_AlbedoMap", "_AlbedoTex", "_Main", "_Albedo");

            // try the unity method of setting too.
            material.mainTexture = texture;
        }


        public static Texture GetNormalMap(this Material material)
        {
            // Add custom shader propery names at the end of this list (don't forget to add them to Set... too).
            return material.GetTextureOrNull("_BumpMap", "_NormalMap", "_Bump", "_Normal", "_MainNormalMap", "_ParallaxMap");
        }

        public static void SetNormalMap(this Material material, Texture texture)
        {
            material.SetTexture(texture, "_BumpMap", "_NormalMap", "_Bump", "_Normal", "_MainNormalMap", "_ParallaxMap");
        }


        public static Texture GetSpecularMap(this Material material)
        {
            // Add custom shader propery names at the end of this list (don't forget to add them to Set... too).
            return material.GetTextureOrNull("_SpecGlossMap", "_SpecularColorMap", "_SpecularMap", "_Specular", "_MainSpecularMap");
        }

        public static void SetSpecularMap(this Material material, Texture texture)
        {
            material.SetTexture(texture, "_SpecGlossMap", "_SpecularColorMap", "_SpecularMap", "_Specular", "_MainSpecularMap");
        }


        public static Texture GetMetallicMap(this Material material)
        {
            // Add custom shader propery names at the end of this list (don't forget to add them to Set... too).
            return material.GetTextureOrNull("_MetallicGlossMap", "_MetallicColorMap", "_MetallicMap", "_Metallic", "_MainMetallicMap");
        }

        public static void SetMetallicMap(this Material material, Texture texture)
        {
            material.SetTexture(texture, "_MetallicGlossMap", "_MetallicColorMap", "_MetallicMap", "_Metallic", "_MainMetallicMap");
        }


        public static Texture GetEmissionMap(this Material material)
        {
            // Add custom shader propery names at the end of this list (don't forget to add them to Set... too).
            return material.GetTextureOrNull("_EmissionMap", "_EmissiveColorMap", "_Emission", "_EmissiveMap", "_Emissive", "_MainEmissiveMap");
        }

        public static void SetEmissionMap(this Material material, Texture texture)
        {
            material.SetTexture(texture, "_EmissionMap", "_EmissiveColorMap", "_Emission", "_EmissiveMap", "_Emissive", "_MainEmissiveMap");
        }


        public static Texture GetOcclusionMap(this Material material)
        {
            // Add custom shader propery names at the end of this list (don't forget to add them to Set... too).
            return material.GetTextureOrNull("_OcclusionMap", "_OcclusionColorMap", "_Occlusion", "_MainOcclusionMap");
        }

        public static void SetOcclusionMap(this Material material, Texture texture)
        {
            material.SetTexture(texture, "_OcclusionMap", "_OcclusionColorMap", "_Occlusion", "_MainOcclusionMap");
        }
    }
}
