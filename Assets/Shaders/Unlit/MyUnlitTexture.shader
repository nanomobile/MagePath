// Unlit shader. Simplest possible textured shader.
// - no lighting
// - no lightmap support

Shader "My/Unlit/Texture" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Color ("Main Color", Color) = (1,1,1,1)
	}
	
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 100
		
		Pass {
			Lighting Off
			SetTexture [_MainTex] { 
				constantColor [_Color]
                Combine texture * constant, texture * constant
			}
		}
	}
}
