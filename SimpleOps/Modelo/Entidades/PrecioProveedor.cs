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
    /// Último precio de un producto dado por un proveedor.
    /// </summary>
    [ControlInserción(ControlConcurrencia.Optimista)]
    class PrecioProveedor : Precio {


        #region Propiedades

        public Proveedor? Proveedor { get; set; } // Obligatorio.
        public int ProveedorID { get; set; } // Clave foránea que con ProductoID forman la clave principal.

        #endregion Propiedades>


        #region Constructores

        private PrecioProveedor() : base(null, 0) { } // Solo para que Entity Framework no saque error.

        public PrecioProveedor(Producto producto, Proveedor proveedor, decimal valor) : base(producto, valor)
            => (ProveedorID, Proveedor) = (proveedor.ID, proveedor);

        #endregion Constructores>


        #region Métodos y Funciones

        public override string ToString() => $"de {ATexto(Producto, ProductoID)} por {ATexto(Proveedor, ProveedorID)}";

        #endregion Métodos y Funciones>


    } // PrecioProveedor>



} // SimpleOps.Modelo>
