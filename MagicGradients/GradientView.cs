using MagicGradients.Renderers;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System.ComponentModel;
using Xamarin.Forms;

namespace MagicGradients
{
    [ContentProperty(nameof(GradientSource))]
    public class GradientView : SKCanvasView
    {
        static GradientView()
        {
            StyleSheets.RegisterStyle("background", typeof(GradientView), nameof(GradientSourceProperty));
        }

        public static readonly BindableProperty GradientSourceProperty = BindableProperty.Create(nameof(GradientSource), 
            typeof(IGradientSource), typeof(GradientView), propertyChanged: OnGradientSourceChanged);

        public IGradientSource GradientSource
        {
            get => (IGradientSource)GetValue(GradientSourceProperty);
            set
            {                
                ((IGradientSource)value).PropertyChanged -= GradientView_PropertyChanged;
                ((IGradientSource)value).PropertyChanged += GradientView_PropertyChanged;
                SetValue(GradientSourceProperty, value);
            }
        }

        private void GradientView_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            InvalidateSurface();
        }

        static void OnGradientSourceChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var gradientView = (GradientView)bindable;
            gradientView.GradientSource = (IGradientSource)newValue;
        }

        protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
        {
            base.OnPaintSurface(e);

            var canvas = e.Surface.Canvas;
            canvas.Clear();

            if (GradientSource == null)
                return;

            using (var paint = new SKPaint())
            {
                var context = new RenderContext(canvas, paint, e.Info);

                foreach (var gradient in GradientSource.GetGradients())
                {
                    gradient.Measure();
                    gradient.Render(context);
                }
            }
        }
    }
}
