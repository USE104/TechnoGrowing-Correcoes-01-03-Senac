using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Utilities.Collections;
using System.IO;

namespace InterfaceONGs
{
    public partial class frmVagaONG : Form
    {

        public string Nome = "";

        public frmVagaONG()
        {
            InitializeComponent();

        }


            public frmVagaONG(string Nome)
            {

            InitializeComponent();
            this.Nome = Nome;
            cbbNomeONG.Text = Nome;

            if (cbbNomeONG.Text != null)
            {
                btnAlterarVaga.Enabled = true;
                btnExcluirVaga.Enabled=true;

                txtDescricaoVagaONG.Enabled = true;
                cbbNomeONG.Enabled = true;
                txtPeriodo.Enabled = true;
                cbbCategoriaVagaONG.Enabled = true;
                txtHorario.Enabled = true;
                btnCriarVaga.Enabled = true;
                mkdVaga.Enabled = true;

                BuscaCodVaga();
            }

        }

        const int MF_BYCOMMAND = 0X400;
        [DllImport("user32")]
        static extern int RemoveMenu(IntPtr hMenu, int nPosition, int wFlags);
        [DllImport("user32")]
        static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
        [DllImport("user32")]
        static extern int GetMenuItemCount(IntPtr hWnd);



        private void frmVagaONG_Load(object sender, EventArgs e)
        {
            IntPtr hMenu = GetSystemMenu(this.Handle, false);
            int MenuCount = GetMenuItemCount(hMenu) - 1;
            RemoveMenu(hMenu, MenuCount, MF_BYCOMMAND);

            Categorias();

            ONG();
        }


        private void btnNovoVaga_Click(object sender, EventArgs e)
        {
            limpar();

            mkdVaga.Enabled = true;
            txtDescricaoVagaONG.Enabled = true;
            cbbNomeONG.Enabled = true;
            txtPeriodo.Enabled = true;
            cbbCategoriaVagaONG.Enabled = true;
            mkdVaga.Enabled = true;
            txtHorario.Enabled = true;
            btnCriarVaga.Enabled = true;

            cbbNomeONG.Focus();
        }


        private void btnCriarVaga_Click(object sender, EventArgs e)
        {
            if (cbbNomeONG.Text != string.Empty && mkdVaga.Text != string.Empty && cbbCategoriaVagaONG.Text != string.Empty && txtDescricaoVagaONG.Text != string.Empty && txtPeriodo.Text != string.Empty && txtHorario.Text != string.Empty)
            {


                MySqlCommand comm = new MySqlCommand();

                comm.CommandText = "insert into tbVagaONG (Nome,Vaga,Categoria,Descricao,Periodo,Horario)" + "values (@Nome,@Vaga,@Categoria,@Descricao,@Periodo,@Horario)";
                comm.CommandType = CommandType.Text;
                comm.Parameters.Clear();
                comm.Parameters.Add("@Nome", MySqlDbType.VarChar, 45).Value = cbbNomeONG.Text;
                comm.Parameters.Add("@Vaga", MySqlDbType.VarChar, 45).Value = mkdVaga.Text;
                comm.Parameters.Add("@Categoria", MySqlDbType.VarChar, 20).Value = cbbCategoriaVagaONG.Text;
                comm.Parameters.Add("@Descricao", MySqlDbType.VarChar, 255).Value = txtDescricaoVagaONG.Text;
                comm.Parameters.Add("@Periodo", MySqlDbType.VarChar, 45).Value = txtPeriodo.Text;
                comm.Parameters.Add("@Horario", MySqlDbType.VarChar, 20).Value = txtHorario.Text;
                //comm.Parameters.Add("@img",MySqlDbType.Blob);

                //comm.Parameters["@img"].Value = img;                

                comm.Connection = Conexao.obterConexao();

                comm.ExecuteNonQuery();


                if (MessageBox.Show("Confirma a vinculação da "+cbbNomeONG.Text+ " a vaga ?", "Mensagem do Sistema", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.OK)
                {

                    MessageBox.Show("Vinculação da " +(cbbNomeONG.Text)+" confirmada com sucesso !", "Mensagem do Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    limpar();

                    btnNovoVaga.Focus();

                }
                else
                {
                    MessageBox.Show("Vinculação da " + (cbbNomeONG.Text) + " foi cancelada!", "Mensagem do Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                Conexao.fecharConexao();


            }
            else
            {

                MessageBox.Show("Preencha todos os campos para vincular a vaga a ONG", "Mensagem do Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                limpar() ;
            }

        }
            


        public string codVaga = "";
        private void BuscaCodVaga()
        {
            MySqlCommand comm = new MySqlCommand();
            comm.CommandText = "SELECT * FROM tbVagaONG where Nome like '%" +cbbNomeONG.Text + "%'";
            comm.CommandType = CommandType.Text;
            comm.Connection = Conexao.obterConexao();


            MySqlDataReader dr;

            dr = comm.ExecuteReader();

            while (dr.Read())
            {
                codVaga = dr.GetString(0);
                mkdVaga.Text = dr.GetString(2);
                cbbCategoriaVagaONG.Text = dr.GetString(3);
                txtDescricaoVagaONG.Text = dr.GetString(4);
                txtPeriodo.Text = dr.GetString(5);
                txtHorario.Text = dr.GetString(6);
            }
           

        }

        private void Categorias()
        {
            cbbCategoriaVagaONG.Items.Add("Designer");
            cbbCategoriaVagaONG.Items.Add("Front End");
            cbbCategoriaVagaONG.Items.Add("Back End");
            cbbCategoriaVagaONG.Items.Add("MySql");
        }

        private void limpar()
        {
            cbbNomeONG.Text = "";
            cbbCategoriaVagaONG.Text = "";
            txtHorario.Text = "";
            txtPeriodo.Text = "";
            txtDescricaoVagaONG.Text = "";
            mkdVaga.Text = "";

        }

        public bool apagar = false;

        private void btnLimparVaga_Click(object sender, EventArgs e)
        {
            if (cbbNomeONG.Text == string.Empty)
            {
                MessageBox.Show("É necessário ter informações nos campos para poder apagar","Mensagem do Sistema",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }

            else if(MessageBox.Show("Confirma a limpeza dos campos da "+(cbbNomeONG.Text)+" ? ", "Mensagem do Sistema", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.OK)
            {
                MessageBox.Show("Limpeza dos campos da " + (cbbNomeONG.Text) + " concluída", "Mensagem do Sistema");
                limpar();
            }
            else
            {
                MessageBox.Show("Limpeza  dos campos da " + (cbbNomeONG.Text) +"cancelada", "Mensagem do Sistema");
            }

        }

        private void btnAlterarVaga_Click(object sender, EventArgs e)
        {
           if( MessageBox.Show("Confirma a alteração das informações da vaga da " + (cbbNomeONG.Text), " ?" + "Mensagem do Sistema", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) == DialogResult.OK)
            {

                alterarVaga();
                limpar();

                mkdVaga.Enabled = false;
                txtDescricaoVagaONG.Enabled = false;
                cbbNomeONG.Enabled = false;
                txtPeriodo.Enabled = false;
                cbbCategoriaVagaONG.Enabled = false;
                mkdVaga.Enabled = false;
                txtHorario.Enabled = false;
                btnCriarVaga.Enabled = false;
                btnExcluirVaga.Enabled = false;

                btnNovoVaga.Focus();


            }
            else
            {
                MessageBox.Show("Alteração das informações da" +(cbbNomeONG.Text)+" foi cancelada", "Mensagem do Sistema");
            }

        }

        private void alterarVaga()
        { 
            MySqlCommand comm = new MySqlCommand();
            comm.CommandText = comm.CommandText = "update tbVagaONG set Nome = Nome, Vaga = @Vaga, Categoria = @Categoria ,Periodo = @Periodo , Horario = @Horario where codVaga =" + codVaga+"; ";

            comm.CommandType = CommandType.Text;
            comm.Connection = Conexao.obterConexao();
            comm.Parameters.Clear();
            comm.Parameters.Add("@Nome", MySqlDbType.VarChar, 45).Value = cbbNomeONG.Text;
            comm.Parameters.Add("@Vaga", MySqlDbType.VarChar, 45).Value = mkdVaga.Text;
            comm.Parameters.Add("@Categoria", MySqlDbType.VarChar, 20).Value = cbbCategoriaVagaONG.Text;
            comm.Parameters.Add("@Descricao", MySqlDbType.VarChar, 255).Value = txtDescricaoVagaONG.Text;
            comm.Parameters.Add("@Periodo", MySqlDbType.VarChar, 45).Value = txtPeriodo.Text;
            comm.Parameters.Add("@Horario", MySqlDbType.VarChar, 20).Value = txtHorario.Text;
            
            comm.ExecuteNonQuery();

            Conexao.fecharConexao();
        }

        private void btnPesquisar_Click(object sender, EventArgs e)
        {
            frmPesquisarVagasONG vagasONG = new frmPesquisarVagasONG();
            vagasONG.Show();
            this.Hide();
        }

        public bool Buscar = false;


        private void btnExcluirVaga_Click(object sender, EventArgs e)
        {
            MySqlCommand comm = new MySqlCommand();
            comm.CommandText = "delete from tbVagaONG where codVaga = @codVaga";
            comm.CommandType = CommandType.Text;
            comm.Connection = Conexao.obterConexao();

            comm.Parameters.Clear();
            comm.Parameters.Add("@codVaga", MySqlDbType.VarChar, 15).Value = codVaga;


            if (MessageBox.Show("Confirma a desvinculação  da vaga com a " + (cbbNomeONG.Text) + " ?", "Mensagem do Sistema", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.OK)
            {
                comm.ExecuteNonQuery();
                MessageBox.Show("A desvinculação da " +(cbbNomeONG.Text)+"com  a vaga foi realizada com sucesso !", "Mensagem do Sistema");
                limpar();


                mkdVaga.Enabled = false;
                txtDescricaoVagaONG.Enabled = false;
                cbbNomeONG.Enabled = false;
                txtPeriodo.Enabled = false;
                cbbCategoriaVagaONG.Enabled = false;
                mkdVaga.Enabled = false;
                txtHorario.Enabled = false;
                btnCriarVaga.Enabled = false;
                btnExcluirVaga.Enabled = false;

                btnNovoVaga.Focus();
            }
            else
            {
                MessageBox.Show("A desvinculação da " + cbbNomeONG.Text + " com a vaga foi cancelada !", "Mensagem do Sistema");
            }
        }

        private void pctLogo_Click(object sender, EventArgs e)
        {

        }

        private void btnVoltar_Click(object sender, EventArgs e)
        {
            frmAdministrador abrirAdministrador = new frmAdministrador();
            abrirAdministrador.Show();
            this.Hide();
        }


        public string codONG = "";

        private void ONG()
        {
            MySqlCommand comm = new MySqlCommand();
            comm.CommandText = "SELECT nome FROM tbONG";
            comm.CommandType = CommandType.Text;
            comm.Parameters.Clear();
            comm.Connection = Conexao.obterConexao();
            MySqlDataReader dr;

            dr = comm.ExecuteReader();

            while(dr.Read())
            {
                cbbNomeONG.Items.Add(dr.GetString(0));
            } 

            Conexao.fecharConexao();
        }

        private void btnProcurar_Click(object sender, EventArgs e)
        {
            OpenFileDialog opf = new OpenFileDialog();
            opf.Filter = "Procurar Imagem(*.jpg,*png;) | *.jpg; *.png";
            if (opf.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image = Image.FromFile(opf.FileName);
            }
        }

        private void btnEnviar_Click(object sender, EventArgs e)
        {
           


        }
    }

}
