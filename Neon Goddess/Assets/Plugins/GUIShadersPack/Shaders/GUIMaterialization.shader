Shader "FrenzyGames/GUI/Materialization"
{
    Properties
    {
        [PerRendererData] _MainTex("_MainTex", 2D) = "white" {}
		[Header(Dematerialization)]
		_NoiseTex("_NoiseTex (r - dissappera progress, gb - direction, world space UV)", 2D) = "white" {}
		[Toggle(TEST_NOISE_TILING)] TEST_NOISE_TILING("Test noise tiling? (shows how looks _NoiseTex in world UV)", Float) = 0
		_Progress("_Progress", Range(0,1)) = 0
		_FadeOutRange("_FadeOutRange", Range(0.0001, 1)) = 0.1
		_MoveDistance("_MoveDistance", Float) = 100
		_TriangleDensity("_TriangleDensity", Range(1,100)) = 5

		[Space]
		[Header(Burn color)]
		_BurnColor("_BurnColor (alpha is transparency to basic color)", Color) = (1,1,1,1)
		_BurnIntensity("_BurnIntensity", Range(0,2)) = 1
		[Toggle(USE_BURN_TEX)] USE_BURN_TEX("Use _BurnTex gradient?", Float) = 0
		_BurnTex("_BurnTex (gradient)", 2D) = "white" {}
		
		[Space]
		[Header(Stencil)]
        _StencilComp("Stencil Comparison", Float) = 8
        _Stencil("Stencil ID", Float) = 0
        _StencilOp("Stencil Operation", Float) = 0
        _StencilWriteMask("Stencil Write Mask", Float) = 255
        _StencilReadMask("Stencil Read Mask", Float) = 255
		[HideInInspector]_ColorMask ("Color Mask", Float) = 15
			
		[Space]
		[Header(Text)]
		[Toggle(UNITY_TEXT)] UNITY_TEXT("Is on Unity Text component?", Float) = 0
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
            #pragma hull hull
            #pragma domain domain
            #pragma vertex tessvert
            #pragma fragment frag                      
			#pragma geometry geom
            #pragma target 5.0

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"
            #include "Tessellation.cginc"

            #pragma multi_compile __ UNITY_UI_CLIP_RECT
            #pragma shader_feature __ USE_BURN_TEX
			#pragma shader_feature __ TEST_NOISE_TILING
			#pragma shader_feature __ UNITY_TEXT
		
            struct VertData
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 texcoord : TEXCOORD0;
            };
				
            struct VertOutput 
			{
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : TEXCOORD1;
            };

            struct TessVertex 
			{
                float4 vertex : INTERNALTESSPOS;
                float2 texcoord : TEXCOORD0;
                fixed4 color : TEXCOORD1;
            };

            struct FragData
            {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float2 uv : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
                float2 uvNoise : TEXCOORD2;
				fixed vertexOffsetProgress : TEXCOORD3;
            };
			
			struct OutputPatchConstant 
			{
                float edge[3]         : SV_TessFactor;
                float inside          : SV_InsideTessFactor;
            };

            float4 _ClipRect;

            sampler2D _MainTex;
			fixed _Progress;
            sampler2D _NoiseTex;
            float4 _NoiseTex_ST;
			
			fixed _FadeOutRange;
			float _TriangleDensity;
			float _MoveDistance;

		#ifdef USE_BURN_TEX
            sampler2D _BurnTex;
		#endif
			fixed4 _BurnColor;
			half _BurnIntensity;
			
            TessVertex tessvert(VertData v) 
			{
                TessVertex o;
                o.vertex = v.vertex;
                o.texcoord = v.texcoord;
                o.color = v.color;
                return o;
            }

            float4 tessellation(TessVertex v, TessVertex v1, TessVertex v2)
			{
                float tv = _TriangleDensity;
                float tv1 = _TriangleDensity;
                float tv2 = _TriangleDensity;
                return float4( tv1+tv2, tv2+tv, tv+tv1, tv+tv1+tv2 ) * float4(0.5, 0.5, 0.5, 0.33333);
				//return UnityDistanceBasedTess(v.vertex, v1.vertex, v2.vertex, _ElementSize, _ElementSize * 100, 100);
            }

            OutputPatchConstant hullconst(InputPatch<TessVertex,3> v) 
			{
                OutputPatchConstant o;
                float4 ts = tessellation( v[0], v[1], v[2] );
				o.edge[0] = ts.x;
                o.edge[1] = ts.y;
                o.edge[2] = ts.z;
                o.inside = ts.w;
 
                return o;
            }

            [domain("tri")]
            [partitioning("fractional_odd")]
            [outputtopology("triangle_cw")]
            [patchconstantfunc("hullconst")]
            [outputcontrolpoints(3)]
            TessVertex hull(InputPatch<TessVertex,3> v, uint id : SV_OutputControlPointID) 
			{
				return v[id];
            }

            VertOutput vert(VertData v)
            {
                VertOutput OUT;
                OUT.vertex = v.vertex;
                OUT.uv = v.texcoord;
                OUT.color = v.color;
                return OUT;
            }

            [domain("tri")]
            VertOutput domain(OutputPatchConstant tessFactors, const OutputPatch<TessVertex,3> vi, float3 bary : SV_DomainLocation)
			{
                VertData v;
                v.vertex = vi[0].vertex*bary.x + vi[1].vertex*bary.y + vi[2].vertex*bary.z;
                v.texcoord = vi[0].texcoord*bary.x + vi[1].texcoord*bary.y + vi[2].texcoord*bary.z;
                v.color = vi[0].color*bary.x + vi[1].color*bary.y + vi[2].color*bary.z;
                VertOutput o = vert(v);
                return o;
            }
			
			[maxvertexcount(3)]
            void geom(triangle VertOutput p[3], inout TriangleStream<FragData> triStream)
            {   
				float4 middleVertexWPos = (p[0].vertex + p[1].vertex + p[2].vertex) * 0.33333;
				float2 uvNoise = mul(unity_ObjectToWorld, middleVertexWPos).xy * _NoiseTex_ST.xy + _NoiseTex_ST.zw;
				fixed3 noise = tex2Dlod(_NoiseTex, float4(uvNoise, 0, 0));
				fixed fadeOut = noise.r;
				fixed vertexOffsetProgress = saturate((_Progress * (1 + _FadeOutRange) - fadeOut) / _FadeOutRange);

				float4 vertexOffset = float4(_MoveDistance * vertexOffsetProgress * (normalize(noise.gb + 0.0001 /*in case of 0 value*/) * 2 - 1), 0, 0);
				
				FragData pIn;
				pIn.uvNoise = uvNoise;
				pIn.vertexOffsetProgress = vertexOffsetProgress;

				for (int i = 0; i < 3; i++)
				{
					pIn.worldPosition = p[i].vertex +vertexOffset;
					pIn.vertex = UnityObjectToClipPos(pIn.worldPosition);
					pIn.color = p[i].color;
					pIn.uv = p[i].uv;
					triStream.Append(pIn);
				}

				triStream.RestartStrip();
		
            }

            half4 frag(FragData IN) : SV_Target
			{ 
			#ifdef TEST_NOISE_TILING
				return half4(tex2D(_NoiseTex, IN.uvNoise).rgb, 1);
			#endif
				clip(0.9999 - IN.vertexOffsetProgress);
				
			#ifdef UNITY_TEXT
				half4 color = tex2D(_MainTex, IN.uv).aaaa * IN.color;
			#else
                half4 color = tex2D(_MainTex, IN.uv) * IN.color;	
			#endif

				fixed burnVisibility = step(0.001, IN.vertexOffsetProgress);
			#ifdef USE_BURN_TEX
				float2 burnUV = float2(1.0 - IN.vertexOffsetProgress, 1);
				half4 burnColor = tex2D(_BurnTex, burnUV) * _BurnColor;
				color.rgb = lerp(color, burnColor.rgb * _BurnIntensity, burnVisibility * burnColor.a);
			#else
				color.rgb = lerp(color, _BurnColor.rgb * _BurnIntensity, burnVisibility * _BurnColor.a);
			#endif

            #ifdef UNITY_UI_CLIP_RECT
                color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
            #endif
				
                return color;
            }
        ENDCG
        }
    }
    FallBack "UI/Default"
}