using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using строка = System.String;
using System.IO;
using Файл = System.IO.StreamReader;
using System.Text.RegularExpressions;

namespace igruchka7
{
    public partial class Form1 : Form
    {
        DataView nView;
        const string ColId = "id";
        const string nameOrg = "nameOrg";
        const string innLic = "inn";
        const string dvEgrul = "dvEgrul";
        const string Egrul = "Egrul";
        const string adress = "adress";
        const string telefon = "telefon";
        const string nameMo = "mo";
        const string nomerBlanka = "nomerBlanka";
        const string NomLicenz = "licenz";
        const string Delo_No = "Delo_No";
        const string sub_no = "sub_no";
        DataSet dsEnd = new DataSet();
        const string ConnectionString = @"Data Source=games.mdb;Provider=Microsoft.Jet.OLEDB.4.0";
        public Form1()
        {
            InitializeComponent();
            try
            {
                Файл sr = new Файл(@"C:\3.txt", Encoding.Default);
                каталогToolStripMenuItem.Enabled = true;
            }
            catch
            {
                каталогToolStripMenuItem.Enabled = false;
            }
            dsEnd.Tables.Add("test");
            DataColumn iddecl = new DataColumn("iddecl");
            DataColumn subNo = new DataColumn("idraion");
            DataColumn deloNo = new DataColumn("delo_no");
            DataColumn idobj = new DataColumn("obj_id");
            DataColumn fDate = new DataColumn("DatePodachi");
            DataColumn inicialOperator = new DataColumn("nameOperator");

            dsEnd.Tables["test"].Columns.Add(iddecl);
            dsEnd.Tables["test"].Columns.Add(subNo);
            dsEnd.Tables["test"].Columns.Add(deloNo);
            dsEnd.Tables["test"].Columns.Add(idobj);
            dsEnd.Tables["test"].Columns.Add(fDate);
            dsEnd.Tables["test"].Columns.Add(inicialOperator);

            /*string commandString = "INSERT INTO kategoriy (name) VALUES ('test')";
            igruchka7.clas.dbclas Insert = new igruchka7.clas.dbclas();
            Insert.inserdb(commandString, ConnectionString);*/
            zapolnenie_kataloga();
            
        }
        /* 
         * Начало экспорта из файла в катлог продукции
         * */
        private void экспортToolStripMenuItem_Click(object sender, EventArgs e)
        {
            igruchka7.clas.dbclas select = new igruchka7.clas.dbclas();
            DataSet ds1 = select.selectdb("SELECT * FROM kategoriy", ConnectionString);
            Файл sr = new Файл(@"C:\3.txt", Encoding.Default);
            string separator = "\n";
            string separator1 = "\t";
            string txtFull = sr.ReadToEnd();
            string parrent = string.Format("{0}(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))", separator);
            string[] result = Regex.Split(txtFull, parrent);
            for (int i = 0; i < result.Length; i++)
            {
                string[] new_result = Regex.Split(result[i], parrent);
                if (new_result.Length > 1)
                    MessageBox.Show("найдено значение больше одного");
                read_data(new_result[0], 0);
            }

            sr.Close();
            dataGridView2.Columns.Clear();
            dataGridView2.AutoGenerateColumns = false;
            dataGridView2.DataSource = dsEnd.Tables[0];
            save_data(dsEnd);
            zapolnenie_kataloga();
            MessageBox.Show("Всё");
        }
        private void read_data(string sr, int m)
        {
            int i = 0;
            string separator1 = ":::";
            string parrent = string.Format("{0}(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))", separator1);
            string[] test1 = Regex.Split(sr, parrent);
            if (test1.Length > 4)
            {
                dsEnd.Tables["test"].Rows.Add(new object[] { test1[0], test1[1], test1[2], test1[3], test1[4], test1[5] });
            }
            else
            {
                try
                {
                    dsEnd.Tables["test"].Rows.Add(new object[] { test1[0], test1[1], test1[2], test1[3] });
                }
                catch { }
            }
        }
        private void save_data(DataSet ds)
        {
            string razdel_name = "";
            int id = 0;
            igruchka7.clas.dbclas delete = new igruchka7.clas.dbclas();
            delete.deletedb("DELETE FROM kategoriy", ConnectionString);
            igruchka7.clas.dbclas DeleteKatalog = new igruchka7.clas.dbclas();
            DeleteKatalog.deletedb("DELETE FROM katalog", ConnectionString);
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                if (ds.Tables[0].Rows[i]["delo_no"].ToString().Trim().Length > 1)
                {
                    if (razdel_name != ds.Tables[0].Rows[i]["delo_no"].ToString().Trim())
                    {
                        //проверяем наличие в базе
                        igruchka7.clas.dbclas prov = new igruchka7.clas.dbclas();
                        DataSet proverca = prov.selectdb("Select id, name from kategoriy where name='" + ds.Tables[0].Rows[i]["delo_no"].ToString().Trim() + "'", ConnectionString);
                        if (proverca.Tables[0].Rows.Count == 0)
                        {
                            igruchka7.clas.dbclas savekategoriy = new igruchka7.clas.dbclas();//записали первое наименование раздела
                            savekategoriy.inserdb("INSERT INTO kategoriy (name) VALUES ('" + ds.Tables[0].Rows[i]["delo_no"] + "')", ConnectionString);
                            razdel_name = ds.Tables[0].Rows[i]["delo_no"].ToString();
                            igruchka7.clas.dbclas loadkatategoriy = new igruchka7.clas.dbclas();
                            DataSet temp = loadkatategoriy.selectdb("SELECT id FROM kategoriy WHERE  name='" + razdel_name + "'", ConnectionString);
                            id = Convert.ToInt32(temp.Tables[0].Rows[0]["id"]);
                            save_katalog(id, ds.Tables[0], i);
                        }
                        else
                        {
                            save_katalog(Convert.ToInt32(proverca.Tables[0].Rows[0]["id"]), ds.Tables[0], i);
                        }
                    }
                    else
                    {
                        save_katalog(id, ds.Tables[0], i);
                    }
                }
            }

        }
        private void save_katalog(int id, DataTable dt, int i)
        {
            string fotoString = dt.Rows[i]["nameOperator"].ToString().Trim() + ".jpg";
            string commandString = @"INSERT INTO [katalog]([id_kategoriy], [name], [cena], [foto], [isbn])"
                + " VALUES('" + id + "' ,'" + dt.Rows[i]["idraion"].ToString().Trim().Replace('\'', '.') + "', '" + dt.Rows[i]["obj_id"].ToString().Trim().Replace('\'', '.') + "', '" + fotoString + "', '" + dt.Rows[i]["iddecl"].ToString().Trim() + "')";

            igruchka7.clas.dbclas saveKatalog = new igruchka7.clas.dbclas();
            saveKatalog.inserdb(commandString, ConnectionString);
        }
        /*
         *  Окончание экспорта в базу
         */
        private void zapolnenie_kataloga()
        {
            igruchka7.clas.dbclas select = new igruchka7.clas.dbclas();
            DataSet dsKatalog = select.selectdb("SELECT id, name FROM kategoriy ORDER BY name", ConnectionString);
            nView = new DataView(dsKatalog.Tables[0]);
            dataGridView1.Columns.Clear();
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.DataSource = nView;
            SetupColumnsKatalog(dsKatalog);
        }
        void OnDataGridMouseDown(object sender, MouseEventArgs e)
        {
            CurrencyManager currencyManager = (CurrencyManager)BindingContext[dataGridView1.DataSource, dataGridView1.DataMember];
            DataRowView rowView = (DataRowView)currencyManager.Current;
            DataRow row = rowView.Row;
            zapolnenie_tavara(Convert.ToInt32(row["id"]));
        }
        private void zapolnenie_tavara(int id)
        {
            igruchka7.clas.dbclas select = new igruchka7.clas.dbclas();
            DataSet ds1 = select.selectdb("SELECT * FROM katalog WHERE id_kategoriy='" + id + "' ORDER BY name", ConnectionString);
            dataGridView2.Columns.Clear();
            dataGridView2.AutoGenerateColumns = false;
            dataGridView2.DataSource = ds1.Tables[0];
            SetupColumns(ds1);
        }
        private void dataGridView2_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            CurrencyManager currencyManager = (CurrencyManager)BindingContext[dataGridView2.DataSource, dataGridView2.DataMember];
            DataRowView rowView = (DataRowView)currencyManager.Current;
            DataRow row = rowView.Row;
            foto_screen(row["foto"].ToString());
        }
        private void dataGridView2_MouseClick(object sender, MouseEventArgs e)
        {
            CurrencyManager currencyManager = (CurrencyManager)BindingContext[dataGridView2.DataSource, dataGridView2.DataMember];
            DataRowView rowView = (DataRowView)currencyManager.Current;
            DataRow row = rowView.Row;
            foto_screen(row["foto"].ToString());
        }
        private void foto_screen(string fotoText)
        {
            
            // read the image file
            try
            {
                string path = Path.GetDirectoryName(Application.ExecutablePath) + @"\_image\" + fotoText;
                Image theImage = Image.FromFile(path);
                pictureBox1.Image = Image.FromFile(path);
            }
            catch
            {
                string path = Path.GetDirectoryName(Application.ExecutablePath) + @"\_image\none.jpg";
                Image theImage = Image.FromFile(path);
                pictureBox1.Image = Image.FromFile(path);
            }
        }
        private void dataGridView2_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            CurrencyManager currencyManager = (CurrencyManager)BindingContext[dataGridView2.DataSource, dataGridView2.DataMember];
            DataRowView rowView = (DataRowView)currencyManager.Current;
            DataRow row = rowView.Row;
            try
            {
                foto_screen(row["foto"].ToString());
            }
            catch
            { //MessageBox.Show(row["foto"].ToString() + "");
                foto_screen("none.jpg");
            }
        }
        private void dataGridView1_MouseClick(object sender, MouseEventArgs e)
        {
            CurrencyManager currencyManager = (CurrencyManager)BindingContext[dataGridView1.DataSource, dataGridView1.DataMember];
            DataRowView rowView = (DataRowView)currencyManager.Current;
            DataRow row = rowView.Row;
            zapolnenie_tavara(Convert.ToInt32(row["id"]));
        }
        private void dataGridView1_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            CurrencyManager currencyManager = (CurrencyManager)BindingContext[dataGridView1.DataSource, dataGridView1.DataMember];
            DataRowView rowView = (DataRowView)currencyManager.Current;
            DataRow row = rowView.Row;
            zapolnenie_tavara(Convert.ToInt32(row["id"]));
        }
        private void textBox_TextChanged(object sender, EventArgs e)
        {
            nView.RowFilter = "name like '" + textBox1.Text + "%'";
        }
        private void закрытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void SetupColumns_all(DataSet ds)
        {
            DataGridViewTextBoxColumn inn = new DataGridViewTextBoxColumn();
            inn.DataPropertyName = "iddecl";
            inn.HeaderText = "ISBN";
            inn.ValueType = typeof(string);
            inn.Frozen = true;
            dataGridView2.Columns.Add(inn);

            DataGridViewTextBoxColumn name = new DataGridViewTextBoxColumn();
            name.DataPropertyName = "idraion";
            name.HeaderText = "Наименование товара";
            name.ValueType = typeof(string);
            name.Frozen = true;
            dataGridView2.Columns.Add(name);

            DataGridViewTextBoxColumn adres = new DataGridViewTextBoxColumn();
            adres.DataPropertyName = "delo_no";
            adres.HeaderText = "раздел";
            adres.ValueType = typeof(string);
            adres.Frozen = true;
            dataGridView2.Columns.Add(adres);

            DataGridViewTextBoxColumn dvEgrul = new DataGridViewTextBoxColumn();
            dvEgrul.DataPropertyName = "obj_id";
            dvEgrul.HeaderText = "цена";
            dvEgrul.ValueType = typeof(string);
            dvEgrul.Frozen = true;
            dataGridView2.Columns.Add(dvEgrul);

            DataGridViewTextBoxColumn DatePodachi = new DataGridViewTextBoxColumn();
            DatePodachi.DataPropertyName = "DatePodachi";
            DatePodachi.HeaderText = "Наименование для фото";
            DatePodachi.ValueType = typeof(string);
            DatePodachi.Frozen = true;
            dataGridView2.Columns.Add(DatePodachi);

            DataGridViewTextBoxColumn nameOperator = new DataGridViewTextBoxColumn();
            nameOperator.DataPropertyName = "nameOperator";
            nameOperator.HeaderText = "фото";
            nameOperator.ValueType = typeof(string);
            nameOperator.Frozen = true;
            dataGridView2.Columns.Add(nameOperator);
        }
        private void SetupColumns(DataSet ds)
        {
            DataGridViewTextBoxColumn inn = new DataGridViewTextBoxColumn();
            inn.DataPropertyName = "isbn";
            inn.HeaderText = "ISBN";
            inn.Width = 50;
            inn.ValueType = typeof(string);
            inn.Frozen = true;
            dataGridView2.Columns.Add(inn);

            DataGridViewTextBoxColumn name = new DataGridViewTextBoxColumn();
            name.DataPropertyName = "name";
            name.HeaderText = "Наименование товара";
            name.Width = 300;
            name.ValueType = typeof(string);
            name.Frozen = true;
            dataGridView2.Columns.Add(name);

            DataGridViewTextBoxColumn dvEgrul = new DataGridViewTextBoxColumn();
            dvEgrul.DataPropertyName = "cena";
            dvEgrul.HeaderText = "цена";
            dvEgrul.ValueType = typeof(string);
            dvEgrul.Frozen = true;
            dataGridView2.Columns.Add(dvEgrul);

            DataGridViewTextBoxColumn img = new DataGridViewTextBoxColumn();
            img.DataPropertyName = "foto";
            img.HeaderText = "Фото";
        }
        private void SetupColumnsKatalog(DataSet ds)
        {
            DataGridViewTextBoxColumn name = new DataGridViewTextBoxColumn();
            name.DataPropertyName = "name";
            name.HeaderText = "Наименование товара";
            name.ValueType = typeof(string);
            name.Width = 350;
            name.Frozen = true;
            dataGridView1.Columns.Add(name);
        }
    }
}
