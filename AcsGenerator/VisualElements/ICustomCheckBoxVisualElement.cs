namespace Vipl.AcsGenerator.VisualElements
{
    public interface ICustomCheckBoxVisualElement : ICheckBoxVisualElement
    {
        public string Icon { get; }
        string GetCustomCheckBox(string style) =>
            $@"acs_trait_item_custom_{style} = {{    
    blockoverride ""acs_trait_item_action"" {{
        onclick = ""[GetScriptedGui( '{Variable}' ).Execute( GuiScope.SetRoot( GetPlayer.MakeScope ).End )]""
    }}
    blockoverride ""acs_checkbox_state"" {{
        {CheckBoxFrameSelector}
    }}
    blockoverride ""acs_filter_name"" {{  
        text = ""{Variable}""
    }}
    blockoverride ""acs_custom_image"" {{  
        texture = {Icon}
    }}
}}";
    }
}