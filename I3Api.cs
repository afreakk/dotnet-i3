using Newtonsoft.Json;
using System.IO.Pipes;

namespace i3
{
    class I3Api {
        private I3IpcCommunicator com;
        public I3Api(NamedPipeClientStream _pipeClient) {
            com = new I3IpcCommunicator(_pipeClient);
        }
        public Version GetVersion() {
            com.write(MessageType.GetVersion, "");
            var responsePayload = com.read();
            return JsonConvert.DeserializeObject<Version>(responsePayload);
        }
        public Subscribe Subscribe() {
            com.write(MessageType.Subscribe, "window");
            var responsePayload = com.read();
            return JsonConvert.DeserializeObject<Subscribe>(responsePayload);
        }
        public string Listen() {
            return com.read();
        }
    }
}