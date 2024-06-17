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
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Diagnostics;
using System.Xml.Linq;


namespace BDFARMACIA
{
    public partial class Factura : Form
    {
        Conexion conMysql = new Conexion();
        public Factura()
        {
            InitializeComponent();
        }

        private void Factura_Load(object sender, EventArgs e)
        {
            dataGridView1.ColumnCount = 6;
            dataGridView1.ColumnHeadersVisible = true;

            DataGridViewCellStyle columnHeaderStyle = new DataGridViewCellStyle();
            columnHeaderStyle.Font = new System.Drawing.Font("Verdana", 8, System.Drawing.FontStyle.Bold);
            columnHeaderStyle.BackColor = Color.Beige;

            dataGridView1.ColumnHeadersDefaultCellStyle = columnHeaderStyle;

            dataGridView1.Columns[0].Name = "Cliente";
            dataGridView1.Columns[1].Name = "Producto";
            dataGridView1.Columns[2].Name = "Precio";
            dataGridView1.Columns[3].Name = "Cantidad";
            dataGridView1.Columns[4].Name = "Total";
            dataGridView1.Columns[5].Name = "id_producto"; // Columna para el ID del producto (oculta)


            dataGridView1.Columns[0].Width = 80;
            dataGridView1.Columns[1].Width = 90;
            dataGridView1.Columns[2].Width = 120;
            dataGridView1.Columns[3].Width = 60;
            dataGridView1.Columns[4].Width = 140;
            dataGridView1.Columns[5].Visible = false; // Ocultar la columna del ID del producto

            // Columna para editar
            DataGridViewButtonColumn editarButtonColumn = new DataGridViewButtonColumn();
            editarButtonColumn.Name = "Editar";
            editarButtonColumn.HeaderText = "Editar";
            editarButtonColumn.Text = "Editar";
            editarButtonColumn.UseColumnTextForButtonValue = true;
            dataGridView1.Columns.Add(editarButtonColumn);

            // Columna para eliminar
            DataGridViewButtonColumn eliminarButtonColumn = new DataGridViewButtonColumn();
            eliminarButtonColumn.Name = "Eliminar";
            eliminarButtonColumn.HeaderText = "Eliminar";
            eliminarButtonColumn.Text = "Eliminar";
            eliminarButtonColumn.UseColumnTextForButtonValue = true;
            dataGridView1.Columns.Add(eliminarButtonColumn);

            conMysql.Connect();
            String sql = "select id, Nombre from clientes";

            String sql2 = "select id_producto,Producto from productos";

            conMysql.CargarCombo(comboBoxcliente, sql, "Nombre", "id");
            conMysql.CargarCombo(comboBoxproducto, sql2, "Producto", "id_producto");
        }



        

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex >= 0 && dataGridView1.Columns[e.ColumnIndex] is DataGridViewButtonColumn && dataGridView1.Columns[e.ColumnIndex].HeaderText == "Editar")
            {
                int rowIndex = e.RowIndex;
                EditForm editForm = new EditForm(dataGridView1.Rows[rowIndex]);

                // Suscribirte al evento ProductoEditado del formulario de edición
                editForm.ProductoEditado += (s, args) =>
                {
                    // Actualizar los datos del producto en el dataGridView1
                    dataGridView1.Rows[rowIndex].Cells["Producto"].Value = args.Nombre;
                    dataGridView1.Rows[rowIndex].Cells["Precio"].Value = args.Precio;
                    dataGridView1.Rows[rowIndex].Cells["Cantidad"].Value = args.Cantidad;
                    dataGridView1.Rows[rowIndex].Cells["Total"].Value = args.Total;
                };

                editForm.ShowDialog();
            }

            // Verificar si el clic fue en una celda de botón y si es la columna de eliminar
            if (e.ColumnIndex >= 0 && dataGridView1.Columns[e.ColumnIndex] is DataGridViewButtonColumn && dataGridView1.Columns[e.ColumnIndex].HeaderText == "Eliminar")
            {
                // Obtener la fila en la que se hizo clic
                int rowIndex = e.RowIndex;

                // Mostrar un mensaje de confirmación antes de eliminar el producto
                DialogResult result = MessageBox.Show("¿Estás seguro de que deseas eliminar este producto?", "Confirmar eliminación", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    // Eliminar la fila correspondiente del dataGridView1
                    dataGridView1.Rows.RemoveAt(rowIndex);
                }
            }
        }




        private void buttonagregar_Click(object sender, EventArgs e)
        {
            agregarcarrito();
        }

        private void buttontotalcompra_Click(object sender, EventArgs e)
        {
            totalCuenta();
        }

        private void Guardar_Click(object sender, EventArgs e)
        {
            guardar();

        }

        private void buttonsalir_Click(object sender, EventArgs e)
        {
            limpiar();
        }

        public void limpiar()
        {
            textBoxunidad.Text = "";
            textBoxtotal.Text = "";

            comboBoxcliente.Text = "";
            comboBoxproducto.Text = "";
            dataGridView1.DataSource = "";
        }



        public void totalCuenta()
        {
            double sumatoria = 0;

            if (dataGridView1 != null && dataGridView1.Rows != null)
            {
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (row.Cells["Total"].Value != null)
                    {
                        sumatoria += Convert.ToDouble(row.Cells["Total"].Value);
                    }
                }
            }

            textBoxtotal.Text = Convert.ToString(sumatoria);
        }


        public void agregarcarrito()
        {
            // Desenlazar el DataGridView antes de agregar filas
            dataGridView1.DataSource = null;

            // Verificar si se ha ingresado una cantidad válida
            if (string.IsNullOrEmpty(textBoxunidad.Text))
            {
                MessageBox.Show("Ingrese una cantidad válida.");
                return;
            }

            // Verificar si se ha seleccionado un producto válido
            if (comboBoxproducto.SelectedIndex == -1)
            {
                MessageBox.Show("Seleccione un producto válido.");
                return;
            }

            // Obtener el precio del producto
            String sql3 = "select id_producto, Precio from productos where id_producto = " + comboBoxproducto.SelectedValue;
            DataRow producto = conMysql.getRow(sql3);

            decimal total = 0;
            decimal unidad = decimal.Parse(textBoxunidad.Text);
            var precio_unidad = (decimal)producto["Precio"];
            total = precio_unidad * unidad;

            string productoId = producto["id_producto"].ToString();
            string nombreCliente = comboBoxcliente.Text;

            // Agregar el producto como una nueva fila en el DataGridView
            dataGridView1.Rows.Add(nombreCliente, comboBoxproducto.Text, precio_unidad, unidad, total, productoId);

        }



        public void guardar()
        {
            // Verificar si alguno de los campos requeridos está vacío
            if (string.IsNullOrEmpty(textBoxunidad.Text.Trim()) || string.IsNullOrEmpty(textBoxtotal.Text.Trim()))
            {
                MessageBox.Show("Error: Rellene todos los campos.");
                return;
            }

            // Obtener la fecha actual
            dateTimePicker1.Value = DateTime.Now;

            // Construir la consulta para insertar la factura en la tabla factura
            string sqlFactura = string.Format("INSERT INTO factura (fecha,Total, Vendedor) VALUES('{0}','{1}', '{2}')",
                                              dateTimePicker1.Value.ToString("yyyy-MM-dd"),
                                              textBoxtotal.Text.Trim(),
                                              INICIARSESION.UsuarioActual);

            try
            {
                // Ejecutar la consulta para insertar la factura
                if (conMysql.Query(sqlFactura) == 1)
                {
                    MessageBox.Show("Se realizó el Ticket");
                }
                else
                {
                    MessageBox.Show("No se realizó el Ticket");
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }

            // Obtener el ID de la factura recién insertada
            DataRow busq1 = conMysql.getRow("SELECT MAX(id) FROM factura");

            // Construir la consulta para insertar los productos comprados en la tabla factura1
            string sqlDetalleFactura = string.Format("INSERT INTO factura1 (Id_Factura,id_producto,Cantidad) VALUES('{0}','{1}','{2}')",
                                                     busq1[0], comboBoxcliente.SelectedValue, textBoxunidad.Text.Trim());

            try
            {
                // Ejecutar la consulta para insertar los productos comprados
                if (conMysql.Query(sqlDetalleFactura) == 1)
                {
                    // Generar el PDF solo si hay datos en el dataGridView1
                    if (dataGridView1 != null)
                    {
                        generarPDF(busq1[0].ToString(), comboBoxcliente.Text, dataGridView1, dateTimePicker1.Value);
                    }

                    limpiar();
                }
                else
                {
                    MessageBox.Show("Error al guardar el detalle de la factura.");
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }

            // Actualizar la cantidad de existencias de los productos comprados
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                string productoId = row.Cells["id_producto"].Value.ToString();
                string cantidadComprada = row.Cells["Cantidad"].Value.ToString();

                // Construir la consulta para restar la cantidad comprada de la existencia del producto
                string sqlActualizarCantidad = $"UPDATE productos SET Cantidad_Existencia = Cantidad_Existencia - {cantidadComprada} WHERE id_producto = {productoId}";

                try
                {
                    // Ejecutar la consulta para actualizar la cantidad de existencias
                    int rowsAffected = conMysql.Query(sqlActualizarCantidad);

                    // Verificar si se afectaron filas
                    if (rowsAffected == 0)
                    {
                        MessageBox.Show($"No hay suficientes existencias del producto con ID {productoId} para realizar la venta.");
                        return;
                    }
                }
                catch (MySql.Data.MySqlClient.MySqlException ex)
                {
                    MessageBox.Show("Error al actualizar la cantidad de productos: " + ex.Message);
                }
            }
        }







        public void generarPDF(string numeroFactura, string cliente, DataGridView dataGridView, DateTime fecha)
        {
            if (dataGridView == null || dataGridView.Rows == null || dataGridView.Rows.Count == 0)
            {
                MessageBox.Show("No hay datos para generar el PDF.");
                return;
            }

            Document doc = new Document();
            try
            {
                // Verificar existencias antes de intentar actualizar
                foreach (DataGridViewRow row in dataGridView.Rows)
                {
                    if (row.IsNewRow)
                        continue;

                    string productoId = row.Cells["id_producto"].Value?.ToString();
                    string cantidadComprada = row.Cells["Cantidad"].Value?.ToString();

                    if (productoId == null || cantidadComprada == null)
                    {
                        MessageBox.Show("Error al obtener los datos del producto.");
                        return;
                    }

                    // Construir la consulta para restar la cantidad comprada de la existencia del producto
                    string sqlActualizarCantidad = $"UPDATE productos SET Cantidad_Existencia = Cantidad_Existencia - {cantidadComprada} WHERE id_producto = {productoId}";

                    try
                    {
                        // Ejecutar la consulta para actualizar la cantidad de existencias
                        int rowsAffected = conMysql.Query(sqlActualizarCantidad);

                        // Verificar si se afectaron filas
                        if (rowsAffected == 0)
                        {
                            MessageBox.Show($"No hay suficientes existencias del producto con ID {productoId} para realizar la venta.");
                            return;
                        }
                    }
                    catch (MySql.Data.MySqlClient.MySqlException ex)
                    {
                        MessageBox.Show("Error al actualizar la cantidad de productos: " + ex.Message);
                        return;
                    }
                }

                // Generar el PDF
                PdfWriter.GetInstance(doc, new FileStream("Ticket.pdf", FileMode.Create));
                doc.Open();

                // Agregar el logotipo de tu farmacia al PDF
                try
                {
                    iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance("C:\\Users\\coros\\OneDrive\\Imágenes\\Logotipo psicologa moderno verde y azul.png");
                    logo.ScaleToFit(440, 100); // Cambia width y height para ajustar el tamaño del logotipo

                    float x = (doc.PageSize.Width - 60) / 4;
                    float y = doc.PageSize.Height - 120; // Ajusta el margen superior según sea necesario

                    logo.SetAbsolutePosition(x, y);
                    doc.Add(logo);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al agregar el logotipo al PDF: " + ex.Message);
                }

                // Agregar contenido al PDF
                Paragraph p = new Paragraph(
                    "\n\n\n\n\n" +
                    "TICKET DE COMPRA\n\n" +
                    " Registro de ticket éxitoso \n" +
                    "Vendedor: " + INICIARSESION.UsuarioActual + "\n" +
                    " Numero Ticked: " + numeroFactura + "\n" +
                    " Fecha: " + fecha.ToString() + "\n" +
                    " Cliente: " + cliente + "\n\n"
                );
                doc.Add(p);

                // Detalles de los productos
                PdfPTable tabla = new PdfPTable(4);
                tabla.WidthPercentage = 100;
                tabla.SetWidths(new float[] { 2, 3, 2, 2 });
                tabla.AddCell("Producto");
                tabla.AddCell("Precio");
                tabla.AddCell("Cantidad");
                tabla.AddCell("Subtotal");

                double totalCompra = 0; // Variable para almacenar el total de la compra

                foreach (DataGridViewRow row in dataGridView.Rows)
                {
                    if (!row.IsNewRow) // Verificar si la fila no es una fila nueva
                    {
                        string producto = row.Cells["Producto"].Value?.ToString() ?? "";
                        string precio = row.Cells["Precio"].Value?.ToString() ?? "";
                        string cantidad = row.Cells["Cantidad"].Value?.ToString() ?? "";
                        string total = row.Cells["Total"].Value?.ToString() ?? "";

                        tabla.AddCell(producto);
                        tabla.AddCell("$ " + precio);
                        tabla.AddCell(cantidad);
                        tabla.AddCell("$ " + total);

                        // Sumar el total de este producto al total de la compra
                        if (!string.IsNullOrEmpty(total))
                        {
                            totalCompra += Convert.ToDouble(total);
                        }
                    }
                }

                // Agregar fila con el total de la compra
                iTextSharp.text.Font fontBold = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10f, iTextSharp.text.Font.BOLD);
                PdfPCell cellTotal = new PdfPCell(new Phrase("Total de la compra:", fontBold));
                cellTotal.Colspan = 3;
                cellTotal.HorizontalAlignment = Element.ALIGN_RIGHT;
                tabla.AddCell(cellTotal);

                PdfPCell cellTotalValue = new PdfPCell(new Phrase("$ " + totalCompra.ToString("0.00"), fontBold));
                cellTotalValue.HorizontalAlignment = Element.ALIGN_RIGHT;
                tabla.AddCell(cellTotalValue);

                doc.Add(tabla);

                // Mensaje de agradecimiento
                Paragraph agradecimiento = new Paragraph("\nGRACIAS POR SU COMPRA");
                doc.Add(agradecimiento);

                MessageBox.Show("¿Desea imprimir el ticket ahora?");

                // Abrir el archivo PDF con el visor de PDF predeterminado del sistema
                Process.Start("Ticket.pdf");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al generar el PDF: " + ex.Message);
            }
            finally
            {
                doc.Close();
            }
        }



        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }


        private void textBoxunidad_KeyPress_1(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
                 (e.KeyChar != '.'))
            {
                e.Handled = true;
            }
        }



        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void buttonImprimirFact_Click(object sender, EventArgs e)
        {
            //generarPDF();
        }




        private void comboBoxcliente_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBoxproducto_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void textBoxunidad_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBoxtotal_TextChanged(object sender, EventArgs e)
        {

        }

    }
}


public class EditForm : Form

{
    private TextBox textBoxNombre;
    private TextBox textBoxPrecio;
    private TextBox textBoxCantidad;
    private Button buttonGuardar;


    private DataGridViewRow selectedRow;
    // Define un evento que se activará cuando se edite un producto
    public event EventHandler<ProductoEditadoEventArgs> ProductoEditado;

    // Método para invocar el evento ProductoEditado
    protected virtual void OnProductoEditado(ProductoEditadoEventArgs e)
    {
        ProductoEditado?.Invoke(this, e);
    }
    public EditForm(DataGridViewRow row)
    {
        selectedRow = row;
        InitializeComponents();
    }

    // Clase para contener la información del producto editado
    public class ProductoEditadoEventArgs : EventArgs
    {
        public string Nombre { get; set; }
        public decimal Precio { get; set; }
        public int Cantidad { get; set; }
        public decimal Total { get; set; }

        // Constructor
        public ProductoEditadoEventArgs(string nombre, decimal precio, int cantidad, decimal total)
        {
            Nombre = nombre;
            Precio = precio;
            Cantidad = cantidad;
            Total = total;
        }
    }



    private void InitializeComponents()
    {
        // Crear y configurar los TextBox para editar los datos del producto
        textBoxNombre = new TextBox();
        textBoxNombre.Text = selectedRow.Cells["Producto"].Value.ToString();
        textBoxNombre.Location = new System.Drawing.Point(10, 10);
        textBoxNombre.Width = 200;
        Controls.Add(textBoxNombre);

        textBoxPrecio = new TextBox();
        textBoxPrecio.Text = selectedRow.Cells["Precio"].Value.ToString();
        textBoxPrecio.Location = new System.Drawing.Point(10, 40);
        textBoxPrecio.Width = 200;
        Controls.Add(textBoxPrecio);

        textBoxCantidad = new TextBox();
        textBoxCantidad.Text = selectedRow.Cells["Cantidad"].Value.ToString();
        textBoxCantidad.Location = new System.Drawing.Point(10, 70);
        textBoxCantidad.Width = 200;
        Controls.Add(textBoxCantidad);

        buttonGuardar = new Button();
        buttonGuardar.Text = "Guardar";
        buttonGuardar.Location = new System.Drawing.Point(10, 100);
        buttonGuardar.Click += buttonGuardar_Click; // Asignar el evento Click del botón
        Controls.Add(buttonGuardar);

        // Configurar el tamaño del formulario
        this.ClientSize = new System.Drawing.Size(250, 150);
    }
    private void buttonGuardar_Click(object sender, EventArgs e)
    {
        // Obtener los datos editados del producto
        string nombre = textBoxNombre.Text;
        decimal precio = decimal.Parse(textBoxPrecio.Text);
        int cantidad = int.Parse(textBoxCantidad.Text);
        decimal total = precio * cantidad;

        // Crear un objeto ProductoEditadoEventArgs con los nuevos datos
        ProductoEditadoEventArgs args = new ProductoEditadoEventArgs(nombre, precio, cantidad, total);

        // Invocar el evento ProductoEditado y pasar los datos editados
        OnProductoEditado(args);

        // Cerrar el formulario de edición
        this.Close();
    }

}
