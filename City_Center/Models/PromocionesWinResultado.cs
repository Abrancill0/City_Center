﻿using System;
using System.Collections.Generic;

namespace City_Center.Models
{
    public class PromocionesWinResultado
    {
        public class PromocionesWinDetalle
        {
            public int pro_id { get; set; }
            public int pro_id_evento { get; set; }
            public int pro_id_locacion { get; set; }
            public string pro_nombre { get; set; }
            public string pro_descripcion { get; set; }
            public string pro_imagen { get; set; }
            public string pro_tipo_promocion { get; set; }
            public string pro_codigo { get; set; }
            public int pro_compartidos_codigo { get; set; }
            public int pro_destacado { get; set; }
            public string pro_fecha_duracion_ini { get; set; }
            public string pro_fecha_duracion_fin { get; set; }
            public string pro_importe_decuento { get; set; }
            public string pro_porcentaje_decuento { get; set; }
            public int pro_id_usuario_creo { get; set; }
            public string pro_fecha_hora_creo { get; set; }
            public int pro_id_usuario_modifico { get; set; }
            public string pro_fecha_hora_modifico { get; set; }
            public string pro_tipo { get; set; }
            public string pro_estatus { get; set; }
        }

        public class PromocionesWinReturn
        {
            public int estatus { get; set; }
            public string mensaje { get; set; }
            public List<PromocionesWinDetalle> resultado { get; set; }
        }
    }
}
