﻿using System;
using MySql.Data.MySqlClient;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;


namespace BDFARMACIA
{
    internal class Conexion
    {

        const string HOST = "localhost";
        const string USER = "abi";
        const string PASS = "ochoceros";
        const string DB = "farmacia";

        MySqlConnection Ocon = new MySqlConnection();

        public Conexion()
        {
            this.Connect();
        }

        public void Connect()
        {

            if (Ocon.State == ConnectionState.Closed)
            {
                Ocon.ConnectionString = String.Format(@"Server={0}; Database={1}; User ID={2}; Password={3}; Pooling=false;", HOST, DB, USER, PASS);
                Ocon.Open();
            }

        }
        public MySqlConnection GetConnection()
        {
            return Ocon;
        }


        public T GetScalar<T>(string sql)
        {
            T result = default(T);

            using (MySqlCommand command = new MySqlCommand(sql, Ocon))
            {
                object scalarValue = command.ExecuteScalar();
                if (scalarValue != null)
                {
                    result = (T)Convert.ChangeType(scalarValue, typeof(T));
                }
            }

            return result;
        }

        public int getLastInsertId()
        {
            String sql = "SELECT LAST_INSERT_ID()";
            MySqlCommand command = new MySqlCommand(sql, Ocon);
            object result = command.ExecuteScalar();
            if (result != null)
            {
                return Convert.ToInt32(result);
            }
            else
            {
                return -1; // Indicar un valor de error en caso de fallo
            }
        }


        //Insert, Update, Delete
        public int Query(string sql)
        {
            MySqlCommand command = new MySqlCommand(sql, Ocon);
            return command.ExecuteNonQuery();
        }

        //Select
        public DataTable getData(string sql)
        {
            this.Connect();
            DataTable table = new DataTable();
            MySqlDataAdapter adapter = new MySqlDataAdapter(sql, Ocon);
            adapter.Fill(table);
            return table;
        }

        //Obtener una fila de la tabla retornada por getData
        public DataRow getRow(string sql)
        {
            DataRow row = null;
            if (this.getData(sql).Rows.Count == 0)
            {
                return null;
            }
            row = this.getData(sql).Rows[0];
            return row;
        }


        //Metodo para cargar comboBox
        public void CargarCombo(ComboBox cbo, String sql, String mostrar, String seleccionar)
        {
            this.Connect();
            DataTable datos = this.getData(sql);

            if (datos.Rows.Count > 0)
            {
                cbo.DataSource = null;
                cbo.DataSource = datos;
                cbo.DisplayMember = mostrar;
                cbo.ValueMember = seleccionar;
            }
            else
            {
                cbo.Text = "No hay registros";
                cbo.SelectedIndex = -1;
            }

        }

        // Ejemplo de implementación básica de un método GetScalar en la clase Conexion



    }
}
