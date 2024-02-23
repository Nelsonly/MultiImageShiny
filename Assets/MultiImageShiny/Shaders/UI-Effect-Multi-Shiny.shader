Shader "UI/Hidden/UI-Effect-Multi-Shiny"
{
    Properties
    {
        [PerRendererData] _MainTex ("Main Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255

        _ColorMask ("Color Mask", Float) = 15
        _WorldSpaceUVs ("World Space UVs", Vector) = (0,0,0,0)
        _Width ("Width", Float) = 0.5
        _Soft ("Soft", Float) = 0.5
        _Brightness ("Brightness", Float) = 0.5
        _Gloss ("Gloss", Float) = 0.5
        _MyRotate("MyRotate", Int) = 45
//        _High("High",Float) = 0.5  高度用到的地方很少，目前先不用
        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
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
        ColorMask [_ColorMask]

        Pass
        {
            Name "Default"

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #pragma multi_compile __ UNITY_UI_ALPHACLIP

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
                half2 param : TEXCOORD2;
            };

            fixed4 _Color;
            fixed4 _TextureSampleAdd;
            float4 _ClipRect;
            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float4 _WorldSpaceUVs;
            float _Width;
            float _Soft;
            float _Brightness;
            float _Gloss;
            fixed _MyRotate;
            float _High;
            
            v2f vert(appdata_t IN)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                OUT.worldPosition = IN.vertex;

                OUT.vertex = UnityObjectToClipPos(IN.vertex);

                OUT.color = IN.color * _Color;
                float3 wordPos = mul(unity_ObjectToWorld, IN.vertex).xyz;
                
                float3 value = wordPos - _WorldSpaceUVs;
                OUT.param.x = value.x;
                OUT.param.y = value.y;
                OUT.texcoord = IN.texcoord;
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                half4 color = (tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd) * IN.color;

                color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);

                float rad = _MyRotate * 0.017453292f;
               
                fixed nomalizedPos = IN.param.x * sin(rad) - IN.param.y * cos(rad);
                // - 10 * step(_High,pow(IN.param.y,2) + pow(IN.param.x,2)); 高度用到的地方很少，目前先不用
                half normalized = 1 - saturate(abs((nomalizedPos) / _Width));
                half shinePower = smoothstep(0, _Soft * 2, normalized);
                half3 reflectColor = lerp(1, color.rgb * 10, _Gloss);
                color.rgb += color.a * (shinePower / 2) * _Brightness * reflectColor;

                #ifdef UNITY_UI_ALPHACLIP
				clip (color.a - 0.001);
                #endif

                return color;
            }
            ENDCG
        }
    }
}