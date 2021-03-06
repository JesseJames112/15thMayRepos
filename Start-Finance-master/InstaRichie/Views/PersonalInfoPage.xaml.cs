﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Popups;
using StartFinance.Models;
using SQLite.Net;
// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace StartFinance.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PersonalInfoPage : Page
    {
        SQLiteConnection conn; // adding an SQLite connection
        string path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "Findata.sqlite");

        public PersonalInfoPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            /// Initializing a database
            conn = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path);

            // Creating table
            Results();
        }

        public void Results()
        {
            // Creating table
            conn.CreateTable<PersonalInfo>();
            var query = conn.Table<PersonalInfo>();
            TransactionList.ItemsSource = query.ToList();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Results();
        }

        private async void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // checks if account name is null
                if (txtFirstName.Text.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("Input First Name", "Oops..!");
                    await dialog.ShowAsync();
                }
                else if (txtLastName.Text.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("Input Last Name", "Oops..!");
                    await dialog.ShowAsync();
                }
                else if (txtDOB.Text.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("Input DOB", "Oops..!");
                    await dialog.ShowAsync();
                }
                else if (txtGender.Text.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("Input Gender", "Oops..!");
                    await dialog.ShowAsync();
                }
                else if (txtEmail.Text.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("Input Email", "Oops..!");
                    await dialog.ShowAsync();
                }
                else if (txtMobilePhone.Text.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("Input Mobile Phone", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {   // Inserts the data
                    conn.Insert(new PersonalInfo()
                    {
                        FirstName = txtFirstName.Text,
                        LastName = txtLastName.Text,
                        DOB = txtDOB.Text,
                        Gender = txtGender.Text,
                        Email = txtEmail.Text,
                        MobilePhone = txtMobilePhone.Text,
                        //OverDraft = Drafting()
                    });
                    Results();
                }

            }
            catch (Exception ex)
            {   // Exception to display when amount is invalid or not numbers
                if (ex is FormatException)
                {
                    MessageDialog dialog = new MessageDialog("You forgot to enter the Amount or entered an invalid data", "Oops..!");
                    await dialog.ShowAsync();
                }   // Exception handling when SQLite contraints are violated
                else if (ex is SQLiteException)
                {
                    MessageDialog dialog = new MessageDialog("Account Name already exist, Try Different Name", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {
                    /// no idea
                }

            }
        }

        private async void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            MessageDialog ShowConf = new MessageDialog("Deleting this Personal Info will delete all transactions of this account", "Important");
            ShowConf.Commands.Add(new UICommand("Yes, Delete")
            {
                Id = 0
            });
            ShowConf.Commands.Add(new UICommand("Cancel")
            {
                Id = 1
            });
            ShowConf.DefaultCommandIndex = 0;
            ShowConf.CancelCommandIndex = 1;

            var result = await ShowConf.ShowAsync();
            if ((int)result.Id == 0)
            {
                // checks if data is null else inserts
                try
                {
                    string FirstName = ((PersonalInfo)TransactionList.SelectedItem).FirstName;
                    string LastName = ((PersonalInfo)TransactionList.SelectedItem).LastName;
                    var querydel = conn.Query<PersonalInfo>("DELETE FROM PersonalInfo WHERE FirstName='" + FirstName + "' AND LastName='" + LastName + "'");
                    Results();

                    //conn.CreateTable<Transactions>();
                    //var querytable = conn.Query<Transactions>("DELETE FROM Transactions WHERE Account='" + AccountsLabel + "'");
                }
                catch (NullReferenceException)
                {
                    MessageDialog ClearDialog = new MessageDialog("Please select the item to Delete", "Oops..!");
                    await ClearDialog.ShowAsync();
                }
            }
            else
            {
                //
            }
        }
    }
}
