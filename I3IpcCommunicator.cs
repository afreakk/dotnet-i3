using System;
using System.Text;
using System.IO.Pipes;


namespace i3
{
    class I3IpcCommunicator
    {
        private NamedPipeClientStream pipeClient;
        private static byte[] magicStr = Encoding.UTF8.GetBytes("i3-ipc");
        public I3IpcCommunicator(NamedPipeClientStream _pipeClient) {
            pipeClient = _pipeClient;
        }
        public void Write(MessageType messageType, string payload) {
            var payloadAsBytes = Encoding.UTF8.GetBytes(payload);
            var payloadLenAsBytes = BitConverter.GetBytes((UInt32) payloadAsBytes.Length);
            var messageTypeAsBytes = BitConverter.GetBytes((UInt32) messageType);
            var pkg = new byte[
                magicStr.Length +
                payloadLenAsBytes.Length +
                payloadLenAsBytes.Length +
                payloadAsBytes.Length
            ];

            var offset = 0;
            magicStr.CopyTo(pkg, offset);
            offset += magicStr.Length;

            payloadLenAsBytes.CopyTo(pkg, offset);
            offset += payloadLenAsBytes.Length;

            messageTypeAsBytes.CopyTo(pkg, offset);
            offset += messageTypeAsBytes.Length;

            payloadAsBytes.CopyTo(pkg, offset);

            pipeClient.Write(pkg, 0, pkg.Length);
            Console.WriteLine("WROTE AND FLUSHED:"+UTF8Encoding.UTF8.GetString(pkg));
        }
        public (bool,UInt32,string) Read() {
            var magic = new byte[magicStr.Length];
            pipeClient.Read(magic, 0, magic.Length);
            Console.WriteLine("Got magicstr: " + UTF8Encoding.UTF8.GetString(magic));

            var msgLen = new byte[4];
            pipeClient.Read(msgLen, 0, msgLen.Length);
            var uMsgLen = BitConverter.ToUInt32(msgLen, 0);
            Console.WriteLine("Got msglen: " + uMsgLen);

            var msgType = new byte[4];
            pipeClient.Read(msgType, 0, msgType.Length);
            var uMsgType = BitConverter.ToUInt32(msgType, 0);
            var isEvent = (uMsgType >> 31) == 1;

            if (isEvent) {
                uMsgType = (uMsgType & 0x7F);
                Console.WriteLine("Got eventType: " + uMsgType);
            }
            else {
                Console.WriteLine("Got msgtype: " + uMsgType);
            }

            var payload = new byte[uMsgLen];
            pipeClient.Read(payload, 0, payload.Length);

            var sPayload = UTF8Encoding.UTF8.GetString(payload);
            Console.WriteLine("Got payload: " + sPayload);

            return (isEvent, uMsgType, sPayload);
        }
    }
}