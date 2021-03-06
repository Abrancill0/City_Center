﻿using System;
using System.Collections.Generic;
using City_Center.Clases;
using Xamarin.Forms;
using System.Threading.Tasks;
using System.Net.Http;
using static City_Center.Models.MensajesPendientesResultado;

namespace City_Center.Page
{
    public partial class ChatIos : ContentPage
    {
        public ChatIos()
        {
            InitializeComponent();

            VariablesGlobales.ChatPresente = 1;

            #if __IOS__
            NavigationPage.SetTitleIcon(this, "logo@x2.png");

            Task.Delay(800);

            string Nombre = Application.Current.Properties["NombreCompleto"].ToString();
            string Email = Application.Current.Properties["Email"].ToString();
            string TipoChat = VariablesGlobales.TipoChat;
            string n = Convert.ToString(Int64.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
            string VariableChat = "chat_" + TipoChat + "_" + n;

            if (VariablesGlobales.TipoChat == "casino")
            {

                if (Application.Current.Properties["Casino"].ToString() == "1")
                {
                    Application.Current.Properties["VariableChatCasino"] = VariableChat;

                    var uri = new Uri("https://citycenter-rosario.com.ar/chat_app/" + TipoChat + "/" + VariableChat + "/" + Nombre + "/" + Email + "/" + Application.Current.Properties["Casino"].ToString());

                    var nsurl = new Foundation.NSUrl(uri.GetComponents(UriComponents.HttpRequestUrl, UriFormat.UriEscaped));

                    WebViewChat.Source = nsurl.ToString();

                    var uri1 = new Uri("https://citycenter-rosario.com.ar/chat_app/" + TipoChat + "/" + VariableChat + "/" + Nombre + "/" + Email + "/" + "0");

                    var nsurl1 = new Foundation.NSUrl(uri1.GetComponents(UriComponents.HttpRequestUrl, UriFormat.UriEscaped));

                    Application.Current.Properties["RutaChatCasino"] = nsurl1.ToString();
                        
                    Application.Current.Properties["Casino"] = "0";
                }
                else
                {
                    var uri = new Uri(Application.Current.Properties["RutaChatCasino"].ToString());

                    var nsurl = new Foundation.NSUrl(uri.GetComponents(UriComponents.HttpRequestUrl, UriFormat.UriEscaped));

                    WebViewChat.Source = nsurl.ToString();
                }


            }
            else if (VariablesGlobales.TipoChat == "hotel")
            {

                if (Application.Current.Properties["Hotel"].ToString() == "1")
                {

                    Application.Current.Properties["VariableChatHotel"] = VariableChat;

                    var uri = new Uri("https://citycenter-rosario.com.ar/chat_app/" + TipoChat + "/" + VariableChat + "/" + Nombre + "/" + Email + "/" + Application.Current.Properties["Hotel"].ToString());

                    var nsurl = new Foundation.NSUrl(uri.GetComponents(UriComponents.HttpRequestUrl, UriFormat.UriEscaped));

                    WebViewChat.Source = nsurl.ToString();


                    var uri1 = new Uri("https://citycenter-rosario.com.ar/chat_app/" + TipoChat + "/" + VariableChat + "/" + Nombre + "/" + Email + "/" + "0");

                    var nsurl1 = new Foundation.NSUrl(uri1.GetComponents(UriComponents.HttpRequestUrl, UriFormat.UriEscaped));


                    Application.Current.Properties["RutaChatHotel"] = nsurl1.ToString();

                    Application.Current.Properties["Hotel"] = "0";
                }
                else
                {
                    var uri = new Uri(Application.Current.Properties["RutaChatHotel"].ToString());

                    var nsurl = new Foundation.NSUrl(uri.GetComponents(UriComponents.HttpRequestUrl, UriFormat.UriEscaped));

                    WebViewChat.Source = nsurl.ToString();
                }


            }

            Application.Current.SavePropertiesAsync();

            #endif
        }

        void Handle_Clicked(object sender, System.EventArgs e)
        {
            #if __IOS__

            if (VariablesGlobales.TipoChat == "casino")
            {
                var uri = new Uri("https://citycenter-rosario.com.ar/chat/terminar_chat_app/" + Application.Current.Properties["VariableChatCasino"].ToString());

                var nsurl = new Foundation.NSUrl(uri.GetComponents(UriComponents.HttpRequestUrl, UriFormat.UriEscaped));

                WebViewChat1.Source = nsurl.ToString();
                    
                Application.Current.Properties["RutaChatCasino"] = "";
                //Application.Current.Properties["VariableChatCasino"] = "";
                Application.Current.Properties["Casino"] = 1;

            }
            else if (VariablesGlobales.TipoChat == "hotel")
            {
                var uri = new Uri("https://citycenter-rosario.com.ar/chat/terminar_chat_app/" + Application.Current.Properties["VariableChatHotel"].ToString());

                var nsurl = new Foundation.NSUrl(uri.GetComponents(UriComponents.HttpRequestUrl, UriFormat.UriEscaped));

                WebViewChat1.Source = nsurl.ToString();
                    
                //Application.Current.Properties["VariableChatHotel"] = "";
                Application.Current.Properties["RutaChatHotel"] = "";

                Application.Current.Properties["Hotel"] = 1;

            }

            Application.Current.SavePropertiesAsync();

            Task.Delay(1000);

            Navigation.PopAsync();

            #endif
        }

        void Handle_Clicked_1(object sender, System.EventArgs e)
        {
            #if __IOS__

            if (VariablesGlobales.TipoChat == "casino")
            {

                var uri = new Uri(Application.Current.Properties["RutaChatCasino"].ToString() + "/" + Mensajito.Text);

                var nsurl = new Foundation.NSUrl(uri.GetComponents(UriComponents.HttpRequestUrl, UriFormat.UriEscaped));

                WebViewChat1.Source = nsurl.ToString();

                Mensajito.Text = string.Empty;

            }
            else if (VariablesGlobales.TipoChat == "hotel")
            {

                var uri = new Uri(Application.Current.Properties["RutaChatHotel"].ToString() + "/" + Mensajito.Text);

                var nsurl = new Foundation.NSUrl(uri.GetComponents(UriComponents.HttpRequestUrl, UriFormat.UriEscaped));

                WebViewChat1.Source = nsurl.ToString();

                Mensajito.Text = string.Empty;
            }
            #endif
        }
    
        protected async override void OnDisappearing()
        {
            base.OnDisappearing();

            VariablesGlobales.ChatPresente = 0;

            if (VariablesGlobales.TipoChat == "casino" == true)
            {

                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("ccn_chat", Application.Current.Properties["VariableChatCasino"].ToString()),
                    new KeyValuePair<string, string>("ccn_email", Application.Current.Properties["Email"].ToString())
                 });


                Restcliente Mensajitos = new Restcliente();

                var response = await Mensajitos.Get<MensajesPendientesReturn>("/chat/marcar_visto_mensaje_web", content);

                if (VariablesGlobales.MensajeVisto == 1)
                {
                    GlobalResources.Current.ImagenChat = "chat@2x";

                }
            }
            else if (VariablesGlobales.TipoChat == "hotel")
            {


                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("ccn_chat", Application.Current.Properties["VariableChatHotel"].ToString()),
                    new KeyValuePair<string, string>("ccn_email", Application.Current.Properties["Email"].ToString())
                });


                Restcliente Mensajitos = new Restcliente();

                var response = await Mensajitos.GetReal<MensajesPendientesReturn>("/chat/marcar_visto_mensaje_web/", content);
                if (VariablesGlobales.MensajeVisto == 1)
                {

                    GlobalResources.Current.ImagenChat = "chat@2x";
                }
            }

        }
    
    }
}
