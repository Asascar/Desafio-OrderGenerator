using QuickFix;
using QuickFix.Fields;
using System;

namespace Executor
{
    public class OrderGenerator : QuickFix.MessageCracker, IApplication
    {
        private Session _session;
        private int seqNum = 1;

        public void OnCreate(SessionID sessionID)
        {
            _session = Session.LookupSession(sessionID);
        }

        public void OnLogon(SessionID sessionID)
        {
            Console.WriteLine("Logged in to session: " + sessionID);
        }

        public void FromApp(Message message, SessionID sessionID)
        {
            if (message is QuickFix.FIX42.ExecutionReport)
            {
                Console.WriteLine("Execution Reporter from OrderAccumulator" + message.ToString());

            }
            if (message is QuickFix.FIX42.OrderCancelReject)
            {
                Console.WriteLine("Cancel Reject from OrderAccumulator" + message.ToString());

            }
            Console.WriteLine("order: " + message.ToString());
        }

        public void ToApp(Message message, SessionID sessionID)
        { }

        public void OnLogout(SessionID sessionID)
        {
            Console.WriteLine("Logged out from session: " + sessionID);
        }

        public void OnMessage(QuickFix.FIX42.NewOrderSingle order, SessionID sessionID)
        {
            Console.WriteLine("Order Sent: OrderID=" + order.ClOrdID + " Symbol=" + order.Symbol.getValue() +
            " Side=" + order.Side.getValue() + " Quantity=" + order.OrderQty.getValue() + " Price=" + order.Price.getValue());

            Session.SendToTarget(order, _session.SessionID);
        }
        public void Start()
        {
            var random = new Random();
            var symbols = new[] { "PETR4", "VALE3", "VIIA4" };
            var sides = new[] { Side.BUY, Side.SELL };
            var timer = new System.Timers.Timer(1000);
            timer.Elapsed += (sender, e) =>
            {
                var symbol = symbols[random.Next(symbols.Length)];
                var side = sides[random.Next(sides.Length)];
                var quantity = random.Next(100000);
                var price = Math.Round((decimal)(random.NextDouble() * 1000), 2);

                var order = new QuickFix.FIX42.NewOrderSingle(
                    new ClOrdID(Guid.NewGuid().ToString()),
                    new HandlInst('1'),
                    new Symbol(symbol),
                    new Side(side),
                    new TransactTime(DateTime.Now),
                    new OrdType(OrdType.LIMIT)
                );
                order.Set(new OrderQty(quantity));
                order.Set(new Price(price));
                order.Header.SetField(new QuickFix.Fields.MsgSeqNum(seqNum++));

                var mySessionID = new SessionID("FIX.4.2", "OrderGenerator", "OrderAccumulator");
                Session.SendToTarget(order, mySessionID);

            };
            timer.Start();
        }

        public void ToAdmin(Message message, SessionID sessionID)
        {
            Console.WriteLine("Called Admin :" + message.ToString());
        }

        public void FromAdmin(Message message, SessionID sessionID)
        {

            Console.WriteLine("response from AdmClient     " + message.ToString());

        }

    }
}