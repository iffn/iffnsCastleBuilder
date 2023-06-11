// Made with Amplify Shader Editor v1.9.1.5
// Available at the Unity Asset Store - http://u3d.as/y3X 
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "BlockLineShader" 
{
    Properties
	{
        _ColorLargeGrid("ColorLargeGrid", Color) = (1,0,0,1)
        _ColorSmallGrid("ColorSmallGrid", Color) = (1,0.4447487,0,1)
        _LargeGridSize("LargeGridSize", Float) = 1
        _SmallGridSize("SmallGridSize", Float) = 0.3333333
        _LineWidth("LineWidth", Float) = 0.01

    }
    SubShader
	{
		LOD 0

		

        Tags { "RenderType"="Opaque" }

		CGINCLUDE
		#pragma target 3.0
		ENDCG
		Blend Off
		AlphaToMask Off
		Cull Back
		ColorMask RGBA
		ZWrite On
		ZTest LEqual
		Offset 0 , 0
		
		
		
        Pass
		{
            CGPROGRAM        
                
                #ifndef UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX
				//only defining to not throw compilation error over Unity 5.5
				#define UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input)
				#endif
				
				#pragma vertex vert
                #pragma fragment frag
               
                #include "UnityCG.cginc"
				#define ASE_NEEDS_FRAG_WORLD_POSITION


				struct appdata
				{
					float4 vertex : POSITION;
					float4 color : COLOR;
					
					UNITY_VERTEX_INPUT_INSTANCE_ID
				};
                               
                struct v2f {
                    float4 vertex : SV_POSITION;
					#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
					float3 worldPos : TEXCOORD0;
					#endif
					float4 ase_texcoord1 : TEXCOORD1;
					UNITY_VERTEX_INPUT_INSTANCE_ID
					UNITY_VERTEX_OUTPUT_STEREO
                };
               
                struct fout {
                    float4 color : COLOR;
                    float depth : DEPTH;
                };

				uniform float4 _ColorLargeGrid;
				uniform float _LineWidth;
				uniform float _LargeGridSize;
				uniform float4 _ColorSmallGrid;
				uniform float _SmallGridSize;

               
                v2f vert (appdata v) {
                    v2f o;
					UNITY_SETUP_INSTANCE_ID(v);
					UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
					UNITY_TRANSFER_INSTANCE_ID(v, o);

					o.ase_texcoord1 = v.vertex;
					float3 vertexValue = float3(0, 0, 0);
					#if ASE_ABSOLUTE_VERTEX_POS
					vertexValue = v.vertex.xyz;
					#endif
					vertexValue = vertexValue;
					#if ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
					#else
					v.vertex.xyz += vertexValue;
					#endif
					o.vertex = UnityObjectToClipPos(v.vertex);

					#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
					o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
					#endif
					return o;
                }
             
                fout frag( v2f i ) {        
					UNITY_SETUP_INSTANCE_ID(i);
					UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

					#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
					float3 WorldPosition = i.worldPos;
					#endif
					float temp_output_11_0_g10 = _LineWidth;
					float temp_output_11_0_g11 = _LineWidth;
					float temp_output_47_0 = ( ( 1.0 - sign( ( ( ( abs( WorldPosition.x ) + ( 0.5 * temp_output_11_0_g10 ) ) % _LargeGridSize ) - temp_output_11_0_g10 ) ) ) + ( 1.0 - sign( ( ( ( abs( WorldPosition.z ) + ( 0.5 * temp_output_11_0_g11 ) ) % _LargeGridSize ) - temp_output_11_0_g11 ) ) ) );
					float temp_output_11_0_g8 = _LineWidth;
					float temp_output_11_0_g9 = _LineWidth;
					float temp_output_17_0 = ( ( 1.0 - sign( ( ( ( abs( WorldPosition.x ) + ( 0.5 * temp_output_11_0_g8 ) ) % _SmallGridSize ) - temp_output_11_0_g8 ) ) ) + ( 1.0 - sign( ( ( ( abs( WorldPosition.z ) + ( 0.5 * temp_output_11_0_g9 ) ) % _SmallGridSize ) - temp_output_11_0_g9 ) ) ) );
					
					float clampResult44 = clamp( temp_output_17_0 , 0.0 , 1.0 );
					float4 unityObjectToClipPos39 = UnityObjectToClipPos( i.ase_texcoord1.xyz );
					

                    fout returnValue;

                    returnValue.color = ( ( ( _ColorLargeGrid * temp_output_47_0 ) + ( _ColorSmallGrid * ( 1.0 - temp_output_47_0 ) ) ) * temp_output_17_0 );
					returnValue.depth = ( clampResult44 * ( unityObjectToClipPos39.z / unityObjectToClipPos39.w ) );
                    
					return returnValue;
                }
            ENDCG
            }
   
    }
    Fallback "Diffuse"
	
	CustomEditor "ASEMaterialInspector"
}/*ASEBEGIN
Version=19105
Node;AmplifyShaderEditor.SimpleDivideOpNode;40;574.7457,161.6437;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;42;774.4213,-4.57028;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;44;513.4333,-21.11297;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.UnityObjToClipPosHlpNode;39;266.5497,319.4745;Inherit;False;1;0;FLOAT3;0,0,0;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PosVertexDataNode;41;-23.59719,322.8264;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;17;-335.2375,-17.45959;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;22;-660.2888,-92.09045;Inherit;False;BlockGridMask;-1;;8;7eb3d0c261ba3714fb2584bdab48c597;0;3;10;FLOAT;0;False;15;FLOAT;0;False;11;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;23;-661.5219,46.65482;Inherit;False;BlockGridMask;-1;;9;7eb3d0c261ba3714fb2584bdab48c597;0;3;10;FLOAT;0;False;15;FLOAT;0;False;11;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;47;-339.0253,-407.1024;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;49;-664.0768,-481.7332;Inherit;False;BlockGridMask;-1;;10;7eb3d0c261ba3714fb2584bdab48c597;0;3;10;FLOAT;0;False;15;FLOAT;0;False;11;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;50;-665.3099,-342.988;Inherit;False;BlockGridMask;-1;;11;7eb3d0c261ba3714fb2584bdab48c597;0;3;10;FLOAT;0;False;15;FLOAT;0;False;11;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;1;-984.7774,-154.2491;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.OneMinusNode;56;-196.3739,-270.8281;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;25;572.4073,-387.0543;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;55;115.7261,-351.3281;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;57;284.6261,-396.8281;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;53;121.1261,-500.528;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;59;1025.827,-248.76;Float;False;True;-1;2;ASEMaterialInspector;0;12;BlockLineShader;1c06d3445852c2c4c9ba5fba1fff9251;True;SubShader 0 Pass 0;0;0;SubShader 0 Pass 0;3;False;True;0;1;False;;0;False;;0;1;False;;0;False;;True;0;False;;0;False;;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;1;RenderType=Opaque=RenderType;True;2;False;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;0;Diffuse;0;0;Standard;1;Vertex Position,InvertActionOnDeselection;1;0;0;1;True;False;;False;0
Node;AmplifyShaderEditor.RangedFloatNode;52;-973.5046,-331.5561;Inherit;False;Property;_LargeGridSize;LargeGridSize;2;0;Create;True;0;0;0;False;0;False;1;0.3333333;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;3;-986.8453,38.08668;Inherit;False;Property;_SmallGridSize;SmallGridSize;3;0;Create;True;0;0;0;False;0;False;0.3333333;0.3333333;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;5;-971.6187,132.5673;Inherit;False;Property;_LineWidth;LineWidth;4;0;Create;True;0;0;0;False;0;False;0.01;0.01;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;54;-249.2987,-815.445;Inherit;False;Property;_ColorLargeGrid;ColorLargeGrid;0;0;Create;True;0;0;0;False;0;False;1,0,0,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;24;-239.6157,-594.7234;Inherit;False;Property;_ColorSmallGrid;ColorSmallGrid;1;0;Create;True;0;0;0;False;0;False;1,0.4447487,0,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
WireConnection;40;0;39;3
WireConnection;40;1;39;4
WireConnection;42;0;44;0
WireConnection;42;1;40;0
WireConnection;44;0;17;0
WireConnection;39;0;41;0
WireConnection;17;0;22;0
WireConnection;17;1;23;0
WireConnection;22;10;1;1
WireConnection;22;15;3;0
WireConnection;22;11;5;0
WireConnection;23;10;1;3
WireConnection;23;15;3;0
WireConnection;23;11;5;0
WireConnection;47;0;49;0
WireConnection;47;1;50;0
WireConnection;49;10;1;1
WireConnection;49;15;52;0
WireConnection;49;11;5;0
WireConnection;50;10;1;3
WireConnection;50;15;52;0
WireConnection;50;11;5;0
WireConnection;56;0;47;0
WireConnection;25;0;57;0
WireConnection;25;1;17;0
WireConnection;55;0;24;0
WireConnection;55;1;56;0
WireConnection;57;0;53;0
WireConnection;57;1;55;0
WireConnection;53;0;54;0
WireConnection;53;1;47;0
WireConnection;59;0;25;0
WireConnection;59;1;42;0
ASEEND*/
//CHKSM=493ED1A301086341018D8A22E4BF4F8A44BD97B4