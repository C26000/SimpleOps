﻿// Copyright Notice:
//
// SimpleOps® is a free ERP software for small businesses and independents.
// Copyright© 2021 Vixark (vixark@outlook.com).
// For more information about SimpleOps®, see https://simpleops.net.
//
// This program is free software: you can redistribute it and/or modify it under the terms of the GNU Affero
// General Public License as published by the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY, without even the
// implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public
// License for more details.
//
// You should have received a copy of the GNU Affero General Public License along with this program. If not,
// see https://www.gnu.org/licenses.
//
// This License does not grant permission to use the trade names, trademarks, service marks, or product names
// of the Licensor, except as required for reasonable and customary use in describing the origin of the Work
// and reproducing the content of the NOTICE file.
//
// Removing or changing the above text is not allowed.
//

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using static Vixark.General;
using static SimpleOps.Global;



namespace SimpleOps.Modelo {


    /// <summary>
    /// Sucursal, oficina o bodega de un cliente.
    /// </summary>
    [ControlInserción(ControlConcurrencia.Optimista)]
    class Sede : Actualizable { // Es Actualizable porque todos sus datos pueden actualizarse, pero no se hace Rastreable porque no es de mucho interés conocer la fecha de creación y es una tabla que puede crecer mucho.


        #region Propiedades

        [Key]
        public int ID { get; set; }

        /// <summary>
        /// Nombre identificador. Único por cada Cliente.
        /// </summary>
        /// <MaxLength>50</MaxLength>
        [MaxLength(50)] 
        public string Nombre { get; set; } = null!; // Obligatorio.

        public Cliente? Cliente { get; set; } // Obligatorio.
        public int ClienteID { get; set; }

        /// <summary>
        /// Si se asocia la sede a un contacto se le podrá notificar el progreso del envío.
        /// </summary>
        public Contacto? Contacto { get; set; } // Opcional.
        public int? ContactoID { get; set; }

        /// <summary>
        /// Observaciones de envío que se escribirán en las etiquetas de envío.
        /// </summary>
        /// <MaxLength>500</MaxLength>
        [MaxLength(500)] 
        public string? ObservacionesEnvío { get; set; }
    
        [NotMapped]
        public DirecciónCompleta? DirecciónCompleta { get; set; }


        public int MunicipioID { get; set; }
        public Municipio? _Municipio;
        [ForeignKey("MunicipioID")]
        public Municipio? Municipio { // Obligatorio.
            get => _Municipio;
            set {
                _Municipio = value;
                DirecciónCompleta = DirecciónCompleta.CrearDirecciónCompleta(_Municipio, _Dirección);
            }
        }


        public string _Dirección = null!;
        /// <MaxLength>100</MaxLength>
        [MaxLength(100)]
        public string Dirección { // Obligatorio.
            get => _Dirección;
            set {
                _Dirección = value;
                DirecciónCompleta = DirecciónCompleta.CrearDirecciónCompleta(_Municipio, _Dirección);
            }
        }


        #endregion Propiedades>


        #region Constructores

        private Sede() { } // Solo para que Entity Framework no saque error.

        public Sede(string nombre, Cliente cliente, string dirección, Municipio municipio)
            => (Nombre, ClienteID, MunicipioID, Dirección, Cliente, Municipio) = (nombre, cliente.ID, municipio.ID, dirección, cliente, municipio);

        #endregion Constructores>


        #region Métodos y Funciones

        public override string ToString() => $"{Nombre} de {ATexto(Cliente, ClienteID)}";

        #endregion Métodos y Funciones>


    } // Sede>



} // SimpleOps.Modelo>
