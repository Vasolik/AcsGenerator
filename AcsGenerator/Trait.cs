using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Vipl.AcsGenerator.SaveLoad;
using Vipl.AcsGenerator.VisualElements;

namespace Vipl.AcsGenerator
{
    [DebuggerDisplay("{" + nameof(Name) + "} - {" + nameof(Variable) + "}")]
    public sealed class Trait : SimpleCheckBoxVisualElement, ILogicalElement, ISavable
    {
        public Trait(string variable, string name)
        {
            Variable = variable;
            HiddenTraits = new List<string>();
            Name = name;
            All[name] = this;
        }

        public static IDictionary<string, Trait> All { get; } = new Dictionary<string, Trait>();
        
        public List<string> HiddenTraits { get;  }
        public string Name { get; }

        public string Trigger => @$"{Variable} = {{
    OR = {{
        {PositiveTrigger.Intend(2)}
        {NegativeTrigger.Intend(2)}
    }}
}}";

        public string PositiveTrigger =>
$@"AND = {{
    {((ISimpleVisualElement)this).VariableYesCondition}
    {HasTrigger.Intend(1)}
}}";
        public string InvertedNegativeTrigger =>
$@"AND = {{
    {((ISimpleVisualElement)this).VariableNoCondition}
    {HasTrigger.Intend(1)}
}}";

        public string NegativeTrigger =>
$@"AND = {{
    {((ISimpleVisualElement)this).VariableNoCondition}
    {DontHaveTrigger.Intend(1)}
}}";
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

        public string NotSelectedTrigger =>
@$"OR = {{
    {((ISimpleVisualElement)this).DontHaveVariableCondition}
    {((ISimpleVisualElement)this).VariableNoCondition}
}}";

        public string DontHaveTrigger =>
            HiddenTraits.Any() ? MultiDontHaveTrigger : $"has_trait = {Name}";

        public string HasTrigger =>
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
        public string FlagGenerator =>
            @$"if = {{
    limit = {{ {(this as ISimpleVisualElement).HasVariableCondition} }}
    add_to_global_variable_list = {{ name = acs_active_filter_list target = flag:{Name} }}
    change_global_variable = {{ name = acs_active_filters_count add = 1 }}  
}}";

        public string Switch => $"flag:{Name} = {{ {Variable} = yes }}";
        
        public override Trait[] Traits => new [] {this};
        public override string[] Localizations => new string[0];
        public string DefaultCheck => (this as ISimpleVisualElement).HasVariableCondition;
        public string ResetValue => (this as ISimpleVisualElement).ClearVariableScript;
        public string GetSlotCheck(int slot, string slotPrefix = "")
=> $@"OR = {{
    NOR = {{
        {(this as ISimpleVisualElement).HasVariableCondition}
        has_global_variable = {this.MakeVariable(slot, slotPrefix)}
    }}
    AND = {{
        {(this as ISimpleVisualElement).HasVariableCondition}
        has_global_variable = {this.MakeVariable(slot, slotPrefix)}
        global_var:{Variable} = global_var:{this.MakeVariable(slot, slotPrefix)}
    }}
}}";

        public string SaveToSlot(int slot, string slotPrefix = "", bool fromPrev = false)
 => @$"if = {{
    limit = {{  has_global_variable = {this.MakePrevVariable(slot, slotPrefix, fromPrev)} }}
    set_global_variable = {{ name = {this.MakeVariable(slot, slotPrefix)} value = global_var:{this.MakePrevVariable(slot, slotPrefix, fromPrev)} }}
}}
else = {{
    remove_global_variable = {this.MakeVariable(slot, slotPrefix)}
}}";
        public string LoadFromSlot(int slot, string slotPrefix = "", bool toPrev = false)
=>@$"if = {{
    limit = {{  has_global_variable = {this.MakeVariable(slot, slotPrefix)} }}
    set_global_variable = {{ name = {this.MakePrevVariable(slot, slotPrefix, toPrev)} value = global_var:{this.MakeVariable(slot, slotPrefix)} }}
}}
else = {{
    remove_global_variable = {this.MakePrevVariable(slot, slotPrefix, toPrev)}
}}";
    }
}