using System.Xml;
using Vipl.AcsGenerator.LogicalElements;
using Vipl.AcsGenerator.SaveLoad;
using Vipl.AcsGenerator.VisualElements;

namespace Vipl.AcsGenerator.Layouts;

public class Option: ILocalizable, ILogicalElement
{
    public Option(XmlElement element)
    {
        Localization = new LocalizationEntry {Key = element.GetAttribute("LocalizationKey"), Localization = element.GetAttribute("Localization"), File = LocalizationFiles.DropDownFile};
        Trigger = element.InnerText;
            
    }
    public LocalizationEntry Localization { get;  }
    public LocalizationEntry[] Localizations => Localization.Localization.IsNullOrEmpty() ? Array.Empty<LocalizationEntry>() : Localization.MakeArray();
    public string Trigger { get;  }
    public string? SwitchTrigger => IndexInGroup > 0 ?
        $@"{Index+ IndexInGroup - 1} = {{
    {Trigger.Intend(1)}
}}" : null;
    public ISavable Owner { get; init; }
    public int NumberOfFlagsNeeded => 1;
    public int Index { get; set; }
    public int IndexInGroup { get; set; }
    public string SelectedItem =>
        $@"acs_dropdown_selected_value = {{
    blockoverride ""visibility"" {{
        visible = ""[ScriptedGui.IsShown( {(Owner as IVisualElementWithScriptedGui)?.GetSetScopes(IndexInGroup)} )]""
    }}
    blockoverride ""text_button_drop"" {{
        text = ""{Localization.Key}""
    }}
}}";
    public string OtherItem =>
        $@"acs_filter_dropdown_other_value = {{
    blockoverride ""text_button_drop"" {{
        text = ""{Localization.Key}""
        onclick = ""[ScriptedGui.Execute( {(Owner as IVisualElementWithScriptedGui)?.GetSetScopes(IndexInGroup)} )]""
    }}
}}";
}
public class DropDown : ISavable, ILogicalElement, IVisualElementWithScriptedGui, ILayout
{

    public static IList<DropDown> All { get; } = new List<DropDown>();
    public DropDown(XmlElement element)
    {
        Name = element.GetAttribute(nameof(Name));
        DefaultValue = element.GetAttribute(nameof(DefaultValue)).ToInt();
        IsSpecial = element.GetAttribute(nameof(IsSpecial)).ToBool();
        Localization = new LocalizationEntry {Key = element.GetAttribute("LocalizationKey"), Localization = element.GetAttributeNode("Localization")?.Value, File = LocalizationFiles.DropDownFile};
        Options = element.ChildNodes.OfType<XmlElement>().Select(e => new Option(e){ Owner = this }).ToArray();
        All.Add(this);
        if(IsSpecial)
            ISavable.All.Add(this);
    }
    public LocalizationEntry Localization { get; }
    public LocalizationEntry[] Localizations => Localization.Localization.IsNullOrEmpty() ? Options.SelectMany(o => o.Localizations).ToArray() : Localization.MakeEnumerable().Concat(Options.SelectMany(o => o.Localizations)).ToArray();
    public string Name { get;  }

    public string Variable => $"acs_v_filter_{Name}";
    public string ScriptedGui => IsSpecial ? $"acs_sg_filter_{Name}" : "acs_filter_simple_dropdown";
    public int DefaultValue { get; }
        
    public Option[] Options { get; }
        
    public bool IsSpecial { get; init; }
    public string MakeReducedListAndCount => null;

    public List<ILogicalElement> Elements => Options.Cast<ILogicalElement>().ToList();
    public string ScriptedGuiName => null;


    public string GetSetScopes(int value)
        => $"GuiScope.AddScope( 'ctrl_value',  {this.GetDigit(value)} )" +
           (!IsSpecial ? $".AddScope( 'position' , {this.GetDigit(Index)} )" +
                         $".AddScope( 'total_element_count' , {this.GetDigit(Options.Length - 1)} )" : "")+
           ".End";
        


    public string? DefaultCheck => $"var:{Variable} = {DefaultValue}";

    public string? ResetValue
        => $"set_variable = {{ name = {Variable} value = {DefaultValue} }}";

    public string GetSlotCheck
        => $"var:{Variable} = $SLOT2$.var:{Variable}";

    public string CopySlots => 
$@"if = {{
    limit = {{ has_variable = {Variable} }}
    $TO$ = {{ set_variable = {{ name = {Variable} value = $FROM$.var:{Variable} }} }}
}}";

    public string ClearSlot => $"remove_variable = {Variable}";

    public bool HaveSomethingToSave => false;
    public string SwitchTrigger => IsSpecial ? null : Options.Select(o => o.SwitchTrigger).Join();
    public ISavable Owner { get; init; }
    public int NumberOfFlagsNeeded => IsSpecial ? 0 : Options.Length;
    private int _index;
    public int Index
    {
        get => _index;
        set
        {
            _index = value;
            var i = 0;
            foreach (var option in Options)
            {
                option.Index = value;
                option.IndexInGroup = i++;
            }
        }
    }

    public int IndexInGroup { get; set; }
    public string GuiElement =>@$"acs_filter_item = {{
    datacontext = ""[GetScriptedGui( '{ScriptedGui}' )]""
    blockoverride ""filter_name"" {{  
        text = ""{Localization.Key}""
    }}
    blockoverride ""acs_dropdown_selected_values"" {{
        {Options.Select(o => o.SelectedItem).Join(2)}
    }}
    blockoverride ""acs_dropdown_other_values"" {{
        {Options.Select(o => o.OtherItem).Join(2)}
    }}
}}";
}