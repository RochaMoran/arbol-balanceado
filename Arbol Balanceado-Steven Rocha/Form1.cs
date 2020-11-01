using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Arbol_Balanceado_Steven_Rocha
{
    public partial class Form1 : Form
    {
        //Variables para trabajar. Manejar la cantidad de clientes y el indice
        public int cantClientes { get; set; }

        int axu,
            posX,
            posY,
            i = 0;

        //Variable de tipo arbol la cual posteriormente instanciaremos. Y variable de tipo Graphics la cual nos servira para dibujar el arbol
        Arbol arbol;
        Graphics nodo;

        //Arreglo de registro para almacenar valores
        public struct datos
        {
            public string cliente { get; set; }
            public double saldoPendiente { get; set; }
            public int abono { get; set; }
            public double saldoActual { get; set; }
        }

        private void btnCantClientes_Click(object sender, EventArgs e)
        {
            Cantidad();
        }

        //Arreglo de registro unidimensional
        datos[] dato;
        public Form1()
        {
            InitializeComponent();
            nodo = CreateGraphics();
            arbol = new Arbol(nodo, Font);
            gbEliminar.Enabled = false;
            gbRecorridos.Enabled = false;
            gbDatos.Enabled = false;
            axu = 0;
        }

        private void btnDatos_Click(object sender, EventArgs e)
        {
            Insertar();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            arbol.MostrarArbol(e, this.BackColor);
        }

        #region Metodo Cantidad
        public void Cantidad()
        {
            try
            {
                cantClientes = Convert.ToInt32(txtCantClientes.Text);
                axu = 0;

                if (cantClientes < 0 || txtCantClientes.Text.Length > 8)
                {
                    MessageBox.Show("Cantidad ingresada no valida", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    dato = new datos[cantClientes];
                    MessageBox.Show($"Se podran registrar un total de {cantClientes} clientes", ":)", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    gbDatos.Enabled = true;
                }
            }
            catch (Exception)
            {

                MessageBox.Show("Cantidad ingresada no valida", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion
        private void btnInOrden_Click(object sender, EventArgs e)
        {
            arbol.InOrden(lstRecorridos, lblRecorridos);
        }

        private void btnPosOrden_Click(object sender, EventArgs e)
        {
            arbol.PosOrden(lstRecorridos, lblRecorridos);
        }

        private void btnPreOrden_Click(object sender, EventArgs e)
        {
            arbol.PreOrden(lstRecorridos, lblRecorridos);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void barraNavegacion_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
            {
                posX = e.X;
                posY = e.Y;
            }
            else
            {
                Left += (e.X - posX);
                Top += (e.Y - posY);
            }
        }


        private void btnEliminar_Click(object sender, EventArgs e)
        {
            double x = Convert.ToDouble(txtEliminar.Text);

            if (arbol.Eliminar(x))
            {
                EliminarData(x);
                Refresh();
            }
            else
            {
                MessageBox.Show("No se ha encontrado el nodo", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }

        #region Eliminar datagridview
        private void EliminarData(double x)
        {
            bool encontrado = false;

            for (int i = 0; i < dato.Length - 1; i++)
            {
                if (x == dato[i].saldoActual)
                {
                    for (int j = i; j < dato.Length - 1; j++)
                    {
                        dato[j].cliente = dato[j + 1].cliente;
                        dato[j].abono = dato[j + 1].abono;
                        dato[j].saldoActual = dato[j + 1].saldoActual;
                        dato[j].saldoPendiente = dato[j + 1].saldoPendiente;

                        if (j == dato.Length - 1)
                        {
                            dato[j].cliente = null;
                            dato[j].abono = Convert.ToInt32(null);
                            dato[j].saldoActual = Convert.ToDouble(null);
                            dato[j].saldoPendiente = Convert.ToDouble(null);
                        }
                    }
                    encontrado = true;
                }
            }

            if (encontrado) i--; axu--;

            dataGridView1.Rows.Clear();

            for (int j = 0; j < axu; j++)
            {
                dataGridView1.Rows.Add(dato[j].cliente, dato[j].saldoActual);
            }
        }
        #endregion

        #region Metodo para insertar nodos/datos
        public void Insertar()
        {
            if (i == cantClientes)
            {
                MessageBox.Show("Ya no puede ingresar mas elementos", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                try
                {
                    dato[i].cliente = txtCliente.Text;
                    dato[i].saldoPendiente = double.Parse(txtSaldoPendiente.Text);
                    dato[i].abono = Convert.ToInt32(txtAbono.Text);
                    dato[i].saldoActual = dato[i].saldoPendiente - dato[i].abono;

                    if (arbol.InsertarDatos(dato[i].saldoActual))
                    {
                        MessageBox.Show("No se pueden ingresar valores duplicados.\nIngrese su precio nuevamente", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtAbono.Clear();
                        txtAbono.Focus();
                    }
                    else
                    {
                        dataGridView1.Rows.Add(dato[i].cliente, dato[i].saldoActual);
                        Refresh();
                        LimpiarCampos();
                        txtCliente.Focus();
                        i++; axu++;
                    }

                    if (i > 0)
                    {
                        gbRecorridos.Enabled = true;
                    }
                    if (i == cantClientes)
                    {
                        gbEliminar.Enabled = true;
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Datos erroneos.\nRevisar los datos previamente ingresados", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        #endregion

        #region Limpiar los campos
        public void LimpiarCampos()
        {
            txtCliente.Clear();
            txtSaldoPendiente.Clear();
            txtAbono.Clear();
        }
        #endregion

        #region Eliminar del datagridview
        public void Eliminar(double x)
        {
            for (int i = 0; i < dato.Length; i++)
            {
                if (dato[i].saldoActual == x)
                {
                    for (int j = i; j < dato.Length - 1; j++)
                    {
                        dato[j].saldoActual = dato[j + 1].saldoActual;
                        dato[j].cliente = dato[j + 1].cliente;
                        dato[j].abono = dato[j + 1].abono;

                        if (j == dato.Length)
                        {
                            dato[j].saldoActual = Convert.ToDouble(""); ;
                            dato[j].cliente = null;
                            dato[j].abono = Convert.ToInt32(null);
                        }
                    }
                }
            }

            axu--;
            i--;

            dataGridView1.Rows.Clear();

            for (int i = 0; i < axu; i++)
            {
                dataGridView1.Rows.Add(dato[i].cliente, dato[i].saldoActual);
            }
        }
        #endregion
    }
}
