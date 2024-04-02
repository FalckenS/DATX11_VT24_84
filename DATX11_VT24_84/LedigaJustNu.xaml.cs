using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DATX11_VT24_84
{
    public partial class LedigaJustNu : ContentPage, IHasBackButton
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
        List<Room> availableRooms = await BackEnd.GetAllRoomsAvailableNow();
        var groupedRooms = availableRooms.GroupBy(room => room.Building);

        foreach (var group in groupedRooms)
        {
            var frame = new Frame
            {
                BackgroundColor = Color.FromHex("#36474F"),
                CornerRadius = 10,
                Margin = new Thickness(0, 10, 0, 10)
            };

            var titleLabel = new Label
            {
                Text = group.Key,
                TextColor = Color.White,
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                FontAttributes = FontAttributes.Bold,
                HorizontalOptions = LayoutOptions.Center
            };

            frame.Content = titleLabel;

            var roomsStackLayout = new StackLayout();

            foreach (var room in group)
            {
                var grid = new Grid();

                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

                var label1 = new Label { Text = $"{room.Name}", TextColor = Color.White, VerticalOptions = LayoutOptions.CenterAndExpand, FontAttributes = FontAttributes.Bold };
                var button = new Button { Text = "Boka", TextColor = Color.White, BackgroundColor = Color.FromHex("#00ACFF"), VerticalOptions = LayoutOptions.CenterAndExpand, CornerRadius = 30, FontAttributes = FontAttributes.Bold };

                Grid.SetColumn(label1, 0);
                Grid.SetColumn(button, 2);

                grid.Children.Add(label1);
                grid.Children.Add(button);

                button.Clicked += async (sender, e) =>
                {
                    var roomName = label1.Text;
                    var building = room.Building;
                    var floor = room.Floor;
                    var today = DateTime.Today; // Get the current date
                    await Navigation.PushModalAsync(new BokaRum(roomName, building, floor, today), false);
                };

                roomsStackLayout.Children.Add(grid);
            }

            frame.Content = new StackLayout
            {
                Children = { titleLabel, roomsStackLayout }
            };

            StackLayoutFrames.Children.Add(frame);
        }
    }

    private void AddTrianglesAndBackButton()
    {
        SizeChanged += (sender, e) =>
        {
            UIUtility.AddTopTriangles(MainLayout, Width, Height);
        };
    }
}

    
    
}