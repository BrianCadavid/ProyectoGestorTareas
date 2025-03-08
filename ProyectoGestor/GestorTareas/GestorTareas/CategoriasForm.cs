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
                    MessageBox.Show("Conexión exitosa con la base de datos.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            try
            {
                if (dtgCategorias.CurrentRow != null)
                {
                    int id = (int)dtgCategorias.CurrentRow.Cells["id"].Value;
                    string nombre = txtNombre.Text;
                    string descripcion = txtDescripcion.Text;

                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        string query = "UPDATE Categorias SET nombre = @nombre, descripcion = @descripcion WHERE id = @id";
                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@nombre", nombre);
                        cmd.Parameters.AddWithValue("@descripcion", descripcion);
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                    }
                    LoadCategorias();
                    LimpiarControles();
                }
                else
                {
                    MessageBox.Show("Seleccione una categoría para editar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                }

            }catch (SqlException ex)
            {
                MessageBox.Show("Error SQL al editar: " + ex.Message, "Error SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }catch (Exception ex)
            {
                MessageBox.Show("Error al editar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

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

        // Asegúrate de que LoadCategorias() esté implementado correctamente
        //private void LoadCategorias()
        //{
            // ... tu código para cargar las categorías en dtgCategorias ...
        //}
    }
}

        
    