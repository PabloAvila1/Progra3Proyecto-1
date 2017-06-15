using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReproductorMP3
{
    class BibliotecaCanciones:DatosMP3
    {
        string nombre;
        string url;
        string calidad;
public string Nombre
        {
            get
            {
                return nombre;
            }

            set
            {
                nombre = value;
            }
        }

        public string Url
        {
            get
            {
                return url;
            }

            set
            {
                url = value;
            }
        }

        public string Calidad
        {
            get
            {
                return calidad;
            }

            set
            {
                calidad = value;
            }
        }
    }
}

