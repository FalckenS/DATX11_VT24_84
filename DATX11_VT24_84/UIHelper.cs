using System.Reflection;
using Xamarin.Forms;
using Xamarin.Forms.Shapes;

namespace DATX11_VT24_84
{
    public static class UIHelper
    {
        public static void UpdateBackgroundColor(ContentPage page)
        {
            page.BackgroundColor = Color.FromHex("#232E34");
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
        
        public static void AddBackButton(RelativeLayout layout, IHasBackButton backButtonPage)
        {
            ImageButton imageButton = new ImageButton
            {
                Source = ImageSource.FromResource("DATX11_VT24_84.Images.black_x_icon.png", typeof(EmbeddedImage).GetTypeInfo().Assembly),
                BackgroundColor = Color.Green,
                WidthRequest = 60,
                HeightRequest = 60,
                CornerRadius = 20
            };

            backButtonPage.AddClickedMethod(imageButton);
            
            Constraint xConstraint = Constraint.RelativeToParent(parent => parent.Width - 50);
            Constraint yConstraint = Constraint.RelativeToParent(parent => 20);
            Constraint widthConstraint = Constraint.Constant(60);
            Constraint heightConstraint = Constraint.Constant(60);
            
            layout.Children.Add(imageButton, xConstraint, yConstraint, widthConstraint, heightConstraint);
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
    }
}