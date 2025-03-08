using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GestorTareas
{
    public partial class CategoriasForm: Form
    {
        private string connectionString = "Server=(local)\\SQLEXPRESS;Database=TareasDB1;Integrated Security=True;";
        public CategoriasForm()
        {
            InitializeComponent();
            LoadCategorias();
            dtgCategorias.AllowUserToAddRows = false;
        }


        private int categoriaIdSeleccionada = -1; // ID de la categoría en edición

        private void CategoriasForm_Load(object sender, EventArgs e)
        {
            TestConexion();
            
        }

        private void TestConexion()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
               //     MessageBox.Show("Conexión exitosa con la base de datos.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error de conexión: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }


        private void LoadCategorias()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT id, nombre, descripcion, fechaCreacion FROM Categorias";
                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dtgCategorias.Columns.Clear();
                    dtgCategorias.DataSource = dt;

                    // 🔹 Evitar filas vacías
                    foreach (DataGridViewRow row in dtgCategorias.Rows)
                    {
                        if (row.IsNewRow) continue; // Ignorar fila de nueva entrada
                        if (row.Cells["nombre"].Value == null || row.Cells["descripcion"].Value == null)
                        {
                            dtgCategorias.Rows.Remove(row); // Eliminar fila vacía
                        }
                    }

                    dtgCategorias.Refresh();
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Error SQL: " + ex.Message, "Error SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error general: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        

        private void label2_Click(object sender, EventArgs e)
        {

        }

      

       

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            try
            {
                string nombre = txtNombre.Text.Trim();
                string descripcion = txtDescripcion.Text.Trim();

                // Validar que los campos no estén vacíos
                if (string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(descripcion))
                {
                    MessageBox.Show("Los campos Nombre y Descripción no pueden estar vacíos.",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return; // Detener la ejecución si los campos están vacíos
                }

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "INSERT INTO Categorias (nombre, descripcion) VALUES (@nombre, @descripcion)";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@nombre", nombre);
                    cmd.Parameters.AddWithValue("@descripcion", descripcion);
                    cmd.ExecuteNonQuery();
                }

                LoadCategorias(); // Recargar la lista de categorías
                LimpiarControles();
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Error SQL al agregar: " + ex.Message, "Error SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al agregar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (dtgCategorias.CurrentRow != null)
            {
                // Guardar el ID de la categoría seleccionada
                categoriaIdSeleccionada = Convert.ToInt32(dtgCategorias.CurrentRow.Cells["id"].Value);
                txtNombre.Text = dtgCategorias.CurrentRow.Cells["nombre"].Value.ToString();
                txtDescripcion.Text = dtgCategorias.CurrentRow.Cells["descripcion"].Value.ToString();
            }
            else
            {
                MessageBox.Show("Seleccione una categoría para editar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            try
            {
                if (dtgCategorias.CurrentRow != null) // Verifica si hay una fila seleccionada
                {
                    int id = (int)dtgCategorias.CurrentRow.Cells["id"].Value;

                    if (MessageBox.Show("¿Estás seguro de eliminar esta categoría?", "Confirmar", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        using (SqlConnection conn = new SqlConnection(connectionString))
                        {
                            conn.Open();
                            string query = "DELETE FROM Categorias WHERE ID = @id";
                            SqlCommand cmd = new SqlCommand(query, conn);
                            cmd.Parameters.AddWithValue("@id", id);
                            cmd.ExecuteNonQuery();
                        }
                        LoadCategorias();
                        LimpiarControles();
                    }
                }
                else
                {
                    MessageBox.Show("Seleccione una categoría para eliminar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Error SQL al eliminar: " + ex.Message, "Error SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LimpiarControles()
        {
            txtNombre.Text = "";
            txtDescripcion.Text = "";
        }

        private void btnGuardarCambios_Click(object sender, EventArgs e)
        {
            if (categoriaIdSeleccionada == -1)
            {
                MessageBox.Show("No hay ninguna categoría seleccionada para actualizar.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "UPDATE Categorias SET nombre = @nombre, descripcion = @descripcion WHERE id = @id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@nombre", txtNombre.Text.Trim());
                    cmd.Parameters.AddWithValue("@descripcion", txtDescripcion.Text.Trim());
                    cmd.Parameters.AddWithValue("@id", categoriaIdSeleccionada);
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Categoría actualizada correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Recargar la tabla y actualizar ComboBox en TareasForm
                LoadCategorias();
                ActualizarCategoriasEnTareasForm();

                // Limpiar campos y resetear variable
                categoriaIdSeleccionada = -1;
                LimpiarControles();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar la categoría: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void ActualizarCategoriasEnTareasForm()
        {
            foreach (Form frm in Application.OpenForms)
            {
                if (frm is TareasForm tareasForm)
                {
                    tareasForm.LoadCategorias(); // Llama al método de TareasForm
                    break;
                }
            }
        }

        private void btnSalirCateg_Click(object sender, EventArgs e)
        {
            this.Hide(); // Oculta CategoriasForm

            // Verifica si TareasForm ya está abierto
            foreach (Form frm in Application.OpenForms)
            {
                if (frm is TareasForm tareasForm)
                {
                    tareasForm.Show();
                    return;
                }
            }

            // Si no está abierto, crea una nueva instancia
            TareasForm nuevoTareasForm = new TareasForm();
            nuevoTareasForm.Show();
            this.Close();

        }
    }  
}

        
    