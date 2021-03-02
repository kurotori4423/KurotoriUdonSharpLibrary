// Copyright (c) 2020 momoma
// Released under the MIT license
// https://opensource.org/licenses/mit-license.php

Shader "MomomaShader/Fragment/DummyMirror"
{
	Properties
	{
		_UVScale ("UV Scale", Float) = 10.0
		_Clip ("Clip", Range(0, 1)) = 0.0
		_ClipUVScale ("Clip UV Scale", Float) = 0.2
		_Height ("Height", Range(0, 1)) = 0.02
		[HDR] _BorderColor ("Border Color", Color) = (1, 1, 1, 1)
	}
	SubShader
	{
		Tags { "RenderType" = "TransparentCutout" "Queue" = "AlphaTest+1" "DisableBatching" = "True" }
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float3 worldPos : TEXCOORD2;
				float3x3 matrix_W2O : TEXCOORD3;
				float4 vertex : SV_POSITION;
			};
			
			fixed _UVScale, _Clip, _ClipUVScale, _Height;
			fixed4 _BorderColor;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				o.matrix_W2O = float3x3(normalize(unity_WorldToObject[0].xyz), normalize(unity_WorldToObject[1].xyz), normalize(unity_WorldToObject[2].xyz));
				o.uv = mul(o.matrix_W2O, o.worldPos).xy * _UVScale;
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}

			inline float2 hash22(float2 p)
			{
				static const float2 k = float2(0.3183099, 0.3678794);
				p = p * k + k.yx;
				return frac(16.0 * k * frac(p.x * p.y * (p.x + p.y))) * 2.0 - 1.0;
			}

			float simplexNoise2D(float2 p)
			{
				const float K1 = 0.366025404;//(sqrt(3)-1)/2;
				const float K2 = 0.211324865;//(3-sqrt(3))/6;
	
				float2 i = floor(p + (p.x + p.y) * K1);
				float2 a = p - i + (i.x + i.y) * K2;
				float2 o = (a.x > a.y) ? float2(1.0, 0.0) : float2(0.0, 1.0);
				float2 b = a - o + K2;
				float2 c = a - 1.0 + 2.0 * K2;
				float3 h = max(0.5 - float3(dot(a, a), dot(b, b), dot(c, c) ), 0.0);	
				float3 n = h * h * h * h * float3(dot(a, hash22(i)), dot(b, hash22(i + o)), dot(c, hash22(i + 1.0)));
	
				return (n.x + n.y + n.z) * 35.0 + 0.5;
			}
			
			float3 getNormal(float2 uv)
			{
				float2 dx = ddx(uv);
				float2 dy = ddy(uv);
				float h0 = _Height * simplexNoise2D(uv);
				float h1 = _Height * simplexNoise2D(uv + dx);
				float h2 = _Height * simplexNoise2D(uv + dy);
				return normalize(cross(float3(dx, h1 - h0), float3(dy, h2 - h0)));
			}
			
			inline float3 boxProjection(float3 normalizedDir, float3 worldPosition, float4 probePosition, float3 boxMin, float3 boxMax)
			{
				#if UNITY_SPECCUBE_BOX_PROJECTION
					if (probePosition.w > 0)
					{
						float3 magnitudes = ((normalizedDir > 0 ? boxMax : boxMin) - worldPosition) / normalizedDir;
						float magnitude = min(min(magnitudes.x, magnitudes.y), magnitudes.z);
						normalizedDir = normalizedDir * magnitude + (worldPosition - probePosition);
					}
				#endif

				return normalizedDir;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				float clipDist = simplexNoise2D(i.uv * _ClipUVScale) * (1 + _ClipUVScale * 0.2) - _Clip;
				clip(clipDist - 0.001);
				float3 normal = mul(getNormal(i.uv), i.matrix_W2O);
				float3 direction = normalize(i.worldPos - _WorldSpaceCameraPos);
				float3 reflDir = reflect(direction, normal);
				float3 reflDir0 = boxProjection(reflDir, i.worldPos, unity_SpecCube0_ProbePosition, unity_SpecCube0_BoxMin, unity_SpecCube0_BoxMax);
				float3 reflDir1 = boxProjection(reflDir, i.worldPos, unity_SpecCube1_ProbePosition, unity_SpecCube1_BoxMin, unity_SpecCube1_BoxMax);
				float4 refColor0 = UNITY_SAMPLE_TEXCUBE_LOD(unity_SpecCube0, reflDir0, 0);
				float4 refColor1 = UNITY_SAMPLE_TEXCUBE_SAMPLER_LOD(unity_SpecCube1, unity_SpecCube0, reflDir1, 0);
				refColor0.rgb = DecodeHDR(refColor0, unity_SpecCube0_HDR);
				refColor1.rgb = DecodeHDR(refColor1, unity_SpecCube1_HDR);
				float4 refColor = lerp(refColor1, refColor0, unity_SpecCube0_BoxMin.w);
				float4 c = lerp(refColor, _BorderColor, smoothstep(_ClipUVScale * 0.2, _ClipUVScale * 0.1, clipDist));
				
				UNITY_APPLY_FOG(i.fogCoord, c);
				return c;
			}
			ENDCG
		}
	}
	FallBack "Standard"
}
