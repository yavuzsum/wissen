﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;

namespace AdoGiris
{
    /*
    ADO.NET=Active Data Object
    Veritabanı ile C# dilini konuşturmak için kullanılan teknolojilerden birisidir.
    1.Connected(Bağlantılı model)
    2.Disconnected(Bağlantısız model) 

        DataSource<-->DataProvider-->DataSet
        -->Asp.Net | -->WindowsForm | -->WebService | -->Security etc.
    */
    public partial class Form1 : Form
    {
        SqlCommand uygula;
        SqlDataReader oku;
        SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["CON"].ToString());
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ComboDoldur();
            Listele();
        }

        private void Listele()
        {
            Liste.Rows.Clear();
            int i = 0;
            conn.Open();
            uygula = new SqlCommand("Select * from tblDoktors",conn);
            SqlDataReader oku = null;
            oku = uygula.ExecuteReader();

            foreach(var k in oku)
            {
                Liste.Rows.Add();
                Liste.Rows[i].Cells[0].Value = oku["Id"].ToString();
                Liste.Rows[i].Cells[1].Value = oku["Unvan"].ToString();
                Liste.Rows[i].Cells[2].Value = oku["DoktorAdi"].ToString();
                Liste.Rows[i].Cells[3].Value = oku["SehirId"].ToString();
                i++;
            }
            Liste.AllowUserToAddRows = false;
            oku.Close();
            conn.Close();
        }

        private void ComboDoldur()
        {
            txtSehir.AutoCompleteSource=AutoCompleteSource.CustomSource;
            AutoCompleteStringCollection veri = new AutoCompleteStringCollection();
            string query = "Select * from iller";
            uygula = new SqlCommand(query, conn);
            uygula.Connection.Open();
            oku = uygula.ExecuteReader(CommandBehavior.CloseConnection);
            while (oku.Read())
            {
                if (txtSehir.Items.Contains(oku.GetString(1).ToString()) != true)
                {
                    veri.Add(oku.GetString(1));
                    txtSehir.Items.Add(oku.GetString(1));
                }
            }
            conn.Close();
            oku.Close();
            txtSehir.AutoCompleteCustomSource = veri;
        }

        private void btnKayit_Click(object sender, EventArgs e)
        {
            YeniKaydet();
            Listele();
        }

        private void YeniKaydet()
        {
            uygula = new SqlCommand();
            uygula.CommandType = CommandType.StoredProcedure;
            uygula.CommandText ="[S_Doktor]";
            uygula.Connection = conn;
            uygula.Connection.Open();

            uygula.Parameters.Add("@unvan", SqlDbType.NVarChar, 50).Value = txtUnvan.Text;
            uygula.Parameters.Add("@doktorAdi", SqlDbType.NVarChar, 50).Value = txtAd.Text;
            uygula.Parameters.Add("@sehirId", SqlDbType.Int).Value = txtSehirId.Text;

            uygula.ExecuteNonQuery();
            uygula.Connection.Close();
            conn.Close();
            MessageBox.Show("Kayıt Başarılı..");
        }

        private void txtSehir_SelectedIndexChanged(object sender, EventArgs e)
        {
            string query = $"Select il_no from iller where isim='{txtSehir.Text}'";
            uygula = new SqlCommand(query);
            uygula.Connection = conn;
            uygula.Connection.Open();
            txtSehirId.Text = uygula.ExecuteScalar().ToString();
            uygula.Connection.Close();

            txtIlceler.Items.Clear();
            txtIlceler.AutoCompleteSource = AutoCompleteSource.CustomSource;
            AutoCompleteStringCollection veri = new AutoCompleteStringCollection();
            string query1 = $"Select * from ilceler where il_no='{txtSehirId.Text}'";
            uygula = new SqlCommand(query1, conn);
            uygula.Connection.Open();
            oku = uygula.ExecuteReader(CommandBehavior.CloseConnection);
            while (oku.Read())
            {
                if (txtIlceler.Items.Contains(oku.GetString(1).ToString()) != true)
                {
                    veri.Add(oku.GetString(1));
                    txtIlceler.Items.Add(oku.GetString(1));
                }
            }
            conn.Close();
            oku.Close();
            txtIlceler.AutoCompleteCustomSource = veri;
        }

        private void btnSil_Click(object sender, EventArgs e)
        {
            DialogResult mesaj;
            mesaj = MessageBox.Show("Bu kaydı silmek üzeresiniz", "Silme işlemi", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (mesaj == DialogResult.Yes)
            {
                Sil();
            }
            
        }

        private void Sil()
        {
            if (secimId > 0)
            {
                uygula = new SqlCommand();
                uygula.CommandType = CommandType.StoredProcedure;
                uygula.CommandText = "[d_Doktor]";
                uygula.Connection = conn;
                uygula.Connection.Open();

                uygula.Parameters.Add("@Id", SqlDbType.Int).Value = secimId;
                uygula.ExecuteNonQuery();
                uygula.Connection.Close();
                conn.Close();
                MessageBox.Show("Kayıt Silinmiştir.");
                Temizle();
                Listele();
            }
        }

        private void Temizle()
        {
            txtAd.Text = "";
            txtSehir.Text = "";
            txtSehirId.Text = "";
            txtUnvan.Text = "";
            secimId = -1;
            
        }

        public int secimId = -1;
        private void Liste_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                secimId = int.Parse(Liste.CurrentRow.Cells[0].Value.ToString());
            }
            catch (Exception)
            {
                secimId = -1;
            }
            if (secimId > 0)
            {
                Ac();
            }
        }

        private void Ac()
        {
            txtAd.Text = Liste.CurrentRow.Cells[2].Value.ToString();
            txtSehirId.Text = Liste.CurrentRow.Cells[3].Value.ToString();
            txtUnvan.Text = Liste.CurrentRow.Cells[1].Value.ToString();
        }

        private void btnGuncelle_Click(object sender, EventArgs e)
        {
            Guncelle();
            Listele();
        }

        private void Guncelle()
        {
            uygula = new SqlCommand();
            uygula.CommandType = CommandType.StoredProcedure;
            uygula.CommandText = "[U_Doktor]";
            uygula.Connection = conn;
            uygula.Connection.Open();

            uygula.Parameters.Add("@Id", SqlDbType.Int).Value = secimId;
            uygula.Parameters.Add("@unvan", SqlDbType.NVarChar, 50).Value = txtUnvan.Text;
            uygula.Parameters.Add("@doktorAdi", SqlDbType.NVarChar, 50).Value = txtAd.Text;
            uygula.Parameters.Add("@sehirId", SqlDbType.Int).Value = txtSehirId.Text;

            uygula.ExecuteNonQuery();
            uygula.Connection.Close();
            conn.Close();
            MessageBox.Show("Güncelleme Başarılı..");
        }


    }
}
