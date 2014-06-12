﻿using DS4Control;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace ScpServer
{
    public partial class SaveWhere : Form
    {
        private bool multisaves;
        public SaveWhere(bool multisavespots)
        {
            InitializeComponent();
            Icon = Properties.Resources.DS4;
            multisaves = multisavespots;
            lbMultiSaves.Visible = multisaves;
            cBDeleteOther.Visible = multisaves;
            if (multisaves)
                lbPickWhere.Text += ", other location files will be deleted";
            if (Global.AdminNeeded())
                bnPrgmFolder.Enabled = false;
        }

        private void bnPrgmFolder_Click(object sender, EventArgs e)
        {
            Global.SaveWhere(Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName);
            if (multisaves && !cBDeleteOther.Checked)
            {
                try { Directory.Delete(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\DS4Tool", true); }
                catch { }
            }
            else if (!multisaves)
                Save(Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName + "Profiles.xml");
            Close();
        }

        private void bnAppdataFolder_Click(object sender, EventArgs e)
        {

            if (multisaves && !cBDeleteOther.Checked)
                try
                {
                    Directory.Delete("Profiles", true);
                    File.Delete("Profiles.xml");
                    File.Delete("Auto Profiles.xml");
                }
                catch (UnauthorizedAccessException) { MessageBox.Show("Cannot Delete old settings, please manaully delete", "DS4Windows"); }
            else if (!multisaves)
                Save(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\DS4Tool\\Profiles.xml");
            Global.SaveWhere(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\DS4Tool");
            Close();
        }

        public bool Save(String path)
        {
            Boolean Saved = true;
            XmlDocument m_Xdoc = new XmlDocument();
            try
            {
                XmlNode Node;

                m_Xdoc.RemoveAll();

                Node = m_Xdoc.CreateXmlDeclaration("1.0", "utf-8", String.Empty);
                m_Xdoc.AppendChild(Node);

                Node = m_Xdoc.CreateComment(String.Format(" Profile Configuration Data. {0} ", DateTime.Now));
                m_Xdoc.AppendChild(Node);

                Node = m_Xdoc.CreateWhitespace("\r\n");
                m_Xdoc.AppendChild(Node);

                Node = m_Xdoc.CreateNode(XmlNodeType.Element, "Profile", null);

                m_Xdoc.AppendChild(Node);

                m_Xdoc.Save(path);
            }
            catch { Saved = false; }

            return Saved;
        }

        private void SaveWhere_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (String.IsNullOrEmpty(Global.appdatapath))
                if (MessageBox.Show("Close DS4Windows?\nA location must be picked to continue", "DS4Windows",
             MessageBoxButtons.YesNo) == DialogResult.No)
                    e.Cancel = true;
        }
    }
}
