using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using SkiaSharp;

namespace OpenTKSkia
{
    public class SkiaGLWidget : GLWidget
    {
        public new event EventHandler<SKSurface> RenderFrame;

        const SKColorType COLOR_TYPE = SKColorType.Rgba8888;
        const GRSurfaceOrigin SURFACE_ORIGIN = GRSurfaceOrigin.BottomLeft;

        GRContext grContext;
        GRBackendRenderTarget renderTarget;
        SKSurface surface;

        public SkiaGLWidget()
        {
        }

        public override void Dispose()
        {
            surface?.Dispose();
            renderTarget?.Dispose();
            grContext?.Dispose();
            base.Dispose();
        }

        protected override void OnRenderFrame()
        {
            // create the contexts if not done already
            if (grContext == null)
            {
                var glInterface = GRGlInterface.Create();
                grContext = GRContext.CreateGl(glInterface);
            }
            // manage the drawing surface
            var alloc = Allocation;
            var res = (int)Math.Max(1.0, Gdk.Screen.Default.Resolution / 96.0);
            var w = Math.Max(0, alloc.Width * res);
            var h = Math.Max(0, alloc.Height * res);
            if (renderTarget == null || surface == null || renderTarget.Width != w || renderTarget.Height != h)
            {
                // create or update the dimensions
                renderTarget?.Dispose();
                GL.GetInteger(GetPName.FramebufferBinding, out var framebuffer);
                GL.GetInteger(GetPName.StencilBits, out var stencil);
                GL.GetInteger(GetPName.Samples, out var samples);
                var maxSamples = grContext.GetMaxSurfaceSampleCount(COLOR_TYPE);
                if (samples > maxSamples)
                    samples = maxSamples;
                var glInfo = new GRGlFramebufferInfo((uint)framebuffer, COLOR_TYPE.ToGlSizedFormat());
                renderTarget = new GRBackendRenderTarget(w, h, samples, stencil, glInfo);
                // create the surface
                surface?.Dispose();
                surface = SKSurface.Create(grContext, renderTarget, SURFACE_ORIGIN, COLOR_TYPE);
            }
            using (new SKAutoCanvasRestore(surface.Canvas, true))
            {
                RenderFrame?.Invoke(this, surface);
            }
            // update the control
            surface.Canvas.Flush();
            grContext.Flush();
        }
    }
}
