Shader "FrenzyGames/GUI/Scripted/GlowWithEmissionTex"
{
    Properties
    {
        [PerRendererData] _MainTex("_MainTex", 2D) = "white" {}

		[Header(NOTE Requires GUIShaderSupport script attached to Image or Text)]
        _EmissionTex("_EmissionTex", 2D) = "white" {}
		[Header(Glow)]
		_GlowIntensity("_GlowIntensity", Range(0,10)) = 1
		
		[Space]
		[Header(Animation)]
		[Toggle(ANIMATE)] ANIMATE("Animate?", Float) = 0
		_Freqency("_Freqency", Float) = 1
		_SecondGlowIntensity("_SecondGlowIntensity", Range(0,10)) = 1

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
			#pragma shader_feature __ ANIMATE
			#pragma shader_feature __ UNITY_TEXT

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
                UNITY_VERTEX_OUTPUT_STEREO
            };

            float4 _ClipRect;

            sampler2D _MainTex;
            sampler2D _EmissionTex;
            float4 _EmissionTex_ST;
			half _GlowIntensity;
		#ifdef ANIMATE
			half _SecondGlowIntensity;
			half _Freqency;
		#endif
			float4 _WorldPosToUV;
				
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
				OUT.uv2 = TRANSFORM_TEX(worldUV, _EmissionTex);
                OUT.color = v.color;
                return OUT;
            }


            half4 frag(v2f IN) : SV_Target
            {
			#ifdef UNITY_TEXT
                half4 color = tex2D(_MainTex, IN.uv).aaaa;
			#else
                half4 color = tex2D(_MainTex, IN.uv);			
			#endif
				
				half4 emission = tex2D(_EmissionTex, IN.uv2);	
			#ifdef ANIMATE
				color += (emission * IN.color) * lerp(_GlowIntensity, _SecondGlowIntensity, sin(_Time.y * _Freqency) * 0.5 + 0.5);
			#else
				color += (emission * IN.color) * _GlowIntensity;
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