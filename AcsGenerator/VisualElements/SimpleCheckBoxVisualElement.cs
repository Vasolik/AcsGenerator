using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Vipl.AcsGenerator.LogicalElements;
using Vipl.AcsGenerator.SaveLoad;

namespace Vipl.AcsGenerator.VisualElements
{
    [DebuggerDisplay("{" + nameof(Variable) + "}")]
    public abstract class SimpleCheckBoxVisualElement :  ICheckBoxVisualElement, ICheckboxLogicalElement
    {
        protected SimpleCheckBoxVisualElement(string id )
        {
            All[id] = this;
        }
        public static IDictionary<string, SimpleCheckBoxVisualElement> All { get; } = new Dictionary<string, SimpleCheckBoxVisualElement>();
        public string CheckBoxFrameSelector
            => $@"frame = ""[Select_int32( GetScriptedGui( '{Variable}' ).IsShown( GuiScope.SetRoot( GetPlayer.MakeScope ).End ) , '(int32)2', Select_int32( GetScriptedGui( '{Variable}_negative' ).IsShown( GuiScope.SetRoot( GetPlayer.MakeScope ).End ) , '(int32)3', '(int32)1' ) )]""";

        public string Variable { get; protected init; }
        public abstract string GetGuiElement(string style);
        public string ScriptedGui => CheckBoxLogicalOwner is null || !CheckBoxLogicalOwner.IsSmall ? ScriptedGuiInLargeGroup : ScriptedGuiInSmallGroup;
        public string ScriptedGuiInLargeGroup =>
$@"{Variable}_negative = {{
    is_shown = {{
        any_in_global_list = {{
            variable = {this.GetListVariable()}
            {NegativeFlag} = this
        }}
    }}
}}

{Variable} = {{
 
    is_shown = {{ 
        any_in_global_list = {{
            variable = {this.GetListVariable()}
            {PositiveFlag} = this
        }}
    }}  
        
    effect = {{
        acs_save_undo_0_filters = yes
        if = {{
            limit = {{
                NOT = {{
                    any_in_global_list = {{
                        variable = {this.GetListVariable()}
                        OR = {{
                            {PositiveFlag} = this
                            {NegativeFlag} = this
                        }}
                    }} 
                }} 
            }}
            add_to_global_variable_list = {{ name = {this.GetListVariable()} target = {PositiveFlag} }}{UpdateMainPositive.Intend(3)}
        }}
        else = {{
            if = {{
                limit = {{ 
                    any_in_global_list = {{
                        variable = {this.GetListVariable()}
                        {PositiveFlag} = this
                    }}
                }}
                remove_list_global_variable = {{ name = {this.GetListVariable()} target = {PositiveFlag} }}
                add_to_global_variable_list = {{ name = {this.GetListVariable()} target = {NegativeFlag} }}
            }}
            else = {{
                remove_list_global_variable = {{ name = {this.GetListVariable()} target = {NegativeFlag} }}{UpdateMainNegative.Intend(3)}
            }}
        }}
       
        acs_auto_apply_sorting_and_filters = yes
    }}
}}";

        private string CountInListDuringUpdate => $@"
set_local_variable = {{ name = acs_temp value = 0 }}
every_in_global_list = {{
    variable = {this.GetListVariable()}
    prev = {{ change_local_variable = {{ name = acs_temp add = 1 }} }}
}}";
        private string UpdateMainPositive => CheckBoxLogicalOwner is null ? "" : $@"
{CountInListDuringUpdate}
if = {{
    limit = {{
        local_var:acs_temp = 1
    }}
    add_to_global_variable_list = {{ name = {MainSavable.Instance.ListVariable()} target = {CheckBoxLogicalOwner.Flag} }}
}}";
        private string UpdateMainNegative => CheckBoxLogicalOwner is null ? "" : $@"
{CountInListDuringUpdate}
if = {{
    limit = {{
        local_var:acs_temp = 0
    }}
    remove_list_global_variable = {{ name = {MainSavable.Instance.ListVariable()} target = {CheckBoxLogicalOwner.Flag} }}
}}";

        private string GetTransformation(string[] transformationPair)
=> $@"else_if = {{
    limit = {{
        any_in_global_list = {{
            variable = {this.GetListVariable()}
            {transformationPair[0]} = this
        }}
    }}
    remove_list_global_variable = {{ name = {this.GetListVariable()} target = {transformationPair[0]} }}
    add_to_global_variable_list = {{ name = {this.GetListVariable()} target = {transformationPair[1]} }}
}}";
        public string ScriptedGuiInSmallGroup =>
            $@"{Variable}_negative = {{
    is_shown = {{
        any_in_global_list = {{
            variable = {this.GetListVariable()}
            OR = {{
                {CheckBoxLogicalOwner.GetFlags(this, 2).Select(x => $"{x} = this").Join(4)}
            }}
        }}
    }}
}}

{Variable} = {{
 
    is_shown = {{ 
        any_in_global_list = {{
            variable = {this.GetListVariable()}
            OR = {{
                {CheckBoxLogicalOwner.GetFlags(this, 1).Select(x => $"{x} = this").Join(4)}
            }}
        }}
    }}  
        
    effect = {{
        acs_save_undo_0_filters = yes
        if = {{
            limit = {{
                NOT = {{
                    any_in_global_list = {{
                        variable = {this.GetListVariable()}
                        OR = {{
                            {CheckBoxLogicalOwner.AllFlags.Select(x => $"{x} = this").Join(7)}
                        }}
                    }} 
                }} 
            }}
            add_to_global_variable_list = {{ name = {this.GetListVariable()} target = {CheckBoxLogicalOwner.GetZeroFlag(this, 1)} }}
        }}
        {CheckBoxLogicalOwner.GetAllTransformation(this, 0, 1).Select(GetTransformation).Join(2)}
        {CheckBoxLogicalOwner.GetAllTransformation(this, 1, 2).Select(GetTransformation).Join(2)}
        {CheckBoxLogicalOwner.GetAllTransformation(this, 2, 0).Select(GetTransformation).Join(2)}
        else = {{
            remove_list_global_variable = {{ name = {this.GetListVariable()} target = {CheckBoxLogicalOwner.GetZeroFlag(this, 2) } }}
        }}
        acs_auto_apply_sorting_and_filters = yes
    }}
}}";

        public abstract string PositiveFlag { get; }
        public abstract string NegativeFlag { get; }
        public abstract Trait[] Traits { get; }
        public ISavable Owner { get; set; }
        ISavable ILogicalElement.Owner => Owner;
        public CheckboxLogicalGroup CheckBoxLogicalOwner => Owner as CheckboxLogicalGroup;
        public virtual string NegativeTrigger => null;
        public virtual string PositiveTrigger => null;
        public abstract string[] Localizations { get; }
        
        public string PassVariable => Owner.IsMain || !Owner.HaveSomethingToSave ? "acs_filter_passed" : "acs_filter_local_passed";
        public string Switch =>
            $@"{PositiveFlag} = {{
    if = {{
        limit = {{
            {PositiveTrigger.Intend(3)} 
        }}
        change_global_variable = {{ name = {PassVariable} add = 1 }} 
    }} 
}}
{NegativeFlag} = {{ 
     if = {{
        limit = {{
            {NegativeTrigger.Intend(3)} 
        }}
        change_global_variable = {{ name = {PassVariable} add = 1 }} 
    }}  
}}";
    }
}