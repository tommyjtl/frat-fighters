Shader "Custom/HitFlash" {

	Properties {
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Blend("Amount", Range(0.0, 1.0)) = 0
		_Color ("Tint", Color) = (1,1,1,1)
		_HitFlashColor ("HitFlash Color", Color) = (1,1,1,1)
	}

	SubShader {

		Tags { 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

		Pass {

		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile _ PIXELSNAP_ON
			#include "UnityCG.cginc"
			
			struct appdata_t {
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				float2 texcoord  : TEXCOORD0;
			};
			
			fixed4 _Color;
			sampler2D _MainTex;
			float _Blend;
			fixed4 _HitFlashColor;

			v2f vert(appdata_t IN) {
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color * _Color;
				return OUT;
			}

			fixed4 frag(v2f IN) : SV_Target {
				fixed4 _col = IN.color; //get sprite color
				fixed4 c = tex2D (_MainTex, IN.texcoord); //get sprite texture
				fixed4 c2 = lerp(c * _col, _HitFlashColor, _Blend); //blend sprite texture and hitflash color

				//fix alpha
				c2 *= c.a;
				return c2;
			}
		ENDCG
		}
	}
	FallBack "Sprites/Default"
}