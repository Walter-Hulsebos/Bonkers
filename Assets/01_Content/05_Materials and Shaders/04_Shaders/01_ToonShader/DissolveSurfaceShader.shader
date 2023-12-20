Shader "Custom/DissolveSurfaceShader" 
{
	Properties 
	{
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_DissolveScale ("Dissolve Progression", Range(0.0, 1.0)) = 0.0
		_DissolveTex("Dissolve Texture", 2D) = "white" {}
		_GlowIntensity("Glow Intensity", Range(0.0, 5.0)) = 0.05
		_GlowScale("Glow Size", Range(0.0, 5.0)) = 1.0
		_Glow("Glow Color", Color) = (1, 1, 1, 1)
		_GlowEnd("Glow End Color", Color) = (1, 1, 1, 1)
		_GlowColFac("Glow Colorshift", Range(0.01, 2.0)) = 0.75
		_DissolveStart("Dissolve Start Point", Vector) = (1, 1, 1, 1)
		_DissolveEnd("Dissolve End Point", Vector) = (0, 0, 0, 1)
		_DissolveBand("Dissolve Band Size", Float) = 0.25
	}
	SubShader 
	{
		Tags 
		{ 
			"Queue" = "Transparent"
			"RenderType"="Fade" 
		}
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows alpha:fade vertex:vert

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		struct Input 
		{
			float2 uv_MainTex;
			float dGeometry;
		};

		fixed4 _Color;
		sampler2D _MainTex;
		half _DissolveScale;
		sampler2D _DissolveTex;
		half _GlowIntensity;
		half _GlowScale;
		fixed4 _Glow;
		fixed4 _GlowEnd;
		half _GlowColFac;
		float3 _DissolveStart;
		float3 _DissolveEnd;
		half _DissolveBand;

		//Precompute dissolve direction and magnitude.
		static float3 dDir = normalize(_DissolveEnd - _DissolveStart);
		static float dMag = length(_DissolveEnd - _DissolveStart);
		static float dAdjust = ( _DissolveBand * clamp(_GlowScale, 1.0, 100.0) - _DissolveBand) / dMag;

		//Convert dissolve progression to new space accounting for band size and glow prediction.
		static half dConverted = _DissolveScale * (1.0f + dAdjust) - dAdjust;

		//Precompute gradient start position.
		static float3 dissolveStartConverted = _DissolveStart - _DissolveBand * dDir;
		static float dBandFactor = 1.0f / _DissolveBand;

		void vert (inout appdata_full v, out Input o) 
		{
			UNITY_INITIALIZE_OUTPUT(Input,o);

			//Calculate geometry-based dissolve coefficient.
			//Compute top of dissolution gradient according to dissolve progression.
			float3 dPoint = lerp(dissolveStartConverted, _DissolveEnd, dConverted);

			//Project vector between current vertex and top of gradient onto dissolve direction.
			//Scale coefficient by band (gradient) size.
			o.dGeometry = dot(v.vertex - dPoint, dDir) * dBandFactor;		
		}

		void surf (Input IN, inout SurfaceOutputStandard o) 
		{
			//Convert dissolve progression to -1 to 1 scale.
			half dBase = -2.0f * dConverted + 1.0f;

			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			fixed4 dTex = tex2D(_DissolveTex, IN.uv_MainTex);

			//Convert dissolve texture sample based on dissolve progression.
			//(Black dissolves first).
			half dTexRead = dTex.r + dBase;

			//Combine texture factor with geometry coefficient from vertex shader.
			half dFinal = dTexRead + IN.dGeometry;

			//Glow is based on "predicting" what will dissolve next.
			//Shift the computed value based on the scale factor of the glow.
			//Scale the shifted value based on effect intensity.
			half dPredict = (_GlowScale - dFinal) * _GlowIntensity;
			//Change colour interpolation by adding in another factor controlling the gradient.
			half dPredictCol = (_GlowScale * _GlowColFac - dFinal) * _GlowIntensity;

			//Calculate and clamp glow colour.
			fixed4 glowCol = dPredict * lerp(_Glow, _GlowEnd, clamp(dPredictCol, 0.0f, 1.0f));
			glowCol = clamp(glowCol, 0.0f, 1.0f);

			//Clamp alpha.
			half alpha = clamp(dFinal, 0.0f, 1.0f);

			o.Albedo = clamp(lerp(c.rgb, glowCol, clamp(dPredict, 0.0f, 1.0f)), 0.0f, 1.0f);
			o.Alpha = alpha;
			o.Emission = glowCol;
		}
		ENDCG
	}
	FallBack "Diffuse"
}