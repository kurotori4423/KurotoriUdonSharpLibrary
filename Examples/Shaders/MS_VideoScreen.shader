// Copyright (c) 2020 momoma
// Released under the MIT license
// https://opensource.org/licenses/mit-license.php

Shader "MomomaShader/Surface/VideoScreen"
{
	Properties
	{
		_MainTex ("Albedo (RGB)", 2D) = "black" {}
		[Toggle(_EMISSION)] _Emission ("Emission", Float) = 1.0
		_EmissionScale ("Emission Scale", Float) = 1.0
		[Enum(Off, 0, On, 1)] _G2L ("Gamma To Linear", Float) = 0.0
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }

		CGPROGRAM
		#pragma surface surf Standard addshadow fullforwardshadows
		#pragma target 3.0
		#pragma shader_feature _EMISSION

		struct Input
		{
			float2 uv_MainTex;
		};

		sampler2D _MainTex;
		float4 _MainTex_TexelSize;
		fixed _EmissionScale;
		fixed _G2L;

		UNITY_INSTANCING_BUFFER_START(Props)
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutputStandard o)
		{
			float2 uv = IN.uv_MainTex;
			float2 texScale = float2(_MainTex_TexelSize.x * 16.0, _MainTex_TexelSize.y * 9.0);
			texScale /= min(texScale.x, texScale.y);
			uv = uv * texScale - texScale * 0.5 + 0.5;
			fixed4 c = tex2D(_MainTex, uv);
			if (_G2L)
				c.rgb = GammaToLinearSpace(c);
			c *= !(any(uv < 0.0) + any(1.0 < uv));
			o.Albedo = 0;
			o.Emission = c * _EmissionScale;
		}
		ENDCG
	}
	FallBack "Standard"
	CustomEditor "RealtimeEmissiveGammaGUI"
}
