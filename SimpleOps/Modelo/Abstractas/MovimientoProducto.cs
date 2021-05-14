﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using static SimpleOps.Global;
using static Vixark.General;
using SimpleOps.Datos;



namespace SimpleOps.Modelo {



    /// <summary>
    /// Una línea de una <see cref="Factura{E, M}"/> (incluye notas) o de una <see cref="Remisión"/>.
    /// </summary>
    abstract class MovimientoProducto { // No necesitan ser rastreadas porque son entidades de tipo Línea[EntidadPadre] que se crean en una sola operación con la EntidadPadre entonces su información de creación es la misma. Además, no admiten más cambios después de creadas por lo que no hay necesidad de crear propiedades para la información de actualización. No se admitirá actualización porque por ejemplo: si la factura se anula o si aplica nota crédito estos productos devueltos se detallan en las tablas LíneaNotaCrédito (que tampoco son actualizables).


        #region Propiedades

        public Producto? Producto { get; set; } // Obligatorio.
        public int ProductoID { get; set; } // Clave foránea que con CompraID, RemisiónID, VentaID, etc forman la clave principal.

        public int Cantidad { get; set; } // Obligatorio.

        /// <summary>
        /// Precio real del producto. No es cero si es muestra gratis.
        /// </summary>
        public decimal Precio { get; set; } // Obligatorio.

        public decimal CostoUnitario { get; set; } // Obligatorio.

        /// <summary>
        /// Se usa cuando se ha aplicado un descuento comercial general sobre toda la factura. En estos casos se afecta la base tributable 
        /// (<see cref="SubtotalBase"/>) y como los impuestos se deben calcular para cada producto, se calcula el <see cref="PrecioBase"/>
        /// con el que se calcularán los subtotales e impuestos. En caso que se quiera establecer un descuento comercial individual para 
        /// cada producto, se permite hacerlo mediante la interfaz, pero se afecta directamente el valor de <see cref="Precio"/>.<br/>
        /// Esta convención se establece para ahorrar espacio en la base de datos al no tener que almacenar el porcentaje de descuento 
        /// en todos los movimientos, pero permitiendo calcular el precio efectivo de un producto en todos los casos.<br/>
        /// Su valor se establece en <see cref="Factura{E, M}.CalcularTodo"/>. No se almacena en la base de datos, 
        /// se ignora en <see cref="Contexto.OnModelCreating"/>.
        /// </summary>
        public decimal PorcentajeDescuentoComercial { get; set; } = 0;

        /// <summary>
        /// Se usa para calcular el <see cref="PrecioFinal"/> para obtener <see cref="Margen"/>, <see cref="PorcentajeMargen"/> y 
        /// <see cref="PorcentajeGanancia"/>. Su valor se establece en <see cref="Factura{E, M}.CalcularTodo"/>. No se almacena en la
        /// base de datos, se ignora en <see cref="Contexto.OnModelCreating"/>.
        /// </summary>
        public decimal PorcentajeDescuentoCondicionado { get; set; } = 0;

        /// <summary>
        /// No se almacena en la base de datos, se ignora en <see cref="Contexto.OnModelCreating"/>.
        /// </summary>
        public bool MuestraGratis { get; set; } = false; // Se prefiere el término muestra gratis en vez de regalo u obsequio porque describe más claramente el uso más común que se le da a esta propiedad.

        #endregion Propiedades>


        #region Constructores

        public MovimientoProducto(Producto? producto, int cantidad, decimal precio, decimal costoUnitario)
            => (ProductoID, Cantidad, Precio, CostoUnitario, Producto) = (Producto.ObtenerID(producto), cantidad, precio, costoUnitario, producto);

        #endregion Constructores>


        #region Propiedades Autocalculadas  
        // Real se refiere al precio real del producto así sea una muestra gratis. Si no tiene Real el precio es cero si es una muestra gratis, Base se refiere al precio después de aplicar el descuento comercial global en la factura y Final se refiere al precio después de aplicar tanto el descuento comercial como el descuento condicionado.

        public decimal Costo => CostoUnitario * Cantidad;

        /// <summary>
        /// El precio efectivo que se usa para el cálculo del <see cref="SubtotalBase"/> (base tributable) después de descontar el porcentaje 
        /// de descuento comercial. Su valor no es cero si es muestra gratis.
        /// </summary>
        public decimal PrecioBaseReal => MuestraGratis ? Precio : Precio * (1 - PorcentajeDescuentoComercial); // Si es una muestra gratis el valor comercial de esta no se ve afectado por el descuento comercial que se esté aplicando a la factura. 

        /// <summary>
        /// El precio efectivo que se usa para el cálculo del <see cref="SubtotalBase"/> (base tributable) después de descontar el porcentaje 
        /// de descuento comercial. Su valor es cero si es muestra gratis.
        /// </summary>
        public decimal PrecioBase => MuestraGratis ? 0 : PrecioBaseReal;

        /// <summary>
        /// Se usa principalmente para pasar al procedimiento de generación de representación gráfica de documentos que no tiene
        /// acceso a la función ATextoDinero()
        /// </summary>
        public string PrecioBaseTexto => PrecioBase.ATextoDinero(agregarMoneda: false);

        /// <summary>
        /// El precio efectivo al que se está vendiendo el producto después de descontar los porcentajes de descuento comercial y condicionado.
        /// Permite calcular <see cref="Margen"/>, <see cref="PorcentajeMargen"/> y <see cref="PorcentajeGanancia"/>.
        /// Su valor es cero si es muestra gratis.
        /// </summary>
        public decimal PrecioFinal => MuestraGratis ? 0 : Precio * (1 - PorcentajeDescuentoComercial - PorcentajeDescuentoCondicionado);

        /// <summary>
        /// <see cref="PrecioBaseReal"/> * <see cref="Cantidad"/>. Su valor no es cero si es muestra gratis.
        /// Para el cálculo se usa el precio que se le daría al cliente si no fuera una muestra gratis.
        /// </summary>
        public decimal SubtotalBaseReal => PrecioBaseReal * Cantidad;

        /// <summary>
        /// Subtotal directo del producto. No tiene en cuenta ningún descuento. Su valor es cero si es muestra gratis.
        /// </summary>
        public decimal Subtotal => MuestraGratis ? 0 : Precio * Cantidad;

        /// <summary>
        /// <see cref="PrecioBase"/> * <see cref="Cantidad"/>. También llamado base tributable. Incluye el descuento comercial. 
        /// Si es una muestra gratis es cero.
        /// </summary>
        public decimal SubtotalBase => PrecioBase * Cantidad;

        /// <summary>
        /// Se usa principalmente para pasar al procedimiento de generación de representación gráfica de documentos que no tiene
        /// acceso a la función ATextoDinero()
        /// </summary>
        public string SubtotalBaseTexto => SubtotalBase.ATextoDinero(agregarMoneda: false);

        /// <summary>
        /// Se le puede llamar el total de la línea. Se usa principalmente para pasar al procedimiento de generación de representación gráfica 
        /// de documentos que no tiene acceso a la función ATextoDinero()
        /// </summary>
        public string SubtotalBaseConImpuestosTexto => (SubtotalBase + (IVA ?? 0) + (ImpuestoConsumo ?? 0)).ATextoDinero(agregarMoneda: false);

        /// <summary>
        /// Incluye el valor de los productos que sean muestras gratis sin valor comercial pero no incluye los excluídos de IVA.
        /// Se incluye para ser consistente con la definición de productos excluídos de IVA que son los su valor no suma a la base tributable.
        /// </summary>
        public decimal? SubtotalBaseIVA => Producto == null ? (decimal?)null : (Producto.ExcluídoIVA ? 0 : SubtotalBaseReal);

        /// <summary>
        /// Incluye el valor de los productos que sean muestras gratis sin valor comercial pero no incluye los excluídos y exentos de IVA.
        /// Es necesario porque este es el valor que realmente solicita la DIAN cuando pide el subtotal sin impuestos en la factura electrónica.
        /// </summary>
        public decimal? SubtotalBaseIVADian => Producto == null ? (decimal?)null : (IVA == 0 ? 0 : SubtotalBaseReal); // Es necesario usar IVA == 0 para poder incluir los casos en los que el producto es exento de IVA no por ser en si mismo exento si no porque el cliente o el municipio lo son.

        /// <summary>
        /// Incluye el descuento comercial y condicionado. Si es una muestra gratis es cero.
        /// </summary>
        public decimal SubtotalFinal => PrecioFinal * Cantidad;

        public decimal Margen => SubtotalFinal - Costo;

        /// <summary>
        /// Valor del descuento comercial.
        /// </summary>
        public decimal DescuentoComercial => Subtotal - SubtotalBase;

        /// <summary>
        /// Es nulo cuando el precio es cero o es una muestra gratis.
        /// </summary>
        public decimal? PorcentajeMargen => ObtenerPorcentajeMargen(SubtotalFinal, Costo);

        /// <summary>
        /// El porcentaje aplicado al costo unitario para obtener el precio de venta. En inglés: Markup Percentage. Es nulo cuando el precio es cero 
        /// o es una muestra gratis.
        /// </summary>
        public decimal? PorcentajeGanancia => ObtenerPorcentajeGanancia(SubtotalFinal, Costo);


        public decimal? ImpuestoConsumo {

            get {

                if (Producto == null) return null;
                #pragma warning disable CS8524 // Se omite para que no obligue a usar el patrón de descarte _ => porque este oculta la advertencia CS8509 que es muy útil para detectar valores de la enumeración faltantes. No se omite a nivel global porque la desactivaría para los switchs que no tienen enumeraciones, ver https://github.com/dotnet/roslyn/issues/47066.
                return Producto.ModoImpuestoConsumo switch {
                    ModoImpuesto.Exento => 0,
                    ModoImpuesto.Porcentaje => (decimal?)Producto.PorcentajeImpuestoConsumo * SubtotalBase,
                    ModoImpuesto.Unitario => Cantidad * Producto.ImpuestoConsumoUnitario,
                };
                #pragma warning restore CS8524

            }

        } // ImpuestoConsumo>

        /// <summary>
        /// Se usa principalmente para determinar si en una factura se aplican diferentes porcentajes de impuesto al consumo y por lo tanto se 
        /// debe considerar mostrar la columna de impuesto en consumo en la representación gráfica de la factura.
        /// </summary>
        public decimal? PorcentajeEfectivoImpuestoConsumo => ObtenerPorcentajeImpuesto(SubtotalBase, ImpuestoConsumo);

        /// <summary>
        /// Se usa principalmente para pasar al procedimiento de generación de representación gráfica de documentos que no tiene
        /// acceso a la función ATextoDinero()
        /// </summary>
        public string ImpuestoConsumoTexto => ImpuestoConsumo == null ? "0" : ((decimal)ImpuestoConsumo).ATextoDinero(agregarMoneda: false);


        #endregion Propiedades Autocalculadas>


        #region Propiedades Abstractas y Virtuales

        /// <summary>
        /// Valor del IVA.
        /// </summary>
        public abstract decimal? IVA { get; } // Se obliga su implementación en las clases derivadas porque su cálculo depende del producto, el tipo de factura y la entidad económica.

        /// <summary>
        /// Se usa principalmente para determinar si en una factura se aplican diferentes porcentajes de impuesto al consumo y por lo tanto se 
        /// debe considerar mostrar la columna de impuesto en consumo en la representación gráfica de la factura.
        /// </summary>
        public decimal? PorcentajeEfectivoIVA => ObtenerPorcentajeImpuesto(SubtotalBaseIVA, IVA);

        /// <summary>
        /// Se usa principalmente para pasar al procedimiento de generación de representación gráfica de documentos que no tiene
        /// acceso a la función ATextoDinero()
        /// </summary>
        public string IVATexto => IVA == null ? "0" : ((decimal)IVA).ATextoDinero(agregarMoneda: false);

        #endregion Propiedades Abstractas y Virtuales>


        #region Métodos y Funciones

        public decimal ObtenerValorImpuesto(TipoTributo tipoTributo) => tipoTributo == TipoTributo.IVA ? IVA ?? 0 : ImpuestoConsumo ?? 0;


        public ModoImpuesto ObtenerModoImpuesto(TipoTributo tipoTributo)
            => tipoTributo == TipoTributo.IVA ? ModoImpuesto.Porcentaje : 
                  (Producto == null ? throw new Exception("No se esperaba producto nulo.") : Producto.ModoImpuestoConsumo);


        public decimal ObtenerTarifaImpuesto(TipoTributo tipoTributo) {

            var valorImpuesto = ObtenerValorImpuesto(tipoTributo);
            var modoImpuesto = ObtenerModoImpuesto(tipoTributo);
            #pragma warning disable CS8524 // Se omite para que no obligue a usar el patrón de descarte _ => porque este oculta la advertencia CS8509 que es muy útil para detectar valores de la enumeración faltantes. No se omite a nivel global porque la desactivaría para los switchs que no tienen enumeraciones, ver https://github.com/dotnet/roslyn/issues/47066.
            return modoImpuesto switch {
                ModoImpuesto.Exento => 0,
                ModoImpuesto.Porcentaje => ObtenerTarifaImpuestoPorcentual(valorImpuesto, SubtotalBaseReal), // Para efectos de obtener la tarifa del impuesto
                ModoImpuesto.Unitario => valorImpuesto / Cantidad,
            };
            #pragma warning restore CS8524

        } // ObtenerTarifaImpuesto>


        #endregion Métodos y Funciones>


    } // MovimientoProducto>



} // SimpleOps.Modelo>
