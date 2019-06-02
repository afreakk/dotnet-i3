using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Threading;
using Newtonsoft.Json;

namespace i3
{
    class Program
    {
        static void test(BaseEvent y) {
            var z = y as Workspace;
            Console.WriteLine("Subscribe-window-event="+JsonConvert.SerializeObject(z));
        }
        static void Main(string[] args)
        {
            //testing..
            string address = "/run/user/1000/i3/ipc-socket.761";
            using (var pipeClient = new NamedPipeClientStream(".", address, PipeDirection.InOut))
            {
                pipeClient.Connect();
                var x = new I3Api(pipeClient);

                var s = new List<EventType>();
                s.Add(EventType.workspace);
                var subscribeReturn = x.Subscribe(s.ToArray(), test);
                Console.WriteLine("subscribeReturn:"+JsonConvert.SerializeObject(subscribeReturn));

                for (var i=0; i < 2; i++) {
                    var v = x.GetVersion();
                    Console.WriteLine("version: "+JsonConvert.SerializeObject(v));
                }

                Thread.Sleep(10000);
            }
        }
    }
}
