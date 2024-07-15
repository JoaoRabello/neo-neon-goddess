Shader "FrenzyGames/GUI/Blured"
{
    Properties
    {
        [PerRendererData] _MainTex("_MainTex", 2D) = "white" {}
		[Toggle(COLORIZE)] COLORIZE("Colorize?", Float) = 0
        [HDR]_Color("_Color", Color) = (1,1,1,1)
        _BlurRadius("_BlurRadius", Range(-5, 5)) = 1
        _BlurIntensity("_BlurIntensity", Range(0,2)) = 1
		[KeywordEnum(3_Samples, 5_Samples, 9_Samples)] _BlurSamples ("Blur samples (higher are more expensive)", Float) = 0
		
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
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            #pragma multi_compile __ UNITY_UI_CLIP_RECT
			#pragma shader_feature __ COLORIZE
			#pragma shader_feature __ UNITY_TEXT
			#pragma shader_feature _BLURSAMPLES_3_SAMPLES _BLURSAMPLES_5_SAMPLES _BLURSAMPLES_9_SAMPLES
		
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
                float4 worldPosition : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            float4 _ClipRect;

            sampler2D _MainTex;
			float4 _MainTex_TexelSize;
			half _BlurIntensity;
			half _BlurRadius;
			half4 _Color;

            v2f vert(appdata_t v)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                OUT.worldPosition = v.vertex;
                OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);
                OUT.uv = v.texcoord;
                OUT.color = v.color;
                return OUT;
            }
			
			fixed4 getMainTexColor(float2 uv)
			{
				#ifdef UNITY_TEXT
					return tex2D(_MainTex, uv).aaaa;
				#else
					return tex2D(_MainTex, uv);	
				#endif
			}

            fixed4 frag(v2f IN) : SV_Target
            {				
			#ifdef _BLURSAMPLES_3_SAMPLES
				const fixed blurKernel[3] = {0.25, 0.5, 0.25};
				const int blurSteps = 1;
				const int blurIterations = 3;
			#elif _BLURSAMPLES_5_SAMPLES
				const fixed blurKernel[5] = {0.0625, 0.25, 0.375, 0.25, 0.0625};
				const int blurSteps = 2;
				const int blurIterations = 5;
			#elif _BLURSAMPLES_9_SAMPLES
				const fixed blurKernel[9] = {0.00390625, 0.03125, 0.109375, 0.21875, 0.2734375, 0.21875, 0.109375, 0.03125, 0.00390625};
				const int blurSteps = 4;
				const int blurIterations = 9;
			#endif		

				fixed4 mainCol = getMainTexColor(IN.uv);
				fixed4 color = mainCol * blurKernel[blurSteps];
				int i = 0;

				for(i = blurSteps + 1; i < blurIterations; i++)
				{
					float2 uv = IN.uv + _MainTex_TexelSize.xy * (_BlurRadius * float(i - blurSteps));
					fixed4 texCol = getMainTexColor(uv);
					color += texCol * blurKernel[i];
				}

				for(i = 0; i < blurSteps; i++)
				{
					float2 uv = IN.uv + _MainTex_TexelSize.xy * (_BlurRadius * float(i - blurSteps));
					fixed4 texCol = getMainTexColor(uv);
					color += texCol * blurKernel[i];
				}

				color = saturate(lerp(mainCol, color, _BlurIntensity)) * IN.color;

			#ifdef COLORIZE
				color *= _Color;
			#endif

            #ifdef UNITY_UI_CLIP_RECT
                color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
            #endif
				
                return color;
            }
        ENDCG
        }
    }
}