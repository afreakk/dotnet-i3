using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace i3
{
    class I3Api {
        private I3IpcCommunicator com;
        private Task readTask;
        private Dictionary<MessageType, AutoResetEvent> waitForMessage = new Dictionary<MessageType, AutoResetEvent>();
        private Dictionary<EventType, List<Action<BaseEvent>>> eventSubscribers = new Dictionary<EventType, List<Action<BaseEvent>>>();
        private Dictionary<MessageType, string> messages = new Dictionary<MessageType, string>();
        public I3Api(NamedPipeClientStream _pipeClient) {
            com = new I3IpcCommunicator(_pipeClient);
            readTask = new Task(ReadAll);
            readTask.Start();

            foreach(var messageType in Enum.GetValues(typeof(MessageType)).Cast<MessageType>().ToList()) {
                waitForMessage[messageType] = new AutoResetEvent(false);
            }
            foreach(var eventType in Enum.GetValues(typeof(EventType)).Cast<EventType>().ToList()) {
                eventSubscribers[eventType] = new List<Action<BaseEvent>>();
            }
        }
        public string[] GetBarConfigs() {
            com.Write(MessageType.GetBarConfig, "");
            waitForMessage[MessageType.GetBarConfig].WaitOne();
            return JsonConvert.DeserializeObject<string[]>(messages[MessageType.GetBarConfig]);
        }
        public Bar GetBarConfig(string bar) {
            com.Write(MessageType.GetBarConfig, bar);
            waitForMessage[MessageType.GetBarConfig].WaitOne();
            return JsonConvert.DeserializeObject<Bar>(messages[MessageType.GetBarConfig]);
        }
        public Version GetVersion() {
            com.Write(MessageType.GetVersion, "");
            waitForMessage[MessageType.GetVersion].WaitOne();
            return JsonConvert.DeserializeObject<Version>(messages[MessageType.GetVersion]);
        }
        public Subscribe Subscribe(EventType[] eventTypes, Action<BaseEvent> callback) {
            string[] payload = new string[eventTypes.Length];
            for(var i=0; i<eventTypes.Length; i++) {
                eventSubscribers[eventTypes[i]].Add(callback);
                payload[i] = Enum.GetName(typeof(EventType), eventTypes[i]);
            }
            com.Write(MessageType.Subscribe, JsonConvert.SerializeObject(payload));
            waitForMessage[MessageType.Subscribe].WaitOne();
            return JsonConvert.DeserializeObject<Subscribe>(messages[MessageType.Subscribe]);
        }
        private void ReadAll() {
            while(true) {
                var (isEvent, uMsgType, sPayload) = com.Read();
                if (isEvent) {
                    var eventType = (EventType) uMsgType;
                    BaseEvent toBroadcast;
                    switch (eventType) {
                        case EventType.window:
                            toBroadcast = JsonConvert.DeserializeObject<Window>(sPayload);
                            break;
                        case EventType.output:
                            toBroadcast = JsonConvert.DeserializeObject<Output>(sPayload);
                            break;
                        case EventType.workspace:
                            toBroadcast = JsonConvert.DeserializeObject<Workspace>(sPayload);
                            break;
                        default:
                            toBroadcast = null;
                            break;
                        
                    }
                    foreach(var action in eventSubscribers[eventType]) {
                        action(toBroadcast);
                    }
                }
                else {
                    messages[(MessageType) uMsgType] = sPayload;
                    waitForMessage[(MessageType) uMsgType].Set();
                }
            }
        }
    }
}