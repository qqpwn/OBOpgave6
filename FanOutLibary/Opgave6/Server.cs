using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using FanOutLibary;
using Newtonsoft.Json;


namespace Opgave6
{
    public class Server
    {
        public static void Start()
        {
            try
            {
                TcpListener server = null;

                
                int port = 4646;


                server = new TcpListener(IPAddress.Loopback, port);

                server.Start();

                Console.WriteLine("Waiting for a connection........");




                while (true)
                {


                    TcpClient client = server.AcceptTcpClient();
                    Console.WriteLine("Connected!");

                    Task.Run(() =>
                    {
                        TcpClient tempSocket = client;
                        DoClient(tempSocket);
                    });


                }


            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }


        }

        public static void DoClient(TcpClient socket)
        {

            List<FanOutPut> fanlist = new List<FanOutPut>() { new FanOutPut() { Id = 1 }, new FanOutPut() { Id = 2 } };

            Stream ns = socket.GetStream();
            StreamReader sr = new StreamReader(ns);
            StreamWriter sw = new StreamWriter(ns);
            sw.AutoFlush = true;

            sw.WriteLine("You are connected!!!");


            while (true)
            {



                string message = sr.ReadLine();

                if (message.ToLower().Contains("Close"))
                {
                    break;
                }

                switch (message.ToLower())
                {
                    case "GetAll":

                        string data = JsonConvert.SerializeObject(fanlist);
                        sw.WriteLine(data);

                        break;

                    case "Get":
                        string meassage2 = sr.ReadLine();

                        int idet = Convert.ToInt32(meassage2);

                        FanOutPut fan = fanlist.Find(f => f.Id == idet);
                        string data2 = JsonConvert.SerializeObject(fan);

                        sw.WriteLine(data2);
                        break;

                    case "Save":

                        string fanstring = sr.ReadLine();
                        FanOutPut fan2 = JsonConvert.DeserializeObject<FanOutPut>(fanstring);
                        fanlist.Add(fan2);

                        break;

                    default:
                        sw.Write("Please select method");
                        break;

                }




            }
            sw.WriteLine("Close");

            ns.Close();

            Console.WriteLine("Stream closed");

            socket.Close();
            Console.WriteLine("Client closed");
        }
    }
} 
    
