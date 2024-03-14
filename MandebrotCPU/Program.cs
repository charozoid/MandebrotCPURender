using SFML;
using SFML.Graphics;
using SFML.Window;
using SFML.System;
using System.Data;
using System.Threading.Tasks;

namespace MandelbrotCPU
{
    class Program
    {
        static uint width = 1280;
        static uint height = 720;
        static double scale = 1d;
        static double offsetX = 0;
        static double offsetY = 0;
        const uint threads = 16;
        public static void Main(string[] args)
        {
            RenderWindow window = new RenderWindow(new VideoMode(width, height), "Mandelbrot Set");
            window.SetFramerateLimit(60);
            window.Closed += (sender, args) => window.Close();
            window.KeyPressed += KeyPressed;

            Random random = new Random();


            Image image = new Image(width, height);
            RenderTexture renderTexture = new RenderTexture(width, height);
            RenderTexture displayTexture = new RenderTexture(width, height);
            Sprite sprite = new Sprite(renderTexture.Texture);

            int stripHeight = (int)(height / threads);


            int numSamples = 4;
            while (window.IsOpen)
            {
                window.DispatchEvents();
                window.Clear(Color.Black);

                Parallel.For(0, threads, i =>
                {
                    int starty = (int)i * stripHeight;
                    int endy = (int)(i + 1) * stripHeight;
                    for (int y = starty; y < endy; y++)
                    {
                        for (uint x = 0; x < width; x++)
                        {
                            double real = Map(x, 0, width, -2.5, 1);
                            double imaginary = Map(y, 0, height, -1, 1);

                            int maxIterations = 500;
                            int iterations = MandelBrot(real, imaginary, maxIterations);
                            Color color = Color.Black;
                            if (iterations < maxIterations)
                            {
                                color += GetColor(iterations, maxIterations);
                            }


                            image.SetPixel(x, (uint)y, color);
                        }
                    }
                });
                renderTexture.Texture.Update(image);
                RenderTexture temp = renderTexture;
                renderTexture = displayTexture;
                displayTexture = temp;

                sprite = new Sprite(renderTexture.Texture);

                window.Draw(sprite);
                window.Display();
            }
        }
        private static Color GetColor(int iterations, int maxIterations)
        {
            double t = (double)iterations / (double)maxIterations;

            Color startColor = new Color(0, 0, 0, 255);
            Color endColor = new Color(0, 0, 255, 255); ;


            Color lerpColor;

            byte lerpR = (byte)((startColor.R + (endColor.R - startColor.R) * t * 5));
            byte lerpG = (byte)((startColor.G + (endColor.G - startColor.G) * t * 5));
            byte lerpB = (byte)((startColor.B + (endColor.B - startColor.B) * t * 5));
            lerpColor = new Color(lerpR, lerpG, lerpB);


            return lerpColor;
        }
        private static void KeyPressed(object? sender, KeyEventArgs e)
        {
            if (e.Code == Keyboard.Key.Escape)
            {
                RenderWindow window = (RenderWindow)sender;
                window.Close();
            }
            if (e.Code == Keyboard.Key.Up)
            {
                scale *= 0.9;
            }
            if (e.Code == Keyboard.Key.Down)
            {
                scale *= 1.1;
            }
            if (e.Code == Keyboard.Key.W)
            {
                offsetY -= 0.1 * scale;
            }
            if (e.Code == Keyboard.Key.S)
            {
                offsetY += 0.1 * scale;
            }
            if (e.Code == Keyboard.Key.A)
            {
                offsetX -= 0.1 * scale;
            }
            if (e.Code == Keyboard.Key.D)
            {
                offsetX += 0.1 * scale;
            }
        }

        private static void Window_KeyPressed(object? sender, KeyEventArgs e)
        {
            throw new NotImplementedException();
        }

        private static int MandelBrot(double real, double imaginary, int maxIterations)
        {
            real *= scale;
            imaginary *= scale;

            real += offsetX;
            imaginary += offsetY;

            double zReal = 0;
            double zImaginary = 0;

            int i;
            for (i = 0; i < maxIterations; i++)
            {
                double zTemp = zReal * zReal - zImaginary * zImaginary + real;
                zImaginary = 2 * zReal * zImaginary + imaginary;
                zReal = zTemp;
                if (zReal * zReal + zImaginary * zImaginary > 4)
                {
                    break;
                }
            }
            return i;
        }
        private static double Map(double value, double fromLow, double fromHigh, double toLow, double toHigh)
        {
            return (value - fromLow) * (toHigh - toLow) / (fromHigh - fromLow) + toLow;
        }

        private static void Window_Closed(object? sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}