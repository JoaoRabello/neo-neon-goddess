Shader "FrenzyGames/GUI/Hologram"
{
    Properties
    {
        [PerRendererData] _MainTex("_MainTex", 2D) = "white" {}

		[Space]
		[Header(Hologram)]
		_GlowIntensity("_GlowIntensity", Range(0,10)) = 1
		_ColorMax("_ColorMax (visible color)", Color) = (0,0.6,1,1)
		_ColorMin("_ColorMin (invisible color)", Color) = (0,0,0,0)
		_LinesSetup("_LinesSetup (density, speed)", Vector) = (0.1, -2, 0, 0)

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
			half _GlowIntensity;
			fixed _AlphaThreshold;
			half4 _ColorMin;
			half4 _ColorMax;
			half2 _LinesSetup;
				
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
				fixed hologramVisibility = sin(IN.worldPosition.y * _LinesSetup.x + _Time.y * _LinesSetup.y) * 0.5 + 0.5;
				half4 hologramColor = lerp(_ColorMin, _ColorMax, hologramVisibility);
				hologramColor.rgb *= _GlowIntensity;
				
			#ifdef UNITY_TEXT
                half4 color = tex2D(_MainTex, IN.uv).aaaa * IN.color * hologramColor;	
			#else
                half4 color = tex2D(_MainTex, IN.uv) * IN.color * hologramColor;				
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