// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

// Copyright (c) 2020 momoma
// Released under the MIT license
// https://opensource.org/licenses/mit-license.php

Shader "MomomaShader/UI/Cube"
{
	Properties
	{
		[PerRendererData] _MainTex ("", 2D) = "black" {}
		_Color ("Tint", Color) = (1,1,1,1)

		_StencilComp ("Stencil Comparison", Float) = 8
		_Stencil ("Stencil ID", Float) = 0
		_StencilOp ("Stencil Operation", Float) = 0
		_StencilWriteMask ("Stencil Write Mask", Float) = 255
		_StencilReadMask ("Stencil Read Mask", Float) = 255

		_ColorMask ("Color Mask", Float) = 15

		[Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0

		_RimStrength ("Rim Strength", Range(0, 1)) = 1.0
		_MaxRadius ("Max Radius", Float) = -1.0
	}
	SubShader
	{
		Tags
		{
			"RenderType" = "TransparentCutout"
			"Queue" = "AlphaTest"
			"IgnoreProjector"="True"
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}

		Stencil
		{
			Ref [_Stencil]
			Comp [_StencilComp]
			Pass [_StencilOp]
			ReadMask [_StencilReadMask]
			WriteMask [_StencilWriteMask]
		}

		Lighting Off
		ColorMask [_ColorMask]

		Pass
		{
			Name "Default"
			CGPROGRAM
			#pragma target 5.0
			#pragma vertex vert
			#pragma geometry geom
			#pragma fragment frag

			#include "UnityCG.cginc"
			#include "UnityUI.cginc"

			#pragma multi_compile __ UNITY_UI_CLIP_RECT
			#pragma multi_compile __ UNITY_UI_ALPHACLIP

			struct appdata
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				float2 uv : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 worldPos : TEXCOORD1;
				float3 center : TEXCOORD2;
				float3 size : TEXCOORD3;
				float4 color : COLOR;
				float4 vertex : SV_POSITION;
				UNITY_VERTEX_OUTPUT_STEREO
			};

			struct fragOut
			{
				fixed4 color : SV_Target;
				float depth : SV_Depth;
			};

			sampler2D _MainTex;
			fixed4 _Color;
			fixed4 _TextureSampleAdd;
			float4 _ClipRect;

			fixed _RimStrength;
			fixed _MaxRadius;

			v2f vert(appdata v)
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.worldPos = v.vertex;
				o.color = v.color * _Color;
				o.center = 0;
				o.size = 0;
				return o;
			}

			[maxvertexcount(14)]
			void geom(triangle v2f input[3], inout TriangleStream<v2f> outStream)
			{
				v2f o;
				float3 center = float3((input[0].worldPos.xy + input[2].worldPos.xy) * 0.5, input[0].worldPos.z * 0.5);
				float3 size = abs(float3(input[2].worldPos.x - input[1].worldPos.x, input[0].worldPos.y - input[1].worldPos.y, input[0].worldPos.z * 0.99));
				[unroll]
				for (int k = 0; k < 3; ++k)
				{
					o = input[k];
					o.center = center;
					o.size = size;
					outStream.Append(o);
				}
				outStream.RestartStrip();
				[unroll]
				for (k = 2; k > -1; --k)
				{
					o = input[k];
					o.center = center;
					o.size = size;
					o.worldPos.z = 0;
					o.vertex = UnityObjectToClipPos(o.worldPos);
					outStream.Append(o);
				}
				outStream.RestartStrip();

				if (size.z == 0) return;

				[unroll]
				for (k = 0; k < 2; ++k)
				{
					[unroll]
					for (int l = 0; l < 2; ++l)
					{
						o = input[k + l];
						o.center = center;
						o.size = size;
						outStream.Append(o);
						o.worldPos.z = 0;
						o.vertex = UnityObjectToClipPos(o.worldPos);
						outStream.Append(o);
					}
					outStream.RestartStrip();
				}
			}

			inline float4 intersect_RoundedCube(float3 r0, float3 rd, float3 c, float3 s)
			{
				float3 v = c - r0;
				float3 v1 = v / rd - s / abs(rd);
				float t = max(v1.x, max(v1.y, v1.z));
				float r = min(s.x, s.y);
				r = 0 < _MaxRadius ? min(_MaxRadius, r) : r;
				float2 h = max(0.0, s.xy - r);
				v.xy = clamp(t * rd.xy, v.xy - h, v.xy + h);
				float b = dot(v.xy, normalize(rd.xy));
				float d = b * b + r * r - dot(v.xy, v.xy);
				float l = (b - sqrt(max(d, 0.0))) / length(rd.xy);
				float2 tv = (t * rd - v).xy;
				return 0 <= d && abs(l * rd.z - v.z) < s.z ? float4(l, normalize(float3(v.xy - rd.xy * l, 0.0))) : (dot(tv, tv) < r * r ? float4(t, float3(0.0, 0.0, sign(v.z))) : -1.0);
			}

			inline float compute_depth(float4 pos)
			{
				#if UNITY_UV_STARTS_AT_TOP
					return pos.z / pos.w;
				#else
					return (pos.z / pos.w) * 0.5 + 0.5;
				#endif
			}

			fragOut frag(v2f i)
			{
				float4 color = (tex2D(_MainTex, i.uv) + _TextureSampleAdd) * i.color;

				#ifdef UNITY_UI_CLIP_RECT
				color.a *= UnityGet2DClipping(i.worldPos.xy, _ClipRect);
				#endif

				#ifdef UNITY_UI_ALPHACLIP
				clip (color.a - 0.001);
				#endif

				float3 r0 = mul(unity_WorldToObject, float4(_WorldSpaceCameraPos, 1.0));
				float3 rd = normalize(i.worldPos - r0);
				float4 d = intersect_RoundedCube(r0, rd, i.center, i.size * 0.5);
				clip(d.x);
				float rim = 1 - abs(dot(d.yzw, rd));
				color.rgb = lerp(color.rgb, 1.0, rim * rim * rim * _RimStrength);

				fragOut o;
				o.color = color;
				o.depth = compute_depth(UnityObjectToClipPos(r0 + rd * d.x));
				return o;
			}
			ENDCG
		}
	}
}
