﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SimpleOps.Datos;
using SimpleOps.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static SimpleOps.Global;
using static Vixark.General;
using static SimpleOps.Pruebas;
using static SimpleOps.Configuración;
using SimpleOps.Interfaz;
using System.IO;
using SimpleOps.Integración;



namespace SimpleOps.Interfaz {



    #pragma warning disable CA1724 // La mejor palabra para esta ventana es Principal, aunque se también se podría usar Central pero no es tan clara. El conflicto de nombre lo presenta con el espacio de nombres System.Security.Principal que por el momento no se espera usar, entonces omitir esta advertencia no es algo que genere problemas.
    public partial class Principal : Window {
    #pragma warning restore CA1724


        readonly ILogger Rastreador = FábricaRastreadores.CreateLogger<Principal>();

        Integrador? Integrador { get; set; } // Se necesita el objeto público siempre activo durante la ejecución de la aplicación para que pueda interceptar los cambios de archivos en la carpeta de integración.


        /// <summary>
        /// Ventana principal.
        /// </summary>
        public Principal() {

            #region Iniciaciones

            FinalizarSiExisteOtraInstanciaAbierta(NombreAplicación);
            InitializeComponent();
            Rastreador.LogInformation("Iniciando");
            IniciarVariablesGenerales();
            IniciarVariablesGlobales();
            CargarOpciones();
            ConfigurarCarpetasYArchivos();
            OperacionesEspecialesDatos = false; // Se debe establecer en falso después de cargar las opciones.

            #endregion Iniciaciones>


            #region Modos Especiales
            #pragma warning disable CS0162 // Se detectó código inaccesible. Se omite la advertencia porque las variables de este bloque que son constantes pueden ser modificado por el usuario del código en Configuración.cs.

            if (HabilitarRastreoDeDatosSensibles) 
                LblAlerta.Content = $"Está habilitado el rastreo de datos sensibles. Esta función se debe desactivar en producción.{NuevaLínea}";

            if (ModoIntegraciónTerceros) {

                Visibility = Visibility.Visible;
                Integrador = new Integrador();
                if (Integrador.Iniciado) {
                    LblAlerta.Content += $"Está habilitado el modo de integración con programas terceros que permite facturar " +
                                         $"electrónicamente y generar catálogos desde otro programa.";
                } else {
                    LblAlerta.Content += $"Sucedió un error habilitando el modo de integración con programas terceros.";
                }
                
            }         

            if (string.IsNullOrEmpty(LblAlerta.Content?.ToString())) LblAlerta.Visibility = Visibility.Collapsed;

            #pragma warning restore CS0162
            #endregion Modos Especiales>


            #region Inicio de Variables de Estado Globales       
            using var ctx = new Contexto(TipoContexto.Lectura);
            Contexto.LeerMunicipiosDeInterés(ctx);
            #endregion Inicio de Variables de Estado Globales>

            #region Enlace de Datos a Interfaz
            var ordenesCompraPendientes = ctx.ObtenerOrdenesCompraPendientes();
            var aleatorio = new Random();
            ordenesCompraPendientes.ForEach(oc => oc.EstadoOrdenCompra = (EstadoOrdenCompra)aleatorio.Next(0, 6));
            if (ordenesCompraPendientes.Any()) ordenesCompraPendientes.First().EstadoOrdenCompra = EstadoOrdenCompra.PendientePedido;
            DataContext = ordenesCompraPendientes;
            // DataContext = ctx.Clientes.Find(aleatorio.Next(1, 100));
            #endregion Enlace de Datos a Interfaz>

            // LeerBaseDatosCompleta();

            #pragma warning disable CS0162 // Se detectó código inaccesible. Se omite la advertencia porque HabilitarPruebasUnitarias puede ser modificado por el usuario del código en Configuración.cs.
            if (false) { // Poner en verdadero para realizar pruebas.

                DocumentosElectrónicos(); // Prueba para ensayar todos los procedimientos relacionados con la facturación electrónica. No genera facturas electrónicas, las pruebas para generar facturas electrónicas se realizan con un botón.
                // IntegraciónAplicacionesTerceros(); // Esta prueba se usa cuando se quiere simular el comportamiento de un programa tercero que genera archivos de comunicación .json con SimpleOps para el modo de integración de facturación electrónica. Si ya se dispone de un programa tercero generando correctamente los archivos, no es necesario activar esta línea.

            } else { _ = 0; } // Solo se usa esta línea para que no saque advertencia de supresión de CS0162 innecesaria.
            #pragma warning restore CS0162 

        } // Principal>


        #region Eventos


        void CargarDatosIniciales_Clic(object sender, RoutedEventArgs e) {

            Rastreador.LogInformation("CargarDatosIniciales_Clic");

            MostrarInformación("A continuación se cargarán los datos iniciales. Este proceso puede tardar varios minutos");

            var éxito = Contexto.CargarDatosIniciales(ObtenerRutaDatosJson(), out string error);
            if (éxito) {
                MostrarInformación("Se cargaron exitosamente los datos iniciales.");
            } else {
                MostrarError(error);
            }

        } // CargarDatosIniciales_Clic>


        void ReiniciarBaseDatosSQLite_Clic(object sender, RoutedEventArgs e) {

            Rastreador.LogInformation("ReiniciarBaseDatosSQLite_Clic");

            var rutaCopiasSeguridad = ObtenerRutaCopiasSeguridad();
            if (MostrarDiálogo($"Se moverá la base de datos actual a {rutaCopiasSeguridad} y se iniciará una base de datos nueva vacía.{DobleLínea}" +
                               $"¿Deseas continuar?", "¿Reiniciar Base de Datos?", MessageBoxButton.YesNo) == MessageBoxResult.Yes) {

                var nuevaRuta = System.IO.Path.Combine(rutaCopiasSeguridad, ArchivoBaseDatosSQLite.Reemplazar(".db", " [" + AhoraNombresArchivos + "].db"));
                File.Move(RutaBaseDatosSQLite, nuevaRuta);
                File.Copy(RutaBaseDatosVacíaSQLite, RutaBaseDatosSQLite);
                MostrarInformación("Se ha reiniciado la base de datos SQLite exitósamente.");

            }

        } // ReiniciarBaseDatosSQLite_Clic>


        private void PruebasHabilitaciónFacturación_Clic(object sender, RoutedEventArgs e) => Habilitación();

        private void ObtenerClaveTécnicaProducción_Clic(object sender, RoutedEventArgs e) =>
            MostrarInformación($"La clave técnica es:{DobleLínea}{Legal.Dian.ObtenerClaveTécnicaAmbienteProducción()}");

        private void HacerPruebasInternasFacturación_Clic(object sender, RoutedEventArgs e) => Facturación(pruebaHabilitación: false);

        private void GenerarCatálogo_Clic(object sender, RoutedEventArgs e) => GeneraciónCatálogo();

        #endregion Eventos>


    } // Principal>



} // SimpleOps.Interfaz>
