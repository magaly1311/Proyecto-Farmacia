using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace BDFARMACIA
{
    public partial class INICIARSESION : Form
    {
        Conexion conMysql = new Conexion();
        public INICIARSESION()
        {
            InitializeComponent();
            try
            {
                conMysql.Connect();
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
    

        private void INICIARSESION_Load(object sender, EventArgs e)
        {

        }

        private void buttoningresar_Click(object sender, EventArgs e)
        {
            ingresar();
        }

        private void buttoncerrar_Click(object sender, EventArgs e)
        {
            Close();
        }
        public static string UsuarioActual { get; private set; }

        public void ingresar()
        {
            String sql1 = String.Format(@"SELECT Usuario, Contraseña, Rol FROM usuarios WHERE Usuario = '{0}' AND Contraseña = '{1}'", textBoxusuario.Text, textBoxcontraseña.Text);
            DataRow fila = conMysql.getRow(sql1);
            // Verifica el inicio de sesión y obtén el nombre de usuario si es exitoso


            if (fila != null)
            {
                string rol = fila["Rol"].ToString();
                UsuarioActual = textBoxusuario.Text; // Guarda el nombre de usuario en la variable estática
                MessageBox.Show(" BIENVENID@ A SaludTotal Pharma :) " + textBoxusuario.Text + "");
                //MenuPrincipal abrir = new MenuPrincipal();
                // abrir.Show();
                //this.Hide();

                // Verificar el rol para determinar qué formulario abrir
                if (rol == "Administrador")
                {
                    MenuPrincipal abrir = new MenuPrincipal();
                    abrir.Show();
                    this.Hide();
                }
                else if (rol == "Vendedor")
                {
                    MenuVendedores abrir = new MenuVendedores();
                    abrir.Show();
                    this.Hide();
                }
            }


            else
            {

                MessageBox.Show("Error verifique porfavor !!!");
            }
        }

        private void textBoxusuario_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
