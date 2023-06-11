// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader /*ase_name*/ "Hidden/Templates/iffnsDepth" /*end*/ 
{
    Properties
	{
        /*ase_props*/
    }
    SubShader
	{
		/*ase_subshader_options:Name=Additional Options
			Option:Vertex Position,InvertActionOnDeselection:Absolute,Relative:Relative
				Absolute:SetDefine:ASE_ABSOLUTE_VERTEX_POS 1
				Absolute:SetPortName:1,Vertex Position
				Relative:SetPortName:1,Vertex Offset
		*/

        Tags { "RenderType"="Opaque" }

		/*ase_all_modules*/
		
		/*ase_pass*/
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
				/*ase_pragma*/

				struct appdata
				{
					float4 vertex : POSITION;
					float4 color : COLOR;
					/*ase_vdata:p=p;c=c*/
					UNITY_VERTEX_INPUT_INSTANCE_ID
				};
                               
                struct v2f {
                    float4 vertex : SV_POSITION;
					#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
					float3 worldPos : TEXCOORD0;
					#endif
					/*ase_interp(1,):sp=sp.xyzw;wp=tc0*/
					UNITY_VERTEX_INPUT_INSTANCE_ID
					UNITY_VERTEX_OUTPUT_STEREO
                };
               
                struct fout {
                    float4 color : COLOR;
                    float depth : DEPTH;
                };

				/*ase_globals*/
               
                v2f vert (appdata v) {
                    v2f o;
					UNITY_SETUP_INSTANCE_ID(v);
					UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
					UNITY_TRANSFER_INSTANCE_ID(v, o);

					/*ase_vert_code:v=appdata;o=v2f*/
					float3 vertexValue = float3(0, 0, 0);
					#if ASE_ABSOLUTE_VERTEX_POS
					vertexValue = v.vertex.xyz;
					#endif
					vertexValue = /*ase_vert_out:Vertex Offset;Float3*/vertexValue/*end*/;
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
					/*ase_local_var:wp*/float3 WorldPosition = i.worldPos;
					#endif
					/*ase_frag_code:i=v2f*/

                    fout returnValue;

                    returnValue.color = /*ase_frag_out:Frag Color;Float4*/float4(1,1,1,1)/*end*/;
					returnValue.depth = /*ase_frag_out:Frag Depth;Float*/float(1)/*end*/;
                    
					return returnValue;
                }
            ENDCG
            }
   
    }
    FallBack "Diffuse"
}