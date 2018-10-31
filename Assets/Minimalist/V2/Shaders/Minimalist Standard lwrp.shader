shader "Minimalist/v2/LightWeightStandard" {
	Properties{
		//Texture Module
			[HideInInspector][MaterialToggle] _ShowTexture ("Show Texture settngs", Float ) = 0
			_MainTexture ("Main Texture", 2D) = "white" {}
			_MainTexturePower ("Main Texture Power", Range(-1, 1)) = 0
		//Custom Shading
			[HideInInspector][MaterialToggle] _ShowCustomShading ("Show Custom Shading settngs", Float ) = 0

			[HideInInspector][MaterialToggle] _ShowFront ("Front", Float ) = 0
			[HideInInspector][KeywordEnum(VertexColor, SolidColor, Gradient)] _Shading_F("Shading mode", Float) = 0
			[HideInInspector][KeywordEnum(Use Global, Custom)] _GradSettings_F("Shading mode", Float) = 0
			[HideInInspector]_GizmoPosition_F("front gizmo", Vector) = (0, 0, 10, 10)

			_Color1_F("Forward Color 1", Color) = (1, 1, 1, 1)
			_Color2_F("Forward Color 2", Color) = (1, 1, 1, 1) 
			_GradientYStartPos_F ("Gradient start Y", Vector) = (0, 0, 0, 0)
			_GradientHeight_F("Gradient Height", Float) = 1
			_Rotation_F("Rotation", Range(0, 360)) = 0

			[HideInInspector][MaterialToggle] _ShowBack ("Front", Float ) = 0
			[HideInInspector][KeywordEnum(Vertex Color, Solid Color, Gradient)] _Shading_B("Shading mode", Float) = 0
			[HideInInspector][KeywordEnum(Use Global, Custom)] _GradSettings_B("Shading mode", Float) = 0
			[HideInInspector]_GizmoPosition_B("back gizmo", Vector) = (0, 0, 10, 10)

			_Color1_B("Backward Color 1", Color) = (1, 1, 1, 1)
			_Color2_B("Backward Color 2", Color) = (1, 1, 1, 1)
			_GradientYStartPos_B ("Gradient start Y", Vector) = (0, 0, 0, 0)
			_GradientHeight_B("Gradient Height", Float) = 1
			_Rotation_B("Rotation", Range(0, 360)) = 0

			[HideInInspector][MaterialToggle] _ShowLeft ("Front", Float ) = 0
			[HideInInspector][KeywordEnum(Vertex Color, Solid Color, Gradient)] _Shading_L("Shading mode", Float) = 0
			[HideInInspector][KeywordEnum(Use Global, Custom)] _GradSettings_L("Shading mode", Float) = 0
			[HideInInspector]_GizmoPosition_L("Left gizmo", Vector) = (0, 0, 10, 10)

			_Color1_L("Left Color 1", Color) = (1, 1, 1, 1)
			_Color2_L("Left Color 2", Color) = (1, 1, 1, 1)
			_GradientYStartPos_L ("Gradient start Y", Vector) = (0, 0, 0, 0)
			_GradientHeight_L("Gradient Height", Float) = 1
			_Rotation_L("Rotation", Range(0, 360)) = 0

			[HideInInspector][MaterialToggle] _ShowRight ("Front", Float ) = 0
			[HideInInspector][KeywordEnum(Vertex Color, Solid Color, Gradient)] _Shading_R("Shading mode", Float) = 0
			[HideInInspector][KeywordEnum(Use Global, Custom)] _GradSettings_R("Shading mode", Float) = 0
			[HideInInspector]_GizmoPosition_R("Right gizmo", Vector) = (0, 0, 10, 10)

			_Color1_R("Right Color 1", Color) = (1, 1, 1, 1)
			_Color2_R("Right Color 2", Color) = (1, 1, 1, 1)
			_GradientYStartPos_R ("Gradient start Y", Vector) = (0, 0, 0, 0)
			_GradientHeight_R("Gradient Height", Float) = 1
			_Rotation_R("Rotation", Range(0, 360)) = 0

			[HideInInspector][MaterialToggle] _ShowTop ("Top", Float ) = 0
			[HideInInspector][KeywordEnum(Vertex Color, Solid Color, Gradient)] _Shading_T("Shading mode", Float) = 0
			[HideInInspector][KeywordEnum(Use Global, Custom)] _GradSettings_T("Gradient mode", Float) = 0
			[HideInInspector]_GizmoPosition_T("Top gizmo", Vector) = (0, 0, 10, 10)

			_Color1_T ("Top Color 1", Color) = (1, 1, 1, 1)
			_Color2_T ("Top Color 2", Color) = (1, 1, 1, 1)
			_GradientXStartPos_T ("Gradient start X", Vector) = (0, 0, 0, 0)
			_GradientHeight_T("Gradient Height", Float) = 1
			_Rotation_T("Rotation", Range(0, 360)) = 0

			[HideInInspector][MaterialToggle] _ShowBottom ("Botttom", Float ) = 0
			[HideInInspector][KeywordEnum(Vertex Color, Solid Color, Gradient)] _Shading_D("Shading mode", Float) = 0
			[HideInInspector][KeywordEnum(Use Global, Custom)] _GradSettings_D("Gradient mode", Float) = 0
			[HideInInspector]_GizmoPosition_D("Down gizmo", Vector) = (0, 0, 10, 10)

			_Color1_D ("Bottom Color 1", Color) = (1, 1, 1, 1)
			_Color2_D ("Bottom Color 2", Color) = (1, 1, 1, 1)
			_GradientXStartPos_D ("Gradient start X", Vector) = (0, 0, 0, 0)
			_GradientHeight_D("Gradient Height", Float) = 1
			_Rotation_D("Rotation", Range(0, 360)) = 0
		//Ambient Occlution
			[HideInInspector][MaterialToggle] _ShowAO ("AO", Float ) = 0
			[HideInInspector][MaterialToggle] _AOEnable ("Enable", Float ) = 0
			_AOTexture ("AO Texture", 2D) = "white" {}
			_AOColor ("AO Color", Color) = (1, 1, 1, 1)
			_AOPower ("AO Texture Power", Range(0, 3)) = 0
			[HideInInspector][KeywordEnum(uv0, uv1)] _AOuv("UV", Float) = 0
		//Lightmap
			[HideInInspector][MaterialToggle] _ShowLMap ("Lightmap", Float ) = 0
			[HideInInspector][MaterialToggle] _LmapEnable ("Enable", Float ) = 0
			[HideInInspector][KeywordEnum(Add, Multiply, AO)] _LmapBlendingMode("Blend Mode", Float) = 0
			_LMColor ("LightMap Color", Color) = (1, 1, 1, 1)
			_LMPower ("LightMap Power", Range(0, 5.0)) = 0
		//Fog
			[HideInInspector][MaterialToggle] _ShowFog ("ShowFog", Float ) = 0
			[MaterialToggle] _UnityFogEnable ("Fog", Float ) = 0
			[MaterialToggle] _HFogEnable ("Fog", Float ) = 0
			_Color_Fog ("Fog Color",     Color) = (0.5, 0.5, 0.5, 1)
			_FogYStartPos ("Gradient start Y", Float) = 0
			_FogHeight("Gradient Height", Float) = 10
		//Color Correction
			[HideInInspector][MaterialToggle] _ShowColorCorrection ("Color Correction", Float ) = 0
			[HideInInspector][MaterialToggle] _ColorCorrectionEnable ("Enable", Float ) = 0
			_TintColor ("Tint Color", Color) = (1, 1, 1, 1)
			_Saturation ("Saturation", Range(0, 1)) = 1
			_Brightness ("Brightness", Range(-1, 1)) = 0
		//OtherSettings
			[MaterialToggle] _OtherSettings ("OtherSettings", Float ) = 0

			[HideInInspector][MaterialToggle] _ShowGlobalGradientSettings ("Show Global Gradient Settings", Float ) = 0
			_GradientYStartPos_G ("Gradient start Y", Vector) = (0, 0, 0, 0)
			_GradientHeight_G("Gradient Height", Float) = 1
			_Rotation_G("Rotation", Float) = 0

			[HideInInspector][MaterialToggle] _ShowAmbientSettings ("Show Ambient Settings", Float ) = 0
			_AmbientColor("Ambient Color",Color) = (0, 0, 0, 0)
			_AmbientPower("Ambient Power", Range(0, 2.0)) = 0

			[HideInInspector][MaterialToggle] _RimEnable ("Use Rim", Float ) = 0
			_RimColor ("Rim Color", Color) = (1, 1, 1, 1)
			_RimPower ("Power", Range(0, 4)) = 1
            
			[MaterialToggle] _RealtimeShadow ("RealTime Shadow", Float ) = 0
			_ShadowColor("ShadowColor",    Color) = (0.1, 0.1, 0.1, 1)

			[KeywordEnum(World Space, Local Space)] 
			_GradientSpace("Gradient Space", Float) = 0
			[MaterialToggle] _DontMix ("Don't Mix Color", Float ) = 0
	}

	SubShader{
		Tags { "QUEUE"="Geometry" "RenderType"="Opaque" "RenderPipeline" = "LightweightPipeline" "LightMode"="LightweightForward"}

		Pass{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fwdbase_fullshadows
			#pragma multi_compile_fog
			#pragma fragmentoption ARB_precision_hint_fastest
			//shader_features
				#pragma shader_feature TEXTUREMODULE_ON

				#pragma shader_feature FRONTGRADIENT
				#pragma shader_feature BACKGRADIENT
				#pragma shader_feature LEFTGRADIENT
				#pragma shader_feature RIGHTGRADIENT
				#pragma shader_feature TOPGRADIENT
				#pragma shader_feature BOTTOMGRADIENT

				#pragma shader_feature FRONTSOLID
				#pragma shader_feature BACKSOLID
				#pragma shader_feature LEFTSOLID
				#pragma shader_feature RIGHTSOLID
				#pragma shader_feature TOPSOLID
				#pragma shader_feature BOTTOMSOLID

				#pragma shader_feature AO_ON_UV0
				#pragma shader_feature AO_ON_UV1
			
				#pragma shader_feature LIGHTMAP_ADD
				#pragma shader_feature LIGHTMAP_MULTIPLY
				#pragma shader_feature LIGHTMAP_AO

				#pragma shader_feature HEIGHT_FOG
				#pragma shader_feature UNITY_FOG
				#pragma shader_feature SHADOW_ON

				#pragma shader_feature COLORCORRECTION_ON
				#pragma shader_feature USERIM
				#pragma shader_feature DONTMIX

			#include "UnityCG.cginc"
			#include "AutoLight.cginc"

			//Uniforms
				uniform sampler2D _MainTexture; uniform fixed4 _MainTexture_ST;
				uniform half _MainTexturePower;

				uniform half3 _Color1_F;
				uniform half3 _Color2_F;
				uniform half2 _GradientYStartPos_F;
				uniform half _GradientHeight_F;
				uniform half _Rotation_F;

				uniform half3 _Color1_B;
				uniform half3 _Color2_B;
				uniform half2 _GradientYStartPos_B;
				uniform half _GradientHeight_B;
				uniform half _Rotation_B;

				uniform half3 _Color1_L;
				uniform half3 _Color2_L;
				uniform half2 _GradientYStartPos_L;
				uniform half _GradientHeight_L;
				uniform half _Rotation_L;

				uniform half3 _Color1_R;
				uniform half3 _Color2_R;
				uniform half2 _GradientYStartPos_R;
				uniform half _GradientHeight_R;
				uniform half _Rotation_R;

				uniform half3 _Color1_T;
				uniform half3 _Color2_T;
				uniform half2 _GradientXStartPos_T;
				uniform half _GradientHeight_T;
				uniform half _Rotation_T;

				uniform half3 _Color1_D;
				uniform half3 _Color2_D;
				uniform half2 _GradientXStartPos_D;
				uniform half _GradientHeight_D;
				uniform half _Rotation_D;

				uniform sampler2D _AOTexture; uniform fixed4 _AOTexture_ST;
				uniform half3 _AOColor;
				uniform half _AOPower;

				uniform fixed3 _LMColor;
				uniform half _LMPower;

				uniform half3 _Color_Fog;
				uniform half _FogYStartPos;
				uniform half _FogHeight;

				uniform half3 _RimColor;
				uniform half _RimPower;

				uniform half3 _TintColor;
				uniform half _Saturation;
				uniform half _Brightness;

				uniform half3 _AmbientColor;
				uniform half _AmbientPower;
				
				uniform half3 _ShadowColor;
				uniform fixed _GradientSpace;

			//Direction vector constants
				static const half3 FrontDir = half3(0, 0, 1);
				static const half3 BackDir = half3(0, 0, -1);
				static const half3 LeftDir = half3(1, 0, 0);
				static const half3 RightDir = half3(-1, 0, 0);
				static const half3 TopDir = half3(0, 1, 0);
				static const half3 BottomDir = half3(0, -1, 0);
				static const half3 whiteColor = half3(1, 1, 1);

			struct vertexInput{
				float4 vertex : POSITION;
				half3 normal : NORMAL;
				half3 vColor : COLOR;
				float4 uv0 : TEXCOORD0;
				float4 uv1 : TEXCOORD1;
			};

			struct vertexOutput{
				float4 pos : POSITION;
				float4 worldPos : TEXCOORD0;
				#if TEXTUREMODULE_ON
					float2 uv : TEXCOORD1;
				#endif

				#if LIGHTMAP_ADD || LIGHTMAP_MULTIPLY || LIGHTMAP_AO
					float2 lightmapUV : TEXCOORD2;
				#endif
				float3 customLighting : COLOR0;

				#if AO_ON_UV0 || AO_ON_UV1  
					float2 aouv : TEXCOORD4;
				#endif
				#if UNITY_FOG
					UNITY_FOG_COORDS(5)
				#endif
				#if SHADOW_ON
            		LIGHTING_COORDS(6, 7)
				#endif
				#if USERIM
					float rimCol : COLOR1;
				#endif
			};

			vertexOutput vert(vertexInput v)
			{
				vertexOutput o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				half3 normal = normalize(mul(unity_ObjectToWorld, half4(v.normal, 0))).xyz;

				//Maping Texture
				#if TEXTUREMODULE_ON
					o.uv = TRANSFORM_TEX(v.uv0, _MainTexture);
				#endif

				//Maping AO maps
				#if AO_ON_UV0
					o.aouv = TRANSFORM_TEX(v.uv0, _AOTexture);
				#endif
				#if AO_ON_UV1
					o.aouv = TRANSFORM_TEX(v.uv1, _AOTexture);
				#endif

				//getting GradientFactor position (world/local)
				float4 GradPos = lerp(o.worldPos, v.vertex, _GradientSpace);
				
				//Calculating custom shadings
					half3 colorFront, colorBack, colorLeft, colorRight, colorTop, colorDown;
					fixed dirFront  = max(dot(normal, FrontDir),  0.0);
					fixed dirBack   = max(dot(normal, BackDir),   0.0);
					fixed dirLeft   = max(dot(normal, LeftDir),   0.0);
					fixed dirRight  = max(dot(normal, RightDir),  0.0);
					fixed dirTop    = max(dot(normal, TopDir),    0.0);
					fixed dirBottom = max(dot(normal, BottomDir), 0.0);

					colorFront = colorBack = colorLeft = colorRight = colorTop = colorDown = v.vColor;
					
					#if FRONTSOLID 
						colorFront = _Color1_F; 
					#endif
					#if BACKSOLID
						colorBack = _Color1_B;
					#endif
					#if LEFTSOLID
						colorLeft = _Color1_L;
					#endif
					#if RIGHTSOLID
						colorRight = _Color1_R;
					#endif
					#if TOPSOLID
						colorTop = _Color1_T;
					#endif
					#if BOTTOMSOLID
						colorDown = _Color1_D;

					#endif

					#if FRONTGRADIENT
						half RotatedGrad_F = (GradPos.x - _GradientYStartPos_F.x) * sin(_Rotation_F/57.32) + (GradPos.y - _GradientYStartPos_F.y) * cos(_Rotation_F/57.32);
						half GradientFactor_F = saturate(RotatedGrad_F / -_GradientHeight_F);
						colorFront = lerp(_Color1_F, _Color2_F, GradientFactor_F);
					#endif
					#if BACKGRADIENT
						half RotatedGrad_B = (GradPos.x - _GradientYStartPos_B.x) * sin(_Rotation_B/57.32) + (GradPos.y - _GradientYStartPos_B.y) * cos(_Rotation_B/57.32);
						half GradientFactor_B = saturate(RotatedGrad_B / -_GradientHeight_B);
						colorBack  = lerp(_Color1_B, _Color2_B, GradientFactor_B);
					#endif
					#if LEFTGRADIENT
						half RotatedGrad_L = (GradPos.z - _GradientYStartPos_L.x) * sin(_Rotation_L/57.32) + (GradPos.y - _GradientYStartPos_L.y) * cos(_Rotation_L/57.32);
						half GradientFactor_L = saturate(RotatedGrad_L / -_GradientHeight_L);
						colorLeft  = lerp(_Color1_L, _Color2_L, GradientFactor_L);
					#endif
					#if RIGHTGRADIENT
						half RotatedGrad_R = (GradPos.z - _GradientYStartPos_R.x) * sin(_Rotation_R/57.32) + (GradPos.y - _GradientYStartPos_R.y) * cos(_Rotation_R/57.32);
						half GradientFactor_R = saturate(RotatedGrad_R / -_GradientHeight_R);
						colorRight = lerp(_Color1_R, _Color2_R, GradientFactor_R);
					#endif
					#if TOPGRADIENT
						half RotatedGrad_T = (GradPos.z - _GradientXStartPos_T.x) * cos(_Rotation_T/57.32) + (GradPos.x - _GradientXStartPos_T.y) * sin(_Rotation_T/57.32);
						half GradientFactor_T = saturate(RotatedGrad_T / -_GradientHeight_T);
						colorTop   = lerp(_Color1_T, _Color2_T, GradientFactor_T);
					#endif
					#if BOTTOMGRADIENT
						half RotatedGrad_D = (GradPos.z - _GradientXStartPos_D.x) * cos(_Rotation_D/57.32) + (GradPos.x - _GradientXStartPos_D.y) * sin(_Rotation_D/57.32);
						half GradientFactor_D = saturate(RotatedGrad_D / -_GradientHeight_D);
						colorDown  = lerp(_Color1_D, _Color2_D, GradientFactor_D);
					#endif


				fixed3 Maincolor;
				#if DONTMIX
					Maincolor = colorFront * dirFront + colorBack * dirBack + colorLeft * dirLeft + colorRight * dirRight + colorTop * dirTop + colorDown * dirBottom;
				#else
					Maincolor = lerp(colorFront, whiteColor, 1-dirFront) * lerp(colorBack, whiteColor, 1-dirBack) * lerp(colorLeft, whiteColor, 1-dirLeft) * lerp(colorRight, whiteColor, 1-dirRight) * lerp(colorTop, whiteColor, 1-dirTop) * lerp(colorDown, whiteColor, 1-dirBottom);
				#endif
				o.customLighting = Maincolor + (_AmbientColor * _AmbientPower);

				//Lightmap
				#if	LIGHTMAP_ADD || LIGHTMAP_MULTIPLY || LIGHTMAP_AO
					o.lightmapUV = v.uv1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
				#endif
				// Transfer realtime shadows
				#if SHADOW_ON
					TRANSFER_SHADOW(o);
				#endif
				//Apply Unity fog
				#if UNITY_FOG
					UNITY_TRANSFER_FOG(o, o.pos);
				#endif
				
				#if USERIM
					float3 viewDir = normalize(ObjSpaceViewDir(v.vertex));
					o.rimCol = pow(1 - dot(normal, viewDir), _RimPower);
				#endif

				return o;
			}

			fixed4 frag(vertexOutput i) : COLOR
			{
				fixed4 Result = fixed4(whiteColor,1);
				//applying main texture
					#if TEXTUREMODULE_ON
						fixed4 _MainTexture_var = tex2D(_MainTexture, i.uv) + _MainTexturePower;
						_MainTexture_var = clamp(_MainTexture_var, 0.0, 1.0);
						Result *= _MainTexture_var;
					#endif
				//Applying AO
					#if AO_ON_UV0
						half4 AOTexVar = lerp(half4(_AOColor, 1), half4(whiteColor, 1), lerp(half4(1,1,1,1), tex2D(_AOTexture, i.aouv), _AOPower));
						Result *= AOTexVar;
					#endif
					#if AO_ON_UV1
						half4 AOTexVar = lerp(half4(_AOColor, 1), half4(whiteColor, 1), lerp(half4(1,1,1,1), tex2D(_AOTexture, i.aouv), _AOPower));
						Result *= AOTexVar;
					#endif
				//applying custom Lighting Data
					Result *= fixed4(i.customLighting, 1);
				
				//Calculating Lightmap
					#if LIGHTMAP_AO
						half4 lmColor = UNITY_SAMPLE_TEX2D(unity_Lightmap, i.lightmapUV);
						half4 lmPower = lerp(fixed4(1,1,1,1), half4(DecodeLightmap(lmColor), 0), _LMPower);
						Result *= lerp(fixed4(_LMColor, 0), fixed4(1,1,1,1) , lmPower);
					#endif
					#if LIGHTMAP_ADD
						Result += (fixed4(DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, i.lightmapUV)), 0) * _LMPower);
						Result = clamp(Result, 0.0, 1.0);
					#endif
					#if LIGHTMAP_MULTIPLY
						Result *= fixed4(DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, i.lightmapUV)), 0) * _LMPower;
						Result = clamp(Result, 0.0, 1.0);
					#endif
				//Calculating Shadows with shadow color
				#if SHADOW_ON
					fixed4 shadowInfo = SHADOW_ATTENUATION(i);
					Result = lerp(fixed4(_ShadowColor, 1), Result, shadowInfo.a);
				#endif

				#if USERIM
					Result = fixed4(lerp(Result.rgb, _RimColor, i.rimCol), 1);
				#endif

				#if COLORCORRECTION_ON
					Result *= fixed4(_TintColor, 1);
					Result = clamp(Result + Result * _Brightness, 0.0, 1.0);
					fixed maxVal = max(max(Result.r, Result.g), Result.b);
					Result = fixed4(lerp(Result.r, maxVal, 1-_Saturation), lerp(Result.g, maxVal, 1-_Saturation), lerp(Result.b, maxVal, 1-_Saturation), 1);
				#endif

				//HeightFog
				#if HEIGHT_FOG
					half3 fogGradient = lerp(_Color_Fog, Result.rgb,  smoothstep( _FogYStartPos, _FogHeight, i.worldPos.y )).rgb;
					Result = fixed4(fogGradient, 1);
				#endif
				//Unity Fog
				#if UNITY_FOG
					UNITY_APPLY_FOG(i.fogCoord, Result);
				#endif
				return  Result;
			}
			ENDCG
		}

		// This pass it not used during regular rendering, only for lightmap baking.
		Pass
		{
			Name "Meta"
			Tags{ "LightMode" = "Meta" }

			Cull Off

			HLSLPROGRAM
			// Required to compile gles 2.0 with standard srp library
			#pragma prefer_hlslcc gles
			#pragma exclude_renderers d3d11_9x
			#pragma vertex LightweightVertexMeta
			#pragma fragment LightweightFragmentMetaSimple
			
			#pragma shader_feature _EMISSION
			#pragma shader_feature _SPECGLOSSMAP
			
			#include "LWRP/ShaderLibrary/InputSurfaceSimple.hlsl"
			#include "LWRP/ShaderLibrary/LightweightPassMetaSimple.hlsl"

			ENDHLSL
		}
	}
	FallBack "Standard"
    CustomEditor "MinimalistStandardMatV2"
}