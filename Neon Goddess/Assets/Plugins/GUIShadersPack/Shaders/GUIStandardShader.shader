Shader "FrenzyGames/GUI/StandardLightened" 
{
	Properties 
	{
		_Color("_Color", Color) = (1,1,1,1)
        [PerRendererData] _MainTex("_MainTex", 2D) = "white" {}
		_BumpMap("_BumpMap", 2D) = "bump" {}
		_Bumpiness("_Bumpiness", Range(-5,5)) = 1
		_Glossiness("_Glossiness", Range(0,1)) = 0.5
		_Metallic("_Metallic", Range(0,1)) = 0.0

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
	SubShader {
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
		
		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows alpha:fade

		#pragma target 3.0

		struct Input 
		{
			float2 uv_MainTex;
			float2 uv_BumpMap;
			fixed4 color : COLOR;
		};
		
		sampler2D _MainTex;
		sampler2D _BumpMap;
		half _Glossiness;
		half _Metallic;
		half _Bumpiness;
		fixed4 _Color;
		
		void surf (Input IN, inout SurfaceOutputStandard o) 
		{
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color * IN.color;
			o.Albedo = c.rgb;
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
			o.Normal = lerp(fixed3(0,0,1), UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap)), _Bumpiness);
		}
		ENDCG
	}
}
