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
        private BaseMessage message;
        private Dictionary<EventType, List<Action<BaseEvent>>> eventSubscribers = new Dictionary<EventType, List<Action<BaseEvent>>>();
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
        public Version GetVersion() {
            com.Write(MessageType.GetVersion, "");
            waitForMessage[MessageType.GetVersion].WaitOne();
            return message as Version;
        }
        public Subscribe Subscribe(List<EventType> eventTypes, Action<BaseEvent> callback) {
            foreach(var type in eventTypes) {
                eventSubscribers[type].Add(callback);
            }
            if (!waitForMessage.ContainsKey(MessageType.Subscribe)) {
                waitForMessage[MessageType.Subscribe] = new AutoResetEvent(false);
            }
            var payload = JsonConvert.SerializeObject(eventTypes.Select(type => Enum.GetName(typeof(EventType), type)));
            com.Write(MessageType.Subscribe, payload);
            waitForMessage[MessageType.Subscribe].WaitOne();
            return message as Subscribe;
        }
        public void ReadAll() {
            while(true) {
                var (isEvent, uMsgType, sPayload) = com.Read();
                if (isEvent) {
                    BaseEvent toBroadcast = null;
                    switch ((EventType) uMsgType) {
                        case EventType.window:
                            toBroadcast = JsonConvert.DeserializeObject<Window>(sPayload);
                            break;
                    }
                    foreach(var action in eventSubscribers[(EventType) uMsgType]) {
                        action(toBroadcast);
                    }
                }
                else {
                    switch ((MessageType) uMsgType) {
                        case MessageType.GetVersion:
                            message = JsonConvert.DeserializeObject<Version>(sPayload);
                            waitForMessage[MessageType.GetVersion].Set();
                            break;
                        case MessageType.Subscribe:
                            message = JsonConvert.DeserializeObject<Subscribe>(sPayload);
                            waitForMessage[MessageType.Subscribe].Set();
                            break;
                    }
                }
            }
        }
    }
}