using System;
using QuickFix;
using Acceptor;
using QuickFix.Transport;

namespace Executor
{
    class Program
    {
        private const string HttpServerPrefix = "http://127.0.0.1:5080/";

        [STAThread]
        static void Main(string[] args)
        {
            Console.WriteLine("=============");
            Console.WriteLine("Desafio ATG Alexandre Borges.");
            Console.WriteLine();
            Console.WriteLine("                                                    ! ! !");
            Console.WriteLine("              Orquestrador de pedidos onde gero uma ordem por segundo e envio para o OrderAccumulator");
            Console.WriteLine("                                                    ! ! !");
            Console.WriteLine();
            Console.WriteLine("=============");

            try
            {
                OrderGenerator app = new OrderGenerator();
                QuickFix.SessionSettings settings = new SessionSettings(@"simpleacc.cfg");
                IApplication application = new OrderGenerator();
                QuickFix.IMessageStoreFactory storeFactory = new QuickFix.FileStoreFactory(settings);
                QuickFix.ILogFactory logFactory = new QuickFix.ScreenLogFactory(settings);
                DefaultMessageFactory messageFactory = new DefaultMessageFactory();
                SocketInitiator initiator = new SocketInitiator(application, storeFactory, settings, logFactory, messageFactory);

                initiator.Start();
                app.Start();
                Console.WriteLine("press <enter> to quit");
                Console.Read();
                initiator.Stop();
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }
    }
}
