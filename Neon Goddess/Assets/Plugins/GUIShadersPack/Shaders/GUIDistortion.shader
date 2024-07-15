Shader "FrenzyGames/GUI/Distortion"
{
    Properties
    {
        [PerRendererData] _MainTex("_MainTex", 2D) = "white" {}
		[Header(Distortion)]
		_DistortionPower("_DistortionPower", Range(0, 0.2)) = 0
		_DistortionTex("_DistortionTex (world space UV)", 2D) = "bump" {}
		_DistortionMoveDir("_DistortionMoveDir (higher values increase speed)", Vector) = (0,1,0,0)
		[Toggle(TEST_DISTORTION_TILING)] TEST_DISTORTION_TILING("Test distortion tiling? (shows how looks _DistortionTex in world UV)", Float) = 0
			
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
            "CanUseSpriteAtlas"="False"
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
			#pragma shader_feature __ TEST_DISTORTION_TILING
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
                float2 uvDistortion : TEXCOORD2;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            float4 _ClipRect;

            sampler2D _MainTex;
			float _DistortionPower;
            sampler2D _DistortionTex;
            float4 _DistortionTex_ST;
			float2 _DistortionMoveDir;
				
            v2f vert(appdata_t v)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                OUT.worldPosition = v.vertex;
                OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);
                OUT.uv = v.texcoord;
                OUT.color = v.color;
                OUT.uvDistortion = (mul(unity_ObjectToWorld, v.vertex).xy + _DistortionMoveDir.xy * _Time.y) * _DistortionTex_ST.xy + _DistortionTex_ST.zw;
                return OUT;
            }


            fixed4 frag(v2f IN) : SV_Target
            {
			#ifdef TEST_DISTORTION_TILING
				return fixed4(UnpackNormal(tex2D(_DistortionTex, IN.uvDistortion)), 1);
			#endif
				float2 distortion = UnpackNormal(tex2D(_DistortionTex, IN.uvDistortion)).xy * _DistortionPower;
				
			#ifdef UNITY_TEXT
                fixed4 color = tex2D(_MainTex, IN.uv + distortion).aaaa * IN.color;		
			#else
                fixed4 color = tex2D(_MainTex, IN.uv + distortion) * IN.color;		
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