Shader "FrenzyGames/GUI/BurnToColor"
{
    Properties
    {
        [PerRendererData] _MainTex("_MainTex", 2D) = "white" {}

	
		[Space]
		[Header(Burn effect)]
		[HDR]_BurnedColor("_BurnedColor", Color) = (1,1,1,1)
		_BurnedSaturation("_BurnedSaturation", Float) = 1


		[Header(Dissolve)]
		_DissolveProgress("_DissolveProgress", Range(0,1)) = 0
		_DissolveTex("_DissolveTex (world space UV)", 2D) = "white" {}
		[Toggle(TEST_DISSOLVE_TILING)] TEST_DISSOLVE_TILING("Test dissolve tiling? (shows how looks _DissolveTex in world UV)", Float) = 0

		[Space]
		[Header(Burn color)]
		_BurnColor("_BurnColor", Color) = (1,1,1,1)
		_BurnRange("_BurnRange", Range(0.001,1)) = 0.02
		_BurnIntensity("_BurnIntensity", Range(0,3)) = 1
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
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            #pragma multi_compile __ UNITY_UI_CLIP_RECT
            #pragma shader_feature __ USE_BURN_TEX
			#pragma shader_feature __ TEST_DISSOLVE_TILING
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
                float2 uvDissolve : TEXCOORD2;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            float4 _ClipRect;

            sampler2D _MainTex;
			half4 _BurnedColor;
			half _BurnedSaturation;

			fixed _DissolveProgress;
            sampler2D _DissolveTex;
            float4 _DissolveTex_ST;

		#ifdef USE_BURN_TEX
            sampler2D _BurnTex;
		#endif
			fixed4 _BurnColor;
			fixed _BurnRange;
			half _BurnIntensity;

            v2f vert(appdata_t v)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                OUT.worldPosition = v.vertex;
                OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);
                OUT.uv = v.texcoord;
                OUT.uvDissolve = mul(unity_ObjectToWorld, v.vertex).xy * _DissolveTex_ST.xy + _DissolveTex_ST.zw;
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
				
				fixed dissolve = tex2D(_DissolveTex, IN.uvDissolve).r * 0.9999;
			#ifdef TEST_DISSOLVE_TILING
				return half4(dissolve, dissolve, dissolve, 1);
			#endif
				fixed clippedDissolve = (dissolve + _BurnRange) / (1 + _BurnRange);
				fixed clippingDiff = clippedDissolve - _DissolveProgress;

				fixed burnedProgress = step(clippingDiff, 0);
				color = lerp(color, color * _BurnedColor, burnedProgress);

				fixed colorMiddle = (color.r + color.g + color.b) * 0.3333;
				color.rgb = lerp(half3(colorMiddle, colorMiddle, colorMiddle), color.rgb, lerp(1, _BurnedSaturation, burnedProgress));

				fixed burnVisibility = step(clippingDiff - _BurnRange * saturate(_DissolveProgress / _BurnRange), 0);
			#ifdef USE_BURN_TEX
				float2 burnUV = float2(1.0 - saturate(clippingDiff / _BurnRange), 1);
				half4 burnColor = tex2D(_BurnTex, burnUV) * _BurnColor;
				color.rgb = lerp(color, burnColor.rgb * _BurnIntensity, burnVisibility * burnColor.a * step(0, clippingDiff));
			#else
				color.rgb = lerp(color, _BurnColor.rgb * _BurnIntensity, burnVisibility * _BurnColor.a * step(0, clippingDiff));
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