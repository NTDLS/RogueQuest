using Library.Utility;
using Game.Engine;
using Game.Terrain;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.Engine;

namespace Game.Maps
{
    public class MapHome : MapBase
    {
        #region Public properties.

        private EngineCore _core { get; set; }

        #endregion


        enum PathDirection
        {
            Up,
            Down,
            Left,
            Right
        }

        public MapHome(EngineCore core)
            : base(core)
        {
            _core = core;
            //DrawIntroScreen();

            //DrawPath(0, 200, PathDirection.Right);
        }

        #region Perlin.
        /*
        public int cubesToGenerate = 50;

        float[] GetEmptyArray(int width)
        {
            return new float[width];
        }

        float[] GenerateWhiteNoise(int width)
        {
            float[] noise = GetEmptyArray(width);

            for (int i = 0; i < width; i++)
            {
                noise[i] = (float)MathUtility.RandomNumber(1, 8);
            }

            return noise;
        }

        float Interpolate(float x0, float x1, float alpha)
        {
            return x0 * (1 - alpha) + alpha * x1;
        }

        float[] GenerateSmoothNoise(float[] baseNoise, int octave)
        {
            int width = baseNoise.Length;
            float[] smoothNoise = GetEmptyArray(width);
            int samplePeriod = 1 << octave; // calculates 2 ^ k
            float sampleFrequency = 1.0f / samplePeriod;

            for (int i = 0; i < width; i++)
            {
                //calculate the horizontal sampling indices
                int sample_i0 = (i / samplePeriod) * samplePeriod;
                int sample_i1 = (sample_i0 + samplePeriod) % width; //wrap around
                float horizontal_blend = (i - sample_i0) * sampleFrequency;

                smoothNoise[i] = Interpolate(baseNoise[sample_i0],
                      baseNoise[sample_i1], horizontal_blend);
            }

            return smoothNoise;
        }

        float[] GeneratePerlinNoise(float[] baseNoise, int octaveCount)
        {
            int width = baseNoise.Length;
            float[][] smoothNoise = new float[octaveCount][]; //an array of 2D arrays containing
            float persistance = 0.5f;

            //generate smooth noise
            for (int i = 0; i < octaveCount; i++)
            {
                smoothNoise[i] = GenerateSmoothNoise(baseNoise, i);
            }

            float[] perlinNoise = GetEmptyArray(width);
            float amplitude = 1.0f;
            float totalAmplitude = 0.0f;

            //blend noise together
            for (int octave = octaveCount - 1; octave >= 0; octave--)
            {
                amplitude *= persistance;
                totalAmplitude += amplitude;

                for (int i = 0; i < width; i++)
                {
                    perlinNoise[i] += smoothNoise[octave][i] * amplitude;
                }
            }

            //normalisation
            for (int i = 0; i < width; i++)
            {
                perlinNoise[i] /= totalAmplitude;
            }

            return perlinNoise;
        }

        private void DrawPath(double startX, double startY, PathDirection direction, int recurse = 0)
        {
            float[] perlinNoise = GeneratePerlinNoise(GenerateWhiteNoise(cubesToGenerate), 5);

            var dirtTile = new TerrainDirt(_core);
            int tileHeight = dirtTile.Size.Height;
            int tileWidth = dirtTile.Size.Width;

            for (int i = 0; i < cubesToGenerate; i++)
            {
                double thisX = startX + (i * tileWidth);
                double thisY = (4 * perlinNoise[i]) + startY;

                var dirt = _core.AddNewTerrain<TerrainDirt>(thisX, thisY);

                if (recurse < 1 && MathUtility.ChanceIn(20))
                {
                    double startYGitter = thisY + MathUtility.RandomNumberNegative(-tileHeight, tileHeight) * 2;

                    DrawPath(thisX, startYGitter, direction, recurse + 1);
                }
            }
        }
        */
        #endregion

        private void _DrawPath(double startX, double startY, PathDirection direction)
        {
            var dirtTile = new TerrainDirt(_core);
            int tileHeight = dirtTile.Size.Height;
            int tileWidth = dirtTile.Size.Width;

            for (double x = startX; x < _core.Display.DrawingSurface.Width * 10; x += tileWidth)
            {
                if (MathUtility.ChanceIn(100))
                {
                    double startYGitter = startY + MathUtility.RandomNumberNegative(-tileHeight, tileHeight) * 2;

                    //DrawPath(x, startYGitter, direction);
                }

                //var dirt = _core.AddNewTerrain<TerrainDirt>(x, startY);
            }
        }

        private void DrawIntroScreen()
        {
            var dirtTile = new TerrainDirt(_core);

            for (int y = 100; y < _core.Display.DrawingSurface.Height - 100; y += dirtTile.Size.Height)
            {
                for (int x = 100; x < _core.Display.DrawingSurface.Width - 100; x += dirtTile.Size.Width)
                {
                    //var dirt = _core.AddNewTerrain<TerrainDirt>(x, y);
                }
            }

            _core.AddNewTextBlock("Consolas", Brushes.Black, 20, 200, 150, true, "Welcome to Rougue Quest");
            _core.AddNewTextBlock("Consolas", Brushes.Black, 10, 300, 180, true, "2021 (C) NetworkDLS");
        }

    }
}
