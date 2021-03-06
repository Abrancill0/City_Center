﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Windows.Input;
using City_Center.Models;
using City_Center.Page;
using City_Center.Page.SlideMenu;
using City_Center.Services;
using GalaSoft.MvvmLight.Command;
using Xamarin.Forms;
using static City_Center.Models.EventosResultado;
using static City_Center.Models.TorneoResultado;
using City_Center.Clases;
using static City_Center.Models.TarjetaUsuarioResultado;
using Acr.UserDialogs;
using static City_Center.Models.PromocionesResultado;
using static City_Center.Models.RestaurantResultado;

namespace City_Center.ViewModels
{
    public class InicioViewModel : BaseViewModel
    {
        #region Services
        private ApiService apiService;
        #endregion

        #region Attributes
        private RestaurantReturn listRestaruant;
        private ObservableCollection<GastronomiaItemViewModel> restaurantDetalle;

        private PromocionesReturn listPromociones;
       
        private ObservableCollection<TorneoItemViewModel> torneoDetalle;

        private TarjetaUsuarioReturn listaTarjetausuario;
        private ObservableCollection<TarjetaUsuarioDetalle> tarjetaUsuarioDetalle;


        private ObservableCollection<PromocionesItemViewModel> promocionesDetalle;

        private string imagenTarjeta;
        private int puntosWin;
        private string noSocio;

        string fechaInicio;
        string horaInicio;
        string noPersonas;
        string nombreRestaurante;
        string sillaNiños;
        string nombre;
        string correo;
        string telefono;
        bool verTarjeta;
        bool muestraFlechas;
        bool muestraFlechasPromo;

        bool deshabilitaBoton;

        private string fechaShowInicio;
        private string fechaShowFin;
        #endregion

        #region Properties

        public ObservableCollection<GastronomiaItemViewModel> RestaurantDetalle
        {
            get { return this.restaurantDetalle; }
            set { SetValue(ref this.restaurantDetalle, value); }
        }

        public bool DeshabilitaBoton
        {
            get { return this.deshabilitaBoton; }
            set { SetValue(ref this.deshabilitaBoton, value); }
        }

        public string FechaShowInicio
        {
            get { return this.fechaShowInicio; }
            set { SetValue(ref this.fechaShowInicio, value); }
        }

        public string FechaShowFin
        {
            get { return this.fechaShowFin; }
            set { SetValue(ref this.fechaShowFin, value); }
        }


        public ObservableCollection<TorneoItemViewModel> TorneoDetalle
        {
            get { return this.torneoDetalle; }
            set { SetValue(ref this.torneoDetalle, value); }
        }

        public ObservableCollection<TarjetaUsuarioDetalle> TarjetaUsuarioDetalle
        {
            get { return this.tarjetaUsuarioDetalle; }
            set { SetValue(ref this.tarjetaUsuarioDetalle, value); }
        }

        public string FechaInicio
        {
            get { return this.fechaInicio; }
            set { SetValue(ref this.fechaInicio, value); }
        }

		public string HoraInicio
        {
            get { return this.horaInicio; }
            set { SetValue(ref this.horaInicio, value); }
        }

        public string NoPersonas
        {
            get { return this.noPersonas; }
            set { SetValue(ref this.noPersonas, value); }
        }

        public string NombreRestaurante
        {
            get { return this.nombreRestaurante; }
            set { SetValue(ref this.nombreRestaurante, value); }
        }

        public string SillaNiños
        {
            get { return this.sillaNiños; }
            set { SetValue(ref this.sillaNiños, value); }
        }

        public string Nombre
        {
            get { return this.nombre; }
            set { SetValue(ref this.nombre, value); }
        }

        public string Correo
        {
            get { return this.correo; }
            set { SetValue(ref this.correo, value); }
        }

        public string Telefono
        {
            get { return this.telefono; }
            set { SetValue(ref this.telefono, value); }
        }

        public string ImagenTarjeta
        {
            get { return this.imagenTarjeta; }
            set { SetValue(ref this.imagenTarjeta, value); }
        }

        public int PuntosWin
        {
            get { return this.puntosWin; }
            set { SetValue(ref this.puntosWin, value); }
        }

        public string NoSocio
        {
            get { return this.noSocio; }
            set { SetValue(ref this.noSocio, value); }
        }


        public bool VerTarjeta
        {
            get { return this.verTarjeta; }
            set { SetValue(ref this.verTarjeta, value); }
        }


        public bool MuestraFlechas
        {
            get { return this.muestraFlechas; }
            set { SetValue(ref this.muestraFlechas, value); }
        }

        public bool MuestraFlechasPromo
        {
            get { return this.muestraFlechasPromo; }
            set { SetValue(ref this.muestraFlechasPromo, value); }
        }

        public ObservableCollection<PromocionesItemViewModel> PromocionesDetalle
        {
            get { return this.promocionesDetalle; }
            set { SetValue(ref this.promocionesDetalle, value); }
        }

        #endregion

        #region Commands
        public ICommand ShowCommand
        {
            get
            {
                return new RelayCommand(TodoShows);
            }
        }

        private async void TodoShows()
        {
           MainViewModel.GetInstance().Torneo = new TorneoViewModel();

            ((MasterPage)Application.Current.MainPage).IsPresented = false;

            await ((MasterPage)Application.Current.MainPage).Detail.Navigation.PushAsync(new Torneos());
          
        }

        public ICommand ReservarCommand
        {
            get
            {
                return new RelayCommand(Reservar);
            }
        }

        private async void Reservar()
        {
            this.DeshabilitaBoton = false;

            if (this.NombreRestaurante == "Seleccionar...")
            {
                await Mensajes.Alerta("Campo restaurante es requerida");

                this.DeshabilitaBoton = true;

                return;
            }

            if (this.FechaInicio=="00/00/0000")
            {
                await Mensajes.Alerta("Campo fecha es requerida");
                this.DeshabilitaBoton = true;
                return;
            }


            if (this.HoraInicio == "00:00")
            {
                await Mensajes.Alerta("Campo hora es requerida");
                this.DeshabilitaBoton = true;
                return;
            }

          
            if (string.IsNullOrEmpty(this.Nombre))
            {
                await Mensajes.Alerta("Nombre y Apellido requerido");
                this.DeshabilitaBoton = true;
                return;
            }

            if (string.IsNullOrEmpty(this.Correo))
            {
                await Mensajes.Alerta("correo electrónico requerido");
                this.DeshabilitaBoton = true;
                return;
            }

            if (!ValidaEmailMethod.ValidateEMail(this.Correo))
            {
                await Mensajes.Alerta("Correo electrónico mal estructurado");
                return;
            }

            if (string.IsNullOrEmpty(this.Telefono))
            {
                await Mensajes.Alerta("Numero telefonico requerido");
                this.DeshabilitaBoton = true;
                return;
            }


            //string CuerpoMensaje = "Fecha:" + this.FechaInicio + "\n" +
                                   //"Hora: " + this.HoraInicio + "\n" +
                                   //"Personas: " + this.NoPersonas + "\n" +
                                   //"Restaurant: " + this.NombreRestaurante + "\n" +
                                   //"Silla para niños: " + this.SillaNiños + "\n" +
                                   //"Nombre y apellido: " + this.Nombre + "\n" +
                                   //"Correo electrónico: " + this.Correo + "\n" +
                                   //"Teléfono: " + this.Telefono;
   

			var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("nombre", this.Nombre), 
                new KeyValuePair<string, string>("email", this.Correo), 
                new KeyValuePair<string, string>("telefono", this.Telefono), 
                new KeyValuePair<string, string>("restaurant", this.NombreRestaurante),
                new KeyValuePair<string, string>("fecha", this.FechaInicio),
                new KeyValuePair<string, string>("hora", this.HoraInicio),
                new KeyValuePair<string, string>("personas", this.SillaNiños),
                new KeyValuePair<string, string>("silla_ninos", this.Telefono),
            });


            var response = await this.apiService.Get<GuardadoGenerico>("/es/gastronomia/reserva", "/correo", content);

                  
            if (!response.IsSuccess)
            {
                await Mensajes.Alerta("Ocurrio un error al enviar el correo");
                this.DeshabilitaBoton = true;
                return;
            }

            await Mensajes.Alerta("La información ha sido enviada correctamente");
            this.DeshabilitaBoton = true;
			this.FechaInicio = "00/00/0000";
			this.HoraInicio = "00:00";
            this.NoPersonas = "2";
            this.NombreRestaurante = "Seleccionar...";
            this.SillaNiños = "No";
            this.Correo = string.Empty;
            this.Telefono = string.Empty;
            this.Nombre = string.Empty;

        }

        public ICommand VerShowsFiltroCommand
        {
            get
            {
                return new RelayCommand(VerShowsFiltro);
            }
        }

        private async void VerShowsFiltro()
        {
            
            if (this.FechaShowInicio=="00/00/0000")
            {
              await  Mensajes.Alerta("Fecha inicio obligatoria para consulta de shows");
                return;
            }

            if (this.FechaShowFin == "00/00/0000")
            {
                await Mensajes.Alerta("Fecha final obligatoria para consulta de shows");
                return;
            }


            string Dia = this.FechaShowInicio.Substring(0, 2);
            string Mes = this.FechaShowInicio.Substring(3, 2);
            string Año = this.FechaShowInicio.Substring(6, 4);


            string Dia2 = this.FechaShowFin.Substring(0, 2);
            string Mes2 = this.FechaShowFin.Substring(3, 2);
            string Año2 = this.FechaShowFin.Substring(6, 4);


            VariablesGlobales.FechaShowInicio = Año + "-" + Mes + "-" + Dia;
            VariablesGlobales.FechaShowFinal = Año2 + "-" + Mes2 + "-" + Dia2;

            MainViewModel.GetInstance().Shows = new ShowsViewModel();

            ((MasterPage)Application.Current.MainPage).IsPresented = false;

            await ((MasterPage)Application.Current.MainPage).Detail.Navigation.PushAsync(new Show());


        }

        #endregion

        #region Methods
           
        private async void LoadTorneo()
        {
            try
            {
                var connection = await this.apiService.CheckConnection();

                if (!connection.IsSuccess)
                {
                    await Mensajes.Alerta("Parece que no tenés conexión a internet, intentalo mas tarde");

                    return;
                }

                string IDUsuario;

                try
                {
                    IDUsuario = Application.Current.Properties["IdUsuario"].ToString();
                }
                catch (Exception)
                {
                    IDUsuario = "";
                }


                var content = new FormUrlEncodedContent(new[]
                {
                new KeyValuePair<string, string>("usu_id", IDUsuario),
                });

                var response = await this.apiService.Get<TorneoReturn>("/casino/torneos", "/indexApp", content);

                if (!response.IsSuccess)
                {
                  //  await Mensajes.Alerta("Error al cargar Torneos");

                    return;
                }


                MainViewModel.GetInstance().listTorneo = (TorneoReturn)response.Result;

#if __ANDROID__
                TorneoDetalle = new ObservableCollection<TorneoItemViewModel>(this.ToTorneosItemViewModel2());
               #endif

#if __IOS__
                  TorneoDetalle = new ObservableCollection<TorneoItemViewModel>(this.ToTorneosItemViewModel());

#endif


                if (TorneoDetalle.Count>0)
                {
                    MuestraFlechas = true;
                    VariablesGlobales.RegistrosTorneo = TorneoDetalle.Count-1;
                    //VariablesGlobales.RegistrosTorneo = TorneoDetalle.Count;
                   
                }
                else
                {
                    MuestraFlechas = false;   
                }
            }
            catch (Exception)
            {
                MuestraFlechas = false; 
            }

           
        }

        private IEnumerable<TorneoItemViewModel> ToTorneosItemViewModel()
        {
            return MainViewModel.GetInstance().listTorneo.resultado.Select(l => new TorneoItemViewModel
            {
                tor_id = l.tor_id,
                tor_nombre = l.tor_nombre,
                tor_descripcion = l.tor_descripcion,
                tor_imagen = l.tor_imagen,
                tor_imagen_2 = l.tor_imagen_2,
                tor_fecha_hora_inicio = l.tor_fecha_hora_inicio,
                tor_fecha_hora_fin = l.tor_fecha_hora_fin,
                tor_destacado = l.tor_destacado,
                tor_id_usuario_creo = l.tor_id_usuario_creo,
                tor_fecha_hora_creo = l.tor_fecha_hora_creo,
                tor_id_usuario_modifico = l.tor_id_usuario_modifico,
                tor_fecha_hora_modifico = l.tor_fecha_hora_modifico,
                tor_estatus = l.tor_estatus,
                tor_guardado = l.tor_guardado,
                tor_id_guardado = l.tor_id_guardado,
                oculta = !(bool)l.tor_guardado
            });
        }
       
        private IEnumerable<TorneoItemViewModel> ToTorneosItemViewModel2()
        {
            return MainViewModel.GetInstance().listTorneo.resultado.Where(l =>l.tor_id>0).Select(l => new TorneoItemViewModel
            {
                tor_id = l.tor_id,
                tor_nombre = l.tor_nombre,
                tor_descripcion = l.tor_descripcion,
                tor_imagen = l.tor_imagen,
                tor_imagen_2 = l.tor_imagen_2,
                tor_fecha_hora_inicio = l.tor_fecha_hora_inicio,
                tor_fecha_hora_fin = l.tor_fecha_hora_fin,
                tor_destacado = l.tor_destacado,
                tor_id_usuario_creo = l.tor_id_usuario_creo,
                tor_fecha_hora_creo = l.tor_fecha_hora_creo,
                tor_id_usuario_modifico = l.tor_id_usuario_modifico,
                tor_fecha_hora_modifico = l.tor_fecha_hora_modifico,
                tor_estatus = l.tor_estatus,
                tor_guardado = l.tor_guardado,
                tor_id_guardado = l.tor_id_guardado,
                oculta = !(bool)l.tor_guardado
            });
        }

        private async void LoadTarjetaUsuario()
        {
            try
            {

                var connection = await this.apiService.CheckConnection();

                if (!connection.IsSuccess)
                {
                    //await Mensajes.Alerta("Parece que no tenés conexión a internet, intentalo mas tarde");
                    //probar abrham
                    //return;
                }

                string idusuario = Application.Current.Properties["IdUsuario"].ToString();

                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("usu_id",idusuario )
                });


                var response = await this.apiService.Get<TarjetaUsuarioReturn>("/tarjetas", "/tarjetaUsuario", content);

                if (!response.IsSuccess)
                {
                   // await Mensajes.Alerta("Error al cargar Tarjeta");

                    ImagenTarjeta = "";

                    PuntosWin = 0;
                    NoSocio = "";

                    ImagenTarjeta = "";
                    VerTarjeta = false;
                    PuntosWin = 0;
                    NoSocio = "";

                    return;
                }

                this.listaTarjetausuario = (TarjetaUsuarioReturn)response.Result;

                ImagenTarjeta = VariablesGlobales.RutaServidor + listaTarjetausuario.resultado.tar_imagen;

                PuntosWin = listaTarjetausuario.resultado.tar_puntos;
                NoSocio = listaTarjetausuario.resultado.tar_id;

                VerTarjeta = true;

            }
            catch (Exception)
            {
                //await Mensajes.Error(ex.ToString());

                ImagenTarjeta = "";
                VerTarjeta = false;
                PuntosWin = 0;
                NoSocio = "";
            }

        }

        private async void LoadPromociones()
        {
            try
            {
                var connection = await this.apiService.CheckConnection();

                if (!connection.IsSuccess)
                {
                   // await Mensajes.Alerta("Parece que no tenés conexión a internet, intentalo mas tarde");
                    //probar abrham
                  //  return;
                }

                var content = new FormUrlEncodedContent(new[]
                {
                new KeyValuePair<string, string>("", ""),
                });


                var response = await this.apiService.Get<PromocionesReturn>("/promociones", "/indexApp", content);

                if (!response.IsSuccess)
                {
                   // await Mensajes.Alerta("Error al cargar Promociones");

                    return;
                }

               // this.listPromociones = (PromocionesReturn)response.Result;
                MainViewModel.GetInstance().listPromociones = (PromocionesReturn)response.Result;

#if __ANDROID__
                PromocionesDetalle = new ObservableCollection<PromocionesItemViewModel>(this.ToPromocionesItemViewModel2());
#endif

#if __IOS__
                PromocionesDetalle = new ObservableCollection<PromocionesItemViewModel>(this.ToPromocionesItemViewModel());
#endif

                if (PromocionesDetalle.Count > 0)
                {
                    MuestraFlechasPromo = true;
                    VariablesGlobales.RegistrosPromociones = PromocionesDetalle.Count-1;

                }
                else
                {
                    MuestraFlechasPromo = false; 
                }
              



               // MuestraFlechasPromo
            }
            catch (Exception ex)
            {
                MuestraFlechasPromo = false;
                //await Mensajes.Error("Home - Promociones" + ex.ToString());
            }

        }

        private IEnumerable<PromocionesItemViewModel> ToPromocionesItemViewModel()
        {
            return MainViewModel.GetInstance().listPromociones.resultado.Select(l => new PromocionesItemViewModel
            {
                pro_id = l.pro_id,
                pro_id_evento = l.pro_id_evento,
                pro_id_locacion = l.pro_id_locacion,
                pro_nombre = l.pro_nombre,
                pro_descripcion = l.pro_descripcion,
                pro_imagen = l.pro_imagen,
                pro_imagen_2 = l.pro_imagen_2,
                pro_tipo_promocion = l.pro_tipo_promocion,
                pro_codigo = l.pro_codigo,
                pro_compartidos_codigo = l.pro_compartidos_codigo,
                pro_destacado = l.pro_destacado,
                pro_fecha_duracion_ini = l.pro_fecha_duracion_ini,
                pro_fecha_duracion_fin = l.pro_fecha_duracion_fin,
                pro_importe_decuento = l.pro_importe_decuento,
                pro_porcentaje_decuento = l.pro_porcentaje_decuento,
                pro_id_usuario_creo = l.pro_id_usuario_creo,
                pro_fecha_hora_creo = l.pro_fecha_hora_creo,
                pro_id_usuario_modifico = l.pro_id_usuario_modifico,
                pro_fecha_hora_modifico = l.pro_fecha_hora_modifico,
                pro_tipo = l.pro_tipo,
                pro_estatus = l.pro_estatus,
                loc_nombre = l.loc_nombre,
                pro_vinculo = l.pro_vinculo,
                pro_telefono = l.pro_telefono,
                pro_url = l.pro_url,
                pro_nombre_btn = l.pro_nombre_btn,
                pro_ejecutar_url = l.pro_ejecutar_url
            });
        }

        private IEnumerable<PromocionesItemViewModel> ToPromocionesItemViewModel2()
        {
            return MainViewModel.GetInstance().listPromociones.resultado.Where(l => l.pro_id > 0).Select(l => new PromocionesItemViewModel
            {
                pro_id = l.pro_id,
                pro_id_evento = l.pro_id_evento,
                pro_id_locacion = l.pro_id_locacion,
                pro_nombre = l.pro_nombre.ToUpper(),
                pro_descripcion = l.pro_descripcion,
                pro_imagen = l.pro_imagen,
                pro_imagen_2 = l.pro_imagen_2,
                pro_tipo_promocion = l.pro_tipo_promocion,
                pro_codigo = l.pro_codigo,
                pro_compartidos_codigo = l.pro_compartidos_codigo,
                pro_destacado = l.pro_destacado,
                pro_fecha_duracion_ini = l.pro_fecha_duracion_ini,
                pro_fecha_duracion_fin = l.pro_fecha_duracion_fin,
                pro_importe_decuento = l.pro_importe_decuento,
                pro_porcentaje_decuento = l.pro_porcentaje_decuento,
                pro_id_usuario_creo = l.pro_id_usuario_creo,
                pro_fecha_hora_creo = l.pro_fecha_hora_creo,
                pro_id_usuario_modifico = l.pro_id_usuario_modifico,
                pro_fecha_hora_modifico = l.pro_fecha_hora_modifico,
                pro_tipo = l.pro_tipo,
                pro_estatus = l.pro_estatus,
                loc_nombre = l.loc_nombre
            });
        }


        private async void LoadRestaurantes()
        {
            try
            {
                var connection = await this.apiService.CheckConnection();

                if (!connection.IsSuccess)
                {
                    await Mensajes.Alerta("Verificá tu conexión a Internet");

                    return;
                }


                var content = new FormUrlEncodedContent(new[]
                {
                new KeyValuePair<string, string>("", ""),
                });


                var response = await this.apiService.GetReal<RestaurantReturn>("/gastronomia", "/obtenerRestaurantBar", content);

                if (!response.IsSuccess)
                {
                    //await Mensajes.Alerta("Error al cargar Restaurantes/Bar");

                    return;
                }

                this.listRestaruant = (RestaurantReturn)response.Result;

                RestaurantDetalle = new ObservableCollection<GastronomiaItemViewModel>(this.ToRestaurantItemViewModel());

                int contador = 0;

              foreach (var item in RestaurantDetalle)
                {
                    VariablesGlobales.ArregloRestaurantes[contador] = item.reb_nombre;

                    contador = contador + 1;
                }
                VariablesGlobales.NumeroArreglo = contador-1;


            }
            catch (Exception ex)
            {
             
            }

        }

        private IEnumerable<GastronomiaItemViewModel> ToRestaurantItemViewModel()
        {
            return this.listRestaruant.resultado.Where(l => l.reb_reservas==1).Select(l => new GastronomiaItemViewModel
            {
                reb_id = l.reb_id,
                reb_nombre = l.reb_nombre.ToUpper(),
                reb_descripcion = l.reb_descripcion,
                reb_descripcion_horario = l.reb_descripcion_horario,
                reb_ver_hotel_spa = l.reb_ver_hotel_spa,
                reb_reservas = l.reb_reservas,
                reb_tipo = l.reb_tipo,
                reb_imagen_1 = l.reb_imagen_1,
                reb_imagen_2 = l.reb_imagen_2,
                reb_imagen_3 = l.reb_imagen_3,
                reb_imagen_4 = l.reb_imagen_4,
                reb_id_usuario_creo = l.reb_id_usuario_creo,
                reb_fecha_hora_creo = l.reb_fecha_hora_creo,
                reb_id_usuario_modifico = l.reb_id_usuario_modifico,
                reb_fecha_hora_modifico = l.reb_fecha_hora_modifico,
                reb_estatus = l.reb_estatus,
            });
        }

        #endregion

        #region Contructors
        public InicioViewModel()
        {
            this.apiService = new ApiService();

           
            this.LoadPromociones();
            this.LoadTorneo();
            this.LoadTarjetaUsuario();

            this.LoadRestaurantes();

            this.FechaInicio = String.Format("{0:dd/MM/yyyy}", DateTime.Today);// "00/00/0000";
			this.HoraInicio = "00:00";

            this.FechaShowInicio = String.Format("{0:dd/MM/yyyy}", DateTime.Today);
            this.fechaShowFin = String.Format("{0:dd/MM/yyyy}", DateTime.Today.AddDays(1));
			this.NombreRestaurante = "Seleccionar";
			this.SillaNiños = "No";

            DeshabilitaBoton = true;

        }
        #endregion
    }
}
