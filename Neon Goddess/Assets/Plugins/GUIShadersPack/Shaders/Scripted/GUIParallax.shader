Shader "FrenzyGames/GUI/Scripted/Parallax"
{
    Properties
    {
        [PerRendererData] _MainTex("_MainTex", 2D) = "white" {}
	
		[Header(NOTE Requires GUIShaderSupport script attached to Image)]

		[Header(Parallax)]
        _HeightMap("_HeightMap", 2D) = "white" {}
		_HeightScale("_HeightScale", Range(-0.1,0.1)) = 0.01
		_ParallaxSamples("_ParallaxSamples", Int) = 5
		[Toggle(CLAMP_PARALLAX)] _ClampParallax("Clamp parallax offset?", Float) = 0
		[Toggle(INVERSE_HEIGHTMAP)] _InverseHeightMap("Inverse height map?", Float) = 0
		//_LookDir("_LookDir(world space)", Vector) = (0,0,1,0)//modified by script
		
		[Space]
		[Header(Stencil)]
        _StencilComp("Stencil Comparison", Float) = 8
        _Stencil("Stencil ID", Float) = 0
        _StencilOp("Stencil Operation", Float) = 0
        _StencilWriteMask("Stencil Write Mask", Float) = 255
        _StencilReadMask("Stencil Read Mask", Float) = 255
		[HideInInspector]_ColorMask ("Color Mask", Float) = 15
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
            "DisableBatching"="True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            #pragma multi_compile __ UNITY_UI_CLIP_RECT
			#pragma shader_feature CLAMP_PARALLAX
			#pragma shader_feature INVERSE_HEIGHTMAP

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 uv  : TEXCOORD0;
                float2 uv2  : TEXCOORD1;
                float4 worldPosition : TEXCOORD2;//its in canvas scale
                float3 viewDir : TEXCOORD3;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            float4 _ClipRect;

            sampler2D _MainTex;
            sampler2D _HeightMap;
            float4 _HeightMap_ST;
			float _HeightScale;
			float3 _LookDir;
			float4 _WorldPosToUV;
			int _ParallaxSamples;
				
            v2f vert(appdata_t v)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                OUT.worldPosition = v.vertex;
                OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);
                OUT.uv = v.texcoord;
				float4 wPos = mul(unity_ObjectToWorld, v.vertex);
				float2 worldUV = float2((wPos.x - _WorldPosToUV.x) / _WorldPosToUV.z,
										(wPos.y - _WorldPosToUV.y) / _WorldPosToUV.w);
				OUT.uv2 = TRANSFORM_TEX(worldUV, _HeightMap);
                OUT.color = v.color;
				
				OUT.viewDir = _LookDir;

                return OUT;
            }

			float2 parallaxOffset(float3 viewDir, float2 heightMapUV, sampler2D heightMap, int maxSamples) 
			{
				float parallaxLimit = length(viewDir.xy);
				if(parallaxLimit == 0)
					return float2(0,0);
				float2 offsetDir = viewDir.xy / parallaxLimit;
				parallaxLimit = -parallaxLimit * _HeightScale;
	
				float2 maxOffset = offsetDir * parallaxLimit;
	
				float stepSize = 1.0 / (float)maxSamples;
	
				float2 dx = ddx(heightMapUV);
				float2 dy = ddy(heightMapUV);
	
				float currRayHeight = 1.0;
				float2 currentOffset = float2(0, 0);
				float2 lastOffset = float2(0, 0);

				float lastSampledHeight = 1;
				float currSampledHeight = 1;

				int currentSample = 0;
					
				while ( currentSample < maxSamples )
				{
				#if INVERSE_HEIGHTMAP
					currSampledHeight = 1 - tex2Dgrad(heightMap, heightMapUV + currentOffset, dx, dy).r;
				#else
					currSampledHeight = tex2Dgrad(heightMap, heightMapUV + currentOffset, dx, dy).r;
				#endif
					if(currSampledHeight > currRayHeight)
					{
						float delta1 = currSampledHeight - currRayHeight;
						float delta2 = (currRayHeight + stepSize) - lastSampledHeight;

						float ratio = delta1 / (delta1+delta2);

						currentOffset = ratio * lastOffset + (1.0 - ratio) * currentOffset;

						currentSample = maxSamples + 1;
					}
					else
					{
						currentSample++;

						currRayHeight -= stepSize;

						lastOffset = currentOffset;
						currentOffset += stepSize * maxOffset;

						lastSampledHeight = currSampledHeight;
					}
				}
	
				return currentOffset;
			}

            half4 frag(v2f IN) : SV_Target
			{
				float2 fromHeightMapOffset = parallaxOffset(IN.viewDir, IN.uv2, _HeightMap, _ParallaxSamples);

			#if CLAMP_PARALLAX
                half4 color = tex2D(_MainTex, saturate(IN.uv + fromHeightMapOffset));	
			#else
                half4 color = tex2D(_MainTex, IN.uv + fromHeightMapOffset);	
			#endif
				
            #ifdef UNITY_UI_CLIP_RECT
                color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
            #endif
                return color * IN.color;
            }
        ENDCG
        }
    }
}