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
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using static Vixark.General;
using static SimpleOps.Global;



namespace SimpleOps.Modelo {



    /// <summary>
    /// Referencia propia del cliente asociada a un producto. Es de utilidad para realizar informes y agregarlas a las facturas o remisiones de entrega para facilidad para los clientes.
    /// </summary>
    [ControlInserción(ControlConcurrencia.Optimista)]
    class ReferenciaCliente : ReferenciaEntidadEconómica {


        #region Propiedades

        public Cliente? Cliente { get; set; } // Obligatorio.
        public int ClienteID { get; set; } // Clave foránea que con ProductoID forman la clave principal.

        #endregion Propiedades>


        #region Constructores

        private ReferenciaCliente() : base(null, null!) { } // Solo para que Entity Framework no saque error.

        public ReferenciaCliente(Producto producto, Cliente cliente, string referencia) : base(producto, referencia) 
            => (ClienteID, Cliente) = (cliente.ID, cliente);

        #endregion Constructores>


        #region Métodos y Funciones

        public override string ToString() => $"de {ATexto(Cliente, ClienteID)} para {ATexto(Producto, ProductoID)}";

        #endregion Métodos y Funciones>


    } // ReferenciaCliente>



} // SimpleOps.Modelo>
