Shader "Custom/Portal 3d Hole" {
	SubShader{
		Tags{ "Queue" = "Geometry+2" }

		ZWrite Off
		ColorMask 0

		Pass{
			Cull Front
			Stencil{
				Ref 3
				Comp Always
				Pass Keep
				ZFail Replace
			}
		}
		
		Pass{
			Stencil{
				Ref 5
				Comp Always
				Pass Keep
				ZFail Replace
			}
		}
	}
}