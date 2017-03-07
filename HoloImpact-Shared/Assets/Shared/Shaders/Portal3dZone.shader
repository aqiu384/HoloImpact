Shader "Custom/Portal 3d Zone" {
	Properties{
		_PortalBase("Zone Color", Color) = (0.5,0,0,0)
	}

	SubShader{
		Tags{ "Queue" = "Transparent" }
		ZWrite Off

		Pass{
			ColorMask 0
			Cull Front
			Stencil{
				Ref 3
				Comp Always
				Pass Keep
				ZFail Replace
			}
		}
		
		Pass{
			ColorMask 0
			Stencil{
				Comp Always
				Pass Keep
				ZFail Zero
			}
		}

		Pass{
			Color[_PortalBase]
			ZTest Always
			Cull Front
			Blend One One
			Stencil{
				Ref 3
				Comp Equal
				Pass Zero
				Fail Zero
				ZFail Zero
			}
		}
	}
}