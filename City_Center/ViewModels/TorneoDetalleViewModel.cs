﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Windows.Input;
using City_Center.Clases;
using City_Center.Models;
using City_Center.Page;
using City_Center.Services;
using GalaSoft.MvvmLight.Command;
using Plugin.Share;
using Xamarin.Forms;
using static City_Center.Models.TorneoResultado;

namespace City_Center.ViewModels
{
	public class TorneoDetalleViewModel:BaseViewModel
    {
        #region Attributes
        private ApiService apiService;

		private string nombre;
		private string correo;
		private string tipoDocumento;
		private string numeroDocumento;
		private DateTime fecha;
		private string nacionalidad;
		private string pais;
		private string provincia;
		private string ciudad;
        
        #endregion

		#region Properties
        public TorneoDetalle td
        {
            get;
            set;
        }
        
		public string Correo
        {
			get { return this.correo; }
			set { SetValue(ref this.correo, value); }
        }

        public string Nombre
        {
            get { return this.nombre; }
            set { SetValue(ref this.nombre, value); }
        }
  
		public string TipoDocumento
        {
			get { return this.tipoDocumento; }
			set { SetValue(ref this.tipoDocumento, value); }
        }

		public string NumeroDocumento
        {
			get { return this.numeroDocumento; }
			set { SetValue(ref this.numeroDocumento, value); }
        }

		public string Nacionalidad
        {
			get { return this.nacionalidad; }
			set { SetValue(ref this.nacionalidad, value); }
        }

        public DateTime Fecha
        {
            get { return this.fecha; }
            set { SetValue(ref this.fecha, value); }
        }

		public string Pais
        {
			get { return this.pais; }
			set { SetValue(ref this.pais, value); }
        }


		public string Provincia
        {
			get { return this.provincia; }
			set { SetValue(ref this.provincia, value); }
        }

		public string Ciudad
        {
			get { return this.ciudad; }
			set { SetValue(ref this.ciudad, value); }
        }

        #endregion



        #region Command
        public ICommand CompartirCommand
        {
            get
            {
                return new RelayCommand(CompartirUrl);
            }
        }

        private async void CompartirUrl()
        {
            Plugin.Share.Abstractions.ShareMessage Compartir = new Plugin.Share.Abstractions.ShareMessage();

            Compartir.Text = td.tor_descripcion;
            Compartir.Title = td.tor_nombre;
            Compartir.Url = "http://cc.comprogapp.com/es/casino/torneo-detail/" + td.tor_id+ "/" + td.tor_nombre;

            await CrossShare.Current.Share(Compartir);

        }

        public ICommand GuardaFavoritoCommand
        {
            get
            {
                return new RelayCommand(GuardaFavorito);
            }
        }

        private async void GuardaFavorito()
        {
            try
            {

                bool isLoggedIn = Application.Current.Properties.ContainsKey("IsLoggedIn") ?
                                     (bool)Application.Current.Properties["IsLoggedIn"] : false;

                if (isLoggedIn)
                {
                    var content = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("gua_id_usuario", Application.Current.Properties["IdUsuario"].ToString()),
                        new KeyValuePair<string, string>("gua_id_torneo", Convert.ToString(td.tor_id)),
                    });

                    var response = await this.apiService.Get<GuardadoGenerico>("/guardados", "/store", content);

                    if (!response.IsSuccess)
                    {
                        await Mensajes.Error("Error al guardar Guardados");
                        return;
                    }

                    var list = (GuardadoGenerico)response.Result;




                    await Mensajes.success("Guardado Correctamente");

                }
                else
                {
                    await Mensajes.Info("Inicia Sesion para guardar este torneo");
                }
            }
            catch (Exception)
            {
                await Mensajes.Info("Inicia Sesion para guardar este torneo");
            }

        }
        
        public ICommand EnviaCorreoCommand
		{
			get
			{
				return new RelayCommand(EnviaCorreo);	
			}
		}

        private async void EnviaCorreo()
		{
         
			if (string.IsNullOrEmpty(Nombre))
			{
			await Mensajes.Info("Nombre Requerido");
				return;
			}

			if (string.IsNullOrEmpty(Correo))
            {
                await Mensajes.Info("Correo Requerido");
                return;
            }

			if (string.IsNullOrEmpty(NumeroDocumento))
            {
				await    Mensajes.Info("Numero de documento Requerido");
                return;
            }

			if (string.IsNullOrEmpty(Nacionalidad))
            {
				await  Mensajes.Info("Nacionalidad Requerido");
                return;
            }

			if (string.IsNullOrEmpty(Provincia))
            {
				await  Mensajes.Info("Provincia Requerido");
                return;
            }

			if (string.IsNullOrEmpty(TipoDocumento))
            {
				await  Mensajes.Info("Tipo de documento Requerido");
                return;
            }

			if (string.IsNullOrEmpty(Pais))
            {
				await  Mensajes.Info("Pais Requerido");
                return;
            }

			if (string.IsNullOrEmpty(Ciudad))
            {
				await  Mensajes.Info("Ciudad Requerido");
                return;
            }

			var content = new FormUrlEncodedContent(new[]
            {
				new KeyValuePair<string, string>("email", Correo), 
				new KeyValuePair<string, string>("tor_nombre", td.tor_nombre),            
				new KeyValuePair<string, string>("nombre", Convert.ToString(Nombre)),            
				new KeyValuePair<string, string>("numero_documento", NumeroDocumento),
				new KeyValuePair<string, string>("nacionalidad", Nacionalidad),
				new KeyValuePair<string, string>("provincia", Provincia),
				new KeyValuePair<string, string>("tipo_de_documento", TipoDocumento),
				new KeyValuePair<string, string>("fecha_nac", Convert.ToString(Fecha.ToString("dd-MM-yyyy"))),
				new KeyValuePair<string, string>("pais", Pais),
				new KeyValuePair<string, string>("ciudad", Ciudad)

            });


			var response = await this.apiService.Get<GuardadoGenerico>("/casino/torneos", "/registro_torneo", content);

            if (!response.IsSuccess)
            {
                await Mensajes.Error(response.Message);
				return;
            }

            await Mensajes.success("Correo enviado exitosamente");

			Correo = string.Empty;
            Nombre=string.Empty;         
			NumeroDocumento=string.Empty;
			Nacionalidad=string.Empty;
			Provincia = string.Empty;
			TipoDocumento =string.Empty;
			pais =string.Empty;
			Ciudad =string.Empty;
            
		}
              
        #endregion
      
        public TorneoDetalleViewModel(TorneoDetalle td)
        {
            this.apiService = new ApiService();
            this.td = td;

			Fecha = DateTime.Today;

        }
    }
}
