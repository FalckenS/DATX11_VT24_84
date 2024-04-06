using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DATX11_VT24_84
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Boka : ContentPage
    {
        private DateTime _currentDate;

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
            // Check if it's within the allowed range
            if (_currentDate < DateTime.Today.AddDays(14))
            {
                // Go to the next date (add one day)
                _currentDate = _currentDate.AddDays(1);
                UpdateDateLabel();
                UpdateDatePicker(); // Update the DatePicker
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
            string formattedDate = _currentDate.ToString("dddd, d MMMM yyyy", new System.Globalization.CultureInfo("sv-SE"));
            formattedDate = char.ToUpper(formattedDate[0]) + formattedDate.Substring(1);
            DateDisplayLabel.Text = formattedDate;
        }

        private void UpdateDatePicker()
        {
            DatePicker.Date = _currentDate;
            DatePicker.MinimumDate = DateTime.Today.AddDays(0); //Kan inte kolla på bokningar bak i tiden
            DatePicker.MaximumDate = DateTime.Today.AddDays(14); // Kan boka 14 dar i framtiden
        }
    }
}