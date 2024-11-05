#if OPENGL
    #define SV_POSITION POSITION
    #define VS_SHADERMODEL vs_3_0
    #define PS_SHADERMODEL ps_3_0
#else
    #define VS_SHADERMODEL vs_4_0_level_9_1
    #define PS_SHADERMODEL ps_4_0_level_9_1
#endif

Texture2D SpriteTexture;

float4x4 MatrixTransform;

sampler2D SpriteTextureSampler = sampler_state
{
    Texture = <SpriteTexture>;
};

struct VertexShaderInput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float2 TextureCoordinates : TEXCOORD0;
    float4 Source : TEXCOORD1;
    float4 NineSlice : TEXCOORD2;
    float2 SourceSizePixels : TEXCOORD3;
    float2 DestinationSizePixels : TEXCOORD4;
    int Mode : TEXCOORD5;
};

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float2 TextureCoordinates : TEXCOORD0;
    float4 Source : TEXCOORD1;
    float2 SourceDestinationRatio : TEXCOORD2;
    float4 NineSlice : TEXCOORD3;
    int Mode : TEXCOORD4;
};

VertexShaderOutput MainVertexShader(in VertexShaderInput input)
{
    VertexShaderOutput output = (VertexShaderOutput) 0;
    
    // Transform the vertex position by the WorldViewProjection matrix
    output.Position = mul(input.Position, MatrixTransform);

    // Pass through the color and texture coordinates
    output.Color = input.Color;
    output.TextureCoordinates = input.TextureCoordinates;

    output.Source = input.Source;
    output.SourceDestinationRatio = input.SourceSizePixels / input.DestinationSizePixels;
    output.NineSlice = input.NineSlice;
    output.Mode = input.Mode;
    
    return output;
}

float4 MainNineSlice(VertexShaderOutput input) : COLOR
{
	// The tex coords within the source rectangle, in destination-space, from 0 to 1.
    float2 sourceCoordinates = float2((input.TextureCoordinates.x - input.Source.x) / input.Source.z, (input.TextureCoordinates.y - input.Source.y) / input.Source.w);
	
	// The tex coords within the destination rectangle, in source-space. Where an axis is 1 when the destination pixel is equal to the source pixel. This means this can go over 1 on either axis.
    float2 destinationCoordinates = sourceCoordinates / input.SourceDestinationRatio;

    float2 sampleCoordinates = float2(0, 0);

	// Left.
    if (destinationCoordinates.x < input.NineSlice.x)
        sampleCoordinates.x = destinationCoordinates.x;
	// Right.
    else if (destinationCoordinates.x >= (1 / input.SourceDestinationRatio.x) - (1 - input.NineSlice.y))
    {
        float destinationRightBorder = (1 / input.SourceDestinationRatio.x) - (1 - input.NineSlice.y);
        sampleCoordinates.x = input.NineSlice.y + (destinationCoordinates.x - destinationRightBorder);
    }
    else
    {
        float width = input.NineSlice.y - input.NineSlice.x;
        float centreCoordinate = (destinationCoordinates.x - input.NineSlice.x) / width;
        sampleCoordinates.x = input.NineSlice.x + ((centreCoordinate - floor(centreCoordinate)) * width);
    }
	
	// Top.
    if (destinationCoordinates.y < input.NineSlice.z)
        sampleCoordinates.y = destinationCoordinates.y;
	// Bottom.
    else if (destinationCoordinates.y >= (1 / input.SourceDestinationRatio.y) - (1 - input.NineSlice.w))
    {
        float destinationBottomBorder = (1 / input.SourceDestinationRatio.y) - (1 - input.NineSlice.w);
        sampleCoordinates.y = input.NineSlice.w + (destinationCoordinates.y - destinationBottomBorder);
    }
    else
    {
        float height = input.NineSlice.w - input.NineSlice.z;
        float centreCoordinate = (destinationCoordinates.y - input.NineSlice.z) / height;
        sampleCoordinates.y = input.NineSlice.z + ((centreCoordinate - floor(centreCoordinate)) * height);
    }
	
    sampleCoordinates = input.Source.xy + (sampleCoordinates * input.Source.zw);
	
    return tex2D(SpriteTextureSampler, sampleCoordinates) * input.Color;
}

float4 MainStretchImage(VertexShaderOutput input) : COLOR
{
    return tex2D(SpriteTextureSampler, input.TextureCoordinates) * input.Color;
}

float4 MainTileImage(VertexShaderOutput input) : COLOR
{
    // The tex coords within the source rectangle, in destination-space, from 0 to 1.
    float2 sourceCoordinates = float2((input.TextureCoordinates.x - input.Source.x) / input.Source.z, (input.TextureCoordinates.y - input.Source.y) / input.Source.w);
	
	// The tex coords within the destination rectangle, in source-space. Where an axis is 1 when the destination pixel is equal to the source pixel. This means this can go over 1 on either axis.
    float2 destinationCoordinates = sourceCoordinates / input.SourceDestinationRatio;
    
    destinationCoordinates = destinationCoordinates - floor(destinationCoordinates);
    destinationCoordinates = input.Source.xy + (destinationCoordinates * input.Source.zw);
    return tex2D(SpriteTextureSampler, destinationCoordinates) * input.Color;
}

float4 MainBranchShader(VertexShaderOutput input) : COLOR
{
    if (input.Mode == 0)
        return MainNineSlice(input);
    else if (input.Mode == 1)
        return MainStretchImage(input);
    else
        return MainTileImage(input);
}

technique NineSlicer
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL MainVertexShader();
        PixelShader = compile PS_SHADERMODEL MainNineSlice();
    }
};

technique StretchImage
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL MainVertexShader();
        PixelShader = compile PS_SHADERMODEL MainStretchImage();
    }
};

technique TileImage
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL MainVertexShader();
        PixelShader = compile PS_SHADERMODEL MainTileImage();
    }
};

technique ShaderTimeBranching
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL MainVertexShader();
        PixelShader = compile PS_SHADERMODEL MainBranchShader();
    }
};