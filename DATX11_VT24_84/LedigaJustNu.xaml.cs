using Xamarin.Forms;

namespace DATX11_VT24_84
{
    public partial class LedigaJustNu : IHasBackButton
    {
        public LedigaJustNu()
        {
            InitializeComponent();
            UIUtility.UpdateBackgroundColor(this);
        }

        public void AddClickedMethod(ImageButton backButton)
        {
            
        }
    }
}