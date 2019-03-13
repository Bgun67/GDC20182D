Shader "Custom/ToonShader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_NormalTex ("Normal (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,10)) = 0.0
		_Threshold ("Threshold", Range(0,1)) = 0.2
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Ramp

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _NormalTex;
		half _Threshold;


		half4 LightingRamp (SurfaceOutput s, half3 lightDir, half atten){
			half NdotL = dot (s.Normal, lightDir);
			//NdotL = abs(NdotL);
			half diff = NdotL * 0.5 + 0.5;
			diff = clamp(diff,0,1);
			if(diff < 0.5 + _Threshold/2 && diff > 0.5 - _Threshold/2){
				diff = 1.0 * ((diff-0.5 + _Threshold/2)/(_Threshold));
			}
			else{
				diff = round(diff);
			}
			
			half4 c;
			c.rgb = (s.Albedo * _LightColor0.rgb) * clamp(diff,0,1) * (1+s.Gloss*diff) * clamp(atten,0,1);
			
			//c.rgb = s.Albedo;
			c.a = s.Alpha;
			return c;
		}

		struct Input {
			float2 uv_MainTex;
			float2 uv_BumpMap;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutput o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			fixed4 n = tex2D (_NormalTex, IN.uv_MainTex);
			o.Albedo = c.rgb;
			o.Normal = UnpackNormal (tex2D (_NormalTex, IN.uv_MainTex));
			// Metallic and smoothness come from slider variables
			//o.Metallic = _Metallic;
			o.Gloss = _Glossiness;
			//o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
