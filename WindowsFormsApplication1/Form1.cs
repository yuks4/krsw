using System;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Collections;
namespace nanka
{
    public partial class Form1 : Form
    {
        String path,crc;

        public Form1()
        {
            InitializeComponent();
        }
    
        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            path = openFileDialog1.FileName;
            textBox1.Text = path;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            CRCManagerClass crcClass = new CRCManagerClass();
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                crc = crcClass.GetCRC32(fs);
                fs.Close();
                textBox2.Text = crc;
                button3.Enabled = true;
                button4.Enabled = true;
                button5.Enabled = true;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string name = Path.GetFileNameWithoutExtension(path) + ".BAK";
            string path3 = Path.Combine(Path.GetDirectoryName(path), name);
            System.IO.File.Copy(path, path3, true);
            MessageBox.Show(string.Format("保存完了", crc), "info", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            string title = null;
            string txtpath = Path.Combine(Application.StartupPath, "save.txt");
            using (StreamReader fs = new StreamReader(txtpath, Encoding.GetEncoding("Shift_JIS")))
            {
                long linenum = 1;
                string line = null;
                while ((line = fs.ReadLine()) != null)
                {
                    if (-1 < line.ToUpper().LastIndexOf(crc))
                    {
                        title = line;
                        break;
                    }
                    linenum++;
                }
                fs.Close();
                if (title == null)
                {
                    StreamWriter sw = new StreamWriter("save.txt", true, Encoding.GetEncoding("shift_jis"));
                    sw.Write(Path.GetFileName(path) + "[" + textBox2.Text + "]\n");
                    sw.Write(textBox3.Text + "\n\n\n");
                    sw.Close();
                    MessageBox.Show(string.Format("追加しました。", crc), "info", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
                else
                {
                    MessageBox.Show(string.Format("既に保存されています", crc), "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void dragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            path = files[0];
            textBox1.Text = path;
        }

        private void dragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // ドラッグ中のファイルやディレクトリの取得
                string[] drags = (string[])e.Data.GetData(DataFormats.FileDrop);
                // ディレクトリの場合は System.IO.Directory.Exsits を使う
                if (drags.Length == 0 || !File.Exists((string)drags[0]))
                {
                    return;
                }
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string title = null;
            ArrayList address = new ArrayList();//書換アドレス
            ArrayList oldByte = new ArrayList();//書換前のデータ
            ArrayList newByte = new ArrayList();//書換後のデータ
            string txtpath = Path.Combine(Application.StartupPath, "save.txt");
            using (StreamReader fs = new StreamReader(txtpath, Encoding.GetEncoding("Shift_JIS")))
            {
                long linenum = 1;
                #region タイトル行までファイルポインタを進める
                string line = null;
                while ((line = fs.ReadLine()) != null)
                {
                    if (-1 < line.ToUpper().LastIndexOf(crc))
                    {
                        title = line;
                        break;
                    }
                    linenum++;
                }
                if (title == null)
                {
                    MessageBox.Show(string.Format("TEXTに{0}が見つかりません。", crc), "OpenPach", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Form f = new Form();
                    TextBox textBox = new TextBox();
                    textBox.Text = crc;
                    f.Controls.Add(textBox);
                    Application.Run(f);
                    return;
                }
                #endregion

                #region 書き換えコードを解析
                StringRegular regular = new StringRegular();

                while ((line = fs.ReadLine()) != null && line != "")
                {
                    linenum++;
                    if (true == regular.Match(line, @"^[0-9A-Fa-f]+"))
                    {
                        int addr = Convert.ToInt32(regular.Replace(line, @"^([0-9A-Fa-f]+).*$", "$1"), 16);//アドレス抽出
                        string tmpstr = regular.Replace(line, @"^[0-9A-Fa-f]+(.*)$", "$1");
                        tmpstr = regular.Replace(tmpstr, @"[^0-9A-Fa-f]", "");
                        if (tmpstr != null && tmpstr.Length > 0 && tmpstr.Length % 4 == 0)
                        {
                            for (int i = 0; i < tmpstr.Length / 4; i++)
                            {
                                address.Add(
                                    addr + i
                                    );
                                oldByte.Add(
                                    Convert.ToInt32(
                                    tmpstr.Substring(i * 2, 2)
                                    , 16));
                                newByte.Add(
                                    Convert.ToInt32(
                                    tmpstr.Substring((tmpstr.Length / 2) + i * 2, 2)
                                    , 16));
                            }
                        }
                        else
                        {
                            MessageBox.Show(string.Format("テキスト解析失敗\n{0}行目", linenum), "ぱっち", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                    else
                    {
                        MessageBox.Show(string.Format("テキスト解析失敗\n{0}行目", linenum), "ぱっち", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                #endregion

                fs.Close();
            }
            if (address.Count == 0)
            {
                MessageBox.Show("TEXTに書き換えコードが記述されていません。", "ぱっち", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


            //パッチ処理
            bool flag = false;
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.ReadWrite))
            {
                Patch patch = new Patch();
                flag = patch.ReWrite(fs, address, oldByte, newByte);
                fs.Close();
            }
            if (true == flag)
            {
                MessageBox.Show("パッチ完了！", "ぱっち", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("パッチ失敗！", "ぱっち", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }


}
