Shader "Custom/Portal 3d Background" {
	Properties{
		_CameraBackground("Camera Background", Color) = (0,0,0,0)
		_PortalBase("Portal Base", Color) = (0.5,0.5,0.5,0)
	}

	SubShader{
		Tags{ "Queue" = "Geometry+3" }
		ZTest Always

		Pass{
			Color[_PortalBase]
			Cull Front
			Stencil{
				Ref 5
				Comp Equal
			}
		}

		Pass{
			Color[_CameraBackground]
			Cull Front
			Stencil{
				Ref 0
				Comp Equal
				Pass Zero
				Fail Zero
				ZFail Zero
			}
		}
	}
}