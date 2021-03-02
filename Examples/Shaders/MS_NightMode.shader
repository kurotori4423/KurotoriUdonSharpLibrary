// Copyright (c) 2020 momoma
// Released under the MIT license
// https://opensource.org/licenses/mit-license.php

Shader "MomomaShader/OverwriteScreen/NightMode"
{
	Properties
	{
		_Alpha ("Alpha", Range(0, 1)) = 1.0
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue" = "Overlay+6000" "IgnoreProjector" = "True" }
		Pass
		{
			ZTest Always
			ZWrite Off
			Blend Zero OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			fixed _Alpha;

			struct appdata
			{
				float4 vertex : POSITION;
				float4 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.pos = float4(2.0 * v.uv.x - 1.0, 1.0 - 2.0 * v.uv.y, 1.0, 1.0);
				o.uv = v.uv;
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				return fixed4(0, 0, 0, _Alpha);
			}
			ENDCG
		}
	} 
}
