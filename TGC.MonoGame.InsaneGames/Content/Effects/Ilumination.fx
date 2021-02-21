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

float Time = 0;

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

texture ModelTexture;
sampler2D textureSampler = sampler_state
{
    Texture = (ModelTexture);
    MagFilter = Linear;
    MinFilter = Linear;
    AddressU = Clamp;
    AddressV = Clamp;
};

struct VertexShaderInput
{
    float4 Position : POSITION0;
    float4 Normal : NORMAL;
    float2 TextureCoordinates : TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float2 TextureCoordinates : TEXCOORD0;
    float4 WorldPosition : TEXCOORD1;
    float4 Normal : TEXCOORD2;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
    VertexShaderOutput output = (VertexShaderOutput) 0;

    float4 worldPosition = mul(input.Position, World);
    
    output.WorldPosition = worldPosition;

    output.Normal = mul(input.Normal, InverseTransposeWorld);
    
    float4 viewPosition = mul(worldPosition, View);

	// Project position
    output.Position = mul(viewPosition, Projection);

	// Propagate texture coordinates
    output.TextureCoordinates = input.TextureCoordinates;

    return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
    
    // Base vectors
    float3 lightDirection = normalize(lightPosition - input.WorldPosition.xyz);
    float3 viewDirection = normalize(eyePosition - input.WorldPosition.xyz);
    float3 halfVector = normalize(lightDirection + viewDirection);
    
    float distance = length(lightPosition - input.WorldPosition.xyz);
    float attenuation = 1.0 / (constant + linearTerm * distance + quadratic * (distance * distance));    
    
	// Get the texture texel
    float4 texelColor = tex2D(textureSampler, input.TextureCoordinates);
    
    // Calculate the ambient light
    float3 ambientLight = ambientColor * KAmbient;
    
	// Calculate the diffuse light
    float NdotL = saturate(dot(input.Normal.xyz, lightDirection));
    float3 diffuseLight = KDiffuse * diffuseColor * NdotL;
    
	// Calculate the specular light
    float NdotH = dot(input.Normal.xyz, halfVector);
    float3 specularLight = sign(NdotL) * KSpecular * specularColor * pow(saturate(NdotH), shininess);
    
    // add attenuation to lights
    ambientLight *= attenuation;
    diffuseLight *= attenuation;
    specularLight *= attenuation;
    
    // Final calculation
    float4 finalColor = float4(saturate(ambientLight + diffuseLight) * texelColor.rgb + specularLight, texelColor.a);
       
    return finalColor;
    //return float4(1, 0, 0, 1);;
}

technique Ilumination
{
    pass Pass0
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
};