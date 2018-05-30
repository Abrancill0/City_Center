﻿using System;
using System.Collections.Generic;

using Xamarin.Forms;
using City_Center.Clases;
using City_Center.ViewModels;

namespace City_Center.Page
{
    public partial class SeleccionTipoChat : ContentPage
    {
        public SeleccionTipoChat()
        {
            InitializeComponent();
        }

      async  void ChatCasino(object sender, System.EventArgs e)
        {

            if (string.IsNullOrEmpty(Application.Current.Properties["RutaChatCasino"].ToString()))
            {
                Application.Current.Properties["Casino"] = 1;
            }
            else
            {
                Application.Current.Properties["Casino"] = 0;  
            }

            await Application.Current.SavePropertiesAsync();

            MainViewModel.GetInstance().Chat = new ChatviewModel();

            VariablesGlobales.TipoChat = "casino";
            await((MasterPage)Application.Current.MainPage).Detail.Navigation.PushAsync(new Chat());
  
        }

        async  void ChatHotel(object sender, System.EventArgs e)
        {

            if  (string.IsNullOrEmpty(Application.Current.Properties["RutaChatHotel"].ToString()))
            {
                Application.Current.Properties["Hotel"] = 1;  
            }
            else
            {
                Application.Current.Properties["Hotel"] = 0;
            }

            await Application.Current.SavePropertiesAsync();
        
            VariablesGlobales.TipoChat = "hotel";
            await((MasterPage)Application.Current.MainPage).Detail.Navigation.PushAsync(new Chat());
 
        }
    }
}
