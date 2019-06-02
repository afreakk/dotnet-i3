using System;
using System.Collections.Generic;
using Newtonsoft.Json;

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
    public class Bar
    { 
        public string id {get;set;}
        public string mode {get;set;}
        public string position {get;set;}
        public string status_command {get;set;}
        public string font {get;set;}
        public bool workspace_buttons {get;set;}
        public bool binding_mode_indicator {get;set;}
        public bool verbose {get;set;}
        public Dictionary<string,string> colors {get;set;}
    }

    public class Subscribe
    {
        public bool success { get;set; }
    }

    public class BaseEvent {}
    public class Workspace : BaseEvent {
        public string change {get;set;}
        public Container current {get;set;}
        public Container old {get;set;}
    }
    public class Window : BaseEvent {
        public string change {get;set;}
        public Container container {get;set;}
    }
    public class Output : BaseEvent {
        public string change {get;set;}
    }
    public class Container {
        public Int64 id {get;set;}
        public string type {get;set;}
        public string orientation {get;set;}
        public string scratchpad_state {get;set;}
        public Nullable<float> percent {get;set;}
        public bool urgent {get;set;}
        public bool focused {get;set;}
        public string output {get;set;}
        public string layout {get;set;}
        public string workspace_layout {get;set;}
        public string last_split_layout {get;set;}
        public string border {get;set;}
        public Int64 current_border_width {get;set;}
        public Rect rect {get;set;}
        public Rect deco_rect {get;set;}
        public Rect window_rect {get;set;}
        public Rect geometry {get;set;}
        public string name {get;set;}
        public Nullable<Int64> window {get;set;}
        public WindowProperties window_properties {get;set;}
        public Int64 fullscreen_mode {get;set;}
        public bool sticky {get;set;}
        public string floating {get;set;}
        // dont know type for this prop yet
        // "nodes":[],
        // "floating_nodes":[],
        // "focus":[],
        // "swallows":[]
    }
    public class WindowProperties {

        [JsonProperty("class")]
        public string class_ {get;set;}
        public string instance {get;set;}
        public string window_role {get;set;}
        public string title {get;set;}
        // dont know type for this prop yet
        // "transient_for":null
    }
    public class Rect {
        public Int64 x {get;set;}
        public Int64 y {get;set;}
        public Int64 width {get;set;}
        public Int64 height {get;set;}
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