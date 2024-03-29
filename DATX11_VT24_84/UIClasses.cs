using System;
using System.Reflection;
using Xamarin.Forms;
using Xamarin.Forms.Shapes;
using Xamarin.Forms.Xaml;

namespace DATX11_VT24_84
{
    public static class UIUtility
    {
        public static void UpdateBackgroundColorMainPages(ContentPage page)
        {
            page.BackgroundColor = Color.FromHex("#232E34");
        }
        
        public static void UpdateBackgroundColorOtherPages(ContentPage page)
        {
            page.BackgroundColor = Color.FromHex("#36474F");
        }
        
        public static void AddTopTriangles(RelativeLayout layout, double width, double height)
        {
            Polygon middleTriangle = CreateMiddleTriangle(width, height);
            Polygon leftTriangle = CreateLeftTriangle(width, height);
            Polygon rightTriangle = CreateRightTriangle(width, height);
            
            AddTriangleToLayout(layout, middleTriangle);
            AddTriangleToLayout(layout, leftTriangle);
            AddTriangleToLayout(layout, rightTriangle);
        }

        private static Polygon CreateLeftTriangle(double width, double height)
        {
            return new Polygon
            {
                Fill = new SolidColorBrush(Color.FromHex("853691")),
                Points = new PointCollection
                {
                    new Point(0, 0),
                    new Point(0.3*width, 0),
                    new Point(0, 0.06*height)
                }
            };
        }
        
        private static Polygon CreateMiddleTriangle(double width, double height)
        {
            return new Polygon
            {
                Fill = new SolidColorBrush(Color.FromHex("F9686D")),
                Points = new PointCollection
                {
                    new Point(0, 0),
                    new Point(width, 0),
                    new Point(width, 0.04*height)
                }
            };
        }
        
        private static Polygon CreateRightTriangle(double width, double height)
        {
            return new Polygon
            {
                Fill = new SolidColorBrush(Color.FromHex("27AD72")),
                Points = new PointCollection
                {
                    new Point(0.6*width, 0.00*height),
                    new Point(1.0*width, 0.00*height),
                    new Point(1.0*width, 0.08*height)
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
        
        public static void AddBackButton(RelativeLayout layout, IHasBackButton backButtonPage)
        {
            const int width = 40;
            
            ImageButton imageButton = new ImageButton
            {
                Source = ImageSource.FromResource("DATX11_VT24_84.Images.grey_back_arrow_icon.png", typeof(EmbeddedImage).GetTypeInfo().Assembly),
                BackgroundColor = Color.Transparent,
                WidthRequest = width,
                HeightRequest = width
            };
            
            backButtonPage.AddClickedMethod(imageButton);
            
            Constraint xConstraint = Constraint.RelativeToParent(parent => parent.Width - width - width/2);
            Constraint yConstraint = Constraint.RelativeToParent(parent => width/2);
            Constraint widthConstraint = Constraint.Constant(width);
            Constraint heightConstraint = Constraint.Constant(width);
            layout.Children.Add(imageButton, xConstraint, yConstraint, widthConstraint, heightConstraint);
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