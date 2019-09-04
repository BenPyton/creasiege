Shader "Hidden/OutlineFX"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        // No culling or depth

        Pass
		{
		//Blend SrcAlpha OneMinusSrcAlpha
		Cull Off ZWrite Off ZTest Always

			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert
			#pragma fragment frag

			sampler2D _MainTex;
			sampler2D _Stencil;
			float4 _Color = float4(1,1,1,1);
			float _Precision = 0.002f;

			struct Prog2Vertex {
				float4 vertex : POSITION; 	//Les "registres" précisés après chaque variable servent
				float4 tangent : TANGENT; 	//A savoir ce qu'on est censé attendre de la carte graphique.
				float3 normal : NORMAL;		//(ce n'est pas basé sur le nom des variables).
				float4 texcoord : TEXCOORD0;
				float4 texcoord1 : TEXCOORD1;
				fixed4 color : COLOR;
				};

		//Structure servant a transporter des données du vertex shader au pixel shader.
		//C'est au vertex shader de remplir a la main les informations de cette structure.
		struct Vertex2Pixel
			{
			float4 pos : SV_POSITION;
			float4 uv : TEXCOORD0;
			float4 depth : TEXCOORD1;
			};

		Vertex2Pixel vert(Prog2Vertex i)
		{
			Vertex2Pixel o;
			o.pos = UnityObjectToClipPos(i.vertex); //Projection du modèle 3D, cette ligne est obligatoire
			o.uv = i.texcoord; //UV de la texture
			return o;
		}

		float4 frag(Vertex2Pixel i) : COLOR
		{
			/*float4 color;
			if (tex2D(_MainTex, i.uv).r > 0.5f)
			{
				color = float4(1,0,0,1);
			}
			else
			{
				color = float4(0, 0, 0, 1);
			}*/

			//return color;
			float4 color = tex2D(_MainTex, i.uv.xy);
			//return color;

			//float4 outline = tex2D(_OutlineTex, i.uv.xy);
			//float4 depth = tex2D(_DepthTex, i.uv.xy);
			//float currentDepth = Linear01Depth(DecodeFloatRG(tex2D(_CameraDepthTexture, i.uv.xy)));


			float precision = _Precision;
			float center = tex2D(_Stencil, i.uv.xy);
			float up = tex2D(_Stencil, float2(i.uv.x, i.uv.y + precision));
			float down = tex2D(_Stencil, float2(i.uv.x, i.uv.y - precision));
			float right = tex2D(_Stencil, float2(i.uv.x - precision, i.uv.y));
			float left = tex2D(_Stencil, float2(i.uv.x + precision, i.uv.y));
			float upleft = tex2D(_Stencil, float2(i.uv.x + precision, i.uv.y + precision));
			float downleft = tex2D(_Stencil, float2(i.uv.x - precision, i.uv.y + precision));
			float upright = tex2D(_Stencil, float2(i.uv.x + precision, i.uv.y - precision));
			float downright = tex2D(_Stencil, float2(i.uv.x - precision, i.uv.y - precision));

			float convolution = 8 * center - up - down - left - right - upleft - downleft - upright - downright;
			float4 contour = 0;
			if (convolution > 0.005f)// && depth.r > currentDepth)
				contour = 1;


			return float4((1-contour) * color.rgb + contour * _Color, color.a);
		}
			ENDCG
	}


	}
		Fallback off
}
