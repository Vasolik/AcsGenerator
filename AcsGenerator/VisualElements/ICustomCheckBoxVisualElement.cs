namespace Vipl.AcsGenerator.VisualElements
{
    public interface ICustomCheckBoxVisualElement : ICheckBoxVisualElement
    {
        public string Icon { get; }
        
        string GetCustomCheckBox(string style) =>
            $@"acs_trait_item_custom_{style} = {{
        datacontext = ""[GetScriptedGui( '{ScriptedGuiName}' )]""  
    blockoverride ""acs_trait_item_action"" {{
        onclick = ""[ScriptedGui.Execute( {GetSetScopes(0)} )]""
    }}
    blockoverride ""acs_checkbox_state"" {{
        {CheckBoxFrameSelector}
    }}
    blockoverride ""acs_filter_name"" {{  
        text = ""{Variable}""
    }}
    blockoverride ""acs_custom_image"" {{  
        texture = ""{Icon}""
    }}
}}";
    }
}