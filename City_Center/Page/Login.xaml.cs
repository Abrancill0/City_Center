﻿using City_Center.Clases;
using City_Center.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using Xamarin.Forms;

namespace City_Center.Page
{
    public partial class Login : ContentPage
    {
        public Login()
        {
            InitializeComponent();
        }

        private  void showPass(object sender, EventArgs e)
        {
            TextContraseña.IsPassword = false;
        }

        private  void Pass(object sender, EventArgs e)
        {
            TextContraseña.IsPassword = true;
        }
    }
}

