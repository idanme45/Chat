using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace VirtualChat
{
    public partial class Form1 : Form
    {
        //משתנים קבועים
        private const int intPortMessages = 8080;
        private const int intPortFiles = 8081;
        //host name-משתנה לפי הרשת 
        private const string hostName = "172.20.10.3";
        private readonly string strDocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        delegate void SetTextCallback(string text);

        TcpClient clientMessages,clientFiles;
        NetworkStream nsMessages,nsFiles;
        Thread tMessages=null,tFiles = null;

        public Form1()
        {
            InitializeComponent();
            ConnectFilePort();
            ConnectMessagesPort();
        }
        //מתחבר לפורט קבצים
        //פותח טריד לקבצים
        private void ConnectFilePort()
        {
            clientFiles = new TcpClient(hostName, intPortFiles);
            nsFiles = clientFiles.GetStream();

            tFiles = new Thread(FileThread);
            tFiles.Start();
        }
        //מתחבר לפורט הודעות
        //פותח טריד להודעות
        private void ConnectMessagesPort()
        {
            clientMessages = new TcpClient(hostName, intPortMessages);
            nsMessages = clientMessages.GetStream();

            tMessages = new Thread(MessageThread);
            tMessages.Start();
        }

        public void MessageThread()
        {
            
            //מניח שהודעה קטנה מ 1024 bytes
            byte[] bytesMessage = new byte[1024];
            while (nsMessages.CanRead)
            {
                try
                {
                    int nBytesRead = nsMessages.Read(bytesMessage, 0, bytesMessage.Length);
                    string strMessage = Encoding.UTF8.GetString(bytesMessage, 0, nBytesRead);
                    //לא נכתוב את ההודעה הראשונה ברשימת ההודעות
                    if (!strMessage.Equals("Connected"))
                    { 
                        this.SetText(strMessage);
                    }
                    if (!nsMessages.CanRead)
                        break;
                }
                catch(Exception e)
                {
                    break;
                }
            }
        }

        private void UploadFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog openDailog = new OpenFileDialog()
            
            {
                //סוג קבצים
                Filter = "Json files (*.json)|*.json|XML Files|*.xml",
                InitialDirectory = @"D:\",  
                Title = "Browse Text Files",  
                CheckFileExists = true,  
                CheckPathExists = true,  
                RestoreDirectory = true,  
                ReadOnlyChecked = true,  
                ShowReadOnly = true
            };
            if (openDailog.ShowDialog() == DialogResult.OK)
            {
                //קריאה של כתובת הקובץ
                string strPath= openDailog.FileName;
                //קריאה של שם הקובץ+שרשור ל-$ על מנת שנוכל לשמור את הקובץ באותו שם
                string FileName = System.IO.Path.GetFileName(strPath)+"$";
                byte[] byteFileName=Encoding.UTF8.GetBytes(FileName);
                byte[] bytesFile = File.ReadAllBytes(strPath);
                //concate two arrays
                Merge2Arrays(byteFileName, bytesFile);
                //send to server file name concate to file's bytes
                nsFiles.Write(byteFileName, 0, byteFileName.Length);
                nsFiles.Flush();
            }
        }

        private static void Merge2Arrays(byte [] arr1,byte [] arr2)
        { 
            int nLength = arr1.Length;
            Array.Resize<byte>(ref arr1, nLength + arr2.Length);
            Array.Copy(arr1, 0, arr1, nLength, arr2.Length);
        }

        public void FileThread()
        {
            byte[] bytes = new byte[1000000];
            while (nsFiles.CanRead)
            {
                try
                {
                    int nBytesRead = nsFiles.Read(bytes, 0, bytes.Length);
                    string strFile = Encoding.UTF8.GetString(bytes, 0, nBytesRead);
                    //נשלח לשרת בחזרה רק הודעות אמיתיות
                    if (!strFile.Equals("Connected"))
                    {
                        string[] words = strFile.Split('$');
                        string strFileName = words[0], strFileData = words[1];
                        //כאשר השרת שלח שם של קובץ נוסיף או,ו לרשימה של הקבצים 
                        this.Invoke((MethodInvoker)(() => this.listFile.Items.Add(strFileName)));
                        //שמירת הקובץ אצל הלקוח
                        byte[] byteFile = Encoding.UTF8.GetBytes(strFileData);
                        string strPath = strDocuments + "\\" + strFileName;
                        File.WriteAllBytes(strPath, byteFile);
                    }
                    //כאשר מקבלים את הודעות ההתחברות מהשרת מחזירים הודעה בחזרה
                    else
                    {
                        String str = "Connected";
                        byte[] byteStr = Encoding.UTF8.GetBytes(str);
                        nsFiles.Write(byteStr, 0, byteStr.Length);
                    }
                }
                catch(Exception e)
                {
                    break;
                }
            }
        }

        //כאשר לקוח מתנתק (לוחץ על איקס) נסגור את שני הסטרימים שלו
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            clientMessages.GetStream().Close();
            clientMessages.Close();
            clientFiles.GetStream().Close();
            clientFiles.Close();
        }
        //עדכון בפועל של ההודעה ברשימת ההודעות
        private void SetText(string text)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.listMessages.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.listMessages.Text = this.listMessages.Text + text + "\r\n";
            }
        }
        private void SendText_Click(object sender, EventArgs e)
        {
            String str = message.Text;
            byte[] byteStr = Encoding.UTF8.GetBytes(str);
            nsMessages.Write(byteStr, 0, byteStr.Length);
        }
    }
}
