using System.Collections.Generic;

namespace i3
{
    public class Version
    {
        public int major { get;set; }
        public int minor { get;set; }
        public int patch { get;set; }
        public string human_readable { get;set; }
        public string loaded_config_file_name { get;set; }
    }

    public class Subscribe
    {
        public string type { get;set; }
        public List<string> payload { get;set; }
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