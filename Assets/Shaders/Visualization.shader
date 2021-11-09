Shader "Carcassonne/Visualization"
{
    Properties
    {
        _DisplayColumns     ("Visible Columns", int)       = 4
        _DisplayRows        ("Visible Rows", int)          = 4
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            static const int2 gridSubDimensions = int2(3, 3); // 3x3 sub-tiles per tile.
            static const int  subcellPerTile = gridSubDimensions.x * gridSubDimensions.y;
            static const int2 totalDimensions = int2(21, 21); // Larger numbers will fail the shader compilation.
            static const int  totalTiles = totalDimensions.x * totalDimensions.y;
            static const int  totalSubtiles = totalTiles * subcellPerTile;
            
            static const float4 colCloister = float4(0.65, 0.12, 0.06, 1.00);
            static const float4 colVillage  = float4(0.41, 0.15, 0.05, 1.00);
            static const float4 colGrass    = float4(0.08, 0.38, 0.01, 1.00);
            static const float4 colRoad     = float4(0.52, 0.56, 0.60, 1.00);
            static const float4 colCity     = float4(0.90, 0.55, 0.20, 1.00);

            static const float4 colPlayer1 = float4(0.00, 0.00, 1.00, 1.00);
            static const float4 colPlayer2 = float4(0.01, 0.60, 0.01, 1.00);
            static const float4 colPlayer3 = float4(1.00, 0.85, 0.02, 1.00);
            static const float4 colPlayer4 = float4(0.68, 0.00, 0.01, 1.00);
            static const float4 colPlayer5 = float4(0.03, 0.03, 0.03, 1.00);
            static const float4 colPlayer6 = float4(0.00, 0.15, 0.00, 1.00);
            static const float4 colPlayer7 = float4(1.00, 0.25, 0.00, 1.00);
            static const float4 colPlayer8 = float4(0.30, 0.00, 0.01, 1.00);

            int _DisplayColumns;          // How many columns of tiles to display.
            int _DisplayRows;             // How many rows of tiles to display.
            float _Tiles[totalSubtiles];  // The tile data. Contains 3x3 sub-tiles for each tile.
                                          //   Left to right, top to bottom.

            float grid(float2 st, float resolution)
            {
                float2 grid = frac(st * resolution);
                return step(resolution, grid.x) * step(resolution, grid.y);
            }

            // If geography >= 100 then there is a meeple placed, otherwise no meeple is placed.
            // Players are indicated by 100s. For example, a grass tile occupied by player 1 => 103
            // or if it was occupied by player 4 => 403.
            float4 geographyToColor(float geography, float iX, float iY)
            {
                uint geo = uint(floor(geography)) % 100; // Extract geography.
                uint player = (uint(geography) - geo) / 100; // Extract Which player the meeple belongs to.

                // Check and draw rectangle representing a meeple.
                if (player > 0) // If player > 0 a meeple of that player has been placed there.
                {
                    float fracX = frac(iX);
                    float fracY = frac(iY);

                    if (fracX >= 0.3 && fracX <= 0.7 &&
                        fracY >= 0.3 && fracY <= 0.7)
                    {
                        if (step(0.4, fracX) * step(0.4, fracY) < 0.1)
                            return 0.0;

                        if (player == 1) return colPlayer1;
                        else if (player == 2) return colPlayer2;
                        else if (player == 3) return colPlayer3;
                        else if (player == 4) return colPlayer4;
                        else if (player == 5) return colPlayer5;
                        else if (player == 6) return colPlayer6;
                        else if (player == 7) return colPlayer7;
                        else if (player == 8) return colPlayer8;
                    }
                }

                // Draw the sub-tile with a color corresponding to a geography.
                if (geo == 0)      return colCloister;
                else if (geo == 1) return colVillage;
                else if (geo == 2) return colRoad;
                else if (geo == 3) return colGrass;
                else if (geo == 4) return colCity;
                
                return 0.5;
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float gridRes = 0.1;
                float2 gridDims = float2(_DisplayColumns, _DisplayRows);
                gridDims.x *= 1.0 / gridRes;
                gridDims.y *= 1.0 / gridRes;

                float2 gridUV = i.uv * gridDims;
                float gridFill = grid(gridUV + 0.05 / gridRes, gridRes);

                float iX = gridUV.x * gridRes * 3.0;
                float iY = (gridDims.y - gridUV.y) * gridRes * 3.0;

                int idx = int(iX) + int(iY) * _DisplayColumns * gridSubDimensions.x;
                float geo = _Tiles[idx];

                if (geo < 0.0) // If has invalid geography
                    return 0.0;
                else
                    return geographyToColor(geo, iX, iY) * gridFill;
            }
            ENDCG
        }
    }
}
