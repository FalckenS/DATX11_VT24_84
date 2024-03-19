using System;
using System.Reflection;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DATX11_VT24_84
{
    /*
    Denna klass används för att kunna använda bilder lagrade som "embedded resources" direkt i XAML-filerna
    */
    public class EmbeddedImage : IMarkupExtension
    {
        public string ResourceId { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (String.IsNullOrWhiteSpace(ResourceId))
            {
                return null;
            }
            return ImageSource.FromResource(ResourceId, Assembly.GetExecutingAssembly());
        }
    }
}