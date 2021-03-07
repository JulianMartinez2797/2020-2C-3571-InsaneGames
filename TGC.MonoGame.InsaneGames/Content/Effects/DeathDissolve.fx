#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

float4x4 World;
float4x4 View;
float4x4 Projection;
float4x4 InverseTransposeWorld;

float3 ambientColor; // Light's Ambient Color
float3 diffuseColor; // Light's Diffuse Color
float3 specularColor; // Light's Specular Color
float KAmbient;
float KDiffuse;
float KSpecular;
float shininess;
float3 lightPosition;
float3 eyePosition; // Camera position

// attenuation
float constant;
float linearTerm;
float quadratic;

static const int NUM_LIGHTS = 3;

float3 lightsPositions[NUM_LIGHTS];

struct VertexShaderInput
{
	float4 Position : POSITION0;
	float4 Color : COLOR0;
    float2 TextureCoordinate : TEXCOORD0;
    float4 Normal : NORMAL;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float2 TextureCoordinate : TEXCOORD0;
    float4 MeshPosition : TEXCOORD1;
    float4 WorldPosition : TEXCOORD2;
    float4 Normal : TEXCOORD3;
};

texture ModelTexture;
sampler2D textureSampler = sampler_state
{
    Texture = (ModelTexture);
    MagFilter = Linear;
    MinFilter = Linear;
    AddressU = Clamp;
    AddressV = Clamp;
};






float Time = 0;
float minY = 0;
float maxY = 0;

float4 calculatePointLight(float3 lightPosition, float3 worldPosition, float2 textureCoordinates, float3 normal)
{
    // Base vectors
    float3 lightDirection = normalize(lightPosition - worldPosition);
    float3 viewDirection = normalize(eyePosition - worldPosition);
    float3 halfVector = normalize(lightDirection + viewDirection);
    
    float distance = length(lightPosition - worldPosition);
    float attenuation = 1.0 / (constant + linearTerm * distance + quadratic * (distance * distance));
    
	// Get the texture texel
    float4 texelColor = tex2D(textureSampler, textureCoordinates);
    
    // Calculate the ambient light
    float3 ambientLight = ambientColor * KAmbient;
    
	// Calculate the diffuse light
    float NdotL = saturate(dot(normal, lightDirection));
    float3 diffuseLight = KDiffuse * diffuseColor * NdotL;
    
	// Calculate the specular light
    float NdotH = dot(normal, halfVector);
    float3 specularLight = sign(NdotL) * KSpecular * specularColor * pow(saturate(NdotH), shininess);
    
    // add attenuation to lights
    ambientLight *= attenuation;
    diffuseLight *= attenuation;
    specularLight *= attenuation;
    
    // Final calculation
    float4 finalColor = float4(saturate(ambientLight + diffuseLight) * texelColor.rgb + specularLight, texelColor.a);
    
    return finalColor;
}


VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;

    float4 worldPosition = mul(input.Position, World);
    
    output.WorldPosition = worldPosition;

    float4 viewPosition = mul(worldPosition, View);
	
	// Project position
    output.Position = mul(viewPosition, Projection);
    output.MeshPosition = input.Position;

	// Propagate texture coordinates
    output.TextureCoordinate = input.TextureCoordinate;

    output.Normal = mul(input.Normal, InverseTransposeWorld);

    return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
    float4 textureColor = tex2D(textureSampler, input.TextureCoordinate);

    float y = input.MeshPosition.z;
    float time = Time * 0.75;
    float range = 0;
    float range2 = 0;
    float cyan = 0;
    if (time != 0)
    {
        range = time * (minY - maxY) + maxY;
        range2 = (time + 0.1) * (minY - maxY) + maxY;
        cyan = step(range2,y);
    }
    else 
        range = minY;

    if (step(range,y) && Time != 0)
        discard;
   
    float4 finalColor = float4(0, 0, 0, 0);
    for (int i = 0; i < NUM_LIGHTS; i++)
    {
        finalColor += calculatePointLight(lightsPositions[i],
                                    input.WorldPosition.xyz,
                                    input.TextureCoordinate,
                                    input.Normal.xyz);
    }
    return lerp(finalColor, float4(0.8, 0, 0, 1), cyan);
}

technique BasicColorDrawing
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};
