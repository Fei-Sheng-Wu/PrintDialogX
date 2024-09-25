using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

//Pre-Build
//"$(DXSDK_DIR)Utilities\Bin\x86\fxc.exe" /T ps_2_0 /E main /Fo"$(SolutionDir)PrintDialogX/Resources/GrayscaleEffect.ps" "$(SolutionDir)PrintDialogX/Effects/Grayscale/GrayscaleEffect.fx"

namespace PrintDialogX.Internal.Effects
{
    internal class GrayscaleEffect : ShaderEffect
    {
        private static PixelShader _pixelShader = new PixelShader() { UriSource = new Uri("/PrintDialogX;component/Resources/GrayscaleEffect.ps", UriKind.Relative) };

        public GrayscaleEffect()
        {
            PixelShader = _pixelShader;

            UpdateShaderValue(InputProperty);
            UpdateShaderValue(DesaturationFactorProperty);
        }

        public static readonly DependencyProperty InputProperty = RegisterPixelShaderSamplerProperty("Input", typeof(GrayscaleEffect), 0);
        public Brush Input
        {
            get { return (Brush)GetValue(InputProperty); }
            set { SetValue(InputProperty, value); }
        }

        public static readonly DependencyProperty DesaturationFactorProperty = DependencyProperty.Register("DesaturationFactor", typeof(double), typeof(GrayscaleEffect), new UIPropertyMetadata(0.0, PixelShaderConstantCallback(0), CoerceDesaturationFactor));
        public double DesaturationFactor
        {
            get { return (double)GetValue(DesaturationFactorProperty); }
            set { SetValue(DesaturationFactorProperty, value); }
        }

        private static object CoerceDesaturationFactor(DependencyObject d, object value)
        {
            GrayscaleEffect effect = (GrayscaleEffect)d;
            double newFactor = (double)value;
            return newFactor < 0.0 || newFactor > 1.0 ? effect.DesaturationFactor : newFactor;
        }
    }
}