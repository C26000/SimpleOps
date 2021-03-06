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
using System.Text;
using static SimpleOps.Global;



namespace SimpleOps.Modelo {



    /// <summary>
    /// Cuando se usan <see cref="ProductoBase"/>, los atributos sirven para diferenciar productos que comparten el mismo producto base entre si.
    /// </summary>
    [ControlInserción(ControlConcurrencia.Optimista)]
    class AtributoProducto : Rastreable { // Es una tabla usualmente pequeña. No hay problema en hacerla Rastreable.


        #region Propiedades

        [Key]
        public int ID { get; set; }

        /// <summary>
        /// Como el nombre del atributo se usa para diferenciar productos específicos entre sí en los documentos gráficos, este debe ser descriptivo 
        /// por si mismo sin necesidad del tipo de atributo. Por ejemplo, un nombre de atributo correcto es "Talla 10", en vez de "10", otro 
        /// atributo correcto sería "Rayas Rojas", en vez de "Rojas". 
        /// </summary>
        /// <MaxLength>50</MaxLength>
        [MaxLength(50)]
        public string Nombre { get; set; } = null!; // Obligatorio y único. Se exige que el atributo sea autodescriptivo y único para simplificar la manera en la que se guardan los atributos en la base de datos al evitar agregar una estructura compleja de datos en la columna atributos. Al tener una estructura de lista simple de atributos, permite realizar una edición manual de estos valores más cómoda. Además, al exigir unicidad, se puede relacionar directamente un atributo con su tipo y se permite que los atributos se usen directamente en las descripciones autogeneradas de los productos específicos sin necesidad de adjuntarles el tipo ni generar lógicas adicionales. En algunas aplicaciones es necesario extraer el valor más puntual sin la palabra diferenciadora, por ejemplo extraer "Rojas" de "Rayas Rojas" del atributo de tipo "Color Rayas", en este caso se implementa una lógica para obtener solo las palabras que no están repetidas en el tipo de atributo obteniendo así el valor individual para ser usado por ejemplo en un selector del atributo que tenga por título "Color Rayas" y valores "Rojas", "Verdes" y "Azules".

        public TipoAtributoProducto? Tipo { get; set; } // Obligatorio. Siempre un atributo debe tener un tipo, incluso si este es de tipo "Sin Tipo".
        public int TipoID { get; set; }

        #endregion Propiedades>


        #region Constructores

        private AtributoProducto() { } // Solo para que Entity Framework no saque error.

        public AtributoProducto(string nombre, TipoAtributoProducto tipo) => (Nombre, Tipo, TipoID) = (nombre, tipo, tipo.ID);

        #endregion Constructores>


        #region Métodos y Funciones

        public override string ToString() => Nombre;

        #endregion Métodos y Funciones>


    } // AtributoProducto>



} // SimpleOps.Modelo>
