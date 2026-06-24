using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Hosting;
using System;
using Microsoft.UI.Composition;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Windows.UI;
using Microsoft.Graphics.Canvas.Effects;
using System.Diagnostics;

namespace VoiceAssistant.Components.Sphere
{
    public class SphereAnimator : ISphereAnimator
    {
        private readonly Compositor _compositor;
        private readonly Visual _sphereVisual;
        private readonly UIElement _ellipseTemplate;
        private readonly Canvas _canvas;

        private CompositionPropertySet _propertySet;
        private ContainerVisual _container;
        private List<SpriteVisual> _rings = new();
        private readonly string[] _colors = { "#f3172d", "#ff8300", "#f2f735", "#49fb35", "#49fb35", "#ae2de3" };

        private Queue<double> _volumeHistory = new();
        private const int _smoothingWindow = 3;

        private float _currentScale = 1f;
        private float _lastTargetScale = 1f;

        public SphereAnimator(Compositor compositor, Visual sphereVisual, UIElement ellipseTemplate, Canvas canvas)
        {
            _compositor = compositor;
            _sphereVisual = sphereVisual;
            _ellipseTemplate = ellipseTemplate;
            _canvas = canvas;

            _propertySet = _compositor.CreatePropertySet();
            _container = _compositor.CreateContainerVisual();
        }

        public void Initialize(int count)
        {
            ElementCompositionPreview.SetElementChildVisual(_canvas, _container);

            float radius = 100f;
            var center = new Vector2((float)_canvas.Width / 2, (float)_canvas.Height / 2);

            var ellipseVisual = ElementCompositionPreview.GetElementVisual(_ellipseTemplate);
            var visualSurface = _compositor.CreateVisualSurface();
            visualSurface.SourceVisual = ellipseVisual;
            visualSurface.SourceSize = new Vector2(16, 16);

            for (int i = 0; i < count; i++)
            {
                float angle = i * 360f / count;
                float rad = (float)(angle * Math.PI / 180);
                var offsetX = radius * (float)Math.Cos(rad);
                var offsetY = radius * (float)Math.Sin(rad);
                var position = new Vector3(center.X + offsetX - 8, center.Y + offsetY - 8, 0);

                var color = GetColorFromHex(_colors[i % _colors.Length]);

                var colorMatrix = new Matrix5x4(
                    color.R / 255f, 0, 0, 0,
                    0, color.G / 255f, 0, 0,
                    0, 0, color.B / 255f, 0,
                    0, 0, 0, color.A / 255f,
                    0, 0, 0, 0);

                var colorMatrixEffect = new ColorMatrixEffect
                {
                    ColorMatrix = colorMatrix,
                    Source = new CompositionEffectSourceParameter("source")
                };

                var effectFactory = _compositor.CreateEffectFactory(colorMatrixEffect);
                var effectBrush = effectFactory.CreateBrush();

                var surfaceBrush = _compositor.CreateSurfaceBrush(visualSurface);
                effectBrush.SetSourceParameter("source", surfaceBrush);

                var ring = CreateRing(position, color, effectBrush);

                _container.Children.InsertAtTop(ring);
                _rings.Add(ring);
            }
        }

        public void Animate()
        {
            _sphereVisual.CenterPoint = new Vector3(
                (float)_canvas.Width / 2,
                (float)_canvas.Height / 2,
                0f);
            _sphereVisual.Scale = new Vector3(1f, 1f, 1f);

            _currentScale = 1f;

            StartWaveAnimation();
        }
        private void StartWaveAnimation()
        {
            if (_rings == null || _rings.Count == 0) return;

            _propertySet.InsertScalar("waveSpeedMultiplier", 1f);
            _propertySet.InsertScalar("time", 0f);
            _propertySet.InsertScalar("waveAmplitude", 0.5f);

            var timeAnimation = _compositor.CreateScalarKeyFrameAnimation();
            timeAnimation.InsertExpressionKeyFrame(1f, "1");
            timeAnimation.Duration = TimeSpan.FromSeconds(1);
            timeAnimation.IterationBehavior = AnimationIterationBehavior.Forever;

            _propertySet.StartAnimation("time", timeAnimation);

            int index = 0;
            foreach (var ring in _rings)
            {
                var exprX = _compositor.CreateExpressionAnimation("1f + source.waveAmplitude * sin((source.time * source.waveSpeedMultiplier + delay) * 6.28f)");
                exprX.SetReferenceParameter("source", _propertySet);
                exprX.SetScalarParameter("delay", index * 0.2f);
                ring.CenterPoint = new Vector3(8, 8, 0);
                ring.StartAnimation("Scale.X", exprX);

                var exprY = _compositor.CreateExpressionAnimation("1f + source.waveAmplitude * sin((source.time * source.waveSpeedMultiplier + delay) * 6.28f)");
                exprY.SetReferenceParameter("source", _propertySet);
                exprY.SetScalarParameter("delay", index * 0.2f);
                ring.StartAnimation("Scale.Y", exprY);

                index++;
            }
        }
        public void UpdateScaleAndSpeed(double volume)
        {
            volume = Math.Clamp(volume, 0.0, 1.0);

            float targetScale = 1f + (float)(volume * 1.5);

            if (Math.Abs(targetScale - _lastTargetScale) > 0.02f)
            {
                _lastTargetScale = targetScale;

                var easing = _compositor.CreateCubicBezierEasingFunction(
                    new Vector2(0.0f, 0.0f),
                    new Vector2(0.3f, 1.0f));

                var scaleAnimation = _compositor.CreateVector3KeyFrameAnimation();

                scaleAnimation.InsertKeyFrame(0f, new Vector3(_currentScale, _currentScale, 1f));
                scaleAnimation.InsertKeyFrame(1f, new Vector3(targetScale, targetScale, 1f), easing);
                scaleAnimation.Duration = TimeSpan.FromMilliseconds(200);
                scaleAnimation.StopBehavior = AnimationStopBehavior.SetToFinalValue;

                _sphereVisual.StartAnimation("Scale", scaleAnimation);

                _currentScale = targetScale;
            }

            float amplitude = 0.2f + (float)volume * 0.8f;
            _propertySet?.InsertScalar("waveAmplitude", amplitude);

            float multiplier = 1f + (float)volume * 3f;
            _propertySet?.InsertScalar("waveSpeedMultiplier", multiplier);
        }

        private SpriteVisual CreateRing(Vector3 position, Color color, CompositionBrush brush)
        {
            var ring = _compositor.CreateSpriteVisual();
            ring.Size = new Vector2(16, 16);
            ring.Brush = brush;
            ring.Offset = position;
            ring.CenterPoint = new Vector3(8, 8, 0);

            var shadow = _compositor.CreateDropShadow();
            shadow.Color = color;
            shadow.BlurRadius = 10f;
            shadow.Opacity = 0.6f;
            ring.Shadow = shadow;

            return ring;
        }
        public double GetSmoothedVolume(double newVolume)
        {
            if (_volumeHistory.Count >= _smoothingWindow)
                _volumeHistory.Dequeue();

            _volumeHistory.Enqueue(newVolume);
            return _volumeHistory.Average();
        }

        private Color GetColorFromHex(string hex)
        {
            return Color.FromArgb(255,
                Convert.ToByte(hex.Substring(1, 2), 16),
                Convert.ToByte(hex.Substring(3, 2), 16),
                Convert.ToByte(hex.Substring(5, 2), 16));
        }
    }
}
