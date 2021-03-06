Shader "Unlit/MiniMap"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        //_SampleEnemy("Vector", Vector) = (0,0,0,0)
        _EnemyPointSize("Float", Range(0,1)) = 0.01
    }
    SubShader
    {
        Tags 
        { 
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
        }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            //#pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                //UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _EnemyPosArray[50];
            float4 _PlayerPos;
            float _EnemyPointSize;

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
                fixed4 col = tex2D(_MainTex, i.uv);

                fixed4 uv = fixed4(i.uv.x, i.uv.y, 0, 0);
                
                [loop]
                for (int index = 0; index < 50; ++index)
                {
                    if (_EnemyPosArray[index].w > 0)
                    {
                        fixed4 enemy = fixed4(_EnemyPosArray[index].x, _EnemyPosArray[index].z, 0, 0);

                        if (distance(uv, enemy) <= _EnemyPointSize)
                        {
                            col.rgba = fixed4(1,0,0,1);
                        }
                    }
                    else
                    {
                        break;
                    }
                }

                if (distance(uv, _PlayerPos) <= _EnemyPointSize)
                {
                    col.rgba = fixed4(0, 0, 1, 1);
                }
                
                return col;
            }
            ENDCG
        }
    }
}
