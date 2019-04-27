﻿using FlightSimulator.Model;
using FlightSimulator.Views.Windows;
using System;
using System.ComponentModel;
using System.Threading;
using System.Windows.Input;

namespace FlightSimulator.ViewModels
{
    public class FlightBoardViewModel : BaseNotify
    {
        private Settings settingsChild = new Settings(); // settings window

        private FlightBoardModel model;

        public event PropertyChangedEventHandler PropertyChanged;

        public double Lon { get; set; }

        public double Lat { get; set; }

        public FlightBoardViewModel()
        {
            model = new FlightBoardModel(InfoServer.Instance);
            model.PropertyChanged += delegate (Object sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName == "Lat") Lat = model.Lat;
                else if (e.PropertyName == "Lon") Lon = model.Lon;
                NotifyPropertyChanged(e.PropertyName);
            };

        }

        #region Setting Command
        private ICommand settingsCommand; // Settings command for settings button
        public ICommand SettingsCommand { get { return settingsCommand ?? (settingsCommand = new CommandHandler(() => OnSttingsClick())); } }

        // Load settings window
        void OnSttingsClick()
        {
            // Allow to create only one settings window:
            if (!settingsChild.IsLoaded)
            {
                settingsChild = new Settings();
                settingsChild.Show();
            }
            else settingsChild.Show();
        }

        #endregion

        #region Connect Command
        private ICommand connectsCommand; // Settings command for settings button
        public ICommand ConnectsCommand { get { return connectsCommand ?? (connectsCommand = new CommandHandler(() => OnConnectClick())); } }

        void OnConnectClick()
        {
            if (model.IsSimulatorConnected()) // if there is a connection, establish new connections to info and commands
            {
                model.StopGetInfo();
                CommandsClient.Instance.Initialize();
                System.Threading.Thread.Sleep(1000); // let info server finish last read
            }
            new Thread(delegate ()
            {
                CommandsClient.Instance.Connect(ApplicationSettingsModel.Instance.FlightServerIP, ApplicationSettingsModel.Instance.FlightCommandPort); // conect to simulator
            }).Start();
            model.OpenServer(ApplicationSettingsModel.Instance.FlightServerIP, ApplicationSettingsModel.Instance.FlightInfoPort); // open info server


        }

        #endregion
        public void NotifyPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
