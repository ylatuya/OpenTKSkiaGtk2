using System;
using Gdk;
using Gtk;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using SkiaSharp;

namespace OpenTKSkia
{
    class MainClass
    {

        public static void Main(string[] args)
        {
            Toolkit.Init();
            Application.Init();
            var mode = GraphicsMode.Default;
            MainWindow win = new MainWindow();
            var glWidget = new SkiaGLWidget();
            win.DeleteEvent += (o, a) => { glWidget.Dispose(); };
            glWidget.RenderFrame += RenderFrame;
            win.Add(glWidget);
            win.ShowAll();
            Application.Run();
        }

        private static void RenderFrame(object sender, SKSurface surface)
        {
            SkiaGLWidget gLWidget = sender as SkiaGLWidget;
            SKCanvas canvas = surface.Canvas;

            canvas.Clear();

            SKPoint center = new SKPoint(gLWidget.Allocation.Width / 2, gLWidget.Allocation.Height / 2);
            float radius = Math.Min(gLWidget.Allocation.Width, gLWidget.Allocation.Height) / 4;


            SKPath path = new SKPath
            {
                FillType = SKPathFillType.EvenOdd
            };

            path.AddCircle(center.X - radius / 2, center.Y - radius / 2, radius);
            path.AddCircle(center.X - radius / 2, center.Y + radius / 2, radius);
            path.AddCircle(center.X + radius / 2, center.Y - radius / 2, radius);
            path.AddCircle(center.X + radius / 2, center.Y + radius / 2, radius);

            SKPaint paint = new SKPaint()
            {
                Style = SKPaintStyle.Fill,
                Color = SKColors.Cyan
            };

            canvas.DrawPath(path, paint);

            paint.Style = SKPaintStyle.Stroke;
            paint.StrokeWidth = 10;
            paint.Color = SKColors.Magenta;

            canvas.DrawPath(path, paint);

        }
    }
}
