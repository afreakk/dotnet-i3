using System;
using System.Collections.Generic;

namespace i3
{
    public class BaseMessage {}
    public class Version : BaseMessage
    {
        public int major { get;set; }
        public int minor { get;set; }
        public int patch { get;set; }
        public string human_readable { get;set; }
        public string loaded_config_file_name { get;set; }
    }

    public class Subscribe : BaseMessage
    {
        public bool success { get;set; }
    }

    public class BaseEvent {}
    public class Window : BaseEvent {
        public string change {get;set;}
        public Container container {get;set;}
    }
    public class Container {
        public Int64 id {get;set;}
        public string type {get;set;}
    }

    public enum EventType {
        workspace,
        output,
        mode,
        window,
        barconfig_update,
        binding,
        shutdown,
        tick,
    }
    public enum MessageType {
        RunCommand,
        GetWorkspaces,
        Subscribe,
        GetOutputs,
        GetTree,
        GetMarks,
        GetBarConfig,
        GetVersion,
        GetBindingModes,
        GetConfig,
        SendTick,
        Sync,
    };
}