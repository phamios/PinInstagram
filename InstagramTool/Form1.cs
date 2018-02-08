using InstaSharp;
using InstaSharp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading;
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
            openFileDialog.RestoreDirectory = true;
            openFileDialog.ShowDialog();
            ReadInTimeSheet(openFileDialog.FileName);
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


        private void ReadInTimeSheet(String filename)
        {

            var fileLines = File.ReadAllLines( filename);

                for (int i = 0; i  < fileLines.Length; i++)
                    {
                        String[] acc = fileLines[i].Split('|');
                        
                        listView1.Items.Add(
                            new ListViewItem(new[]
                            {
                                acc[0],
                                acc[1], 
                            }));
                    }

            // Resize the columns
            for (int i = 0; i < listView1.Columns.Count; i++)
            {
                listView1.Columns[i].Width = -2;
            }
        }




        public static SecureString ConvertToSecureString(string strPassword)
        {
            var secureStr = new SecureString();
            if (strPassword.Length > 0)
            {
                foreach (var c in strPassword.ToCharArray()) secureStr.AppendChar(c);
            }
            return secureStr;
        }




        private  void OnMediaUploadStartedEvent(object sender, EventArgs e)
        {
            listView3.Items.Add("Attempting to upload image");
            
            
        }

        private  void OnmediaUploadCompleteEvent(object sender, EventArgs e)
        {
            listView3.Items.Add("The image was uploaded, but has not been configured yet.");
        }


        private  void OnMediaConfigureStarted(object sender, EventArgs e)
        {
            listView3.Items.Add("The image has started to be configured");
        }

        private  void SuccessfulLoginEvent(object sender, EventArgs e)
        {
            listView3.Items.Add("Logged in! " + ((LoggedInUserResponse)e).FullName);
        }

        private  void OnLoginEvent(object sender, EventArgs e)
        {
            listView3.Items.Add("Event fired for login: " + ((NormalResponse)e).Message);
        }

        private  void OnCompleteEvent(object sender, EventArgs e)
        {

            listView3.Items.Add("Image posted to Instagram, here are all the urls");
            foreach (var image in ((UploadResponse)e).Images)
            {
                listView3.Items.Add("Uploaded: " + image.Url + "  (Size: " + image.Width + "|" + image.Height + ")");
            }
        }

        private  void ErrorEvent(object sender, EventArgs e)
        {
            listView3.Items.Add("Error  " + ((ErrorResponse)e).Message);
        }

        private  void InvalidLoginEvent(object sender, EventArgs e)
        {
            listView3.Items.Add("Error while logging  " + ((ErrorResponse)e).Message);
        }

        private void listView3_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            Thread t = new Thread(runPush);          // Kick off a new thread
            t.Start();
        }

        void runPush()
        {
            if (listView2.InvokeRequired)
            {
                listView2.Invoke((MethodInvoker)delegate ()
                {
                    progressBar1.Maximum = listView2.Items.Count;
                });
            }


            for (int i = 0; i < listView1.Items.Count; i++)
            {

                if (listView1.InvokeRequired)
                {
                    listView1.Invoke((MethodInvoker)delegate ()
                    {
                        
                         //listView1.Items[i].SubItems[0].Text
                         var uploader = new InstagramUploader(listView1.Items[i].SubItems[0].Text, ConvertToSecureString(listView1.Items[i].SubItems[1].Text));
                        uploader.InvalidLoginEvent += InvalidLoginEvent;
                        uploader.ErrorEvent += ErrorEvent;
                        uploader.OnCompleteEvent += OnCompleteEvent;
                        uploader.OnLoginEvent += OnLoginEvent;
                        uploader.SuccessfulLoginEvent += SuccessfulLoginEvent;
                        uploader.OnMediaConfigureStarted += OnMediaConfigureStarted;
                        uploader.OnMediaUploadStartedEvent += OnMediaUploadStartedEvent;
                        uploader.OnMediaUploadeComplete += OnmediaUploadCompleteEvent;

                        if (listView2.InvokeRequired)
                        {
                            listView2.Invoke((MethodInvoker)delegate ()
                            {
                                for (int image = 0; image < listView2.Items.Count; image++)
                                {
                                    progressBar1.Value = image +1 ;
                                    uploader.UploadImage(listView2.Items[image].Text, textBox1.Text);
                                }
                            });
                        }
                        
                        
                    });
                   
                }

                if (label3.InvokeRequired)
                {
                    label3.Invoke((MethodInvoker)delegate ()
                    {
                        label3.Text = "Your DeviceID is: " + InstaSharp.Properties.Settings.Default.deviceId;
                    });
                }


                

            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Stream myStream; 
            OpenFileDialog thisDialog = new OpenFileDialog();
            thisDialog.Filter = "Image File (*.jpg)|*.jpg|All files (*.*)|*.*";
            thisDialog.RestoreDirectory = true;
            thisDialog.Multiselect = true;

            if (thisDialog.ShowDialog() == DialogResult.OK)
            {
                foreach (String file in thisDialog.FileNames)
                {
                    try
                    {
                        if ((myStream = thisDialog.OpenFile()) != null)
                        {
                            using (myStream)
                            {
                                listView2.Items.Add(file);
                            }
                        }
                    }

                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                    }
                }
            }
        }
    }
}
