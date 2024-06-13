Shader "Custom/2DWaterRipple"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _RippleSpeed ("Ripple Speed", Float) = 1.0
        _RippleStrength ("Ripple Strength", Float) = 0.1
    }
    
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        
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
            float _RippleSpeed;
            float _RippleStrength;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                
                // Apply vertex displacement based on time and noise
                float2 offset = float2(
                    sin(_Time.y * _RippleSpeed + v.vertex.x * 0.01),
                    cos(_Time.y * _RippleSpeed + v.vertex.y * 0.01)
                ) * _RippleStrength;
                
                o.vertex.xy += offset;
                
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                return col;
            }
            ENDCG
        }
    }
}
