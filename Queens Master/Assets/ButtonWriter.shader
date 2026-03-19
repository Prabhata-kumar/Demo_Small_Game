Shader "Custom/ButtonWriter"
{
    Properties { _MainTex ("Texture", 2D) = "white" {} }
    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        
        Stencil
        {
            Ref 1
            Comp Always
            Pass Replace
        }

        Pass
        {
            SetTexture [_MainTex] { combine texture }
        }
    }
}