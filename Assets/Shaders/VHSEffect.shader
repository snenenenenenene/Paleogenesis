
Shader "Hidden/VHS"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _NoiseIntensity ("Noise Intensity", Range(0, 1)) = 0.1
        _ScanLineJitter ("Scan Line Jitter", Range(0, 1)) = 0.1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

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

            sampler2D _MainTex;
            float _NoiseIntensity;
            float _ScanLineJitter;
            
            float random(float2 st)
            {
                return frac(sin(dot(st.xy, float2(12.9898,78.233))) * 43758.5453123);
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;
                
                // Scan line jitter
                float jitter = random(float2(_Time.y, uv.y)) * 2 - 1;
                uv.x += jitter * _ScanLineJitter;
                
                // Noise
                float noise = random(uv * _Time.y) * _NoiseIntensity;
                
                // Sample texture
                fixed4 col = tex2D(_MainTex, uv);
                
                // Apply noise and scanlines
                col.rgb += noise;
                col.rgb *= 0.9 + 0.1 * sin(uv.y * 500);
                
                return col;
            }
            ENDCG
        }
    }
}
