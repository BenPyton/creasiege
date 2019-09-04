Shader "Hidden/StencilBufferReader" {
//Properties {
//	_MainTex ("", 2D) = "" {}
//	_OutlineTex("", 2D) = "" {}
//	_DepthTex("", 2D) = "" {}
//}
	SubShader{
		Tags{"RenderType" = "Opaque" "IgnoreProjector" = "True" "Queue" = "Transparent+2"}
		//Blend SrcAlpha OneMinusSrcAlpha
		ZTest Always
		Cull Off
		ZWrite Off //Fog { Mode off } //Parametrage du shader pour éviter de lire, écrire dans le zbuffer, désactiver le culling et le brouillard sur le polygone

		Pass {
			Cull Off ZWrite Off ZTest Always
			Stencil {
				Ref 64
				Comp NotEqual
			}

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			float4 vert(float4 vertex : POSITION) : SV_POSITION{
				return UnityObjectToClipPos(vertex);
			}

			fixed4 frag() : SV_Target {
				return fixed4(1, 1, 1, 1);
			}
			ENDCG
		}

		Pass {
			Cull Off ZWrite Off ZTest Always
			Stencil {
				Ref 64
				Comp Equal
			}

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			float4 vert(float4 vertex : POSITION) : SV_POSITION{
				return UnityObjectToClipPos(vertex);
			}

			fixed4 frag() : SV_Target {
				return fixed4(0, 0, 0, 1);
			}
			ENDCG
		}
	}
	Fallback off
}