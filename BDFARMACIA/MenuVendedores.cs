﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BDFARMACIA
{
    public partial class MenuVendedores : Form
    {
        public MenuVendedores()
        {
            InitializeComponent();
        }


        private void MenuVendedores_Load(object sender, EventArgs e)
        {

        }

        private void buttonproductos_Click(object sender, EventArgs e)
        {
            Medicamentos abrir = new Medicamentos();
            abrir.Show();
        }

        private void buttonfactura_Click(object sender, EventArgs e)
        {
            Factura abrir = new Factura();
            abrir.Show();
        }

        private void buttonInicioSesion_Click(object sender, EventArgs e)
        {
            // Crear una instancia del formulario de inicio de sesión y mostrarlo
            INICIARSESION abrirInicioSesion = new INICIARSESION();
            abrirInicioSesion.Show();
            this.Close();
        }

        private void button1Manual_Click(object sender, EventArgs e)
        {
            Process.Start(@"C:\Users\coros\source\repos\BDFARMACIA-1-\BDFARMACIA\bin\Debug\MANUAL DE FUNCIONAMIENTO DEL SISTEMA POS.pdf");

            try
            {
                // Mostrar un mensaje indicando que se está abriendo el archivo
                MessageBox.Show("El archivo PDF se está abriendo...");


            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo abrir el archivo PDF: " + ex.Message);
            }
        }
    }
}
