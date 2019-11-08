//mostly stolen from unity
//https://docs.unity3d.com/Manual/SL-SurfaceShaderExamples.html
Shader "Custom/Lava" {
	Properties{
	  _MainTex("Albedo", 2D) = "white" {}
	_Albedo("Albedo", Color) = (0,0,0,0)
		_NormalTex("Normal", 2D) = "white" {}

	  _Amount("Amplitude", Range(-1,1)) = 0.5
	_Omega("Frequency", Range(-10,10)) = 1
	_Speed("Wave Speed", Range(-10,10)) = 1

	 _Glow("Glow", Range(0,10)) = 1
	_Metallic("Metallic", Range(0,1)) = 0.5
	_Smoothness("Smoothness", Range(0,1)) = 0.5
	_Emission("Emission", Range(0,1)) = 0.5
	}
		SubShader{
		  Tags { "RenderType" = "Opaque" }
		  CGPROGRAM
		  #pragma surface surf Standard vertex:vert
		  struct Input {
			  float2 uv_MainTex;
			  float2 uv_NormalTex;
		  };
		  float _Amount;
		  float _Speed;
		  float _Omega;
		  void vert(inout appdata_full v) {
			  float time = _Time.y+unity_DeltaTime;
			  time *= _Omega;
			  v.vertex.xyz += v.normal * _Amount * (sin(v.vertex.x*_Speed-time)+ sin(2*v.vertex.y*_Speed - time));// *v.vertex.y * 2;
		  }
		  sampler2D _MainTex;
		  sampler2D _NormalTex;
		  float4 _Albedo;
		  float _Glow;
		  float _Metallic;
		  float _Smoothness;
		  float _Emission;
		  void surf(Input IN, inout SurfaceOutputStandard o) {
			  float time = _Time.y + unity_DeltaTime;
			  o.Albedo = (tex2D(_MainTex, IN.uv_MainTex+0.05*float2(sin(0.1*time), 2*sin(0.05*time))).rgb*_Albedo)*_Glow;
			  o.Normal = tex2D(_NormalTex, IN.uv_NormalTex).rgb+float3(1, 0, 0)*0.3*(sin(2 * time)+1);
			  o.Smoothness = _Smoothness;
			  o.Metallic = _Metallic;
			  o.Emission = _Emission;
		  }
		  ENDCG
	  }
		  Fallback "Diffuse"
}