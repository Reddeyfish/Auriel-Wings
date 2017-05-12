Shader "Custom/AurielWings" {

    Properties {
		_MainTex ("Texture (RGBA)", 2D) = "white" {}
        _WaveTex ("Wave Texture", 2D) = "white" {}
        _Gradient ("Gradient", 2D) = "white" {}
	}

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector" = "True" "PreviewType"="Plane" }

        //subshaders are used for compatibility. If the first subshader isn't compatible, it'll attempt to use the one below it.
        Pass
        {

            Blend SrcAlpha One
            ZWrite Off
            ZTest LEqual
            Lighting Off

            CGPROGRAM
                //begin CG block

                #pragma vertex vert
                //we will use a vertex function, named "vert".

                #pragma fragment frag
                //we will use a fragment function, named "frag"
            
                #include "UnityCG.cginc"
                //use a CGInclude file defining several useful functions, including our vertex function
            
                //declare our external properties
                uniform sampler2D _MainTex;

                uniform sampler2D _WaveTex;
                uniform float4 _WaveTex_ST;

                uniform sampler2D _Gradient;

                //declare input and output structs for vertex and fragment functions

                struct appdata
                {
                    float4 vertex : POSITION;
                    half2 texcoord : TEXCOORD0;
                };

                struct v2f
                {
                    float4 pos : SV_POSITION;
                    half2 uv : TEXCOORD0;
                    half2 uv2 : TEXCOORD1;
                };

                v2f vert( appdata v )
                {
                    v2f o;
                    o.pos = UnityObjectToClipPos (v.vertex);
                    o.uv = v.texcoord;
                    o.uv2 = TRANSFORM_TEX(v.texcoord, _WaveTex);
                    o.uv2 -= float2(_Time.x, _Time.x/20);
                    return o;
                }

                fixed4 frag (v2f i) : SV_Target
                {
                    // : SV_Target semantic marks the return value as the color of the fragment.

                    fixed2 textureCol = tex2D(_MainTex, i.uv).ra;
                    fixed2 waveCol = tex2D(_WaveTex, i.uv2).ra;

                    fixed returnColor = max(textureCol.y * textureCol.x, waveCol.y * waveCol.x);

                    fixed4 gradient = tex2D(_Gradient, fixed2(returnColor, 0.5));

                    gradient.a *= textureCol.y;

                    return gradient;
                }
            ENDCG
        }
    }
    Fallback "Diffuse" //If all of our subshaders aren't compatible, use subshaders from a different shader file
}