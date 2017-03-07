Shader "Custom/Portal 3d Portal Base" {
	SubShader{
		Tags{ "Queue" = "Geometry+1" }

		ZWrite Off
		ColorMask 0

		Pass{
			Cull Front
			Stencil{
				Ref 5
				Comp Always
				Pass Replace
				Fail Zero
				ZFail Replace
			}
		}
	}
}