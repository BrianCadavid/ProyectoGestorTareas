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
            dtgTareas.AllowUserToAddRows = false; // Evita que se agregue una fila en blanco automáticamente
            dtgTareas.DataError += (s, e) => { e.Cancel = true; }; // Previene errores de validación
            dtgTareas.CellValidating += dtgTareas_CellValidating;

        }
        private void TareasForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            dtgTareas.EndEdit();  // Finaliza la edición antes de cerrar
            this.Validate();       // Evita validaciones pendientes
        }
        private void ConfigurarDataGridView()
        {
            dtgTareas.Columns.Clear();

            dtgTareas.Columns.Add("colID", "ID");
            dtgTareas.Columns.Add("colTitulo", "Título");
            dtgTareas.Columns.Add("colDescripcion", "Descripción");
            dtgTareas.Columns.Add("colCategoria", "Categoría");
            dtgTareas.Columns.Add("colEstado", "Estado");
            dtgTareas.Columns.Add("colUsuario", "Usuario"); // 🔹 Nueva columna
            dtgTareas.Columns.Add("colFechaCreacion", "Fecha de Creación"); // 🔹 Nueva columna
            dtgTareas.Columns.Add("colFechaVencimiento", "Fecha de Vencimiento");

            dtgTareas.Columns["colID"].DataPropertyName = "ID";
            dtgTareas.Columns["colTitulo"].DataPropertyName = "titulo";
            dtgTareas.Columns["colDescripcion"].DataPropertyName = "descripcion";
            dtgTareas.Columns["colCategoria"].DataPropertyName = "Categoria";
            dtgTareas.Columns["colEstado"].DataPropertyName = "Estado";
            dtgTareas.Columns["colUsuario"].DataPropertyName = "Usuario"; // 🔹 Asociamos con la consulta
            dtgTareas.Columns["colFechaCreacion"].DataPropertyName = "fechaCreacion"; // 🔹 Asociamos con la consulta
            dtgTareas.Columns["colFechaVencimiento"].DataPropertyName = "fechaVencimiento";

            dtgTareas.Columns["colFechaCreacion"].DefaultCellStyle.Format = "dd/MM/yyyy";
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
                U.nombreUsuario AS Usuario, 
                T.fechaCreacion, 
                T.fechaVencimiento 
            FROM Tareas T 
            INNER JOIN Categorias C ON T.categoriaId = C.ID 
            INNER JOIN Estados E ON T.estadoId = E.ID
            INNER JOIN Usuarios U ON T.usuarioId = U.ID"; // 🔹 Unimos con Usuarios

                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    System.Data.DataTable dt = new System.Data.DataTable();
                    da.Fill(dt);
                    dtgTareas.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar las tareas: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            string titulo = txtTitulo.Text.Trim();
            string descripcion = txtDescripcion.Text.Trim();

            if (string.IsNullOrWhiteSpace(titulo) || string.IsNullOrWhiteSpace(descripcion) ||
                cmbCategoria.SelectedIndex == -1 || cmbEstado.SelectedIndex == -1)
            {
                MessageBox.Show("Todos los campos son obligatorios.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int categoriaId = (int)cmbCategoria.SelectedValue;
            int usuarioId = 1; // ⚠️ Si usas login, cambia esto por el ID del usuario autenticado
            int estadoId = (int)cmbEstado.SelectedValue;
            DateTime fechaVencimiento = dateTimePicker1.Value;
            DateTime fechaCreacion = DateTime.Now; // 🔹 Guardamos la fecha actual

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "INSERT INTO Tareas (titulo, descripcion, categoriaId, usuarioId, estadoId, fechaCreacion, fechaVencimiento) " +
                                   "VALUES (@titulo, @descripcion, @categoriaId, @usuarioId, @estadoId, @fechaCreacion, @fechaVencimiento)";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@titulo", titulo);
                    cmd.Parameters.AddWithValue("@descripcion", descripcion);
                    cmd.Parameters.AddWithValue("@categoriaId", categoriaId);
                    cmd.Parameters.AddWithValue("@usuarioId", usuarioId);
                    cmd.Parameters.AddWithValue("@estadoId", estadoId);
                    cmd.Parameters.AddWithValue("@fechaCreacion", fechaCreacion);
                    cmd.Parameters.AddWithValue("@fechaVencimiento", fechaVencimiento);
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Tarea guardada correctamente.");
                }
                LoadTareas();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar la tarea: " + ex.Message);
            }
        }
            

           

       

        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (dtgTareas.CurrentRow == null)
            {
                MessageBox.Show("Por favor, selecciona una tarea para editar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string titulo = txtTitulo.Text.Trim();
            string descripcion = txtDescripcion.Text.Trim();

            if (string.IsNullOrWhiteSpace(titulo) || string.IsNullOrWhiteSpace(descripcion) ||
                cmbCategoria.SelectedIndex == -1 || cmbEstado.SelectedIndex == -1)
            {
                MessageBox.Show("Todos los campos son obligatorios.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int id = (int)dtgTareas.CurrentRow.Cells[0].Value;
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
                LoadTareas();
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

            this.Show(); // Vuelve a mostrar el formulario actual
            LoadCategorias(); // Recarga las categorías en el ComboBox
        }

        private void dtgTareas_RowValidating(object sender, DataGridViewCellCancelEventArgs e)
        {
            // Evita validación si la fila es nueva o el formulario se está cerrando
            if (dtgTareas.Rows[e.RowIndex].IsNewRow || this.Disposing) return;

            // Verifica si hay celdas vacías en la fila
            foreach (DataGridViewCell cell in dtgTareas.Rows[e.RowIndex].Cells)
            {
                if (cell.Value == null || string.IsNullOrWhiteSpace(cell.Value.ToString()))
                {
                    MessageBox.Show("No se pueden guardar filas con campos vacíos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    e.Cancel = true; // Cancela la edición de la celda
                    return;
                }
            }
        }
        private void dtgTareas_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (dtgTareas.Columns[e.ColumnIndex].Name != "tuColumnaOpcional") // Evita validar columnas que puedan ser opcionales
            {
                if (string.IsNullOrWhiteSpace(e.FormattedValue.ToString()))
                {
                    MessageBox.Show("No se pueden guardar filas con campos vacíos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    e.Cancel = true; // Cancela la edición de la celda
                }
            }
        }

    }
}
