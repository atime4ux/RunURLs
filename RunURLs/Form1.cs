using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using System.Collections;
using System.Threading;
using System.IO;

namespace RunURLs
{
    public partial class Form1 : Form
    {
        OpenFileDialog openFileDialog = new OpenFileDialog();

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog.RestoreDirectory = false;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                //파일경로 표시
                fileLocation.Text = openFileDialog.FileName;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            System.Net.WebClient WC = new System.Net.WebClient();
            
            HtmlDocument doc;
            
            Stream Webstream;//웹스트림
            StreamReader WebSR;
            StreamReader sr;//스트림리더
            

            StringBuilder strBuilder = new StringBuilder();//결과물

            string URL;
            string Result;
            string src;

            src = fileLocation.Text;

            if (src.Length > 0)
            {
                sr = new StreamReader(src, Encoding.Default);

                while (sr.Peek() >= 0)
                {
                    URL = sr.ReadLine();

                    Webstream = WC.OpenRead(URL);
                    WebSR = new StreamReader(Webstream);

                    Result = WebSR.ReadToEnd();
                    strBuilder.AppendLine(URL + "\t" + cutTags(Result).Replace("\r\n", ""));
                }
            }
            else
            {
                MessageBox.Show("파일을 선택하세요.");
            }

            //파일 저장과정
            if (strBuilder.Length > 0)
            {
                try
                {
                    //취소를 누르면 취소
                    //저장을 누르면 저장
                    if (saveFile(strBuilder).Equals("SAVE"))
                    {
                        MessageBox.Show("저장했습니다.");
                    }
                    else
                    {
                        MessageBox.Show("취소되었습니다.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
            else
            {
                MessageBox.Show("저장할 내용이 없습니다.");
            }
        }

        private string cutTags(string Text)
        {
            string Result = "";
            string cut = "";
            Result = Text;

            while (Result.IndexOf("<") > -1)
            {
                cut = Result.Substring(Result.IndexOf("<"), Result.IndexOf(">") - 1);
                Result = Result.Replace(cut, "");
            }

            return Result;
        }

        //파일로 내용 저장
        private string saveFile(StringBuilder text)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            DialogResult Result;

            saveFileDialog.InitialDirectory = fileLocation.Text;
            saveFileDialog.DefaultExt = "txt";
            saveFileDialog.Filter = "텍스트 파일 (*.txt)|*.txt";

            while (saveFileDialog.FileName.Trim().Length == 0)
            {
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    StreamWriter srWriter = new StreamWriter(saveFileDialog.FileName);
                    //null문자 제거 후 저장
                    srWriter.Write(text.ToString().Replace("\0", ""));
                    srWriter.Close();
                }

                if (saveFileDialog.FileName.Trim().Length == 0)
                {
                    Result = MessageBox.Show("저장을 취소하겠습니까?", "Caution", MessageBoxButtons.YesNo);
                    if (Result == DialogResult.Yes)
                    {
                        return "CANCEL";
                    }
                }
            }
            return "SAVE";
        }
    }
}
