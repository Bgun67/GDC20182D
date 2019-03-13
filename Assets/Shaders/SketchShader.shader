Shader "Custom/SketchShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_NormalTex("NormalTex", 2D) = "purple" {}
		_Color ("Main Color", Color) = (1,1,1,1)
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
			sampler2D _NormalTex;
			sampler2D _CameraGBufferTexture2;
			fixed4 _Color;

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				float3 norm = (tex2D(_NormalTex, i.uv));

				float factor = clamp(dot((tex2D(_CameraGBufferTexture2,i.uv).xyz - 0.5) * 2, -_Color.xyz),0,1);
				// just invert the colors
				//This is the trippy code
				//col.rgb = col- col * (1-dot(tex2D(_CameraGBufferTexture2,i.uv), _ForwardVec));
				col.rgb = col.rgb * (factor) + norm * (1-factor);// +  norm * clamp(dot(tex2D(_CameraGBufferTexture2,i.uv).xyz, -_Color.xyz),0.5,1);
				return col;
			}
			ENDCG
		}
	}
}
