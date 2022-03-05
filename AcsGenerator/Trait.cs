using System.Diagnostics;
using System.Xml;
using Vipl.AcsGenerator.VisualElements;

namespace Vipl.AcsGenerator;

[DebuggerDisplay("{" + nameof(Name) + "} - {" + nameof(Variable) + "}")]
public sealed class Trait : SimpleCheckBoxVisualElement
{
 public Trait(XmlElement element)
        : base(element.GetAttribute("Name"))
    {
        Variable = element.GetAttribute("Variable");
        HiddenTraits = element.ChildNodes.OfType<XmlElement>().Select(e => e.GetAttribute("Name")).ToList();
        Name = element.GetAttribute("Name");
        All[element.GetAttribute("Name")] = this;
    }
        
    public new static IDictionary<string, Trait> All { get; } = new Dictionary<string, Trait>();

    public List<string> HiddenTraits { get; }
    public string Name { get; }

    private string MultiHasTrigger =>
        @$"OR = {{
    has_trait = {Name}
    {HiddenTraits.Select(t => $"has_trait = {t}").Join(1)}
}}";
    private string MultiDontHaveTrigger =>
        @$"NOR = {{
    has_trait = {Name}
    {HiddenTraits.Select(t => $"has_trait = {t}").Join(1)}
}}";

    public override string NegativeTrigger =>
        HiddenTraits.Any() ? MultiDontHaveTrigger : $"NOT = {{ has_trait = {Name} }}";

    public override string PositiveTrigger =>
        HiddenTraits.Any() ? MultiHasTrigger : $"has_trait = {Name}";

    public override string GetGuiElement(string style) =>
        $@"acs_trait_item_{style} = {{
    datacontext = ""[GetScriptedGui( '{ScriptedGuiName}' )]""
    blockoverride ""acs_trait_item_action"" {{
        datacontext = ""[GetTrait( '{Name}' )]""
        onclick = ""[ScriptedGui.Execute( {GetSetScopes(0)} )]""
        
    }}
    blockoverride ""acs_checkbox_state"" {{
        {CheckBoxFrameSelector}
    }}
}}";
        
    public override LocalizationEntry[] Localizations => new LocalizationEntry[0];

}