Shader "Custom/Transparent"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent+3" }
        LOD 200
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float3 normal : NORMAL;		//(ce n'est pas basé sur le nom des variables).
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
				float3 norm: TEXCOORD1; //On utilise un registre dispo pour la normale.
				float3 normscreen: TEXCOORD2; //On utilise un registre dispo pour la normale en screenspace. Ne faites pas gaffe au nom.
				float4 screenPos: TEXCOORD3;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.norm = mul((float3x3)unity_ObjectToWorld, float4(v.normal, 1)).xyz;
				o.normscreen = normalize(mul(UNITY_MATRIX_IT_MV, float4(v.normal, 1)).xyz); //Normale dans l'espace écran
				o.screenPos = ComputeScreenPos(o.vertex); //Position du vertice à l'écran, avec la profondeur.

				UNITY_TRANSFER_FOG(o, o.vertex);
				return o;
			}

			float4 frag(v2f i) : SV_Target
			{
				// sample the texture
				//fixed4 col = tex2D(_MainTex, i.uv);
				float2 screenPosition = (i.screenPos.xy / i.screenPos.w);
				float l = length(i.normscreen.xy);
				float4 color = 0.8f * (float4(1, 0.5, 0.0, 0.3f) + float4(1, 1, 0.0, 0.8f) * l*l);
				if ((int)(abs(screenPosition.y) * _ScreenParams.y * 50) % 30 - 20 < 0)
				{
					color.a *= 0.8f;
				}
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return color;
			}
			ENDCG
		}
    }
    FallBack off
}
