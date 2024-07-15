Shader "FrenzyGames/GUI/ColorCorrection"
{
    Properties
    {
        [PerRendererData] _MainTex("_MainTex", 2D) = "white" {}
		[Toggle(COLORIZE)] COLORIZE("Colorize?", Float) = 0
        _Color("_Color", Color) = (1,1,1,1)
		[Toggle(USE_GAMMA)] USE_GAMMA("Use gamma correction?", Float) = 0
        _Gamma("_Gamma", Float) = 1
		[Toggle(USE_SATURATION)] USE_SATURATION("Use saturation correction?", Float) = 0
        _Saturation("_Saturation", Float) = 1
		[Toggle(USE_BRIGHTNESS)] USE_BRIGHTNESS("Use brightness correction?", Float) = 0
        _Brightness("_Brightness", Float) = 1
		[Toggle(USE_CONTRAST)] USE_CONTRAST("Use contrast correction?", Float) = 0
        _Contrast("_Contrast", Float) = 1
		
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
			#pragma shader_feature __ USE_SATURATION
            #pragma shader_feature __ USE_CONTRAST
			#pragma shader_feature __ USE_BRIGHTNESS
			#pragma shader_feature __ USE_GAMMA
			#pragma shader_feature __ COLORIZE
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
                float4 worldPosition : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            float4 _ClipRect;

            sampler2D _MainTex;
			fixed4 _Color;
			half _Saturation;
			half _Contrast;
			half _Brightness;
			half _Gamma;

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
			
            half4 frag(v2f IN) : SV_Target
            {				
			#ifdef UNITY_TEXT
                half4 color = tex2D(_MainTex, IN.uv).aaaa * IN.color;
			#else
                half4 color = tex2D(_MainTex, IN.uv) * IN.color;	
			#endif

			#ifdef COLORIZE
				color *= _Color;
			#endif

			#ifdef USE_GAMMA
				color.rgb = pow(color.rgb, _Gamma);
			#endif

			#ifdef USE_SATURATION
				half avgColor = (color.r + color.g + color.b) * 0.3333;
				color.rgb = lerp(half3(avgColor, avgColor, avgColor), color.rgb, _Saturation);
			#endif

			#ifdef USE_BRIGHTNESS
				color.rgb += _Brightness;
			#endif

			#ifdef USE_CONTRAST
				color.rgb = (color.rgb - 0.5) * _Contrast + 0.5;
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