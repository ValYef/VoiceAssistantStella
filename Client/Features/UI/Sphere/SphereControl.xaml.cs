using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Hosting;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using System;
using Microsoft.UI.Composition;
using VoiceAssistant.Components.Sphere;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml.Media.Media3D;
using System.Numerics;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas;
using System.Net.NetworkInformation;
using System.Diagnostics;

namespace VoiceAssistant.Components.Sphere
{
    public sealed partial class SphereControl : UserControl
    {
        private Compositor _compositor;
        private Visual _sphereVisual;
        private SphereAnimator _animator;

        private const int RingCount = 90;
        private double _maxObservedVolume = 0.001;

        public SphereControl()
        {
            this.InitializeComponent();

            _compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;
            _sphereVisual = ElementCompositionPreview.GetElementVisual(SphereCanvas);

            _animator = new SphereAnimator(_compositor, _sphereVisual, EllipseTemplate, SphereCanvas);
            _animator.Initialize(RingCount);
            _animator.Animate();
        }
        public void Update(double volume)
        {
            if (volume > _maxObservedVolume)
                _maxObservedVolume = volume;

            double normalized = Math.Clamp(volume / _maxObservedVolume, 0.0, 1.0);

            double smoothedVolume = _animator.GetSmoothedVolume(volume);
            _animator?.UpdateScaleAndSpeed(smoothedVolume);
        }
    }
}
