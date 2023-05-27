Shader "Runner/WallShaderTransparent" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_PitColor ("Pit Color", Color) = (1, 0.5, 0, 1)
		_PitY ("Pit Y", Float) = -10
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	SubShader {
		Tags { "Queue" = "Transparent" "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard alpha:fade

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
			float3 worldPos;
		};

		half _Glossiness;
		half _Metallic;
		float _PitY;

		UNITY_INSTANCING_BUFFER_START(Props)
			UNITY_DEFINE_INSTANCED_PROP( fixed4, _Color )
			UNITY_DEFINE_INSTANCED_PROP( fixed4, _PitColor )
		UNITY_INSTANCING_BUFFER_END(Props)

		// source: https://www.ronja-tutorials.com/post/047-invlerp_remap/#inverse-lerp
		float invLerp(float from, float to, float value) {
			value = clamp(value, min(from, to), max(from, to));
			return (value - from) / (to - from);
		}


		void surf (Input IN, inout SurfaceOutputStandard o) {
			const float t = pow( sin( invLerp( 0, _PitY, IN.worldPos.y ) * UNITY_PI * 0.5 ), 2 );
			const float4 col = UNITY_ACCESS_INSTANCED_PROP( Props, _Color );
			const float4 pitCol = UNITY_ACCESS_INSTANCED_PROP( Props, _PitColor );
			o.Albedo = col;
  			o.Emission = lerp( 0, pitCol.rgb, t );
			o.Metallic = lerp( _Metallic, 0.0, t );
			o.Smoothness = lerp( _Glossiness, 1.0, t );
			o.Alpha = lerp( col.a, 0.0, t );
		}
		ENDCG
	}
	FallBack "Diffuse"
}