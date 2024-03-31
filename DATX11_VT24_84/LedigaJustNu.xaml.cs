using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DATX11_VT24_84
{
    public partial class LedigaJustNu :ContentPage,  IHasBackButton
    {
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
        private async Task AddGrids()
        {
            var stackLayout = new StackLayout(); // Create a StackLayout to hold the grids

            // Call the backend method to get the list of available rooms
            List<Room> availableRooms = await BackEnd.GetAllRoomsAvailableNow();

            // Loop through each available room and create a grid for it
            foreach (Room room in availableRooms)
            {
                var grid = new Grid();

                // Define column definitions
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

                // Add labels to the grid
                
                var label1 = new Label { Text = $"{room.Name}", TextColor = Color.White, VerticalOptions = LayoutOptions.CenterAndExpand, FontAttributes = FontAttributes.Bold };
                var button = new Button { Text = "Boka", TextColor = Color.White, BackgroundColor = Color.FromHex("#00ACFF"), VerticalOptions = LayoutOptions.CenterAndExpand, CornerRadius = 30, FontAttributes = FontAttributes.Bold };

                // Set grid column for labels
                
                Grid.SetColumn(label1, 0);
                
                Grid.SetColumn(button, 2);
                
                // Add labels and button to the grid
                grid.Children.Add(label1);
                
                grid.Children.Add(button);

                // Add click event handler for the button
                button.Clicked += (sender, e) =>
                {
                    var roomName = label1.Text;
                    var building = room.Building;
                    var floor = room.Floor;
                    
                    Navigation.PushModalAsync(new BokaRum(roomName, building, floor), false);
                };
                // Add the grid to the StackLayout
                stackLayout.Children.Add(grid);

               
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