Shader "Unlit/OutlineShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Thickness ("Thickness", float) = 0.5
        _Color ("Outline Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
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
            float4 _MainTex_ST;

            sampler2D _SDF;
            float4 _SDF_ST;
            float _Thickness;
            float4 _Color;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv * 2 - 0.5);
                col *= col.a;
                //float2 uv = (i.uv ) * 2 - 0.5;
                float2 ts = _SDF_ST.xy / 2;

                float ddxx = ddx(i.uv.x);
				float ddxy = ddx(i.uv.y);
				float ddyx = ddy(i.uv.x);
				float ddyy = ddy(i.uv.y);
                
                float _Feather = 0.33;
                
                float2 p1 = float2(ddxx+ddyx, ddxy+ddyy)*_Feather;
				float2 p2 = float2(ddxx+ddyx, -(ddxy+ddyy))*_Feather;
				float2 p3 = float2(-(ddxx+ddyx), ddxy+ddyy)*_Feather;
				float2 p4 = float2(-(ddxx+ddyx), -(ddxy+ddyy))*_Feather;
                
                float dist0 = tex2D(_SDF, i.uv).r - _Thickness;
                float dist1 = tex2D(_SDF, i.uv + p1).r - _Thickness;
                float dist2 = tex2D(_SDF, i.uv + p2).r - _Thickness;
                float dist3 = tex2D(_SDF, i.uv + p3).r - _Thickness;
                float dist4 = tex2D(_SDF, i.uv + p4).r - _Thickness;
                /*float dist1 = tex2D(_SDF, i.uv + float2(-ts.x,0)).r - _Thickness;
                float dist2 = tex2D(_SDF, i.uv + float2(0,ts.y)).r - _Thickness;
                float dist3 = tex2D(_SDF, i.uv + float2(ts.x,0)).r - _Thickness;
                float dist4 = tex2D(_SDF, i.uv + float2(0,-ts.y)).r - _Thickness;*/

                float dist = (dist0 + dist1 + dist2 + dist3 + dist4) / 5;
                float step = smoothstep(0.00075,-0.00075,dist);
                col.a = step;
                //if (col.a <= -0) discard;
                //float2 ddist = float2(ddx(dist), ddy(dist));
                //float pixelDist = dist / length(ddist);
                
                //col.a = saturate(0.5 - pixelDist);
                //col *= col.a;
                return col + step * _Color;
            }
            ENDCG
        }
    }
}
