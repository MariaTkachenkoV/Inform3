using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Xml.Schema;

namespace Inform3
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }        
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            uint img = 0, doc = 0, arh = 0, ex = 0, dl = 0, max = 0;
            int total = dataGridView1.Rows.Count, now = 0;
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                now++;
            }
            statusStrip1.Items[1].Text = now + " of " + total + " selected";
            SizeForChart(ref dataGridView1, ref img, ref doc, ref arh, ref ex, ref dl,ref max);
            FillChart(ref img, ref doc, ref arh, ref ex, ref dl, ref max, ref chart1);
            img = 0; doc = 0; arh = 0; ex = 0; dl = 0; now = 0;
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                if (Convert.ToBoolean(dataGridView1[0, i].Value))
                {
                    now++;                    
                }
            }
            statusStrip1.Items[1].Text = now + " of " + total + " selected";
            SizeForChart(ref dataGridView1, ref img, ref doc, ref arh, ref ex, ref dl,ref max);
            FillChart(ref img, ref doc, ref arh, ref ex, ref dl, ref max, ref chart1);
        }
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog file = new FolderBrowserDialog();
            if (file.ShowDialog() == DialogResult.OK)
            {
                treeView1.Nodes.Clear();
                chart1.Series.Clear();
                chart1.Series.Add("first");
                treeView1.BeforeSelect += treeView1_BeforeSelect;
                treeView1.BeforeExpand += treeView1_BeforeExpand;
                FillDriveNodes();
                if (file.SelectedPath != @"D:\" && file.SelectedPath != @"C:\" && file.SelectedPath != @"E:\")
                {
                    file.SelectedPath = file.SelectedPath.Insert(2, @"\");
                    Find(treeView1.Nodes, file.SelectedPath);
                }
                else
                {
                    Find(treeView1.Nodes, file.SelectedPath);
                }
                void Find(TreeNodeCollection Nodes, String str)
                {
                    foreach (TreeNode i in Nodes)
                    {
                        if (i.FullPath == str)
                        {
                            treeView1.SelectedNode = i;
                            return;
                        }
                        Find(i.Nodes, str);
                    }
                }
                DirectoryInfo Dir = new DirectoryInfo(file.SelectedPath);
                MainPoint(ref treeView1, ref dataGridView1, ref chart1, Dir, ref statusStrip1);
            }
        }
        public static void MainPoint(ref TreeView treeView1, ref DataGridView dataGridView1, ref Chart chart1, DirectoryInfo Dir, ref StatusStrip statusStrip1)
        {
            dataGridView1.Rows.Clear();
            FileInfo[] files = Dir.GetFiles();
            if (files.Length != 0)
            {
                uint size = 0;
                int total, now;
                dataGridView1.Rows.Add(files.Length - 1);
                for (int i = 0; i < files.Length; i++)
                {
                    if (i >= dataGridView1.Rows.Count) break;
                    dataGridView1[1, i].Value = files[i].Name;
                    dataGridView1[2, i].Value = files[i].Length / 1024;
                    dataGridView1[3, i].Value = Type(files[i].Name);
                    dataGridView1[0, i].Value = true;
                    size += uint.Parse(dataGridView1[2, i].Value.ToString());
                }
                total = dataGridView1.Rows.Count;
                now = total;
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    try
                    {
                        if (ColorLine(dataGridView1[3, i].Value.ToString()) == 1)
                        {
                            dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.Aqua;
                        }
                        if (ColorLine(dataGridView1[3, i].Value.ToString()) == 2)
                        {
                            dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.Chartreuse;
                        }
                        if (ColorLine(dataGridView1[3, i].Value.ToString()) == 3)
                        {
                            dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.Magenta;
                        }
                        if (ColorLine(dataGridView1[3, i].Value.ToString()) == 4)
                        {
                            dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.DeepSkyBlue;
                        }
                        if (ColorLine(dataGridView1[3, i].Value.ToString()) == 5)
                        {
                            dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.DarkOrchid;
                        }
                    }
                    catch (Exception exep) { }
                }
                uint img = 0, doc = 0, arh = 0, ex = 0, dl = 0, max = 0;
                SizeForChart(ref dataGridView1,ref img,ref doc,ref arh,ref ex,ref dl,ref max);
                FillChart(ref img, ref doc, ref arh, ref ex, ref dl,ref max,ref chart1);
                statusStrip1.Items[0].Text = "Total: " + size + " ";
                statusStrip1.Items[1].Text = now + " of " + total + " items selected";
            }
        }
        public static void FillChart(ref uint img, ref uint doc, ref uint arh, ref uint ex, ref uint dl,ref uint max, ref Chart chart1)
        {
            chart1.Series.Clear();
            chart1.Series.Add("first");
            chart1.ChartAreas[0].AxisY.Minimum = 0;
            chart1.ChartAreas[0].AxisY.Maximum = max;
            chart1.Series[0].Points.AddXY("png,jpg,bmp,gif", img);
            chart1.Series[0].Points.AddXY("docx, xlsx, pdf, txt", doc);
            chart1.Series[0].Points.AddXY("zip, rar, 7z", arh);
            chart1.Series[0].Points.AddXY("exe", ex);
            chart1.Series[0].Points.AddXY("dll", dl);
        }    
        public static int ColorLine(string S)
        {
            string[] gf = new string[] { "png", "jpg", "bmp", "gif" };
            string[] doc = new string[] { "docx", "xlsx", "pdf", "txt" };
            string[] dat = new string[] { "zip", "rar", "7z" };
            if (S == "exe") return 4;
            if (S == "dll") return 5;
            for (int i = 0; i < gf.Length; i++)
            {
                if (S == gf[i])
                {
                    return 1;
                }
                else if (S == doc[i])
                {
                    return 2;
                }
                if (i < dat.Length)
                {
                    if (S == dat[i])
                    {
                        return 3;
                    }
                }
            }
            return -1;
        }
        public static string Type(string S)
        {
            char[] A = S.ToCharArray();
            string tmp = "";
            for (int i = A.Length - 1; i >= 0; i--)
            {
                if (A[i] == '.')
                {
                    return Reverse(tmp);
                }
                tmp += A[i];
            }
            return null;
        }
        public static String Reverse(string S)
        {
            char[] A = S.ToCharArray();
            Array.Reverse(A);
            string tmp = "";
            for (int i = 0; i < A.Length; i++)
            {
                tmp += A[i];
            }
            return tmp;
        }
        private void FillDriveNodes()
        {
            try
            {
                foreach (DriveInfo drive in DriveInfo.GetDrives())
                {
                    TreeNode driveNode = new TreeNode { Text = drive.Name };
                    FillTreeNode(driveNode, drive.Name);
                    treeView1.Nodes.Add(driveNode);
                }
            }
            catch (Exception ex) { }
        }
        private void FillTreeNode(TreeNode driveNode, string path)
        {
            try
            {
                string[] dirs = Directory.GetDirectories(path);
                foreach (string dir in dirs)
                {
                    TreeNode dirNode = new TreeNode();
                    dirNode.Text = dir.Remove(0, dir.LastIndexOf("\\") + 1);
                    driveNode.Nodes.Add(dirNode);
                }
            }
            catch (Exception ex) { }
        }
        void treeView1_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            e.Node.Nodes.Clear();
            string[] dirs;
            try
            {
                if (Directory.Exists(e.Node.FullPath))
                {
                    dirs = Directory.GetDirectories(e.Node.FullPath);
                    if (dirs.Length != 0)
                    {
                        for (int i = 0; i < dirs.Length; i++)
                        {
                            TreeNode dirNode = new TreeNode(new DirectoryInfo(dirs[i]).Name);
                            FillTreeNode(dirNode, dirs[i]);
                            e.Node.Nodes.Add(dirNode);
                        }
                    }
                }
            }
            catch (Exception ex) { }
        }
        void treeView1_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            e.Node.Nodes.Clear();
            string[] dirs;
            try
            {
                if (Directory.Exists(e.Node.FullPath))
                {
                    dirs = Directory.GetDirectories(e.Node.FullPath);
                    if (dirs.Length != 0)
                    {
                        for (int i = 0; i < dirs.Length; i++)
                        {
                            TreeNode dirNode = new TreeNode(new DirectoryInfo(dirs[i]).Name);
                            FillTreeNode(dirNode, dirs[i]);
                            e.Node.Nodes.Add(dirNode);
                        }
                    }
                }
            }
            catch (Exception ex) { }
        }
        public static void Save(string path, DataGridView dataGridView1)
        {
            StreamWriter first = new StreamWriter(File.Open(path, FileMode.OpenOrCreate));
            for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
            {
                first.Write(dataGridView1[1, i].Value.ToString() + " ");
                first.Write(dataGridView1[2, i].Value.ToString() + " ");
                first.Write(dataGridView1[3, i].Value.ToString() + "    ");
            }
            first.Close();
        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            DirectoryInfo Dir = new DirectoryInfo(treeView1.SelectedNode.FullPath);
            FileInfo[] files = Dir.GetFiles();
            if (files.Length != 0)
            {
                dataGridView1.Rows.Clear();
                MainPoint(ref treeView1, ref dataGridView1, ref chart1, Dir, ref statusStrip1);
            }
        }
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog file = new SaveFileDialog();
            file.Filter = "Текстовые файлы (*.txt)|*.txt";
            file.FileName = "InformData";
            if (file.ShowDialog() == DialogResult.OK)
            {
                if (!File.Exists(file.FileName))
                {
                    Save(file.FileName, dataGridView1);
                }
                else
                {
                    File.Delete(file.FileName);
                    Save(file.FileName, dataGridView1);
                }
            }
        }
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form.ActiveForm.Close();
        }
        private void fontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fontDialog1.ShowDialog();
            dataGridView1.DefaultCellStyle.Font = fontDialog1.Font;
            treeView1.Font = fontDialog1.Font;
        }
        private void colorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            colorDialog1.ShowDialog();
            int i = dataGridView1.CurrentRow.Index;
            dataGridView1.Rows[i].DefaultCellStyle.BackColor = colorDialog1.Color;
        }
        public static void SizeForChart(ref DataGridView dataGridView1,ref uint Img,ref uint Doc,ref uint Zip,ref uint Exe,ref uint Dll,ref uint Max)
        {
            string[] img = new string[] { "png", "jpg", "bmp", "gif" };
            string[] doc = new string[] { "docx", "xlsx", "pdf", "txt" };
            string[] dat = new string[] { "zip", "rar", "7z" };
            int count1 = 0, count2 = 0, count3 = 0, count4 = 0, count5 = 0;            
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                Max += uint.Parse(dataGridView1[2, i].Value.ToString());
                if (Convert.ToBoolean(dataGridView1[0, i].Value))
                {
                    for (int j = 0; j < img.Length; j++)
                    {
                        if (dataGridView1[3, i].Value.ToString() == img[j])
                        {
                            Img += uint.Parse(dataGridView1[2, i].Value.ToString());
                            ++count1;
                        }
                        if (dataGridView1[3, i].Value.ToString() == doc[j])
                        {
                            Doc += uint.Parse(dataGridView1[2, i].Value.ToString());
                            ++count2;

                        }
                        if (j < dat.Length)
                        {
                            if (dataGridView1[3, i].Value.ToString() == dat[j])
                            {
                                Zip += uint.Parse(dataGridView1[2, i].Value.ToString());
                                ++count3;
                            }
                        }
                    }
                    if (dataGridView1[3, i].Value.ToString() == "exe")
                    {
                        Exe += uint.Parse(dataGridView1[2, i].Value.ToString());
                        ++count4;
                    }
                    if (dataGridView1[3, i].Value.ToString() == "dll")
                    {
                        Dll += uint.Parse(dataGridView1[2, i].Value.ToString());
                        ++count5;
                    }
                }               
            }
            if (count1 != 0)
            {
                Img /= Convert.ToUInt32(count1);
            }
            if (count2 != 0)
            {
                Doc /= Convert.ToUInt32(count2);
            }
            if (count3 != 0)
            {
                Zip /= Convert.ToUInt32(count3);
            }
            if (count4 != 0)
            {
                Exe /= Convert.ToUInt32(count4);
            }
            if(count5!=0)
            { 
                Dll /= Convert.ToUInt32(count5);
            }
        }
    }
}
