using System.Collections.Generic;

namespace i3
{
    class Version
    {
        public int major;
        public int minor;
        public int patch;
        public string human_readable;
        public string loaded_config_file_name;
    }

    class Subscribe
    {
        public string type;
        public List<string> payload;
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