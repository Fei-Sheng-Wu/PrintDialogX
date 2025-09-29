using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;

//Pre-Build
//"$(DXSDK_DIR)Utilities\Bin\x86\fxc.exe" /T ps_2_0 /E main /Fo"$(SolutionDir)PrintDialogX/Resources/MonochromeEffect.ps" "$(SolutionDir)PrintDialogX/Effects/MonochromeEffect.fx"

namespace PrintDialogX.Internal.Effects
{
    internal class MonochromeEffect : ShaderEffect
    {
        private static PixelShader _pixelShader = new PixelShader()
        {
            UriSource = new Uri("/PrintDialogX;component/Resources/MonochromeEffect.ps", UriKind.Relative),
            ShaderRenderMode = ShaderRenderMode.Auto
        };

        BitmapSource _blueNoiseTex64x64;

        public MonochromeEffect()
        {
            using (var stream = typeof(MonochromeEffect).Assembly.GetManifestResourceStream("PrintDialogX.Resources.BlueNoise.png"))
            {
                var decoder = new PngBitmapDecoder(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);

                _blueNoiseTex64x64 =  decoder.Frames[0];

                BlueNoiseTexture = new ImageBrush(_blueNoiseTex64x64);
            }

            this.PixelShader = _pixelShader;

            UpdateShaderValue(InputProperty);
            UpdateShaderValue(BlueNoiseTextureProperty);
        }

        public static readonly DependencyProperty InputProperty = RegisterPixelShaderSamplerProperty("Input", typeof(MonochromeEffect), 0);
        public Brush Input
        {
            get { return (Brush)GetValue(InputProperty); }
            set { SetValue(InputProperty, value); }
        }

        private static readonly DependencyProperty BlueNoiseTextureProperty = RegisterPixelShaderSamplerProperty("BlueNoiseTexture", typeof(MonochromeEffect), 1);
        private Brush BlueNoiseTexture
        {
            get { return (Brush)GetValue(BlueNoiseTextureProperty); }
            set { SetValue(BlueNoiseTextureProperty, value); }
        }

        public static readonly DependencyProperty ViewportTopProperty = DependencyProperty.Register("ViewportTop", typeof(float), typeof(MonochromeEffect), new PropertyMetadata(PixelShaderConstantCallback(0)));
        public float ViewportTop
        {
            get { return (float)GetValue(ViewportTopProperty); }
            set { SetValue(ViewportTopProperty, value); }
        }

        public static readonly DependencyProperty ViewportLeftProperty = DependencyProperty.Register("ViewportLeft", typeof(float), typeof(MonochromeEffect), new PropertyMetadata(PixelShaderConstantCallback(1)));
        public float ViewportLeft
        {
            get { return (float)GetValue(ViewportLeftProperty); }
            set { SetValue(ViewportLeftProperty, value); }
        }

        public static readonly DependencyProperty ViewportWidthProperty = DependencyProperty.Register("ViewportWidth", typeof(float), typeof(MonochromeEffect), new PropertyMetadata(PixelShaderConstantCallback(2)));
        public float ViewportWidth
        {
            get { return (float)GetValue(ViewportWidthProperty); }
            set { SetValue(ViewportWidthProperty, value); }
        }

        public static readonly DependencyProperty ViewportHeightProperty = DependencyProperty.Register("ViewportHeight", typeof(float), typeof(MonochromeEffect), new PropertyMetadata(PixelShaderConstantCallback(3)));
        public float ViewportHeight
        {
            get { return (float)GetValue(ViewportHeightProperty); }
            set { SetValue(ViewportHeightProperty, value); }
        }
    }
}
