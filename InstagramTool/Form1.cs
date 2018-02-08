using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InstagramTool
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            if (File.Exists(openFileDialog.FileName))
            {
                
                    var lines = File.ReadAllLines(openFileDialog.FileName);
                    if (lines.Length < 3)
                        MessageBox.Show("Error: not enough lines in file: " + openFileDialog.FileName);
                    else
                    {
                        InvoiceNumbertxt.Text = lines[0];
                        InvoiceDatetxt.Text = lines[1];
                        DueDatetxt.Text = lines[2];
                        foreach (var l in lines.Skip(3).Take(6))
                            PersonInfolst.Items.Add(l);

                        // I am assuming that the `ItemProperties` follow, as groups of 3 lines,
                        // consisting of `Item`, `Description`, `Publisher`
                        for (var i = 9; i + 2 < lines.Length; i += 3)
                        {
                            Items.Add(new ItemProperties
                            {
                                Item = lines[i],
                                Description = lines[i + 1],
                                Publisher = lines[i + 2]
                            });
                        }
                        itemlst.ItemsSource = Items;
                    } 
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            listView1.View = View.Details;
            listView1.Columns.Add("Username");
            listView1.Columns.Add("Password"); 

            // Auto-size the columns
            for (int i = 0; i < listView1.Columns.Count; i++)
            {
                listView1.Columns[i].Width = -2;
            }
        }


        private void ReadInTimeSheet()
        {

            var fileLines = File.ReadAllLines(@"C:\filepath\MyTimeSheet.txt");

                for (int i = 0; i + 4 < fileLines.Length; i += 5)
                    {
                        listView1.Items.Add(
                            new ListViewItem(new[]
                            {
                                fileLines[i],
                                fileLines[i + 1], 
                            }));
                    }

                    // Resize the columns
                    for (int i = 0; i < listView1.Columns.Count; i++)
                    {
                        listView1.Columns[i].Width = -2;
                    }
       }


    }
}
