using System;
using System.IO.Pipes;
namespace i3
{
    class Program
    {
        static void Main(string[] args)
        {
            string address = "/run/user/1000/i3/ipc-socket.761";
            using (var pipeClient = new NamedPipeClientStream(".", address, PipeDirection.InOut))
            {
                Console.WriteLine("Isconnected: " + pipeClient.IsConnected);
                pipeClient.Connect();
                Console.WriteLine("Isconnected: " + pipeClient.IsConnected);
                Console.WriteLine("OutBufferSize: " + pipeClient.OutBufferSize);
                Console.WriteLine("CanRead: " +pipeClient.CanRead);
                Console.WriteLine("CanWrite: " +pipeClient.CanWrite);
                var x = new I3Api(pipeClient);
                for (var i=0; i < 20; i++) {
                    var v = x.GetVersion();
                    Console.WriteLine("xxx: "+v.major);
                }
            }
        }
    }
}
