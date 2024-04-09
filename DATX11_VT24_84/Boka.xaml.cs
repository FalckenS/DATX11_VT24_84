using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DATX11_VT24_84
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Boka : ContentPage
    {
        private DateTime _currentDate;
        private bool isListaSelected = true; // Initially, Lista is selected


        public Boka()
        {
            InitializeComponent();
            UIUtility.UpdateBackgroundColorOtherPages(this);
            _currentDate = DateTime.Today;
            UpdateDateLabel();
            UpdateDatePicker();

        }

        private void OnPreviousDateClicked(object sender, EventArgs e)
        {
            if (_currentDate > DateTime.Today.AddDays(0))
            {
                _currentDate = _currentDate.AddDays(-1);
                UpdateDateLabel();
                UpdateDatePicker();
            }
        }

        private void OnNextDateClicked(object sender, EventArgs e)
        {
            if (_currentDate < DateTime.Today.AddDays(7))
            {
                _currentDate = _currentDate.AddDays(1);
                UpdateDateLabel();
                UpdateDatePicker();
            }
        }

        private void OnDateLabelTapped(object sender, EventArgs e)
        {
            DatePicker.Focus();
        }

        private void OnDateSelected(object sender, DateChangedEventArgs e)
        {
            _currentDate = e.NewDate;
            UpdateDateLabel();
            DatePicker.IsVisible = false;
            UpdateDatePicker();
        }

        private void UpdateDateLabel()
        {
            string formattedDate =
                _currentDate.ToString("dddd, d MMMM yyyy", new System.Globalization.CultureInfo("sv-SE"));
            formattedDate = char.ToUpper(formattedDate[0]) + formattedDate.Substring(1);
            DateDisplayLabel.Text = formattedDate;
            DateDisplayLabel.FontAttributes = FontAttributes.Bold;
            DateDisplayLabel.TextColor = Color.Black;
        }

        private void UpdateDatePicker()
        {
            DatePicker.Date = _currentDate;
            DatePicker.MinimumDate = DateTime.Today.AddDays(0); //Kan inte kolla på bokningar bak i tiden
            DatePicker.MaximumDate = DateTime.Today.AddDays(7); // Kan boka 14 dar i framtiden
        }
        private void OnListaLabelTapped(object sender, EventArgs e)
        {
            isListaSelected = true; // Set Lista as selected
            UpdateOverlayPositionLeft();
        }

        private void OnKartaLabelTapped(object sender, EventArgs e)
        {
            isListaSelected = false; // Set Karta as selected
            UpdateOverlayPositionRight();
        }

        private void UpdateOverlayPositionRight()
        {
            OverlayBox.TranslateTo(120, 0, 250);
        }
        private void UpdateOverlayPositionLeft()
        {
            OverlayBox.TranslateTo(0, 0, 250);
        }



        
}
}
