using System;
using System.Collections.Generic;
using System.IO;
using Xamarin.Forms;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;

namespace DATX11_VT24_84
{
    public partial class Karta : ContentPage
    {
        private HashSet<string> availableRoomIds = new HashSet<string>();
        public Karta()
        {
            InitializeComponent(); 
            UIUtility.UpdateBackgroundColorMainPages(this);
            LoadAvailableRoomIdsAsync();
        }
        
        private async Task LoadAvailableRoomIdsAsync()
        {
            // Get the list of rooms available now
            List<Room> availableRooms = await BackEnd.GetAllRoomsAvailableNow();

            // Store the room IDs of available rooms
            foreach (Room room in availableRooms)
            {
                availableRoomIds.Add(room.Name);
            }

            // Trigger canvas view repaint
            canvasView.InvalidateSurface();
        }
        
        private void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            // Retrieve the SKSurface and SKCanvas for drawing
            SKSurface surface = args.Surface;
            SKCanvas canvas = surface.Canvas;

            // Clear the canvas
            //canvas.Clear(SKColors.White);

            // Load your SVG image
            var assembly = typeof(App).GetTypeInfo().Assembly;
            using (Stream stream = assembly.GetManifestResourceStream("DATX11_VT24_84.Images.karta.svg"))
            {
                if (stream != null)
                {
                    // Load the SVG content into an XmlDocument
                    var xmlDoc = new XmlDocument();
                    xmlDoc.Load(stream);
                    
                    // Create a namespace manager for handling namespaces
                    var nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
                    nsmgr.AddNamespace("svg", "http://www.w3.org/2000/svg");

                    // Get all path elements in the SVG
                    var pathNodes = xmlDoc.SelectNodes("//svg:path", nsmgr);
                    if (pathNodes != null)
                    {
                        // Iterate through each path element
                        foreach (XmlNode pathNode in pathNodes)
                        {

                            // Check if the path has the desired id attribute
                            var idAttribute = pathNode.Attributes["id"];
                            if (idAttribute != null)
                            {
                                // Determine the fill color based on room availability
                                string fillColor = availableRoomIds.Contains(idAttribute.Value) ? "green" : "red";

                                // Update the style attribute to change fill color
                                var styleAttribute = xmlDoc.CreateAttribute("style");
                                styleAttribute.Value = $"fill:{fillColor}";
                                pathNode.Attributes.Append(styleAttribute);
                            }
                        }
                    }

                    // Draw the modified SVG onto the canvas
                    var skSvg = new SkiaSharp.Extended.Svg.SKSvg();
                    skSvg.Load(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(xmlDoc.OuterXml)));
                    
                    // Calculate the scaling factor to fit the SVG image within the canvas
                    float scale = Math.Min((float)canvasView.CanvasSize.Width / skSvg.Picture.CullRect.Width,
                        (float)canvasView.CanvasSize.Height / skSvg.Picture.CullRect.Height);

                    // Apply the scaling transformation
                    canvas.Scale(scale);

                    // Draw the SVG onto the canvas
                    canvas.DrawPicture(skSvg.Picture);
                }
                else
                {
                    // Handle the case when the stream is null
                    Console.WriteLine("SVG file not found.");
                }
            }
        }
        
        private async void RefreshButton_Clicked(object sender, EventArgs e)
        {
            await LoadAvailableRoomIdsAsync();
        }
        
    }
}