using System;
using System.Reflection;
using Xamarin.Forms;
using Xamarin.Forms.Shapes;
using Xamarin.Forms.Xaml;

namespace DATX11_VT24_84
{
    public static class UIUtility
    {
        public static void AddTopTriangles(RelativeLayout layout, double width, double height)
        {
            Polygon middleTriangle = CreateMiddleTriangle(width, height);
            Polygon leftTriangle = CreateLeftTriangle(width, height);
            Polygon rightTriangle = CreateRightTriangle(width, height);
            
            AddTriangleToLayout(layout, leftTriangle);
            AddTriangleToLayout(layout, rightTriangle);
            AddTriangleToLayout(layout, middleTriangle);
            
        }

        private static Polygon CreateLeftTriangle(double width, double height)
        {
            return new Polygon
            {
                Fill = new SolidColorBrush(Color.FromHex("#27AD72")),
                Points = new PointCollection
                {
                    new Point(0.0*width, 0),
                    new Point(0.3*width, 0),
                    new Point(0.1*width, 0.05*height),
                    new Point(0.0*width, 0.06*height)
                    
                }
            };
        }
        
        private static Polygon CreateMiddleTriangle(double width, double height)
        {
            return new Polygon
            {
                Fill = new SolidColorBrush(Color.FromHex("#853691")),
                Points = new PointCollection
                {
                    new Point(0*width, 0),
                    new Point(.7*width, 0),
                    new Point(.5*width, 0.04*height),
                    new Point(.2*width, 0.025*height)
                }
            };
        }
        
        private static Polygon CreateRightTriangle(double width, double height)
        {
            return new Polygon
            {
                Fill = new SolidColorBrush(Color.FromHex("#F9686D")),
                Points = new PointCollection
                {
                    new Point(0.3*width, 0),
                    new Point(1.0*width, 0),
                    new Point(1.0*width, 0.04*height)
                }
            };
        }

        private static void AddTriangleToLayout(RelativeLayout layout, Polygon triangle)
        {
            layout.Children.Add(triangle,
                xConstraint: Constraint.RelativeToParent(parent => 0),
                yConstraint: Constraint.RelativeToParent(parent => 0),
                widthConstraint: Constraint.RelativeToParent(parent => parent.Width),
                heightConstraint: Constraint.RelativeToParent(parent => parent.Height)
            );
        }
    }
    
    // Denna klass används för att kunna använda bilder lagrade som "embedded resources" direkt i XAML-filerna
    public class EmbeddedImage : IMarkupExtension
    {
        public string ResourceId { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            return string.IsNullOrWhiteSpace(ResourceId) ? null : ImageSource.FromResource(ResourceId, Assembly.GetExecutingAssembly());
        }
    }
}