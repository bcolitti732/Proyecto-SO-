using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.PerformanceData;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;
using System.IO;
using System.Media;
using System.Net;
using System.Net.Sockets;
using System.Threading;



namespace Trivial
{
    public partial class FormPrincipal : Form
    {
        Socket server;
        Thread atender;
        FormInv invitacion;

        int conn = 0;
        int x = 0;

        string username;

        delegate void DelegadoParaEscribir(string[] conectados);

        List<string> invitados;

        List<FormJuego> tableros;

        FormConsultas formConsultas;

        public FormPrincipal()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            Bitmap inicio = new Bitmap(Application.StartupPath + @"\trivia.jpg");
            this.BackgroundImage = inicio;
            this.BackgroundImageLayout = ImageLayout.Stretch;

            Bitmap host = new Bitmap(Application.StartupPath + @"\JugAzul.png");
            Bitmap jug1 = new Bitmap(Application.StartupPath + @"\JugLila.png");
            Bitmap jug2 = new Bitmap(Application.StartupPath + @"\JugVerde.png");
            Bitmap jug3 = new Bitmap(Application.StartupPath + @"\JugRojo.png");

            List<Bitmap> fichas = new List<Bitmap>();
            Bitmap[] bitlist = new Bitmap[] { host, jug1, jug2, jug3 };
            fichas.AddRange(bitlist);

            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
        }

        //Funcion para que un thread pueda modificar objetos del formulario
        public void ListaConectadosGridView(string[] conectados)
    {
            //Queremos mostrar los datos en un Data Grid View
            labelConectados.Visible = true;
            listaconGridView.Visible = true;
            listaconGridView.ColumnCount = 1;
            listaconGridView.RowCount = conectados.Length;
            listaconGridView.ColumnHeadersVisible = false;
            listaconGridView.RowHeadersVisible = false;
            listaconGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            listaconGridView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            listaconGridView.BackgroundColor = Color.White;
            
            listaconGridView.SelectAll();

            int totalRowHeight = listaconGridView.ColumnHeadersHeight;
            for (int i = 0; i < conectados.Length; i++)
            {
                listaconGridView.Rows[i].DefaultCellStyle.BackColor = Color.Orange;
                listaconGridView.Rows[i].Cells[0].Value = conectados[i];
                totalRowHeight += listaconGridView.Rows[i].Height;
            }
            listaconGridView.Height = totalRowHeight;
            listaconGridView.Show();
        }

        //Funcion para obtener la posicion en la lista de tableros de un id de partida
        private int DamePosicionLista(List<FormJuego> tableros, int idPartida)
        {
            //Retorna el tablero asignado a la partida si todo va bien y -1 si no
            bool encontrado = false;
            int i = 0;
            while (i < tableros.Count && encontrado == false)
            {
                if (tableros[i].DameIdPartida() == idPartida)
                    encontrado = true;
                else
                    i = i + 1;
            }
            if (encontrado == true)
                return i;
            else
                return -1;
        }

        //Funcion que ejecutará el thread de recepción de respuestas del servidor
        private void AtenderServidor()
        {
            bool fin = false;
            while (fin==false)
            {
                try
                {
                    //Recibimos la respuesta del servidor
                    byte[] msg2 = new byte[80];
                    server.Receive(msg2);
                    string[] trozos = Encoding.ASCII.GetString(msg2).Split('/');
                    int codigo = Convert.ToInt32(trozos[0]);
                    string mensaje = trozos[1].Split('\0')[0];
                    switch (codigo)
                    {
                        case 1: //Inicio de sesión
                            if (mensaje == "0")
                            {
                                tableros = new List<FormJuego>();

                                //Establecemos fondo, gridviews, etc.
                                Bitmap fondo = new Bitmap(Application.StartupPath + @"\trivia2.png");
                                this.BackgroundImage = fondo;
                                this.BackgroundImageLayout = ImageLayout.Stretch;

                                inv_lbl.Visible = true;
                                inv_lbl.Text = username + ", pulsa sobre el nombre del jugador para invitarlo";
                                consultasButton.Visible = true;
                                accederBox.Visible = false;
                                registroBox.Visible = false;
                                listaconGridView.Visible = true;
                                labelConectados.Visible = true;


                                conexion.Text = "Desconectar";
                                conexion.Visible = true;
                                conn = 1;

                                regLabel.Visible = false;
                                regVisible.Visible = false;
                                inicio.Visible = false;
                                eliminarLbl.Visible = false;
                                eliminarCuenta.Visible = false;
                            }
                            //Errores 
                            else if (mensaje == "1")
                            {
                                MessageBox.Show("Usuario incorecto");
                                NameBox.Clear();
                                PasswordBox.Clear();
                                fin = true;
                            }
                            else if (mensaje == "2")
                            {
                                MessageBox.Show("Contraseña incorrecta");
                                PasswordBox.Clear();
                                fin = true;
                            }
                            else if (mensaje == "3")
                            {
                                MessageBox.Show("Usuario ya conectado");
                                NameBox.Clear();
                                PasswordBox.Clear();
                                fin = true;
                            }
                            else
                            {
                                MessageBox.Show("Error en la consulta, vuelva a intentarlo");
                                fin = true;
                            }
                            break;

                        case 2: //Registrar
                            if (mensaje == "0")
                            {
                                MessageBox.Show("Se ha registrado correctamente");
                                registroBox.Visible = false;
                                inicio.Visible = false;
                                regVisible.Visible = true;
                                accederBox.Visible = true;
                                regLabel.Visible = true;
                                eliminarLbl.Visible = true;
                                eliminarCuenta.Visible = true;
                            }
                            //Errores
                            else if (mensaje == "1")
                                MessageBox.Show("Este nombre de usuario ya existe.");
                            else
                                MessageBox.Show("Error en la consulta, vuelva a intentarlo");
                            userBox.Clear();
                            password2Box.Clear();
                            mailBox.Clear();
                            fin = true;
                            break;

                        case 3: //(Consulta para ver contrincantes recientes)
                            if (mensaje == "-1")
                                MessageBox.Show("Error en la consulta, vuelva a intentarlo");
                            else
                            {
                                this.formConsultas = new FormConsultas();
                                this.formConsultas.SetPregunta(ConQuienLbl.Text);
                                this.formConsultas.SetDataGrid(codigo, mensaje);
                                this.formConsultas.ShowDialog();
                            }
                            break;

                        case 4: //(Consulta para el resultado de una partida con X jugador/es)
                            if (mensaje == "-1")
                                MessageBox.Show("Error en la consulta, vuelva a intentarlo");
                            else
                            {
                                this.formConsultas = new FormConsultas();
                                this.formConsultas.SetPregunta(companyia.Text);
                                this.formConsultas.SetDataGrid(codigo, mensaje);
                                this.formConsultas.ShowDialog();
                            }
                            break;

                        case 5: //(Consulta del jugador con más puntos)
                            if (mensaje == "-1")
                                MessageBox.Show("Error en la consulta, vuelva a intentarlo");
                            else {
                                this.formConsultas = new FormConsultas();
                                this.formConsultas.SetPregunta(jugMaxBtn.Text);
                                this.formConsultas.SetDataGrid(codigo, mensaje);
                                this.formConsultas.ShowDialog();
                            }
                            break;

                        case 6: //Notificación de actualización de la lista de conectados
                            if (mensaje == "-1")
                                MessageBox.Show("No hay usuarios en línea");
                            else
                            {
                                //Separamos los conectados y los introducimos en un vector
                                string[] conectados = mensaje.Split('*');

                                //Aplicamos el delegado para modificar el DataGridView
                                DelegadoParaEscribir delegado = new DelegadoParaEscribir(ListaConectadosGridView);
                                listaconGridView.Invoke(delegado, new object[] { conectados });
                            }
                            break;

                        case 7: //Petición de invitación
                            if (mensaje == "0")
                                MessageBox.Show("Invitaciones enviadas con éxito");
                            else
                            {
                                string[] idle = mensaje.Split('*');
                                string show = "";
                                for (int n = 0; n < idle.Length; n++)
                                    show = show + idle[n] + ",";
                                show = show.Remove(show.Length - 1);
                                MessageBox.Show("Invitaciones enviadas con éxito\n excepto las de: " + show + "\nInténtalo de nuevo");
                            }
                            break;

                        case 8: //Notificación de invitacion a una partida
                            string[] split = mensaje.Split('*');
                            invitacion = new FormInv();
                            invitacion.SetHost(split[0]);
                            invitacion.ShowDialog();
                            string respuesta = "7/" + invitacion.GetRespuesta() + "/" + split[1] + "\0";
                            byte[] msg = System.Text.Encoding.ASCII.GetBytes(respuesta);
                            server.Send(msg);

                            break;

                        case 9: //Notificación de inicio de partida a los jugadores implicados
                            //Thread de la partida
                            ThreadStart ts = delegate { NuevaPartida(mensaje); };
                            Thread T = new Thread(ts);
                            T.Start();
                            break;

                        case 10://Notificación el fin de la partida a los jugadores indicados
                            int numTablero = DamePosicionLista(tableros, Convert.ToInt32(mensaje.Split('*')[0]));
                            if (numTablero >= 0)
                            {
                                tableros[numTablero].FinalizarPartida();
                                tableros[numTablero].Close();
                            }
                            if (mensaje.Split('*')[1] != username)
                                MessageBox.Show(mensaje.Split('*')[1] + " ha finalizado \nla partida " + mensaje.Split('*')[0]);
                            break;

                        case 11: //Notificación del resultado del dado de uno de los jugadores de la partida al resto
                            int idPartida = Convert.ToInt32(mensaje.Split('*')[0]);
                            numTablero = DamePosicionLista(tableros, idPartida);
                            tableros[numTablero].NuevoMovimiento(mensaje);
                            break;

                        case 12: //Notificación del movimiento de otro jugador

                            idPartida = Convert.ToInt32(mensaje.Split('*')[0]);
                            numTablero = DamePosicionLista(tableros, idPartida);
                            tableros[numTablero].setCasillaJugador(mensaje);
                            break;

                        case 13: //Notificacion del resultado de un jugador (0 - incorrecta, 1 - correcta, 2 - correcta y queso)

                            idPartida = Convert.ToInt32(mensaje.Split('*')[0]);
                            numTablero = DamePosicionLista(tableros, idPartida);
                            tableros[numTablero].ActualizarResultadoPregunta(mensaje);
                            break;

                        case 14: //Notificación que alguien ha conseguido todos los 6 quesitos 

                            idPartida = Convert.ToInt32(mensaje.Split('*')[0]);
                            numTablero = DamePosicionLista(tableros, idPartida);
                            tableros[numTablero].Ganador(mensaje);
                            break;

                        case 15: //Notifica mensaje en el chat

                            idPartida = Convert.ToInt32(mensaje.Split('*')[0]);
                            numTablero = DamePosicionLista(tableros, idPartida);
                            tableros[numTablero].NuevoMensajeChat(mensaje);
                            break;

                        case 16: // Notifica que la partida no comenzará porque X ha rechazado la invitacion

                            idPartida = Convert.ToInt32(mensaje.Split('*')[0]);
                            MessageBox.Show(mensaje.Split('*')[1] + " ha rechazado la partida\nNo se iniciará");
                            break;

                        case 17: //Eliminar cuenta de la base de datos

                            if (mensaje == "0")
                            {
                                MessageBox.Show("Usuario eliminado con éxito");
                                eliminarBox.Visible = false;
                                accederBox.Visible = true;
                                regVisible.Visible = true;
                                regLabel.Visible = true;
                                volverLbl.Visible = false;
                                eliminarLbl.Visible = true;
                                eliminarCuenta.Visible = true;
                                usuarioEliminado.Clear();
                                contrasenyaEliminado.Clear();
                            }
                            else if (mensaje == "-1")

                                MessageBox.Show("Error al eliminar el usuario");
                    
                            else if (mensaje == "1")
                            {
                                MessageBox.Show("El usuario que quiere eliminar no existe");
                                usuarioEliminado.Clear();
                                contrasenyaEliminado.Clear();
                            }
                            else if (mensaje == "2")
                            {
                                MessageBox.Show("Contraseña incorrecta");
                                contrasenyaEliminado.Clear();
                            }
                            else
                            {
                                MessageBox.Show("El usuario esta conectado.\n Para eliminar un usuario, este no puede estar online");
                                usuarioEliminado.Clear();
                                contrasenyaEliminado.Clear();
                            } 
                            fin = true;
                            break;
                        case 18: //Consulta que retorna las partidas jugadas en la fecha y su duración
                            if (mensaje == "-1")
                                MessageBox.Show("Error en la consulta, vuelva a intentarlo");
                            else
                            {
                                this.formConsultas = new FormConsultas();
                                this.formConsultas.SetPregunta(fechaBtn.Text);
                                this.formConsultas.SetDataGrid(codigo, mensaje);
                                this.formConsultas.ShowDialog();
                            }
                            
                            break;
                    }
                }
                catch (SocketException)
                {
                    MessageBox.Show("Server desconectado");
                }
            }
            //Mensaje de desconexion
            byte[] msg0 = System.Text.Encoding.ASCII.GetBytes("0/");
            server.Send(msg0);

            //Desconexión del servidor
            server.Shutdown(SocketShutdown.Both);
            server.Close();
        }

        //Funcion para ejecutar un nuevo thread con el formulario de una partida
        private void NuevaPartida(string mensaje)
        {
            //Creamos el nuevo formulario tablero para la nueva partida
            FormJuego tablero = new FormJuego();
            tablero.SetPartida(mensaje,this.server,this.username);
            tableros.Add(tablero);
            tablero.ShowDialog();
            tableros.Remove(tablero);

            //Acaba el thread para esta partida
        }

        //Iniciacion del Form 
        private void Acceso_Load(object sender, EventArgs e)
        {
            //Partes ocultas al inicio
            registroBox.Visible = false;
            consultaBox.Visible = false;
            consultasButton.Visible = false;
            listaconGridView.Visible = false;
            labelConectados.Visible = false;
            invitarButton.Visible = false;
            conexion.Visible = false;
            invitadosGridView.Visible = false;
            label6.Visible = false;
            inicio.Visible = false;
            eliminarBox.Visible = false;
            inv_lbl.Visible = false;

            //Fondo
            candadoBox.Image = Image.FromFile(".\\candadoCerrado.jpg");
            candadoBox.SizeMode = PictureBoxSizeMode.StretchImage;

            //Fondo
            candadoEliminado.Image = Image.FromFile(".\\candadoCerrado.jpg");
            candadoEliminado.SizeMode = PictureBoxSizeMode.StretchImage;
        }
        //Botón de conexión/desconexión (en la version final SOLO se utiliza el DESCONECTAR, puesto que ya se conecta al iniciar sesión)
        private void conexion_Click(object sender, EventArgs e)
        {
            //Caso Conectado y queremos desconectarnos
            if (conn==1)
            {
                try
                {
                    //Cerramos todos los tableros que haya abiertos
                    for (int i = 0; i < tableros.Count; i++)
                        tableros[i].Close();
                    tableros.Clear();

                    //Mensaje de desconexion
                    string mensaje = "0/";
                    byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                    server.Send(msg);

                    //Detenemos el thread
                    atender.Abort();

                    //Desconexión del servidor
                    server.Shutdown(SocketShutdown.Both);
                    server.Close();
                    conexion.Visible=false;
                    conn = 0;

                    //Cambios de color de fondos
                    this.BackColor = Color.DarkSlateGray;

                    //Establecemos pantalla inicial
                    Bitmap portada = new Bitmap(Application.StartupPath + @"\trivia.jpg");
                    this.BackgroundImage = portada;
                    this.BackgroundImageLayout = ImageLayout.Stretch;
                    consultaBox.Visible = false;
                    consultasButton.Visible = false;
                    accederBox.Visible = true;
                    registroBox.Visible = false;
                    listaconGridView.Visible = false;
                    labelConectados.Visible = false;
                    invitarButton.Text = "Invitar";
                    invitarButton.Visible = false;
                    regLabel.Visible = true;
                    regVisible.Visible = true;
                    eliminarLbl.Visible = true;
                    eliminarCuenta.Visible = true;
                    invitadosGridView.Visible = false;
                    label6.Visible = false;
                    inv_lbl.Visible = false;    


                    //Vaciamos las casillas por si habian quedado rellenadas
                    NameBox.Clear();
                    PasswordBox.Clear();

                }
                catch (Exception)
                {
                    MessageBox.Show("Ya estás desconectado.");
                }
            }
        }
        //Botón para iniciar sesion
        private void Login_Click(object sender, EventArgs e)
        {
            try
            {
                //Se conecta al servidor solamente entrar

                IPAddress direc = IPAddress.Parse("192.168.56.101");
                IPEndPoint ipep = new IPEndPoint(direc, 9080);

                //@IP_Shiva1: 10.4.119.5
                //@IP_LocalHost: 192.168.56.102
                //#Port_Shiva1: 50055
                //#Port_localhost: 9080

                //Creamos el socket 
                this.server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
               
                server.Connect(ipep);

                //Ponemos en marcha el thread que atenderá los mensajes de los clientes
                ThreadStart ts = delegate { AtenderServidor(); };
                atender = new Thread(ts);
                atender.Start();
                
                username = NameBox.Text;
                //Construimos el mensaje y lo enviamos (Codigo 1 - Inicio sesion)
                string mensaje = "1/" + NameBox.Text + "/" + PasswordBox.Text;
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);

            }
            catch (SocketException)
            {
                MessageBox.Show("No he podido conectar con el servidor");
            }
            catch (Exception)
            {
                MessageBox.Show("ERROR: Compruebe que está conectado al servidor.");
            }

        }
        //Botón para registrarse 
        private void Registrarme_Click(object sender, EventArgs e)
        {
            try
            {
                //Creamos el socket y nos conectamos
                IPAddress direc = IPAddress.Parse("192.168.56.101");
                IPEndPoint ipep = new IPEndPoint(direc, 9080);
                this.server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                server.Connect(ipep);

                //@IP_Shiva1: 10.4.119.5
                //@IP_LocalHost: 192.168.56.102
                //#Port_Shiva1: 50055
                //#Port_localhost: 9080

                //Abrimos el thread
                ThreadStart ts = delegate { AtenderServidor(); };
                atender = new Thread(ts);
                atender.Start();

                //Construimos el mensaje y lo enviamos (Codigo 2 - Registrarse)
                if ((userBox.Text != "0") && (password2Box.Text != "0"))
                {
                    string mensaje = "2/" + userBox.Text + "/" + password2Box.Text + "/" + mailBox.Text;
                    byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                    server.Send(msg);
                }
                else
                    MessageBox.Show("Ningún campo puede ser 0");              

            }
            catch (Exception)
            {
                MessageBox.Show("ERROR: Compruebe que está conectado al servidor.");
            }
        }
        

       
        //Mostrar y ocultar las contraseñas.
        private void PasswordBox_TextChanged(object sender, EventArgs e)
        {
            PasswordBox.UseSystemPasswordChar = true;
            candadoBox.Image = Image.FromFile(".\\candadoCerrado.jpg");
        }

        //Cambio del pictureBox Candado en funcion si desea mostrar u ocultar la contraseña
        private void candadoBox_Click(object sender, EventArgs e)
        {
            //Desea poder ver la contraseña
            if (PasswordBox.UseSystemPasswordChar == true)
            {
                PasswordBox.UseSystemPasswordChar = false;
                candadoBox.Image = Image.FromFile(".\\candadoAbierto.jpg");
            }

            //Desea ocultar la contraseña
            else
            {
                PasswordBox.UseSystemPasswordChar = true;
                candadoBox.Image = Image.FromFile(".\\candadoCerrado.jpg");
            }
        }
        //Boton para desplegar/esconder las posibles consultas (para que quede mas ordenado)
        private void consultasButton_Click(object sender, EventArgs e)
        {
            //Si las consultas estan desplegadas y queremos esconderlas
            if (x==0)
            {
                consultaBox.Visible = false;
                consultasButton.Text = "Mostrar\n consultas";
                x = 1;
            }
            //Si las consultas estas escondidas y queremos desplegarlas
            else
            {
                consultaBox.Visible = true;
                consultasButton.Text = "Ocultar\n consultas";
                x = 0;
            }
        }
        //Si cerramos el form directamente tambien tenemos que desconectar el socket y detener el thread
        private void Acceso_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (conn == 1) {

                    //Mensaje de desconexion
                    string mensaje = "0/";
                    byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                    server.Send(msg);

                    //Detención del thread
                    atender.Abort();

                    //Desconexión del servidor
                    server.Shutdown(SocketShutdown.Both);
                    server.Close();

                    //Cerramos todos los tableros que haya abiertos
                    for (int i = 0; i < tableros.Count; i++)
                        tableros[i].Close();
                    tableros.Clear();
                }
            }
            catch (Exception)
            {
             
            }
        }

        private void ConectadosGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                string invitado = listaconGridView.CurrentCell.Value.ToString();

                //Comprobamos que no somos nosotros mismos
                if (invitado == username)
                {
                    MessageBox.Show("No te puedes autoinvitar");
                }
                else
                {
                    if (invitadosGridView.Visible == false)
                    {
                        invitados = new List<string>();
                        invitadosGridView.Visible = true;
                        invitarButton.Visible = true;
                        label6.Visible = true;
                    }

                    if (invitados.Count <= 3)
                    {

                        //Comprobamos que no este ya en la lista para añadirlo
                        int i = 0;
                        bool encontrado = false;
                        while ((i < invitados.Count) && (encontrado == false))
                        {
                            if (invitado == invitados[i])
                                encontrado = true;

                            else
                                i = i + 1;
                        }
                        if (encontrado == false)
                        {
                            invitados.Add(invitado);
                            CrearInvitadosGridView(invitados);
                        }
                    }
                    else
                        MessageBox.Show("El numero máximo de invitados es 3");
                }
                listaconGridView.SelectAll();


            }
            catch (NullReferenceException)
            {

            }   
            
        }

        private void CrearInvitadosGridView(List<string> invitados)
        {
            invitadosGridView.ColumnCount = 1;
            invitadosGridView.RowCount = invitados.Count;
            invitadosGridView.ColumnHeadersVisible = false;
            invitadosGridView.RowHeadersVisible = false;
            invitadosGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            invitadosGridView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            invitadosGridView.SelectAll();
            invitadosGridView.BackgroundColor = Color.White;

            int totalRowHeight = invitadosGridView.ColumnHeadersHeight;
            for (int i = 0; i < invitados.Count; i++)
            {
                invitadosGridView.Rows[i].Cells[0].Value = invitados[i];
                totalRowHeight += invitadosGridView.Rows[i].Height;
            }
            invitadosGridView.Height = totalRowHeight;
            invitadosGridView.Height = invitadosGridView.Height + 5;

        }
        private void invitarButton_Click(object sender, EventArgs e)
        {
            //Construimos el mensaje
            string mensaje = "6/";
            for (int i = 0; i < invitados.Count; i++)
            {
                mensaje = mensaje + invitados[i] + "*";
            }
            mensaje = mensaje.Remove(mensaje.Length - 1);

            //Lo enviamos por el socket (Codigo 6 - Invitar a jugadores)
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);
            label6.Visible = false;
            invitadosGridView.Visible = false;
            invitarButton.Visible = false;
        }

        private void invitadosGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                string eliminado = invitadosGridView.CurrentCell.Value.ToString();
                invitados.Remove(eliminado);
                if (invitados.Count == 0)
                {
                    invitadosGridView.Visible = false;
                    invitarButton.Visible = false;
                    label6.Visible = false;
                }
                else
                    CrearInvitadosGridView(invitados);
            }
            catch (NullReferenceException)
            {

            }
        }

        private void regVisible_Click(object sender, EventArgs e)
        {
            registroBox.Visible = true;
            regVisible.Visible = false;
            accederBox.Visible = false; 
            regLabel.Visible = false;
            inicio.Visible=true;
            eliminarLbl.Visible = false;
            eliminarCuenta.Visible = false;
        }

        private void inicio_Click(object sender, EventArgs e)
        {
            registroBox.Visible = false;
            inicio.Visible = false;
            regVisible.Visible = true;
            accederBox.Visible = true;
            regLabel.Visible = true;
            eliminarLbl.Visible = true;
            eliminarCuenta.Visible = true;
        }

        private void eliminarCuenta_Click(object sender, EventArgs e)
        {
            eliminarBox.Visible = true;
            eliminarLbl.Visible = false;
            eliminarCuenta.Visible = false;
            regVisible.Visible = false;
            accederBox.Visible = false;
            regLabel.Visible = false;
            volverLbl.Visible = true;
        }

        private void volverLbl_Click(object sender, EventArgs e)
        {
            eliminarBox.Visible = false;
            accederBox.Visible = true;
            regVisible.Visible = true;
            regLabel.Visible = true;
            volverLbl.Visible = false;
            eliminarLbl.Visible = true;
            eliminarCuenta.Visible = true;
        }

        //Eliminar usuario de la base de datos
        private void eliminarBtn_Click(object sender, EventArgs e)
        {
            if (usuarioEliminado.Text == "0")
            {
                MessageBox.Show("Nombre de usuario invalido");
            }
            else
            {
                try
                {
                    //Creamos el socket y nos conectamos
                    IPAddress direc = IPAddress.Parse("192.168.56.101");
                    IPEndPoint ipep = new IPEndPoint(direc, 9080);
                    this.server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    server.Connect(ipep);

                    //@IP_Shiva1: 10.4.119.5
                    //@IP_LocalHost: 192.168.56.102
                    //#Port_Shiva1: 50055
                    //#Port_localhost: 9080

                    //Abrimos el thread
                    ThreadStart ts = delegate { AtenderServidor(); };
                    atender = new Thread(ts);
                    atender.Start();

                    //Enviamos el mensaje de eliminar jugador de BBDD
                    string mensaje = "13/"+usuarioEliminado.Text+"/"+contrasenyaEliminado.Text;
                    byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                    server.Send(msg);
                }
                catch (SocketException)
                {
                    MessageBox.Show("Servidor no disponible");
                }
            }
            
        }

        private void candadoEliminado_Click(object sender, EventArgs e)
        {
            //Desea poder ver la contraseña
            if (contrasenyaEliminado.UseSystemPasswordChar == true)
            {
                contrasenyaEliminado.UseSystemPasswordChar = false;
                candadoEliminado.Image = Image.FromFile(".\\candadoAbierto.jpg");
            }

            //Desea ocultar la contraseña
            else
            {
                contrasenyaEliminado.UseSystemPasswordChar = true;
                candadoEliminado.Image = Image.FromFile(".\\candadoCerrado.jpg");
            }
        }

        private void pregBtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (ConQuienLbl.Checked)
                {
                    //Construimos el mensaje y lo enviamos para saber con quien he jugado
                    string mensaje = "3/" + NameBox.Text;
                    byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                    server.Send(msg);

                }
                else if (companyia.Checked)
                {
                    //Construimos el mensaje y lo enviamos para saber qué partidas he jugado con ellos
                    string mensaje = "4/" + nombresBox.Text;
                    byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                    server.Send(msg);
                }
                else if (jugMaxBtn.Checked)
                {
                    //Construimos el mensaje y lo enviamos. Queremos saber el JMV
                    string mensaje = "5/";
                    byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                    server.Send(msg);
                }
                else //Construimos el mensaje y lo enviamos para saber las partidas jugadas durante un día en concreto
                {
                    string fecha = dateTimePicker.Value.ToString().Split(' ')[0];
                    string[] fechas = fecha.Split('/');
                    int año = Convert.ToInt32(fechas[2]) - 2000;
                    string mensaje = "14/" + fechas[0] + "/" + fechas[1] + "/" + año.ToString();
                    byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                    server.Send(msg);

                }
            }
            catch (Exception)
            {
                MessageBox.Show("ERROR: Compruebe que está conectado al servidor.");
            }
        }

    }
    
}
