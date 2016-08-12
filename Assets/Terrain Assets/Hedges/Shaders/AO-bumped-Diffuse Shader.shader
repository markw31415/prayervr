Shader "Custom/Bumped Diffuse Ambient Occlusion" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
		_BumpMap ("Normal Map", 2D) = "bump" {}
		_AOTex ("Ambient Occlusion (RGB)", 2D) = "white" {}
		_AOFac ("Ambient Occlusion factor", Range (0.5, 2)) = 1
		_Cutoff ("Alpha Cutoff", Range (0,1)) = 0.5
	}
	SubShader { 
		Tags { "Queue"="AlphaTest" "RenderType"="TransparentCutout" "IgnoreProjector"="True" }
		LOD 0
		Cull Off

	CGPROGRAM
	#pragma surface surf BlinnPhong addshadow alphatest:_Cutoff
	#pragma target 3.0

	sampler2D _MainTex;
	sampler2D _BumpMap;
	sampler2D _AOTex;
	fixed4 _Color;
	half _AOFac;

	struct Input {
		float2 uv_MainTex;
		float2 uv_BumpMap;
		float2 uv2_AOTex;
	};

	void surf (Input IN, inout SurfaceOutput o) {
		fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
		fixed4 ao = tex2D(_AOTex, IN.uv2_AOTex);
		ao.rgb = ((ao.rgb - 0.5f) * max(_AOFac, 0)) + 0.5f;
		o.Albedo = tex.rgb * _Color.rgb * ao.rgb;
		o.Alpha = tex.a;
		o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
	}
	ENDCG
	}

	FallBack "Transparent/Cutout/Diffuse"
}