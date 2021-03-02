// created by 1001

Shader "Skybox/CloudDomeShader"
{
	Properties
	{
		_cloudscale ("Cloud Scale", float) = 0.05
		_speedroll ("Speed Roll", float) = 0.0025
		_speedwind ("Speed Wind", float) = 0.01
		_clouddark ("Cloud Dark", float) = 0.5
		_cloudlight ("Cloud Light", float) = 0.3
		_skycolor ("Sky Color", Color) = (0.3, 0.5, 0.7, 1.0)
		_count ("Noise Iterations", Int) = 6
	}
	Subshader
	{
		Tags { "Queue"="Geometry+500" "RenderType"="Background" "PreviewType"="Skybox" }
		ZWrite Off
		Lighting Off
		SeparateSpecular Off
		Fog { Mode Off }
		Pass
		{
			CGPROGRAM
			#pragma vertex vertex_shader
			#pragma fragment pixel_shader
			#pragma target 5.0
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "Lighting.cginc"

			#define iTime _Time.g
			
			float _cloudscale;
			float _speedroll;
			float _speedwind;
			float _clouddark;
			float _cloudlight;
			float4 _skycolor;
			int _count;

			#define m float2x2(1.6, 1.2, -1.2, 1.6)

			inline float2 hash(float2 p)
			{
				p = float2(dot(p,float2(127.1,311.7)), dot(p,float2(269.5,183.3)));
				return frac(sin(p)*43758.5453123) - .5;
				// another ver. (see the comment in 'noise' func):
				//p  = frac(p * float2(0.3247122237916646, 0.134707348285849));
				//p += dot(p.xy, p.yx+19.19);
				//return frac(p)-.5;
			}

			inline float skyNoise(float2 p)
			{
				const float K1 = 0.366025404; 
				const float K2 = 0.211324865; 
				float2 i = floor(p + (p.x+p.y)*K1);	
				float2 a = p - i + (i.x+i.y)*K2;
				float2 o = (a.x>a.y) ? float2(1.0,0.0) : float2(0.0,1.0); //float2 of = 0.5 + 0.5*float2(sign(a.x-a.y), sign(a.y-a.x));
				float2 b = a - o + K2;
				float2 c = a - 1.0 + 2.0*K2;
				float3 h = max(0.5-float3(dot(a,a), dot(b,b), dot(c,c) ), 0.0 );
				float3 n = 2.*h*h*h*h*float3( dot(a,hash(i)), dot(b,hash(i+o)), dot(c,hash(i+1.0)));
				return dot(n, float3(70.,70.,70.));	
			}

			inline float fbm(float2 n)
			{
				float total = 0.0, amplitude = 0.1;
				for(int i = 0; i < _count; ++i)
				{
					total += skyNoise(n) * amplitude;
					n = mul(m, n);
					amplitude *= 0.4;
				}
				return total;
			}

			inline float noise( in float2 p )
			{
				return frac((sin(p.x)+cos(p.y))*43758.5453123);
				// my tests showed that another ver. has a tiny speed difference:
				// - a little bit faster on Intel chips,
				// - but a little bit slower on Nvidia:
				//  p  = frac(p * float2(0.3247122237916646, 0.134707348285849));
				//  p += dot(p.xy, p.yx+19.19);
				//  return frac(p.x * p.y);
			}

			float4 sky(in float3 rp, in float3 rd)
			{
				rp += 4. * rd / rd.y;
				float2 uvbase = rp.xz;
				float q = fbm(uvbase * _cloudscale);
				float time = iTime * _speedroll;
				float3 skycolour = _skycolor;//= mix(skycolour2, skycolour1, uvbase.y);
				uvbase = uvbase * _cloudscale + time;

				float2 uv = uvbase - q;
				float r = 0., f = 0., wr = .8, wf = .7; //skyNoise shape
				for(int i = 0; i <= _count; ++i)
				{
					r += abs(wr*skyNoise( uv ));
					f += wf*skyNoise( uv );
					uv = mul(m ,uv + time);
					wr *= .7;
					wf *= .6;
				}
				f *= r + f;
				f = .2 + 8.*f*r;

				float c = 0., weight = .4; //skyNoise colour
				uv = uvbase * 2. - q;
				float2 uv1 = uvbase * 3. - q;
				for(i = 0; i < _count; ++i)
				{
					c += weight*(skyNoise( uv ) + abs(skyNoise( uv1 )));
					uv = mul(m,uv + time + time);
					uv1 = mul(m,uv1 + time*3.);
					weight *= .6;
				}

				float3 cloudcolour = float3(1.1, 1.1, 0.9) * clamp((_clouddark + _cloudlight*c), 0.0, 1.0);
				float3 result = lerp(skycolour, clamp(.5 * skycolour + cloudcolour, 0.0, 1.0), clamp(f + c, 0.0, 1.0));

				result *= smoothstep(0.0, 0.2, rd.y);
				return float4(result, 1.0);
			}

			//-----------------------------------------------------------------------

			float4 drawImage(in float3 ro, in float3 rd )
			{
				float3 rp = ro;

				float4 fragColor = float4(1.0,1.0,1.0,1.0);
				if(true)
					//if(rd.y > -0.08)
						rp += rd * 10.0;
				//fog:
				float dist = length(ro - rp);
				float3 fog = _skycolor;
				fog = lerp(fog, float3(1.0,1.0,1.0), smoothstep(0.5,-0.4, rd.y));
				fragColor.rgb = lerp(fragColor.rgb, fog, smoothstep(3.0, 10.0, dist));

				//sky:
				if(rd.y > 0.02) fragColor = lerp(fragColor, float4(1.,1.,1.,1.), sky(ro, rd));

				//sun halo:
				float halo = 2.053136; //pow(11.0, .3);
				if (any(_WorldSpaceLightPos0.xyz))
				{
					float rdld = dot(rd, _WorldSpaceLightPos0.xyz);
					halo = rdld > 0 ? pow(1.0 - rdld * rdld, .15) : halo;
				}
				fragColor += clamp(1.0 - halo, 0.0, 1.0) * _LightColor0;
				float mx = max(fragColor.r, fragColor.g);
				mx = max(fragColor.b, mx);
				fragColor /= max(1.0, mx); //remove its defect

				// contrast
				const float contr = 0.2;
				return fragColor * .2 + (1.0 - contr) * fragColor * fragColor * (3.0 - 2.0 * fragColor);
			}

			//--------------------------------------------------------------------------

			struct custom_type
			{
				fixed4 screen_vertex : SV_POSITION;
				fixed3 world_vertex : TEXCOORD1;
			};

			custom_type vertex_shader(fixed4 vertex : POSITION, float2 uv:TEXCOORD0)
			{
				custom_type vs;
				vs.screen_vertex = UnityObjectToClipPos(vertex);
				vs.world_vertex = mul(unity_ObjectToWorld, vertex);
				return vs;
			}

			fixed4 pixel_shader(custom_type ps ) : SV_TARGET
			{
				fixed3 viewDirection = normalize(ps.world_vertex - _WorldSpaceCameraPos);
				fixed3 rp = float3(0.0, 0.5, -1.0);
				rp.z += iTime * _speedwind;				
				return drawImage(rp, viewDirection);
			}
			ENDCG
		}
	}
}