Shader "Runner/WallShader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_PitColor ("Pit Color", Color) = (1, 0.5, 0, 1)
		_PitY ("Pit Y", Float) = -10
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard finalcolor:finalColor

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float3 worldPos;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		fixed4 _PitColor;
		float _PitY;
		
		UNITY_INSTANCING_BUFFER_START(Props)
		UNITY_INSTANCING_BUFFER_END(Props)

		// source: https://www.ronja-tutorials.com/post/047-invlerp_remap/#inverse-lerp
		float invLerp(float from, float to, float value) {
			value = clamp(value, min(from, to), max(from, to));
			return (value - from) / (to - from);
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {
			o.Albedo = _Color.rgb;
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
		}

		void finalColor (Input IN, SurfaceOutputStandard o, inout fixed4 color) {
			const float t = sin( invLerp( 0, _PitY, IN.worldPos.y ) * UNITY_PI * 0.5 );
			color = lerp( color, _PitColor, t );
	    }
		ENDCG
	}
	FallBack "Diffuse"
}