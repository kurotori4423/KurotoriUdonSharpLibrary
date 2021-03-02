// Copyright (c) 2020 momoma
// Released under the MIT license
// https://opensource.org/licenses/mit-license.php

Shader "MomomaShader/RayTracing/FakeFloor"
{
	Properties
	{
		_Color ("Color", Color) = (1, 1, 1, 1)
		_MainTex ("Outer Texture", 2D) = "white" {}
		_Plane ("Plane", Vector) = (0, 1, 0, 0.5)
		[Header(Fog Settings)]
		_FogDensity ("Fog Density", Range(0, 1)) = 0.4
		_FogColor ("Fog Color", Color) = (0, 0, 0, 1)
	}
	SubShader
	{
		Tags { "DisableBatching" = "True" }
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
				float3 objPos : TEXCOORD1;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed4 _Color;
			fixed4 _Plane;
			fixed _FogDensity;
			fixed4 _FogColor;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.objPos = v.vertex;
				o.uv = v.uv;
				return o;
			}

			inline float intersect_Plane(float3 r0, float3 rd, float4 p)
			{
				return -(dot(r0, p.xyz) + p.w) / dot(rd, p.xyz);
			}

			fixed4 frag(v2f i) : SV_Target
			{
				float4 c = 1.0;
				float3 r0 = mul(unity_WorldToObject, float4(_WorldSpaceCameraPos, 1.0));
				float3 rd = normalize(i.objPos - r0);

				float d = intersect_Plane(r0, rd, _Plane);
				float3 p = r0 + rd * d;
				bool isPlane = (d > 0);
				c.rgb = isPlane * tex2Dgrad(_MainTex, TRANSFORM_TEX(p.xz, _MainTex), ddx(p.xz), ddy(p.xz)) * _Color;
				float fogFactor = (isPlane ? max(d - distance(i.objPos, r0), 0.0) : 9999) * _FogDensity;
				c.rgb = lerp(_FogColor.rgb, c.rgb, saturate(exp2(-fogFactor * fogFactor)));

				return c;
			}
			ENDCG
		}
	}
}
