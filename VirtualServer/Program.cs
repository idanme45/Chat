using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace VirtualServer
{
    //אובייקט חיבור
    class Connection
    {
        public TcpClient clientMessage;
        public TcpClient clientFile;
        public NetworkStream nsFile;
        public NetworkStream nsMessage;
    }
    class Program
    {
        private const int intPortMessages = 8080;
        private const int intPortFiles = 8081;
        private readonly string MyDocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        //רשימה של החיבורים הנוכחיים בשרת
        List<Connection> cs = new List<Connection>();

        private void ConnectToStream(NetworkStream ns)
        {
            String str = "Connected";
            byte[] byteStr = Encoding.UTF8.GetBytes(str);
            ns.Write(byteStr, 0, byteStr.Length);
        }

        //פונקציה שמוסיפה לקוח לשרת
        //פותחת 2 טרדים(הודעות וקבצים) שירוצו כל עוד הלקוח(או השרת) לא התנתק
        public void addClient(TcpClient clientMessage, TcpClient clientFile)
        {
            //int i = countClients;
            Connection c = new Connection();
            c = new Connection();
            c.clientMessage = clientMessage;
            c.clientFile = clientFile;
            c.nsMessage = clientMessage.GetStream();
            c.nsFile = clientFile.GetStream();

            cs.Add(c);

            ConnectToStream(c.nsMessage);


            Thread mt = new Thread(()=>GetMessage(clientMessage));
            mt.Start();

            Thread ft = new Thread(() => GetFile(clientFile));
            ft.Start();

            ConnectToStream(c.nsFile);

        }
        private string GetAddres(TcpClient c)
        {
            return ((IPEndPoint)c.Client.RemoteEndPoint).Address.ToString();
        }
        private string GetPort(TcpClient c)
        {
            return ((IPEndPoint)c.Client.RemoteEndPoint).Port.ToString();
        }

        //טריד שמנהל את קבלת המידע בפורט הקבצים
        public void GetFile(TcpClient clientFile)
        {
            String strAddr = GetAddres(clientFile);
            String strPort = GetPort(clientFile);

            NetworkStream cn = clientFile.GetStream();

            byte[] bytes = new byte[1000000];
            //כל עוד ניתן לקרוא מהסטרים
            while (cn.CanRead)
            {
                try
                {
                    int nBytesRead = cn.Read(bytes, 0, bytes.Length);
                    string strFileMessage = Encoding.UTF8.GetString(bytes, 0, nBytesRead);
                    if (!strFileMessage.Equals("Connected"))
                    {
                        //נרוץ על כל לקוח בשרת 
                        foreach (Connection c in cs.ToList())
                        {
                            if (c.clientFile.Connected)
                            {
                                //האם זה הלקוח ששלח לשרת
                                if (strAddr.Equals(GetAddres(c.clientFile)) && strPort.Equals(GetPort(c.clientFile)))
                                {
                                    //כאשר סוגרים את הסטרים מוחזר 0 ביטים לכן נמחק את הלקוח מהרשימה 
                                    if (nBytesRead == 0 && cs.Contains(c))
                                        cs.Remove(c);
                                }
                                //לקוח שונה מהלקוח השולח
                                else
                                {
                                    //נשלח את המידע רק כאשר הסטרים פתוח
                                    if (nBytesRead > 0)
                                    {
                                        c.nsFile.Write(bytes, 0, bytes.Length);
                                        c.nsFile.Flush();
                                    }
                                }
                            }
                            //אם הלקוח מנותק נמחק אותו מהרשימה של החיבורים
                            else
                            {
                                if(cs.Contains(c))
                                    cs.Remove(c);
                            }
                        }
                    }
                    //אם הלוקח השולח מנותק כעת נצא מהלולאה
                    if (!clientFile.Connected || nBytesRead == 0)
                        break;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    break;
                }
            }
           
        }
        //טריד שמנהל את קבלת המידע בפורט ההודעות
        public void GetMessage(TcpClient clientMessage)
        {
            
            String strMessage;
            String strAddr = GetAddres(clientMessage);
            String strPort = GetPort(clientMessage);
            NetworkStream cn = clientMessage.GetStream();
            //כל עוד ניתן לקרוא מהסטרים
            while (cn.CanRead)
            {
                try
                {
                    byte[] bytes = new byte[1024];
                    int nBytesRead = cn.Read(bytes, 0, bytes.Length);
                    strMessage = Encoding.UTF8.GetString(bytes, 0, nBytesRead);

                    String strOutputString;
                    //נרוץ על כל הלקוחות המחוברים בשרת
                    foreach (Connection c in cs.ToList())
                    {
                        if (c.clientMessage.Connected)
                        {
                            //נוסיף in\Out בהתאם
                            if (strAddr.Equals(GetAddres(c.clientMessage)) && strPort.Equals(GetPort(c.clientMessage)))
                            {
                                strOutputString = "out < " + strMessage;
                                //כאשר סוגרים את הסטרים מוחזר 0 ביטים לכן נמחק את הלקוח מהרשימה 
                                if (nBytesRead == 0 && cs.Contains(c))
                                    cs.Remove(c);
                            }
                            else
                            {
                                strOutputString = "in < " + strMessage;
                            }
                            //נכתוב רק כאשר הסטרים פתוח
                            if (nBytesRead > 0)
                            {
                                bytes = Encoding.UTF8.GetBytes(strOutputString);
                                c.nsMessage.Write(bytes, 0, bytes.Length);
                            }
                        }
                        //נסיר את כל הלקוחות הלא מחוברים
                        else
                        {
                            if(cs.Contains(c))
                                cs.Remove(c);
                        }
                    }
                    //אם הלוקח השולח מנותק כעת נצא מהלולאה
                    if (!clientMessage.Connected || nBytesRead==0)
                        break;
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.ToString());
                    break;
                }
            }
        }

        [Obsolete]
        public Program()
        {
            //2 ליסינרים שקשיבים לכל IP וב-2 פורטים מסוימים
            TcpListener listenerMessages = new TcpListener(IPAddress.Any, intPortMessages);
            listenerMessages.Start();
            TcpListener listenerFile = new TcpListener(IPAddress.Any,intPortFiles);
            listenerFile.Start();
            while (true)
            {
                try
                {
                    TcpClient clientMessage = listenerMessages.AcceptTcpClient();
                    TcpClient clientFile = listenerFile.AcceptTcpClient();
                    //נוסיף לקוח לשרת כאשר 2 המאזינים קיבלו הודעה בפורט המתאים
                    Thread t = new Thread(() => addClient(clientMessage, clientFile));
                    t.Start();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }

        [Obsolete]
        static void Main(string[] args)
        {
            Program t = new Program();
        }
    }
}