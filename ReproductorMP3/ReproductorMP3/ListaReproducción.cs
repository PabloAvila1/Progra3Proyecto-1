using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;

namespace ReproductorMP3
{
    public partial class ListaReproducción : Form
    {
        Form1 frm = new Form1();
        static XmlDocument documento = new XmlDocument();
        static string ruta = @"biblio.xml";
        List<ReproducirMúsica> listareproduci = new List<ReproducirMúsica>();
        List<DatosMP3> listadatosmp3 = new List<DatosMP3>();
        List<BibliotecaCanciones> listabiblioteca = new List<BibliotecaCanciones>();
        public ListaReproducción()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            openFileDialog1.ShowDialog();
            ReproducirMúsica reprotemp = new ReproducirMúsica();
            reprotemp.Url = openFileDialog1.FileName;
            reprotemp.Nombre = openFileDialog1.SafeFileName.ToString();
            listareproduci.Add(reprotemp);
            cargar();
            xml();
        }

        public void xml()
        {
            XmlDocument doc = new XmlDocument();
            XmlElement raiz = doc.CreateElement("Lista_Favoritos");
            doc.AppendChild(raiz);

            XmlElement cancion = doc.CreateElement("Cancion");


            XmlElement titulo = doc.CreateElement("Titulo");


            XmlElement url = doc.CreateElement("Url");

            for (int i = 0; i < listareproduci.Count(); i++)
            {
                cancion = doc.CreateElement("Cancion");
                raiz.AppendChild(cancion);

                titulo = doc.CreateElement("Titulo");
                titulo.AppendChild(doc.CreateTextNode(listareproduci[i].Nombre));
                cancion.AppendChild(titulo);

                url = doc.CreateElement("Url");
                url.AppendChild(doc.CreateTextNode(listareproduci[i].Url));
                cancion.AppendChild(url);

                doc.Save(@"miXML.xml");
            }
        }

        public void EscribirXml()
        {
            for (int i = 0; i < listabiblioteca.Count(); i++)
            {
                if (label1.Text == listabiblioteca[i].Nombre)
                { 
                    using (XmlTextWriter Writer = new XmlTextWriter(@"biblio.xml", Encoding.UTF8))
                    {
                        Writer.WriteStartDocument();

                        Writer.Formatting = Formatting.Indented;
                        Writer.Indentation = 5;

                        Writer.WriteStartElement("BlibiotecaCanciones");

                        Writer.WriteStartElement("Cancion");

                        Writer.WriteElementString("nombre", listabiblioteca[i].Nombre);
                        Writer.WriteElementString("url", listabiblioteca[i].Url);
                        Writer.WriteElementString("num", listabiblioteca[i].Num);
                        Writer.WriteElementString("album", listabiblioteca[i].Album);

                        Writer.WriteElementString("duracion", listabiblioteca[i].Duracion);
                        Writer.WriteElementString("calidad", listabiblioteca[i].Calidad);

                        Writer.WriteEndElement();
                        Writer.WriteEndDocument();
                        Writer.Flush();
                    }
                }
            }
        }

        public void cargar()
        {
            dataGridView1.DataSource = null;
            dataGridView1.Refresh();
            dataGridView1.DataSource = listareproduci;
            dataGridView1.Columns["url"].Visible = false;
            dataGridView1.Refresh();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            string nom = dataGridView1.CurrentRow.Cells["nombre"].Value.ToString();
            label1.Text = nom;
        }

        private void ListaReproducción_load(object sender, EventArgs e)
        {
            frm.Show();
            button4.Visible = false;
            button8.Visible = false;
            dataGridView1.Visible = true;
            dataGridView2.Visible = false;
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            frm.Hide();
            frm.label1.Text = " ";

            frm.label1.Text = dataGridView1.CurrentRow.Cells["nombre"].Value.ToString();
            frm.WMedia.URL = dataGridView1.CurrentRow.Cells["url"].Value.ToString();

            WMPLib.IWMPPlaylist playlist = frm.WMedia.playlistCollection.newPlaylist("myplaylist");
            WMPLib.IWMPMedia media;

            media = frm.WMedia.newMedia(dataGridView1.CurrentRow.Cells["url"].Value.ToString());
            playlist.appendItem(media);

            frm.WMedia.currentPlaylist = playlist;
            listadatosmp3.RemoveRange(0, listadatosmp3.Count);
            string dat = dataGridView1.CurrentRow.Cells["url"].Value.ToString();
            cargarima(dat);
            tagcan(dat);
            frm.dataGridView1.DataSource = null;
            frm.dataGridView1.Refresh();
            frm.dataGridView1.DataSource = listadatosmp3;
            frm.dataGridView1.Refresh();
            frm.Show();

        }
        public void tagcan(string car)
        {
            TagLib.File file = TagLib.File.Create(car);
            DatosMP3 datmp = new DatosMP3();
            datmp.Titulo = file.Tag.Title;
            datmp.Año = Convert.ToString(file.Tag.Year);
            datmp.Genero = file.Tag.FirstGenre;
            datmp.Duracion = file.Properties.Duration.ToString();
            datmp.Num = Convert.ToString(file.Tag.Track);
            datmp.Artista = file.Tag.TitleSort;
            datmp.Album = file.Tag.Album;
            datmp.Comentario = file.Tag.Comment;
            listadatosmp3.Add(datmp);
        }
        public void cargarima(string dat)
        {
            TagLib.File file = TagLib.File.Create(dat);
            System.Drawing.Image currentImage = null;

            if (file.Tag.Pictures.Length > 0)
            {
                TagLib.IPicture pic = file.Tag.Pictures[0];
                MemoryStream ms = new MemoryStream(pic.Data.Data);
                if (ms != null && ms.Length > 4096)
                {
                    currentImage = System.Drawing.Image.FromStream(ms);
                    frm.caratula.Image = currentImage.GetThumbnailImage(200, 200, null, System.IntPtr.Zero);
                }
                ms.Close();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            int contar = listareproduci.Count();
            int inicio = 0;
            listareproduci.RemoveRange(inicio, contar);
            cargar();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string nomb = label1.Text;
            for (int i = 0; i < listareproduci.Count; i++)
            {
                if (nomb == listareproduci[i].Nombre)
                {
                    listareproduci.RemoveAt(i);
                }
            }
            cargar();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            string nomb = label1.Text;
            for (int i = 0; i < listareproduci.Count; i++)
            {
                BibliotecaCanciones blitmp = new BibliotecaCanciones();
                if (nomb == listareproduci[i].Nombre)
                {
                    blitmp.Url = listareproduci[i].Url;
                    blitmp.Nombre = listareproduci[i].Nombre;
                    TagLib.File file = TagLib.File.Create(listareproduci[i].Url);
                    blitmp.Titulo = file.Tag.Title;
                    blitmp.Año = Convert.ToString(file.Tag.Year);
                    blitmp.Duracion = file.Properties.Duration.ToString();
                    blitmp.Num = Convert.ToString(file.Tag.Track);
                    blitmp.Album = file.Tag.Album;
                    blitmp.Calidad = Convert.ToString(file.Properties.AudioBitrate);

                }
                listabiblioteca.Add(blitmp);
            }
            string archivo = @"biblio.xml";
            if (File.Exists(archivo) == true)
            {
                InsertarXml();
            }
            else { EscribirXml(); }
        }
        string nombre1, url, num, album, dura, cali;

        private void button4_Click(object sender, EventArgs e)
        {
            string nomb = label1.Text;
            for (int i = 0; i < listabiblioteca.Count; i++)
            {
                if (nomb == listabiblioteca[i].Nombre)
                {
                    listabiblioteca.RemoveAt(i);
                }
            }

            ModificarDatosXml(nomb);
            listabiblioteca.RemoveRange(0, listabiblioteca.Count);
            leerbiblio();
            dataGridView2.DataSource = null;
            dataGridView2.Refresh();
            dataGridView2.DataSource = listabiblioteca;
            dataGridView2.Refresh();
        }
        private void button5_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        public void eliminarlisre()
        {
            XmlDocument documento = new XmlDocument();
            string ruta = @"miXML.xml";
            documento = new XmlDocument();
            documento.Load(ruta);
            XmlElement bibliot = documento.DocumentElement;

            XmlNodeList listacancion = documento.SelectNodes("Lista_Favoritos/Cancion");

            foreach (XmlNode item in listacancion)
            {
                for (int i = 0; i < listareproduci.Count; i++)
                {
                    if (item.FirstChild.InnerText == listareproduci[i].Nombre)
                    {
                        XmlNode nodoOld = item;
                        bibliot.RemoveChild(nodoOld);
                    }
                }
                documento.Save(ruta);
            }

        }
        public void actualizar()
        {
            if (listareproduci.Count == 0)
            {
                leerxml();
            }

        }
        public void leerxml()
        {
            XDocument documento = XDocument.Load(@"miXML.xml");
            var listar = from lis in documento.Descendants("Lista_Favoritos") select lis;
            foreach (XElement u in listar.Elements("Cancion"))
            {
                ReproducirMúsica tmp = new ReproducirMúsica();
                tmp.Nombre = u.Element("Titulo").Value;
                tmp.Url = u.Element("Url").Value;
                listareproduci.Add(tmp);

            }
        }
        private void dataGridView2_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            frm.Hide();
            frm.label1.Text = " ";

            frm.label1.Text = dataGridView2.CurrentRow.Cells["Nombre"].Value.ToString();
            frm.WMedia.URL = dataGridView2.CurrentRow.Cells["Url"].Value.ToString();

            WMPLib.IWMPPlaylist playlist = frm.WMedia.playlistCollection.newPlaylist("myplaylist");
            WMPLib.IWMPMedia media;

            media = frm.WMedia.newMedia(dataGridView2.CurrentRow.Cells["Url"].Value.ToString());
            playlist.appendItem(media);

            frm.WMedia.currentPlaylist = playlist;
            listadatosmp3.RemoveRange(0, listadatosmp3.Count);
            string dat = dataGridView2.CurrentRow.Cells["Url"].Value.ToString();
            cargarima(dat);
            tagcan(dat);
            frm.dataGridView1.DataSource = null;
            frm.dataGridView1.Refresh();
            frm.dataGridView1.DataSource = listadatosmp3;
            frm.dataGridView1.Refresh();
            frm.Show();
        }
        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            string nom = dataGridView2.CurrentRow.Cells["Nombre"].Value.ToString();
            label1.Text = nom;
        }
        string urla;
        private void button8_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                urla = openFileDialog1.FileName;
            }
            listadatosmp3.RemoveRange(0, listadatosmp3.Count);



            BibliotecaCanciones blitmp = new BibliotecaCanciones();
            TagLib.File file = TagLib.File.Create(urla);
            blitmp.Url = urla;
            blitmp.Nombre = file.Tag.Title;
            blitmp.Titulo = file.Tag.Title;
            label1.Text = file.Tag.Title;
            blitmp.Año = Convert.ToString(file.Tag.Year);
            blitmp.Duracion = file.Properties.Duration.ToString();
            blitmp.Num = Convert.ToString(file.Tag.Track);
            blitmp.Album = file.Tag.Album;
            blitmp.Calidad = Convert.ToString(file.Properties.AudioBitrate);

            listabiblioteca.Add(blitmp);

            string archivo = @"biblio.xml";
            if (File.Exists(archivo) == true)
            {
                InsertarXml();
            }
            else { EscribirXml(); }
            listabiblioteca.RemoveRange(0, listabiblioteca.Count);
            leerbiblio();
            dataGridView2.DataSource = null;
            dataGridView2.Refresh();
            dataGridView2.DataSource = listabiblioteca;
            dataGridView2.Columns["Url"].Visible = false;
            dataGridView2.Refresh();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            button8.Visible = true;
            button7.Visible = false;
            dataGridView1.Visible = false;
            dataGridView2.Visible = true;
            listareproduci.RemoveRange(0, listareproduci.Count);
            actualizar();
            eliminarlisre();
            listabiblioteca.RemoveRange(0, listabiblioteca.Count);
            listareproduci.RemoveRange(0, listareproduci.Count);

            button2.Visible = false;
            button1.Visible = false;
            button3.Visible = false;
            button4.Visible = true;
            leerbiblio();
            dataGridView2.DataSource = null;
            dataGridView2.Refresh();
            dataGridView2.DataSource = listabiblioteca;
            dataGridView2.Refresh();
        }

        public void leerbiblio()
        {
            XDocument documento = XDocument.Load(@"biblio.xml");
            var listar = from lis in documento.Descendants("BlibiotecaCanciones") select lis;
            foreach (XElement u in listar.Elements("Cancion"))
            {
                BibliotecaCanciones tmp = new BibliotecaCanciones();
                tmp.Nombre = u.Element("Titulo").Value;
                tmp.Url = u.Element("Url").Value;
                tmp.Num = u.Element("No").Value;
                tmp.Album = u.Element("Album").Value;
                tmp.Duracion = u.Element("Duracion").Value;
                tmp.Calidad = u.Element("Calidad").Value;

                listabiblioteca.Add(tmp);

            }
        }
        private void InsertarXml()
        {
            documento = new XmlDocument();
            documento.Load(ruta);

            for (int i = 0; i < listabiblioteca.Count(); i++)
            {

                if (label1.Text == listabiblioteca[i].Nombre)
                {

                    nombre1 = listabiblioteca[i].Nombre;
                    url = listabiblioteca[i].Url;
                    num = listabiblioteca[i].Num;
                    album = listabiblioteca[i].Album;
                    dura = listabiblioteca[i].Duracion;
                    cali = listabiblioteca[i].Calidad;

                }
            }
            XmlNode empleado = this.CrearNodoXml(nombre1, url, num, album, dura, cali);

            XmlNode nodoRaiz = documento.DocumentElement;

            nodoRaiz.InsertAfter(empleado, nodoRaiz.LastChild);  

            documento.Save(ruta);
        }
        private XmlNode CrearNodoXml(string nom1, string url1, string num1, string album1, string dura1, string cali1)
        {
            XmlElement Cancion = documento.CreateElement("Cancion");

            XmlElement nombre = documento.CreateElement("Titulo");
            nombre.InnerText = nom1;
            Cancion.AppendChild(nombre);

            XmlElement Url = documento.CreateElement("Url");
            Url.InnerText = url1;
            Cancion.AppendChild(Url);

            XmlElement num = documento.CreateElement("No");
            num.InnerText = num1;
            Cancion.AppendChild(num);

            XmlElement album = documento.CreateElement("Album");
            album.InnerText = album1;
            Cancion.AppendChild(album);

            XmlElement duracion = documento.CreateElement("Duracion");
            duracion.InnerText = dura1;
            Cancion.AppendChild(duracion);

            XmlElement calidad = documento.CreateElement("Calidad");
            calidad.InnerText = cali1;
            Cancion.AppendChild(calidad);

            return Cancion;
        }
        public void ModificarDatosXml(string url)
        {
            documento = new XmlDocument();
            documento.Load(ruta);
            XmlElement bibliot = documento.DocumentElement;

            XmlNodeList listacancion = documento.SelectNodes("Blibioteca/Cancion");

            foreach (XmlNode item in listacancion)
            {
                if (item.FirstChild.InnerText == url)
                {
                    XmlNode nodoOld = item;
                    bibliot.RemoveChild(nodoOld);
                }
            }
            documento.Save(ruta);
        }
    }
}
