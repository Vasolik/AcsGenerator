using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Vipl.AcsGenerator.SaveLoad;
using Vipl.AcsGenerator.VisualElements;

namespace Vipl.AcsGenerator
{
    [DebuggerDisplay("{" + nameof(Name) + "} - {" + nameof(Variable) + "}")]
    public sealed class Trait : SimpleCheckBoxVisualElement
    {
        public Trait(string variable, string name)
        {
            Variable = variable;
            HiddenTraits = new List<string>();
            Name = name;
            All[name] = this;
        }
        public Trait()
        {
        }

        public static IDictionary<string, Trait> All { get; } = new Dictionary<string, Trait>();

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
    blockoverride ""acs_trait_item_action"" {{
        onclick = ""[GetScriptedGui( '{Variable}' ).Execute( GuiScope.SetRoot( GetPlayer.MakeScope ).End )]""
        datacontext = ""[GetTrait( '{Name}' )]""
    }}
    blockoverride ""acs_checkbox_state"" {{
        {CheckBoxFrameSelector}
    }}
}}";
        public override string PositiveFlag => $"flag:{Name}";
        public override string NegativeFlag => $"flag:{Name}_negative";
        public string PassVariable => LogicalOwner is null ? "acs_filter_passed" : "acs_filter_local_passed";
        public override string Switch =>
            $@"{PositiveFlag} = {{
    if = {{
        limit = {{
            {PositiveTrigger.Intend(2)} 
        }}
        change_global_variable = {{ name = {PassVariable} add = 1 }} 
    }} 
}}
{NegativeFlag} = {{ 
     if = {{
        limit = {{
            {NegativeTrigger.Intend(2)} 
        }}
        change_global_variable = {{ name = {PassVariable} add = 1 }} 
    }}  
}}";
        public LogicalGroup LogicalOwnerImplementation { get; set; }
        public override LogicalGroup LogicalOwner => LogicalOwnerImplementation;
        public override Trait[] Traits => new[] {this};
        public override string[] Localizations => new string[0];

    }
}