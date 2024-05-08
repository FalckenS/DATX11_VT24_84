using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DATX11_VT24_84
{
    public partial class MainPage : IRefreshable
    {
        private const string UserID = "1";
        private List<string> _allRooms;
        
        public MainPage()
        {   
            InitializeComponent();
            AddTopTriangles();
            UpdateReservationCards();
            GetAllRoomSuggestions();
        }

        private async void GetAllRoomSuggestions()
        {
            _allRooms = await BackEnd.GetAllRoomNames();
            SuggestionList.ItemsSource = _allRooms;
        }
        
        private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            List<string> listOfSuggestions = _allRooms
                .Where(s => s.ToLower().Contains(e.NewTextValue.ToLower()))
                .ToList();
            
            // Filter the list of suggestions based on the search text
            SuggestionList.ItemsSource = listOfSuggestions;

            // Show or hide the suggestions list depending on whether there is a search term
            SuggestionList.IsVisible = (listOfSuggestions.Count != 0) && !string.IsNullOrEmpty(e.NewTextValue);
        }

        private void SuggestionList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                SearchBar.Text = e.SelectedItem as string;
                SuggestionList.IsVisible = false;
            }
        }

        private void AddTopTriangles()
        {
            // Av okänd anledning verkar SizeChanged vara det enda sättet att få korrekt Width och Height
            SizeChanged += (sender, e) =>
            {
                UIUtility.AddTopTriangles(MainLayout, Width, Height);
            };
        }
        
        private async void OnLedigaJustNuButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new LedigaJustNu(), false);
        }
        
        private async void OnBokaButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new Boka(), false);
        }
        
        private async void OnMinaBokningarButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new MinaBokningar(this), false);
        }
        
        private async void UpdateReservationCards()
        {
            ActivityIndicator.IsVisible = true;
            ActivityIndicator.IsRunning = true;
            
            ReservationTitle.IsVisible = false;
            ReservationCard1.IsVisible = false;
            ReservationCard2.IsVisible = false;
            
            List<Reservation> ongoingReservations = await BackEnd.GetOngoingReservationsForUser(UserID);
            List<Reservation> upcomingReservationsThisDay = 
                (await BackEnd.GetUpcomingReservationsForUser(UserID)).Where(reservation =>
                    GetRealTime().Year == reservation.StartTime.Year &&
                    GetRealTime().Month == reservation.StartTime.Month &&
                    GetRealTime().Day == reservation.StartTime.Day).ToList();
            
            if (ongoingReservations.Count >= 2)
            {
                ReservationTitle.IsVisible = true;
                
                AddReservationCard(ongoingReservations[0], ReservationCard1Room, ReservationCard1Building,
                    ReservationCard1Time, ReservationCard1TimeUntil, ReservationCard1);
                AddReservationCard(ongoingReservations[1], ReservationCard2Room, ReservationCard2Building,
                    ReservationCard2Time, ReservationCard2TimeUntil, ReservationCard2);

                ReservationCard1.IsVisible = true;
                ReservationCard2.IsVisible = true;
            }
            
            else if (ongoingReservations.Count == 1 && upcomingReservationsThisDay.Count >= 1)
            {
                ReservationTitle.IsVisible = true;
                
                AddReservationCard(ongoingReservations[0], ReservationCard1Room, ReservationCard1Building,
                    ReservationCard1Time, ReservationCard1TimeUntil, ReservationCard1);
                AddReservationCard(upcomingReservationsThisDay[0], ReservationCard2Room, ReservationCard2Building,
                    ReservationCard2Time, ReservationCard2TimeUntil, ReservationCard2);
                
                ReservationCard1.IsVisible = true;
                ReservationCard2.IsVisible = true;
            }
            else if (ongoingReservations.Count == 1 && upcomingReservationsThisDay.Count == 0)
            {
                ReservationTitle.IsVisible = true;
                
                AddReservationCard(ongoingReservations[0], ReservationCard1Room, ReservationCard1Building,
                    ReservationCard1Time, ReservationCard1TimeUntil, ReservationCard1);
                
                ReservationCard1.IsVisible = true;
            }
            
            else if (upcomingReservationsThisDay.Count >= 2)
            {
                ReservationTitle.IsVisible = true;
                
                AddReservationCard(upcomingReservationsThisDay[0], ReservationCard1Room, ReservationCard1Building,
                    ReservationCard1Time, ReservationCard1TimeUntil, ReservationCard1);
                AddReservationCard(upcomingReservationsThisDay[1], ReservationCard2Room, ReservationCard2Building,
                    ReservationCard2Time, ReservationCard2TimeUntil, ReservationCard2);
                
                ReservationCard1.IsVisible = true;
                ReservationCard2.IsVisible = true;
            }
            else if (upcomingReservationsThisDay.Count == 1)
            {
                ReservationTitle.IsVisible = true;
                
                AddReservationCard(upcomingReservationsThisDay[0], ReservationCard1Room, ReservationCard1Building,
                    ReservationCard1Time, ReservationCard1TimeUntil, ReservationCard1);
                
                ReservationCard1.IsVisible = true;
            }
            ActivityIndicator.IsVisible = false;
            ActivityIndicator.IsRunning = false;
        }
        
        private async void AddReservationCard(Reservation reservation, Label roomLabel, Label buildingLabel,
            Label timeLabel, Label timeUntilLabel, Frame reservationCard)
        {
            roomLabel.Text = reservation.RoomName;
            buildingLabel.Text = (await BackEnd.GetRoomInfo(reservation.RoomName)).Building;
            timeLabel.Text =
                reservation.StartTime.ToString("HH:mm") + " - " + reservation.EndTime.ToString("HH:mm");
            TimeSpan timeUntilReservation = reservation.StartTime - GetRealTime();
            if (timeUntilReservation.Ticks > 0 && timeUntilReservation.Hours >= 1)
            {
                timeUntilLabel.Text = "Om " + timeUntilReservation.Hours + " h";
            }
            else if (timeUntilReservation.Ticks > 0 && timeUntilReservation.Minutes > 0)
            {
                timeUntilLabel.Text = "Om " + timeUntilReservation.Minutes + " min";
            }
            else
            {
                timeUntilLabel.Text = "Pågående";
            }
            
            // Gör kortet clickable
            TapGestureRecognizer tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += async (s, e) =>
            {
                await Navigation.PushModalAsync(new Bokning(reservation, this), false);
            };
            reservationCard.GestureRecognizers.Add(tapGestureRecognizer);
        }
        
        public void RefreshData()
        {
            MainLayout.Children.Remove(ReservationCard1);
            MainLayout.Children.Remove(ReservationCard2);
            UpdateReservationCards();
        }

        private static DateTime GetRealTime()
        {
            // Vintertid: +1
            // Sommartid: +2
            return DateTime.Now.AddHours(BackEnd.HourDifference);
        }

       
    }
}