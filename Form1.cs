using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AccountingSoftware;

using Конфа = НоваКонфігурація_1_0;
using Константи = НоваКонфігурація_1_0.Константи;
using Довідники = НоваКонфігурація_1_0.Довідники;

namespace ConfaTrade
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			string pathToConfa = @"D:\AcoountigConfig\ConfaTrade\";

			Exception exception = null;

			Конфа.Config.Kernel = new Kernel();

			bool flag = Конфа.Config.Kernel.Open2(pathToConfa + @"Confa.xml", 
					"localhost",
					"postgres",
					"",
					5432,
					"confatrade", out exception);

			BindingList<Товари> s = new BindingList<Товари>();
			dataGridView1.DataSource = s;

			DataGridViewImageColumn dataGridViewImageColumn = new DataGridViewImageColumn();
			dataGridViewImageColumn.Width = 22;
			dataGridView1.Columns.Add(dataGridViewImageColumn);

			//dataGridView1.Font = new Font("Tahoma", 8);
			//dataGridView1.ForeColor = Color.Blue;
			//dataGridView1.GridColor = Color.Blue;

			s.AddingNew += S_AddingNew;

			dataGridView1.Columns["ID"].Visible = false;
			dataGridView1.Columns["Назва"].Width = 200;
			dataGridView1.Columns["ПовнаНазва"].Width = 200;
			dataGridView1.Columns["Опис"].Width = 200;

			DataGridViewButtonColumn b = new DataGridViewButtonColumn();
			b.UseColumnTextForButtonValue = true;
			b.Text = "..";
			b.Width = 30;

			int a = dataGridView1.Columns.Add(b);

			Довідники.Товари_Select товари_Select = new Довідники.Товари_Select();
			товари_Select.QuerySelect.Field.Add(товари_Select.Alias["Назва"]);
			товари_Select.QuerySelect.Field.Add(товари_Select.Alias["ПовнаНазва"]);
			товари_Select.QuerySelect.Field.Add(товари_Select.Alias["Опис"]);
			товари_Select.Select();

			while (товари_Select.MoveNext())
			{
				s.Add(new Товари(
					товари_Select.Current.UnigueID.ToString(),
					товари_Select.Current.Fields[товари_Select.Alias["Назва"]].ToString(), 
					товари_Select.Current.Fields[товари_Select.Alias["ПовнаНазва"]].ToString(),
					товари_Select.Current.Fields[товари_Select.Alias["Опис"]].ToString()
					));
			}

			//
			BindingList<Ціни> s2 = new BindingList<Ціни>();
			dataGridView2.DataSource = s2;


			Константи.РегламентніЗавдання.Завдання_Розпорядок_TablePart.Record record = new 
				Константи.РегламентніЗавдання.Завдання_Розпорядок_TablePart.Record();
			record.ГодинаПочаток = 10;
			record.ХвилинаПочаток = 30;

			Константи.РегламентніЗавдання.Завдання_Розпорядок_TablePart завдання_Розпорядок_TablePart = new 
				Константи.РегламентніЗавдання.Завдання_Розпорядок_TablePart();
			завдання_Розпорядок_TablePart.Records.Add(record);
			завдання_Розпорядок_TablePart.Save(false);
		}

		private void S_AddingNew(object sender, AddingNewEventArgs e)
		{
			
		}

		private class Товари
		{
			public Товари(string _id, string _Назва, string _ПовнаНазва, string _Опис)
			{
				ID = _id;
				Назва = _Назва;
				ПовнаНазва = _ПовнаНазва;
				Опис = _Опис;
			}

			public string ID { get; set; }
			public string Назва { get; set; }
			public string ПовнаНазва { get; set; }
			public string Опис { get; set; }
		}

		private class Ціни
		{
			public Ціни(DateTime _Дата, decimal _Ціна, string _Опис)
			{
				Дата = _Дата;
				Ціна = _Ціна;
				Опис = _Опис;
			}

			public DateTime Дата { get; set; }
			public decimal Ціна { get; set; }
			public string Опис { get; set; }
		}

		private void button1_Click(object sender, EventArgs e)
		{
			Довідники.Товари_Objest товари_Objest = new Довідники.Товари_Objest();
			товари_Objest.New();

			товари_Objest.Назва = "Назва";
			товари_Objest.ПовнаНазва = "ПовнаНазва";
			товари_Objest.Опис = "ОПИС";

			товари_Objest.Save();
		}

		private void button2_Click(object sender, EventArgs e)
		{

		}

		private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
		{
			
		}

		private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
		{
			BindingList<Ціни> s2 = new BindingList<Ціни>();
			dataGridView2.DataSource = s2;

			string uid = dataGridView1.Rows[e.RowIndex].Cells["ID"].Value.ToString();

			Довідники.Товари_Pointer товари_Pointer = new Довідники.Товари_Pointer(new UnigueID(uid));
			Довідники.Товари_Objest товари_Objest = товари_Pointer.GetDirectoryObject();

			товари_Objest.Ціни_TablePart.Records.Add(new Довідники.Товари_Ціни_TablePart.Record(DateTime.Now, 10.1m));
			товари_Objest.Ціни_TablePart.Save(false);

			Довідники.Товари_Ціни_TablePart товари_Ціни_TablePart = new Довідники.Товари_Ціни_TablePart(товари_Objest);
			товари_Ціни_TablePart.Read();

			foreach (Довідники.Товари_Ціни_TablePart.Record record in товари_Ціни_TablePart.Records)
			{
				s2.Add(new Ціни(record.ДатаЦіни, record.Ціна, record.Опис));
			}

			label1.Text = dataGridView1.VerticalScrollingOffset.ToString();
		}
	}
}
