// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/ShockWave(WorldSpace)" 
{
	Properties 
	{

		_Radius ("Radius", Range(-0.20,0.5)) = 0.2
		_Amplitude ("Amplitude", Range(-10,10)) = 0.05
		_WaveSize  ("WaveSize", Range(0,5)) = 0.2

		_ScreenRatio ("ScreenRatio", Float) = 1 //Width/Height

//	 	[MaterialToggle]  _UseDepth ("_UseDepth", float) = 1
	}
	 
	SubShader 
	{


//        Tags { "Queue" = "Transparent" }

//        Tags
//        { 
//            "Queue"="Transparent" 
//            "IgnoreProjector"="True" 
//            "RenderType"="Transparent" 
//            "PreviewType"="Plane"
//            "CanUseSpriteAtlas"="True"
//        }

        Tags { "Queue"="Transparent" "RenderType"="Transparent" }

        GrabPass {"_GrabTexture"}
//		ZTest Always Cull Off ZWrite Off Fog { Mode Off } //Rendering settings
	 	Blend SrcAlpha OneMinusSrcAlpha


	 	Pass
	 	{
            Cull Off
            ZWrite Off
//            Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
//			#pragma surface surf SimpleSpecular alpha
			#include "UnityCG.cginc"
			//we include "UnityCG.cginc" to use the appdata_img struct
			
//			float _CenterX;
//			float _CenterY;
			float _Radius;
			float _Amplitude;
			float _ScreenRatio;
			float _WaveSize;

			sampler2D _GrabTexture;
            int _UseDepth;

            sampler2D _CameraDepthTexture;

//			struct appdata 
//			{
//				float4 vertex : POSITION;
//				float2 texcoord : TEXCOORD0;
//			};

			struct v2f 
			{
				float4 pos : POSITION;
				half2 uv : TEXCOORD0;
				float4 GrabUV : TEXCOORD2;
			};
	   
			//Our Vertex Shader
			v2f vert (appdata_img v){
				v2f o;
				o.pos = UnityObjectToClipPos (v.vertex);
				o.uv = MultiplyUV (UNITY_MATRIX_TEXTURE0, v.texcoord.xy);
				o.GrabUV = ComputeGrabScreenPos(o.pos);
				return o;
			}

//			sampler2D _MainTex; //Reference in Pass is necessary to let us use this variable in shaders

			//Our Fragment Shader
			fixed4 frag (v2f i) : COLOR
			{

				float2 diff=float2(i.uv.x-0.5, (i.uv.y-0.5) ); 

				float dist=sqrt(diff.x * diff.x + diff.y * diff.y);

//				float2 uv_displaced = float2(i.uv.x,i.uv.y);
				float2 uv_displaced = float2(0,0);

				int transparent = 0;

				if (dist>_Radius) 
				{
					if (dist<_Radius+_WaveSize) 
					{
						float angle=(dist-_Radius)*2*3.141592654/_WaveSize;
						float cossin=(1-cos(angle))*0.5;
						uv_displaced.x-=cossin*diff.x*_Amplitude/dist;
						uv_displaced.y-=cossin*diff.y*_Amplitude/dist;

						transparent = 1;
					}
				}


				fixed4 GUV = i.GrabUV;

				GUV.x += uv_displaced.x;
				GUV.y += uv_displaced.y;


//				if(_UseDepth)
//				{
//					GUV.z = UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.GrabUV)));
//				}



				fixed4 orgCol = tex2Dproj(_GrabTexture, UNITY_PROJ_COORD(GUV));

//				orgCol.a = 0;
//				orgCol.r = 0;
				if (transparent == 0)
				{
					orgCol.a = 0;
				}

//				fixed4 orgCol = fixed4(0,0,0,0);
//				orgCol = tex2D(_GrabTexture, uv_displaced);

//				if (i.uv.x > 0.5)
//				{
//					orgCol = fixed4(1,0,0,1);
//				}
//
				return orgCol;
			}
			ENDCG
		}
	}
	
	FallBack "Diffuse"
}
