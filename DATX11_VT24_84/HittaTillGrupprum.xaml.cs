using System;
using System.IO;
using System.Reflection;
using System.Xml;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using SKSvg = SkiaSharp.Extended.Svg.SKSvg;

namespace DATX11_VT24_84
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HittaTillGrupprum : ContentPage
    {
        private readonly string _roomName;
        
        public HittaTillGrupprum(string roomName)
        {
            InitializeComponent();
            _roomName = roomName;
            CanvasView.InvalidateSurface();
            UpdateText();
        }

        private async void UpdateText()
        {
            Room room = await BackEnd.GetRoomInfo(_roomName);
            RoomNameLabel.Text = room.Name;
            BuildingNameLabel.Text = room.Building;
        }
        
        private void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            // Hämta SKSurface och SKCanvas
            SKSurface surface = args.Surface;
            SKCanvas canvas = surface.Canvas;
            
            // Ladda SVG-bilden
            Assembly assembly = typeof(App).GetTypeInfo().Assembly;
            using (Stream stream = assembly.GetManifestResourceStream("DATX11_VT24_84.Images.karta.svg"))
            {
                if (stream != null)
                {
                    // Ladda SVG-innehållet i ett XmlDocument
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(stream);
                    
                    // Skapar en namespace manager för att hantera namespaces
                    XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
                    nsmgr.AddNamespace("svg", "http://www.w3.org/2000/svg");
                    
                    // Hämta alla sökvägs-element i SVG-bilden
                    XmlNodeList pathNodes = xmlDoc.SelectNodes("//svg:path", nsmgr);
                    if (pathNodes != null)
                    {
                        // Iterera genom varje sökvägs-element
                        foreach (XmlNode pathNode in pathNodes)
                        {
                            // Kontrollera om sökvägen har önskat id-attribut
                            XmlAttribute idAttribute = pathNode.Attributes?["id"];
                            if (idAttribute != null)
                            {
                                // Bestäm fyllningsfärgen baserat på rummets tillgänglighet
                                string fillColor = idAttribute.Value == _roomName ? "green" : "white";
                                
                                // Uppdatera styleAttribute för att ändra fyllningsfärg
                                XmlAttribute styleAttribute = xmlDoc.CreateAttribute("style");
                                styleAttribute.Value = $"fill:{fillColor}";
                                pathNode.Attributes.Append(styleAttribute);
                            }
                        }
                    }
                    // Rita den modifierade SVG:en på canvasen
                    SKSvg skSvg = new SKSvg();
                    skSvg.Load(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(xmlDoc.OuterXml)));
                    
                    // Beräkna skalningsfaktorerna för bredd och höjd
                    float scaleX = CanvasView.CanvasSize.Width / skSvg.Picture.CullRect.Width;
                    float scaleY = CanvasView.CanvasSize.Height / skSvg.Picture.CullRect.Height;
                    
                    // Välj den lägsta skalningsfaktorn för att säkerställa att hela SVG-bilden får plats på canvasen
                    float scale = Math.Min(scaleX, scaleY);
                    
                    // Beräkna den skalade bredden och höjden på SVG-bilden
                    float scaledWidth = skSvg.Picture.CullRect.Width * scale;
                    float scaledHeight = skSvg.Picture.CullRect.Height * scale;
                    
                    // Hörnradien
                    const float cornerRadius = 100f;
                    
                    // Skapa en rundad rektangel
                    SKRoundRect roundedRect = new SKRoundRect(new SKRect(0, 0, scaledWidth, scaledHeight), cornerRadius);
                    
                    // Sätt canvasen till rektangeln
                    canvas.ClipRoundRect(roundedRect);
                    
                    // Scaling
                    canvas.Scale(scale);
                    
                    // Rita SVG-bilden på canvasen
                    canvas.DrawPicture(skSvg.Picture);
                }
                else
                {
                    Console.WriteLine("SVG file not found");
                }
            }
        }
        
        private async void OnBackButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync(false);
        }
    }
}