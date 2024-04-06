using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DATX11_VT24_84
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Boka : ContentPage
    {
        private DateTime currentDate;

        public Boka()
        {
            InitializeComponent();
            UIUtility.UpdateBackgroundColorOtherPages(this);
            currentDate = DateTime.Today;
            UpdateDateLabel();
            UpdateDatePicker(); // Update the DatePicker immediately
        }


        private void OnPreviousDateClicked(object sender, EventArgs e)
        {
            // Check if it's within the allowed range
            if (currentDate > DateTime.Today.AddDays(0))
            {
                // Go to the previous date (subtract one day)
                currentDate = currentDate.AddDays(-1);
                UpdateDateLabel();
                UpdateDatePicker(); // Update the DatePicker
            }
        }

        private void OnNextDateClicked(object sender, EventArgs e)
        {
            // Check if it's within the allowed range
            if (currentDate < DateTime.Today.AddDays(14))
            {
                // Go to the next date (add one day)
                currentDate = currentDate.AddDays(1);
                UpdateDateLabel();
                UpdateDatePicker(); // Update the DatePicker
            }
        }

        private void OnDateLabelTapped(object sender, EventArgs e)
        {
            // Focus the DatePicker to show the calendar
            DatePicker.Focus();
        }

        private void OnDateSelected(object sender, DateChangedEventArgs e)
        {
            // Update the selected date
            currentDate = e.NewDate;
            UpdateDateLabel();

            // Hide the DatePicker
            DatePicker.IsVisible = false;

            // Update the DatePicker after selecting a date
            UpdateDatePicker();
        }

        private void UpdateDateLabel()
        {
            // Update the date label with the current date in Swedish with the first letter capitalized
            string formattedDate = currentDate.ToString("dddd, d MMMM yyyy", new System.Globalization.CultureInfo("sv-SE"));
            formattedDate = char.ToUpper(formattedDate[0]) + formattedDate.Substring(1);
            DateDisplayLabel.Text = formattedDate;
        }

        private void UpdateDatePicker()
        {
            DatePicker.Date = currentDate;
            DatePicker.MinimumDate = DateTime.Today.AddDays(0); //Not possible to look at previous bookings
            DatePicker.MaximumDate = DateTime.Today.AddDays(14); // Allow booking up to 14 days in advance
        }




    }
}