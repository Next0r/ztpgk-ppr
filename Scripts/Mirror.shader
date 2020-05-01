Shader "Hidden/Mirror"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
			sampler2D _MirrorTex;
			sampler2D _FactorTex;
			sampler2D _FrameTex;

			float _offsetX;
			float _offsetY;
			float _scaleX;
			float _scaleY;

			float _frameOffsetX;
			float _frameOffsetY;
			float _frameScaleX;
			float _frameScaleY;

			float _alpha;

			float3 _reflectionColor;

			fixed4 frag(v2f i) : SV_Target
			{
				//fixed4 col = tex2D(_MainTex, float2(tan(2 * i.uv.x + 2.15)/5 + 0.5, tan(2 * i.uv.y + 2.15) / 5 + 0.5));
				fixed4 col = tex2D(_MainTex, i.uv);

				float2 mirrorMapping = float2(-(i.uv.x * _scaleX + _offsetX), i.uv.y * _scaleY + _offsetY);
                //fixed4 mirror = tex2D(_MirrorTex, mirrorMapping);

				fixed4 mirror = tex2D(_MirrorTex, float2(tan(2 * mirrorMapping.x + 2.15) / 5 + 0.5, mirrorMapping.y));

				float2 frameMapping = float2(i.uv.x * _frameScaleX + _frameOffsetX, i.uv.y * _frameScaleY + _frameOffsetY);
				fixed4 frame = tex2D(_FrameTex, frameMapping);
				fixed4 factor = tex2D(_FactorTex, frameMapping);
				
				col.rgb = (mirror.rgb * factor.rgb * _alpha * _reflectionColor.rgb) + (col.rgb * (1 - factor.rgb))  + (col.rgb * factor.rgb * (1 - _alpha));
				col.rgb = col.rgb * (1 - frame.a) + (frame.rgb * frame.a * _alpha) + (col.rgb * frame.a * (1 - _alpha));

                return col;
            }
            ENDCG
        }
    }
}
