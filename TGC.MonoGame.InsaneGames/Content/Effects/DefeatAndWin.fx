#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

Texture2D SpriteTexture;

float Time = 0;

sampler2D SpriteTextureSampler = sampler_state
{
	Texture = <SpriteTexture>;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

float4 MainPS(VertexShaderOutput input) : COLOR
{
    float4 color;
    bool isWhite = (input.Color.r == 1 && input.Color.g == 1 && input.Color.b == 1);
    if (isWhite)
    {
        color = float4(abs(cos(Time * 1.5)), abs(cos(Time * 1.5)), abs(cos(Time * 1.5)), 1);
    }
    else 
    {
        color = input.Color;
    }	
    return tex2D(SpriteTextureSampler, input.TextureCoordinates) * color;
}

technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};