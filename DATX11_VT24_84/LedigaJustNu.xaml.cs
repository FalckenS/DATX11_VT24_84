using System;
using System.Linq;
using Xamarin.Forms;

namespace DATX11_VT24_84
{
    public partial class LedigaJustNu :ContentPage,  IHasBackButton
    {
        public int amountOfFreeRooms = 3;
        public LedigaJustNu()
        {
            InitializeComponent();
            AddGrids();
            AddTrianglesAndBackButton();
            UIUtility.UpdateBackgroundColorMainPages(this);
            
        }

        public void AddClickedMethod(ImageButton backButton)
        {
            
        }
        private void AddGrids()
{
    var stackLayout = new StackLayout(); // Create a StackLayout to hold the grids
    double yOffset = 0;

    for (int i = 0; i < amountOfFreeRooms; i++)
    {
        var grid = new Grid();

        // Define column definitions
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

        // Add labels to the grid
        var label1 = new Label { Text = $"Room {i + 1}", TextColor = Color.White, VerticalOptions = LayoutOptions.CenterAndExpand, FontAttributes = FontAttributes.Bold };
        var label2 = new Label { Text = $"{i + 8}:00 - {i + 9}:00", TextColor = Color.White, VerticalOptions = LayoutOptions.CenterAndExpand, Padding = new Thickness(0, 0, 10, 0), FontAttributes = FontAttributes.Bold };
        var button = new Button { Text = "Boka", TextColor = Color.White, BackgroundColor = Color.FromHex("#00ACFF"), VerticalOptions = LayoutOptions.CenterAndExpand, CornerRadius = 30, FontAttributes = FontAttributes.Bold };

        // Set grid column for labels
        Grid.SetColumn(label1, 0);
        Grid.SetColumn(label2, 1);
        Grid.SetColumn(button, 2);

        // Add labels and button to the grid
        grid.Children.Add(label1);
        grid.Children.Add(label2);
        grid.Children.Add(button);

        // Add click event handler for the button
        button.Clicked += (sender, e) =>
        {
            // Get the room name from the label
            var roomName = label1.Text;
            //var timeFrame = label2.Text;

            // Navigate to the BokaRum page with the room name
            Navigation.PushModalAsync(new BokaRum(roomName),false);
        };

        // Add the grid to the StackLayout
        stackLayout.Children.Add(grid);

        // Increment yOffset to provide additional space between each row
        yOffset += 20;
    }
    // Add the StackLayout to the FrameLayout
    FrameLayout.Content = stackLayout;
}

        
        // Kodduplication men kan nog justeras om senare
        private void AddTrianglesAndBackButton()
                {
                    // Av okänd anledning verkar SizeChanged vara det enda sättet att få korrekt Width och Height
                    SizeChanged += (sender, e) =>
                    {
                        UIUtility.AddTopTriangles(MainLayout, Width, Height);
                    };
                }
    }
    
    
}