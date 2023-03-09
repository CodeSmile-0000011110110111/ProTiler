Shader "Hidden/FadeScreenCutout_Shader"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _CutoutSize("Cutout Size", Range(0.0, 1.5)) = 1
    }
    SubShader
    {
        Tags{ "Queue" = "Transparent" "RenderType" = "Transparent"}
        Cull Off ZWrite Off ZTest Always
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
             CGPROGRAM
             #pragma vertex VertexFunction 
             #pragma fragment FragmentFunction
             #pragma fragmentoption ARB_precision_hint_fastest 
             #include "UnityCG.cginc"

             sampler2D _MainTex;
             float1 _CutoutSize;
             float4 _MainTex_TexelSize;

             struct unity_data
             {
                 float4 vertex   : POSITION;
                 float4 texcoord : TEXCOORD0;
                 float4 color    : COLOR;
             };

             struct v2f
             {
                 float4  pos : SV_POSITION;
                 float2  uv : TEXCOORD0;
                 float4  color : COLOR;
             };

             //The Vertex Function changes the UV of the Cutout to match the Cutout Size
             //This assumes that the object fills the entire screen
             v2f VertexFunction(unity_data v)
             {
                 v2f result;

                 //Adjust the Position to match the Aspect Ratio
                 float aspectRatio = _ScreenParams.x / _ScreenParams.y;
                 result.pos = UnityObjectToClipPos(v.vertex * aspectRatio);

                 //Center the Cutout Image (If I remember correctly)
                 result.uv = v.texcoord.xy;
                 result.uv = (result.uv - 0.5) / _CutoutSize + 0.5;

                 result.color = v.color;
                 return result;
             }

             float4 FragmentFunction(v2f i) : COLOR
             {
                 float4 texcolor = tex2D(_MainTex, i.uv);
                 float4 vertexcolor = i.color;

                 //Dont cutout the very edges of the UV to avoid artifacts
                 if (i.uv.x > 0 && i.uv.y > 0 && i.uv.x < 1 && i.uv.y < 1)
                 {
                     //Subtract the Textures alpha from the base color to achieve the cutout
                     vertexcolor.a -= texcolor.a;
                 }
                 
                 return vertexcolor;
             }

            ENDCG
        }
    }
}
