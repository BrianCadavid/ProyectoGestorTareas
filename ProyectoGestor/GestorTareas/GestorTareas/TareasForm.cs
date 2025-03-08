using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GestorTareas
{
    public partial class TareasForm: Form
    {

        private string connectionString = "Server=(local)\\SQLEXPRESS;Database=TareasDB1;Integrated Security=True;";

        public TareasForm()
        {
            InitializeComponent();
            CargarDatosIniciales();
            ConfigurarDataGridView();
        }

        private void ConfigurarDataGridView()
        {
            // Limpia las columnas existentes
            dtgTareas.Columns.Clear();

            // Agrega las columnas manualmente
            dtgTareas.Columns.Add("colID", "ID");
            dtgTareas.Columns.Add("colTitulo", "Título");
            dtgTareas.Columns.Add("colDescripcion", "Descripción");
            dtgTareas.Columns.Add("colCategoria", "Categoría");
            dtgTareas.Columns.Add("colEstado", "Estado");
            dtgTareas.Columns.Add("colFechaVencimiento", "Fecha de Vencimiento");

            // Configura propiedades adicionales de las columnas
            dtgTareas.Columns["colID"].DataPropertyName = "ID";
            dtgTareas.Columns["colTitulo"].DataPropertyName = "titulo";
            dtgTareas.Columns["colDescripcion"].DataPropertyName = "descripcion";
            dtgTareas.Columns["colCategoria"].DataPropertyName = "categoria";
            dtgTareas.Columns["colEstado"].DataPropertyName = "estado";
            dtgTareas.Columns["colFechaVencimiento"].DataPropertyName = "fechaVencimiento";

            // Configura el formato de la columna de fecha
            dtgTareas.Columns["colFechaVencimiento"].DefaultCellStyle.Format = "dd/MM/yyyy";
        }


        private void CargarDatosIniciales()
        {
            try
            {
                LoadTareas();
                LoadCategorias();
                LoadEstados();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los datos iniciales: " + 
                                ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        private void LoadTareas()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                    SELECT 
                        T.ID, 
                        T.titulo, 
                        T.descripcion, 
                        C.nombre AS Categoria, 
                        E.nombreEstado AS Estado, 
                        T.fechaVencimiento 
                    FROM Tareas T 
                    INNER JOIN Categorias C ON T.categoriaId = C.ID 
                    INNER JOIN Estados E ON T.estadoId = E.ID";
                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    System.Data.DataTable dt = new System.Data.DataTable();
                    da.Fill(dt);
                    dtgTareas.DataSource = dt;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar las tareas: " + ex.Message,
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }
       

        private void LoadCategorias()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT ID, nombre FROM Categorias";
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                System.Data.DataTable dt = new System.Data.DataTable();
                da.Fill(dt);
                cmbCategoria.DataSource = dt;
                cmbCategoria.DisplayMember = "nombre";
                cmbCategoria.ValueMember = "ID";
            }
        }

        private void LoadEstados()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT ID, nombreEstado FROM Estados";
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                System.Data.DataTable dt = new System.Data.DataTable();
                da.Fill(dt);
                cmbEstado.DataSource = dt;
                cmbEstado.DisplayMember = "nombreEstado";
                cmbEstado.ValueMember = "ID";
            }
        }

        private void TareasForm_Load(object sender, EventArgs e)
        {
            
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            string titulo = txtTitulo.Text;
            string descripcion = txtDescripcion.Text;
            int categoriaId = (int)cmbCategoria.SelectedValue;
            int usuarioId = 1;
            int estadoId = (int)cmbEstado.SelectedValue;
            DateTime fechaVencimiento = dateTimePicker1.Value;

            if (usuarioId > 0)
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        string query = "INSERT INTO Tareas (titulo, descripcion, categoriaId, usuarioId, estadoId, fechaVencimiento) VALUES (@titulo, @descripcion, @categoriaId, @usuarioId, @estadoId, @fechaVencimiento)";
                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@titulo", titulo);
                        cmd.Parameters.AddWithValue("@descripcion", descripcion);
                        cmd.Parameters.AddWithValue("@categoriaId", categoriaId);
                        cmd.Parameters.AddWithValue("@usuarioId", usuarioId);
                        cmd.Parameters.AddWithValue("@estadoId", estadoId);
                        cmd.Parameters.AddWithValue("@fechaVencimiento", fechaVencimiento);
                        cmd.ExecuteNonQuery();

                        MessageBox.Show("Tarea guardada correctamente.");
                    }
                }
                catch (Exception ex)
                {


                    MessageBox.Show("Error al guardar la tarea: " + ex.Message);

                }
            }
            else
            {
                MessageBox.Show("No se ha proporcionado un id de usuario válido.");
            }
        }
            

           

       

        private void btnEditar_Click(object sender, EventArgs e)
        {
            // Verifica si se ha seleccionado una tarea en el DataGridView
            if (dtgTareas.CurrentRow == null)
            {
                MessageBox.Show("Por favor, selecciona una tarea para editar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Obtén el ID de la tarea seleccionada
            int id = (int)dtgTareas.CurrentRow.Cells[0].Value;

            // Obtén los valores de los controles del formulario
            string titulo = txtTitulo.Text;
            string descripcion = txtDescripcion.Text;
            int categoriaId = (int)cmbCategoria.SelectedValue;
            int estadoId = (int)cmbEstado.SelectedValue;
            DateTime fechaVencimiento = dateTimePicker1.Value;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                UPDATE Tareas 
                SET 
                    titulo = @titulo, 
                    descripcion = @descripcion, 
                    categoriaId = @categoriaId, 
                    estadoId = @estadoId, 
                    fechaVencimiento = @fechaVencimiento 
                WHERE ID = @id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@titulo", titulo);
                    cmd.Parameters.AddWithValue("@descripcion", descripcion);
                    cmd.Parameters.AddWithValue("@categoriaId", categoriaId);
                    cmd.Parameters.AddWithValue("@estadoId", estadoId);
                    cmd.Parameters.AddWithValue("@fechaVencimiento", fechaVencimiento);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Tarea editada correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadTareas(); // Actualiza el DataGridView después de editar
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al editar la tarea: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            int id = (int)dtgTareas.CurrentRow.Cells[0].Value;

            if (MessageBox.Show("¿Estás seguro de eliminar esta tarea?", "Confirmar", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "DELETE FROM Tareas WHERE ID = @id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }

                LoadTareas();
            }

        }

        private void cmbCategoria_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void dtgTareas_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btnCategorias_Click(object sender, EventArgs e)
        {
            this.Hide(); // Oculta el formulario actual
            using (CategoriasForm categoriasForm = new CategoriasForm())
            {
                categoriasForm.ShowDialog(); // Muestra el formulario de categorías
            }
            this.Show(); // Vuelve a mostrar el formulario actual al cerrar CategoriasForm
        }
    }
}
