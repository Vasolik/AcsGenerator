using System.Diagnostics;
using System.Linq;
using Vipl.AcsGenerator.SaveLoad;

namespace Vipl.AcsGenerator.VisualElements
{
    [DebuggerDisplay("{" + nameof(Variable) + "}")]
    public abstract class SimpleCheckBoxVisualElement : ISimpleVisualElement, ICheckBoxVisualElement, ILogicalElement
    {
        public string CheckBoxFrameSelector
            => $@"frame = ""[Select_int32( GetScriptedGui( '{Variable}' ).IsShown( GuiScope.SetRoot( GetPlayer.MakeScope ).End ) , '(int32)2', Select_int32( GetScriptedGui( '{Variable}_negative' ).IsShown( GuiScope.SetRoot( GetPlayer.MakeScope ).End ) , '(int32)3', '(int32)1' ) )]""";

        public virtual string Variable { get; protected init; }
        public abstract string GetGuiElement(string style);
        public string ScriptedGui => LogicalOwner is null || !LogicalOwner.IsSmall ? ScriptedGuiInLargeGroup : ScriptedGuiInSmallGroup;
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
        private string UpdateMainPositive => LogicalOwner is null ? "" : $@"
{CountInListDuringUpdate}
if = {{
    limit = {{
        local_var:acs_temp = 1
    }}
    add_to_global_variable_list = {{ name = {MainSavable.Instance.ListVariable} target = {LogicalOwner.Flag} }}
}}";
        private string UpdateMainNegative => LogicalOwner is null ? "" : $@"
{CountInListDuringUpdate}
if = {{
    limit = {{
        local_var:acs_temp = 0
    }}
    remove_list_global_variable = {{ name = {MainSavable.Instance.ListVariable} target = {LogicalOwner.Flag} }}
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
                {LogicalOwner.GetFlags(this, 2).Select(x => $"{x} = this").Join(4)}
            }}
        }}
    }}
}}

{Variable} = {{
 
    is_shown = {{ 
        any_in_global_list = {{
            variable = {this.GetListVariable()}
            OR = {{
                {LogicalOwner.GetFlags(this, 1).Select(x => $"{x} = this").Join(4)}
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
                            {LogicalOwner.AllFlags.Select(x => $"{x} = this").Join(7)}
                        }}
                    }} 
                }} 
            }}
            add_to_global_variable_list = {{ name = {this.GetListVariable()} target = {LogicalOwner.GetZeroFlag(this, 1)} }}
        }}
        {LogicalOwner.GetAllTransformation(this, 0, 1).Select(GetTransformation).Join(2)}
        {LogicalOwner.GetAllTransformation(this, 1, 2).Select(GetTransformation).Join(2)}
        {LogicalOwner.GetAllTransformation(this, 2, 0).Select(GetTransformation).Join(2)}
        else = {{
            remove_list_global_variable = {{ name = {this.GetListVariable()} target = {LogicalOwner.GetZeroFlag(this, 2) } }}
        }}
        acs_auto_apply_sorting_and_filters = yes
    }}
}}";
        public abstract string Switch { get; }
        public string SmallSwitchForLargeGroup => null;
        public abstract string PositiveFlag { get; }
        public abstract string NegativeFlag { get; }
        public abstract Trait[] Traits { get; }
        public abstract LogicalGroup LogicalOwner { get;  }
        public virtual string NegativeTrigger => null;
        public virtual string PositiveTrigger => null;
        public abstract string[] Localizations { get; }
    }
}