using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Diagnostics;


namespace BDFARMACIA
{
    public partial class RegistroVentasPorDia : Form
    {
        Conexion conMysql = new Conexion();

        public RegistroVentasPorDia()
        {
            InitializeComponent();

        }


        
            private void RegistroVentasPorDia_Load(object sender, EventArgs e)
            {
                // Obtener la fecha y hora actual
                DateTime fechaActual = DateTime.Now;

                // Configurar el control DateTimePicker para que muestre la fecha actual
                dateTimePickerFecha.Value = fechaActual; // Establecer solo la fecha, sin la hora

                // Mostrar las ventas del día actual al cargar el formulario
                MostrarVentasPorDia(fechaActual);
            }



        
        private void MostrarVentasPorDia(DateTime fechaActual)
        {
            // Limpiar el DataGridView
            dataGridViewVentas.Rows.Clear();


            // Consultar la base de datos para obtener las ventas realizadas en la fecha especificada
            string sql = "SELECT id, Fecha, Hora, Vendedor, Total FROM factura WHERE Fecha = '" + fechaActual.ToString("yyyy-MM-dd") + "'";


            DataTable dt = conMysql.getData(sql);





            // Verificar si se obtuvieron resultados
            if (dt != null && dt.Rows.Count > 0)
            {
                // Recorrer los resultados y agregarlos al DataGridView
                foreach (DataRow row in dt.Rows)
                {
                    dataGridViewVentas.Rows.Add(
                        row["id"],
                        row["Hora"],
                        row["Vendedor"],
                        row["Total"]);

                }
            }
            else
            {
                MessageBox.Show("No hay ventas registradas para el día seleccionado.");
            }

            // Calcular el total de ventas para el día seleccionado
            CalcularTotalVentas();
        }
        private void CalcularTotalVentas()
        {
            decimal totalVentas = 0;

            // Sumar los montos de todas las ventas mostradas en el DataGridView
            foreach (DataGridViewRow row in dataGridViewVentas.Rows)
            {
                totalVentas += Convert.ToDecimal(row.Cells["Total"].Value);
            }

            // Mostrar el total de ventas en el control TextBox
            textBoxTotalVentas.Text = totalVentas.ToString("C");
        }


        private void dateTimePickerFecha_ValueChanged(object sender, EventArgs e)
        {
            // Cuando cambia la fecha seleccionada, mostrar las ventas para esa fecha
            MostrarVentasPorDia(dateTimePickerFecha.Value);
        }

        //------------------


        private void dateTimePickerFecha_ValueChanged_1(object sender, EventArgs e)
        {
            // Cuando cambia la fecha seleccionada, mostrar las ventas para esa fecha
            MostrarVentasPorDia(dateTimePickerFecha.Value);
        }

        private void buttonTotalVentas_Click_1(object sender, EventArgs e)
        {
            CalcularTotalVentas();

        }

        private void buttonConsultaVentas_Click(object sender, EventArgs e)
        {

        }

        private void buttonConsultar_Click(object sender, EventArgs e)
        {
            // Obtener la fecha seleccionada del dateTimePickerFecha
            DateTime fechaActual = dateTimePickerFecha.Value;

            // Llamar al método MostrarVentasPorDia con la fecha obtenida
            MostrarVentasPorDia(fechaActual);
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void dataGridViewVentas_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

       


        




    }
}