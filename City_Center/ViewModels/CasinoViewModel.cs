﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Windows.Input;
using City_Center.Page;
using City_Center.Services;
using GalaSoft.MvvmLight.Command;
using Xamarin.Forms;
using static City_Center.Models.PozosResultado;
using static City_Center.Models.DestacadosResultado;
using static City_Center.Models.SalaPokerResultado;
using static City_Center.Models.GanadoresResultado;
using static City_Center.Models.PromocionesWinResultado;
using static City_Center.Models.EventosResultado;
using static City_Center.Models.TarjetasResultado;
using static City_Center.Models.PromocionesResultado;
using City_Center.Clases;
using static City_Center.Models.TorneoResultado;
using Plugin.Share;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using Plugin.Permissions.Abstractions;
using System.Globalization;
using Plugin.Permissions;
using static City_Center.Models.TarjetaValidaResultado;
using Plugin.DeviceInfo;

namespace City_Center.ViewModels
{
    public class CasinoViewModel : BaseViewModel
    {
        #region Services
        private ApiService apiService;
        #endregion

        #region Attributes

        private PozosReturn listPozos;
        private SalaPokerReturn listSalasPoker;
        private GanadoresReturn listGanadores;
        private ObservableCollection<DestacadosItemViewModel> destacadosDetalle;
        private ObservableCollection<pozosDetalle> pozosDetalle;
        private ObservableCollection<SalaPokerDetalle> salaPokerDetalle;
        private ObservableCollection<GanadoresDetalle> ganadoresDetalle;
        private ObservableCollection<PromocionesWinDetalle> promocionesWinDetalle;

        private PromocionesReturn listPromociones;
        private ObservableCollection<TorneoItemViewModel> torneoDetalle;

        //private EventosReturn list;
        private ObservableCollection<EventosItemViewModel> eventosDetalle;

        private TarjetasReturn listTarjetas;
        private ObservableCollection<TarjetasDetalle> tarjetasDetalle;


        private ObservableCollection<PromocionesItemViewModel> promocionesDetalle;

        private string imagen_Selected;

        private int tamanoGanadores;

        private int tamanoPozos;

        private int tamanoTarjeta;

        bool muestraFlechasPromo = false;

        bool muestraFlechas = false;

        bool muestraFlechaDestacado = false;

        bool muestraTarjetasIOS;
        bool muestraTarjetasAndroid;

        #endregion

        #region Properties

        public bool MuestraTarjetasIOS
        {
            get { return this.muestraTarjetasIOS; }
            set { SetValue(ref this.muestraTarjetasIOS, value); }
        }
        public bool MuestraTarjetasAndroid
        {
            get { return this.muestraTarjetasAndroid; }
            set { SetValue(ref this.muestraTarjetasAndroid, value); }
        }



        public ObservableCollection<DestacadosItemViewModel> DestacadosDetalle
        {
            get { return this.destacadosDetalle; }
            set { SetValue(ref this.destacadosDetalle, value); }
        }

        public ObservableCollection<pozosDetalle> PozosDetalle
        {
            get { return this.pozosDetalle; }
            set { SetValue(ref this.pozosDetalle, value); }
        }

        public ObservableCollection<SalaPokerDetalle> SalaPokerDetalle
        {
            get { return this.salaPokerDetalle; }
            set { SetValue(ref this.salaPokerDetalle, value); }
        }

        public ObservableCollection<GanadoresDetalle> GanadoresDetalle
        {
            get { return this.ganadoresDetalle; }
            set { SetValue(ref this.ganadoresDetalle, value); }
        }

        public ObservableCollection<PromocionesWinDetalle> PromocionesWinDetalle
        {
            get { return this.promocionesWinDetalle; }
            set { SetValue(ref this.promocionesWinDetalle, value); }
        }

        public string Imagen_Selected
        {
            get { return this.imagen_Selected; }
            set { SetValue(ref this.imagen_Selected, value); }
        }

        public int TamanoGanadores
        {
            get { return this.tamanoGanadores; }
            set { SetValue(ref this.tamanoGanadores, value); }
        }

        public ObservableCollection<EventosItemViewModel> EventosDetalle
        {
            get { return this.eventosDetalle; }
            set { SetValue(ref this.eventosDetalle, value); }
        }

        public ObservableCollection<TarjetasDetalle> TarjetasDetalle
        {
            get { return this.tarjetasDetalle; }
            set { SetValue(ref this.tarjetasDetalle, value); }
        }

        public ObservableCollection<PromocionesItemViewModel> PromocionesDetalle
        {
            get { return this.promocionesDetalle; }
            set { SetValue(ref this.promocionesDetalle, value); }
        }

        public ObservableCollection<TorneoItemViewModel> TorneoDetalle
        {
            get { return this.torneoDetalle; }
            set { SetValue(ref this.torneoDetalle, value); }
        }

        public int TamanoPozos
        {
            get { return this.tamanoPozos; }
            set { SetValue(ref this.tamanoPozos, value); }
        }

        public int TamanoTarjeta
        {
            get { return this.tamanoTarjeta; }
            set { SetValue(ref this.tamanoTarjeta, value); }
        }

        public bool MuestraFlechasPromo
        {
            get { return this.muestraFlechasPromo; }
            set { SetValue(ref this.muestraFlechasPromo, value); }
        }


        public bool MuestraFlechas
        {
            get { return this.muestraFlechas; }
            set { SetValue(ref this.muestraFlechas, value); }
        }

        public bool MuestraFlechaDestacado
        {
            get { return this.muestraFlechaDestacado; }
            set { SetValue(ref this.muestraFlechaDestacado, value); }
        }

        #endregion

        #region Commands

        public ICommand UbicacionCasinoCommand
        {
            get
            {
                return new RelayCommand(UbicacionCasino);
            }
        }

        private async void UbicacionCasino()
        {
            try
            {
                Plugin.Share.Abstractions.ShareMessage Compartir = new Plugin.Share.Abstractions.ShareMessage();


                var permissionStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Location);

                if (permissionStatus == PermissionStatus.Denied)
                {
                    var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Location);

                    if (results.ContainsKey(Permission.Location))
                    {
                        if (permissionStatus != PermissionStatus.Granted)
                        {
                            await Mensajes.Alerta("Ubicación denegeada, por favor activa el GPS de tu dispositivo");

                            return;
                        }
                    }
                }


                //var hasPermission = await Utils.CheckPermissions(Permission.Location);
                //if (!hasPermission)
                //return;


                var Posicion = await Ubicacion.GetCurrentPosition();

                Compartir.Text = "Ubicacion Actual";
                Compartir.Title = "Tu ubicacion";
                Compartir.Url = "https://www.google.com/maps/@" + Posicion.Latitude + "," + Posicion.Longitude + "," + "16z";

                await CrossShare.Current.Share(Compartir);
            }
            catch (Exception)
            {
                await Mensajes.Alerta("Ubicación denegeada, por favor activa el GPS de tu dispositivo");
            }

        }

        public ICommand VerRecompensasCommand
        {
            get
            {
                return new RelayCommand(VerRecompensas);
            }
        }

        private async void VerRecompensas()
        {
            MainViewModel.GetInstance().RecompesasWin = new RecompesasWinViewModel();

            ((MasterPage)Application.Current.MainPage).IsPresented = false;

            await ((MasterPage)Application.Current.MainPage).Detail.Navigation.PushAsync(new RecompensasWin());

        }

        public ICommand PokerCommand
        {
            get
            {
                return new RelayCommand(Poker);
            }
        }

        private void Poker()
        {
            if (this.listGanadores == null)
            {
                this.LoadGanadores();
            }

            if (this.listSalasPoker == null)
            {
                this.LoadSalaPoker();
            }

            if (this.TorneoDetalle == null)
            {
                this.LoadTorneo();
            }

        }

        public ICommand WinCommand
        {
            get
            {
                return new RelayCommand(Win);
            }
        }

        private void Win()
        {
            if (this.listTarjetas == null)
            {
                this.LoadTarjetas();
            }

        }


        public ICommand ConsultaPuntosCommand
        {
            get
            {
                return new RelayCommand(ConsultaPuntos);
            }
        }

        public async void ConsultaPuntos()
        {
            bool isLoggedIn = Application.Current.Properties.ContainsKey("IsLoggedIn") ?
                                    (bool)Application.Current.Properties["IsLoggedIn"] : false;

            if (isLoggedIn)
            {

                string Nosocio = Application.Current.Properties["NumeroSocio"].ToString();


                if (Nosocio == "" || Nosocio == "0")
                {
                    await Mensajes.Alerta("No cuenta con una tarjeta asociada, pódes vincularla desde tu perfil");
                    return;
                }

                // var content = new FormUrlEncodedContent(new[]
                //{
                //    new KeyValuePair<string, string>("usu",Application.Current.Properties["IdUsuario"].ToString()),
                //   new KeyValuePair<string, string>("tarjeta",Nosocio )
                //});

                //var response = await this.apiService.Get<TarjetaValidaReturn>("/es/register", "/valida_tarjeta_socio", content);

                //if (!response.IsSuccess)
                //{
                //    await Mensajes.Alerta("No se cuenta con ninguna tarjeta asociada, pódes vincular tu tarjeta desde tu perfil");
                //    return;
                //}


                App.NavPage.BarBackgroundColor = Color.FromHex("#23144B");

                MainViewModel.GetInstance().ConsultaTarjetaWin = new ConsultaTarjetaWinViewModel();

                await ((MasterPage)Application.Current.MainPage).Detail.Navigation.PushAsync(new ConsultaTarjetaWin());
            }
            else
            {
                await Mensajes.Alerta("Iniciá sesión para poder consultar tus puntos y recompensas");
            }

        }


        #endregion

        #region Methods
        private async void LoadDestacados()
        {
            try
            {
                var connection = await this.apiService.CheckConnection();

                if (!connection.IsSuccess)
                {
                    await Mensajes.Alerta("Verificá tu conexión a Internet");
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


                var response = await this.apiService.Get<DestacadosReturn>("/casino/destacados", "/indexApp", content);

                if (!response.IsSuccess)
                {
                    //await Mensajes.Error("Error al cargar Destacados");

                    return;
                }

                MainViewModel.GetInstance().listDestacados = (DestacadosReturn)response.Result;

#if __ANDROID__
                DestacadosDetalle = new ObservableCollection<DestacadosItemViewModel>(this.ToDestacadosItemViewModel2());
#endif

#if __IOS__
                 DestacadosDetalle = new ObservableCollection<DestacadosItemViewModel>(this.ToDestacadosItemViewModel());
#endif

                if (DestacadosDetalle.Count > 0)
                {
                    MuestraFlechaDestacado = true;
                    VariablesGlobales.RegistrosCasinoDestacados = DestacadosDetalle.Count - 1;
                }
                else
                {
                    MuestraFlechaDestacado = false;
                }

            }
            catch (Exception)
            {
                MuestraFlechaDestacado = false;
                //await Mensajes.Error("Casino - Destacados" + ex.ToString());
            }


        }

        private IEnumerable<DestacadosItemViewModel> ToDestacadosItemViewModel()
        {
            return MainViewModel.GetInstance().listDestacados.resultado.Select(l => new DestacadosItemViewModel
            {
                des_imagen = VariablesGlobales.RutaServidor + l.des_imagen,
                des_imagen_2 = VariablesGlobales.RutaServidor + l.des_imagen_2,
                des_descripcion = l.des_descripcion,
                des_fecha_hora_inicio = l.des_fecha_hora_inicio,
                des_nombre = l.des_nombre,
                des_id = l.des_id,
                des_guardado = l.des_guardado,
                des_id_guardado = l.des_id_guardado,
                des_link = l.des_link,
                des_telefono = l.des_telefono,
                oculta = !(bool)l.des_guardado,
                ocultallamada = (string.IsNullOrEmpty(l.des_telefono) ? false : true),
                ocultaonline = (string.IsNullOrEmpty(l.des_link) ? false : true),
            });
        }

        private IEnumerable<DestacadosItemViewModel> ToDestacadosItemViewModel2()
        {
            return MainViewModel.GetInstance().listDestacados.resultado.Where(l => l.des_id > 0).Select(l => new DestacadosItemViewModel
            {
                des_imagen = VariablesGlobales.RutaServidor + l.des_imagen,
                des_imagen_2 = VariablesGlobales.RutaServidor + l.des_imagen_2,
                des_descripcion = l.des_descripcion,
                des_fecha_hora_inicio = l.des_fecha_hora_inicio,
                des_nombre = l.des_nombre,
                des_id = l.des_id,
                des_guardado = l.des_guardado,
                des_id_guardado = l.des_id_guardado,
                des_link = l.des_link,
                des_telefono = l.des_telefono,
                oculta = !(bool)l.des_guardado,
                ocultallamada = (string.IsNullOrEmpty(l.des_telefono) ? false : true),
                ocultaonline = (string.IsNullOrEmpty(l.des_link) ? false : true),
            });
        }

        private async void LoadPozos()
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


                var response = await this.apiService.Get<PozosReturn>("/casino/pozos", "/indexApp", content);

                if (!response.IsSuccess)
                {
                    //await Mensajes.Error("Error al cargar Pozos");

                    return;
                }

                this.listPozos = (PozosReturn)response.Result;

                PozosDetalle = new ObservableCollection<pozosDetalle>(this.ToPozosItemViewModel());

                int contador = PozosDetalle.Count;

                switch (contador)
                {
                    case 1:
                        TamanoPozos = 125;
                        break;
                    case 2:
                        TamanoPozos = 125;
                        break;

                    case 3:
                        TamanoPozos = 125;
                        break;

                    case 4:
                        TamanoPozos = 250;
                        break;

                    case 5:
                        TamanoPozos = 250;
                        break;

                    case 6:
                        TamanoPozos = 250;
                        break;

                    case 7:
                        TamanoPozos = 375;
                        break;

                    case 8:
                        TamanoPozos = 375;
                        break;

                    case 9:
                        TamanoPozos = 375;
                        break;

                    case 10:
                        TamanoPozos = 500;
                        break;

                    case 11:
                        TamanoPozos = 500;
                        break;
                    case 12:
                        TamanoPozos = 500;
                        break;
                }

                TamanoPozos = TamanoPozos + 20;

            }
            catch (Exception)
            {
                //await Mensajes.Error("Casino - Pozos" + ex.ToString());
            }

        }

        private IEnumerable<pozosDetalle> ToPozosItemViewModel()
        {
            return this.listPozos.resultado.Select(l => new pozosDetalle
            {
                poz_imagen = VariablesGlobales.RutaServidor + l.poz_imagen,
                poz_monto = l.poz_monto,
                poz_descripcion = l.poz_descripcion,
                poz_fecha_entrega = l.poz_fecha_entrega

            });
        }

        private async void LoadSalaPoker()
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


                var response = await this.apiService.Get<SalaPokerReturn>("/casino/sala_poker", "/indexApp", content);

                if (!response.IsSuccess)
                {

                    //await Mensajes.Error("Error al cargar Sala de Poker");
                    return;
                }

                this.listSalasPoker = (SalaPokerReturn)response.Result;


                Imagen_Selected = VariablesGlobales.RutaServidor + this.listSalasPoker.resultado[0].spo_imagen;


                SalaPokerDetalle = new ObservableCollection<SalaPokerDetalle>(this.ToSalaPokerItemViewModel());

            }
            catch (Exception ex)
            {
                //await Mensajes.Error("Casino - Sala Poker" + ex.ToString());
            }

        }

        private IEnumerable<SalaPokerDetalle> ToSalaPokerItemViewModel()
        {
            return this.listSalasPoker.resultado.Select(l => new SalaPokerDetalle
            {
                spo_id = l.spo_id,
                spo_descripcion = l.spo_descripcion,
                spo_imagen = VariablesGlobales.RutaServidor + l.spo_imagen,
                spo_id_usuario_creo = l.spo_id_usuario_creo,
                spo_fecha_hora_creo = l.spo_fecha_hora_creo,
                spo_id_usuario_modifico = l.spo_id_usuario_modifico,
                spo_fecha_hora_modifico = l.spo_fecha_hora_modifico,
                spo_estatus = l.spo_estatus,
                spo_eliminado = l.spo_eliminado
            });
        }

        private async void LoadGanadores()
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


                var response = await this.apiService.Get<GanadoresReturn>("/casino/ganadores", "/indexApp", content);

                if (!response.IsSuccess)
                {
                    //await Mensajes.Error("Error al cargar Ganadores");

                    return;
                }

                this.listGanadores = (GanadoresReturn)response.Result;

                GanadoresDetalle = new ObservableCollection<GanadoresDetalle>(this.ToGanadoresViewModel());

                int contador = GanadoresDetalle.Count;

                switch (contador)
                {
                    case 1:
                        TamanoGanadores = 125;
                        break;
                    case 2:
                        TamanoGanadores = 125;
                        break;

                    case 3:
                        TamanoGanadores = 125;
                        break;

                    case 4:
                        TamanoGanadores = 250;
                        break;

                    case 5:
                        TamanoGanadores = 250;
                        break;

                    case 6:
                        TamanoGanadores = 250;
                        break;

                    case 7:
                        TamanoGanadores = 375;
                        break;

                    case 8:
                        TamanoGanadores = 375;
                        break;

                    case 9:
                        TamanoGanadores = 375;
                        break;

                    case 10:
                        TamanoGanadores = 500;
                        break;

                    case 11:
                        TamanoGanadores = 500;
                        break;
                    case 12:
                        TamanoGanadores = 500;
                        break;
                }

            }
            catch (Exception ex)
            {
                //await Mensajes.Error("Casino - Ganadores" + ex.ToString());
            }


        }

        private IEnumerable<GanadoresDetalle> ToGanadoresViewModel()
        {
            return this.listGanadores.resultado.Select(l => new GanadoresDetalle
            {
                gan_id = l.gan_id,
                gan_nombre = l.gan_nombre,
                gan_premio = l.gan_premio,
                gan_imagen = VariablesGlobales.RutaServidor + l.gan_imagen,
                gan_id_usuario_creo = l.gan_id_usuario_creo,
                gan_fecha_hora_creo = l.gan_fecha_hora_creo,
                gan_id_usuario_modifico = l.gan_id_usuario_modifico,
                gan_fecha_hora_modifico = l.gan_fecha_hora_modifico,
                gan_estatus = l.gan_estatus,
                gan_eliminado = l.gan_eliminado

            });
        }

        private async void LoadTarjetas()
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


                var response = await this.apiService.Get<TarjetasReturn>("/tarjetas", "/indexApp", content);

                if (!response.IsSuccess)
                {

                    //await Mensajes.Error("Error al cargar Tarjetas");

                    return;
                }

                this.listTarjetas = (TarjetasReturn)response.Result;

                TarjetasDetalle = new ObservableCollection<TarjetasDetalle>(this.ToTarjetasViewModel());

                int contador = TarjetasDetalle.Count;

#if __IOS__
                TamanoTarjeta = contador * 120;
#endif


#if __ANDROID__
                TamanoTarjeta = contador * 140;
#endif


                this.MuestraTarjetasAndroid = false;
                this.MuestraTarjetasIOS = false;

#if __IOS__
                this.MuestraTarjetasIOS = true;
#endif

#if __ANDROID__
                this.MuestraTarjetasAndroid = true;
#endif

            }
            catch (Exception ex)
            {
                //await Mensajes.Error("Casino - Tarjetas" + ex.ToString());
            }

        }

        private IEnumerable<TarjetasDetalle> ToTarjetasViewModel()
        {
            return this.listTarjetas.resultado.Select(l => new TarjetasDetalle
            {
                tar_id = l.tar_id,
                tar_nombre = l.tar_nombre,
                tar_descripcion = l.tar_descripcion,
                tar_imagen = VariablesGlobales.RutaServidor + l.tar_imagen,
                tar_id_usuario_creo = l.tar_id_usuario_creo,
                tar_fecha_hora_creo = l.tar_fecha_hora_creo,
                tar_id_usuario_modifico = l.tar_id_usuario_modifico,
                tar_fecha_hora_modifico = l.tar_fecha_hora_modifico,
                tar_estatus = l.tar_estatus,
            });
        }

        private void LoadTorneo()
        {
            try
            {

#if __ANDROID__
                TorneoDetalle = new ObservableCollection<TorneoItemViewModel>(this.ToTorneosItemViewModel2());
#endif

#if __IOS__
                TorneoDetalle = new ObservableCollection<TorneoItemViewModel>(this.ToTorneosItemViewModel());
#endif

                if (TorneoDetalle.Count > 0)
                {
                    VariablesGlobales.RegistrosCasinoTorneo = TorneoDetalle.Count - 1;
                    MuestraFlechas = true;
                }
                else
                {
                    MuestraFlechas = false;
                }

            }
            catch (Exception)
            {
                MuestraFlechas = false;

                //await Mensajes.Error("Casino - Torneos" + ex.ToString());
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
            return MainViewModel.GetInstance().listTorneo.resultado.Where(l => l.tor_id > 0).Select(l => new TorneoItemViewModel
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

        private async void LoadPromociones()
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
                    new KeyValuePair<string, string>("pro_tipo", "cas"),
                });


                var response = await this.apiService.Get<PromocionesReturn>("/promociones", "/indexTipoApp", content);

                if (!response.IsSuccess)
                {
                    //await Mensajes.Error("Error al cargar Promociones");

                    return;
                }

                this.listPromociones = (PromocionesReturn)response.Result;

                //var test = MainViewModel.GetInstance().listPromociones;
#if __ANDROID__
                PromocionesDetalle = new ObservableCollection<PromocionesItemViewModel>(this.ToPromocionesItemViewModel2());
#endif

#if __IOS__
                PromocionesDetalle = new ObservableCollection<PromocionesItemViewModel>(this.ToPromocionesItemViewModel());
#endif
                if (PromocionesDetalle.Count > 0)
                {
                    MuestraFlechasPromo = true;

                    VariablesGlobales.RegistrosCasinoPromociones = PromocionesDetalle.Count-1;
                }
                else
                {
                    MuestraFlechasPromo = false;
                }
            
            }
			catch (Exception)
			{
                MuestraFlechasPromo = false;
			}

		}

		private IEnumerable<PromocionesItemViewModel> ToPromocionesItemViewModel()
		{
            return this.listPromociones.resultado.Select(l => new PromocionesItemViewModel
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
            return this.listPromociones.resultado.Where(l => l.pro_id > 0).Select(l => new PromocionesItemViewModel
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

        //return this.listPromociones.resultado.Where(l => l.pro_id > 0).Select(l => new PromocionesItemViewModel


#endregion

#region Contructors
        public CasinoViewModel()
		{
			this.apiService = new ApiService();

			this.LoadDestacados();
			
			this.LoadPromociones();
			this.LoadPozos();
			this.LoadSalaPoker();
			this.LoadTarjetas();


		}


#endregion
	}
}

