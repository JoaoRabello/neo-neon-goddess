Shader "FrenzyGames/GUI/Tween"
{
    Properties
    {
        [PerRendererData] _MainTex("_MainTex", 2D) = "white" {}
		[Toggle(COLORIZE)] COLORIZE("Colorize?", Float) = 0
        [HDR]_Color("_Color", Color) = (1,1,1,1)
			
		[Space]
		[Header(Tween)]
        _OperationsPivot("_OperationsPivot (world space, xy)", Vector) = (0,0,0,0)
		[Space]
		[Toggle(MOVE)] MOVE("Move?", Float) = 0
        _Offset("_Offset (x,y)", Vector) = (0,0,0,0)
		[Space]
		[Toggle(ROTATE)] ROTATE("Rotate?", Float) = 0
        _Rotation("_Rotation (around Z axis)", Float) = 0
		[Space]
		[Toggle(SCALE)] SCALE("Scale?", Float) = 0
        _Scale("_Scale (x,y)", Vector) = (1,1,0,0)
		[Toggle(USE_PROGRESS_TO_TWEEN)] USE_PROGRESS_TO_TWEEN("Use _Progress to move from zero point to final?", Float) = 0
		[Toggle(TWEEN_COLOR)] TWEEN_COLOR("Use _Progress to tween _Color?", Float) = 0
        _Progress("_Progress", Range(0,1)) = 1
		
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
			#pragma shader_feature USE_PROGRESS_TO_TWEEN
			#pragma shader_feature TWEEN_COLOR
			#pragma shader_feature COLORIZE
			#pragma shader_feature MOVE
			#pragma shader_feature ROTATE
			#pragma shader_feature SCALE
			#pragma shader_feature UNITY_TEXT
		
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
			half4 _Color;
			float2 _Offset;
			float _Rotation;
			float2 _Scale;
			fixed _Progress;
			float2 _OperationsPivot;

			inline float3x3 rotationAroundZ(float rad) 
			{
				float s = sin(rad);
				float c = cos(rad);
				return float3x3(
								c, s, 0,
								-s, c, 0,
								0, 0, 1);
			}

            v2f vert(appdata_t v)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                OUT.worldPosition = v.vertex;
			
				v.vertex = mul(unity_ObjectToWorld, v.vertex);
		#if SCALE
			#if USE_PROGRESS_TO_TWEEN
				float2 scale = lerp(1, _Scale, _Progress);
			#else
				float2 scale = _Scale;
			#endif
				v.vertex.xy = lerp(_OperationsPivot, v.vertex.xy, scale);
		#endif

		#if ROTATE
			#if USE_PROGRESS_TO_TWEEN
				float rotation = _Rotation * _Progress;
			#else
				float rotation = _Rotation;
			#endif
				v.vertex.xy = mul(rotationAroundZ(rotation / 180 * 3.1415), float3(v.vertex.xy - _OperationsPivot, 0)).xy + _OperationsPivot;
		#endif
					
		#if MOVE			
			#if USE_PROGRESS_TO_TWEEN
				v.vertex.xy += _Offset * _Progress;
			#else
				v.vertex.xy += _Offset;
			#endif
		#endif
				
				OUT.vertex = mul(UNITY_MATRIX_VP, v.vertex);// UnityObjectToClipPos(OUT.worldPosition);
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
				#if defined(TWEEN_COLOR) && defined(USE_PROGRESS_TO_TWEEN)
					color = lerp(color, color * _Color, _Progress);
				#else
					color *= _Color;
				#endif
			#endif
				
            #ifdef UNITY_UI_CLIP_RECT
                color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
            #endif
				
                return color;
            }
        ENDCG
        }
    }

	CustomEditor "GUITweenShaderEditor"
}