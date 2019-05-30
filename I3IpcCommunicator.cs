using System;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace i3
{
    class I3IpcCommunicator
    {
        private NamedPipeClientStream pipeClient;
        private static byte[] magicStr = Encoding.UTF8.GetBytes("i3-ipc");
        private BinaryReader br;
        private BinaryWriter bw;
        public I3IpcCommunicator(NamedPipeClientStream _pipeClient) {
            pipeClient = _pipeClient;
            br = new BinaryReader(pipeClient);
            bw = new BinaryWriter(pipeClient);
            bw.Flush();
        }
        ~I3IpcCommunicator() {
            br.Dispose();
            bw.Dispose();
            pipeClient.Dispose();
        }
        public void write(MessageType type, string payload) {
            var payloadAsBytes = Encoding.UTF8.GetBytes("");
            bw.Write(magicStr);
            bw.Write((UInt32) payloadAsBytes.Length);
            bw.Write((UInt32) type);
            bw.Write(payloadAsBytes);
            bw.Flush();
        }
        public string read() {
            var magic = UTF8Encoding.UTF8.GetString(br.ReadBytes(magicStr.Length));
            Console.WriteLine("Got magicstr: " + magic);

            var msgLen = br.ReadUInt32();
            Console.WriteLine("Got msglen: " + msgLen);

            var msgType = br.ReadUInt32();
            Console.WriteLine("Got msgtype: " + msgType);

            var payload = UTF8Encoding.UTF8.GetString(br.ReadBytes((int)msgLen));
            Console.WriteLine("Got payload: " + payload);

            return payload;
        }
    }
}