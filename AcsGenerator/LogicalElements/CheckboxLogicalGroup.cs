using System.Xml;
using Vipl.AcsGenerator.SaveLoad;

namespace Vipl.AcsGenerator.LogicalElements;

public class CheckboxLogicalGroup : ILogicalElement, ISavable
{
    public ICheckboxLogicalElement[] Elements { get;  }
    public string ScriptedGuiName => Elements.Length == 2
        ? "acs_filter_2_group_checkbox"
        : Elements.Length == 3
            ? "acs_filter_3_group_checkbox"
            : Name;
    List<ILogicalElement> ISavable.Elements => Elements.Cast<ILogicalElement>().ToList();

    public CheckboxLogicalGroup(XmlElement element)
    {
        Name = element.GetAttribute("Variable");
        Elements = element.ChildNodes.OfType<XmlElement>().Select(x => new Trait(x) {Owner = this}).OfType<ICheckboxLogicalElement>().ToArray();
    }

    public string GetSwitchTriggerForCombo(int[] combo)
    {
        return 
            $@"{GetIndexForCombo(combo)} = {{
    {GetTriggerForCombo(combo).Intend(1)}
}}";
    }
        
    public string GetTriggerForCombo(int[] combo)
    {
        if (combo.Any(x => x == 1))
        {
            var positiveElements = Elements.Where((_, i) => combo[i] == 1).ToArray();
            return positiveElements.Length > 1 ?
                $@"OR = {{
    {positiveElements.Select(e => e.PositiveTrigger).Join(1)}
}}" 
                : positiveElements[0].PositiveTrigger;
        }
        var negativeElements = Elements.Where((_, i) => combo[i] == 2).ToArray();
        return negativeElements.Length > 1 ?
            $@"AND = {{
    {negativeElements.Select(e => e.NegativeTrigger).Join(1)}
}}" 
            : negativeElements[0].NegativeTrigger;
    }
        
    private int GetIndexForCombo(IReadOnlyCollection<int> combo)
    {
        return Index + combo.Select((x, i) => new {X = x, I = i + 1}).Sum( y => (int)Math.Pow(3, combo.Count - y.I) * y.X ) - 1;
    }
        
    public string MakeReducedListAndCount =>
        $@"clear_global_variable_list = {ListReducedVariable}
set_global_variable = {{ name = {this.CountVariable()} value = 0  }} 
every_in_global_list = {{
    variable = {this.ListVariable()}
    save_temporary_scope_value_as = {{ name = to_get_modulo value = this }}
    if = {{
        limit = {{ acs_modulo_2 = 0 }}
        add_to_global_variable_list = {{ name = {ListReducedVariable} target = scope:to_get_modulo }}
        change_global_variable = {{ name = {this.CountVariable()} add = 1 }} 
    }}
}}
if = {{
    limit = {{
        global_var:{this.CountVariable()} > 0
    }}
    set_global_variable = {{ name = {this.CountVariable()} value = 1 }} 
}}
if = {{
    limit = {{
        global_var:{this.CountVariable()} = 0
    }}
    every_in_global_list = {{
        variable = {this.ListVariable()}
        save_temporary_scope_value_as = {{ name = to_get_modulo value = this }}
        if = {{
            limit = {{ acs_modulo_2 = 1 }}
            add_to_global_variable_list = {{ name = {ListReducedVariable} target = scope:to_get_modulo }}
            change_global_variable = {{ name = {this.CountVariable()} add = 1 }} 
        }}
    }}
}}";

    public string SwitchTriggerForSmallGroup =>
        AllFlagIndexes.Select(GetSwitchTriggerForCombo).Join();
    public string SwitchTrigger => IsSmall ? SwitchTriggerForSmallGroup : SwitchTriggerForLargeGroup;
       
        
    public string SwitchTriggerForLargeGroup => 
        $@"{Index} = {{
    save_temporary_scope_as = candidate2
    any_in_global_list = {{
        variable = {ListReducedVariable}
        save_temporary_scope_as = filter2
        scope:candidate2 = {{
            switch = {{
                trigger = scope:filter2
                {Elements.Select(e => e.SwitchTrigger).Join(4)}
            }}
        }}
        count = global_var:{this.CountVariable()}
    }}
}}";
        

    private int[][] AllFlagIndexes =>
        Enumerable.Range(1, (int)Math.Pow(3, Elements.Length) - 1)
            .Select(x => Enumerable.Range(0, Elements.Length).Select(y => x /(int)Math.Pow(3, y) % 3 ).ToArray())
            .OrderBy(GetIndexForCombo)
            .ToArray();

    public string ListReducedVariable => $"{this.ListVariable()}_reduced";
    public ISavable Owner => IsSmall ? MainSavable.Instance : null;
    public int NumberOfFlagsNeeded => IsSmall ? (int)Math.Pow(3, Elements.Length) - 1 : 1;
    public string Name { get; }
    public string DefaultCheck => IsSmall ? null :
        $@"any_in_global_list = {{ variable = {this.ListVariable()} always = yes count = 0 }}";
    public string ResetValue => IsSmall ? null : $@"clear_global_variable_list = {this.ListVariable()}";
    public string GetSlotCheck(int slot, string slotPrefix = "") => 
        $@"any_in_global_list = {{
    variable = {this.MakeListVariable(slot, slotPrefix)}
    save_temporary_scope_as = slot_value
    any_in_global_list = {{ variable = {this.ListVariable()} this = scope:slot_value }}
    count = all
}}
any_in_global_list = {{
    variable = {this.ListVariable()}
    save_temporary_scope_as = slot_value
    any_in_global_list = {{ variable = {this.MakeListVariable(slot, slotPrefix)} this = scope:slot_value }}
    count = all
}}";
    public string LoadFromSlot(int slot, string slotPrefix = "", bool fromPrev = false)
        => $@"clear_global_variable_list = {this.MakePrevListVariable(slot, slotPrefix, fromPrev)}
every_in_global_list = {{
    variable = {this.MakeListVariable(slot, slotPrefix)}
    save_temporary_scope_as = slot_value
    add_to_global_variable_list = {{ name = {this.MakePrevListVariable(slot, slotPrefix, fromPrev)} target = scope:slot_value }}
}}";
    public string SaveToSlot(int slot, string slotPrefix = "", bool toPrev = false)
        => $@"clear_global_variable_list = {this.MakeListVariable(slot, slotPrefix)}
every_in_global_list = {{
    variable = {this.MakePrevListVariable(slot, slotPrefix, toPrev)}
    save_temporary_scope_as = slot_value
    add_to_global_variable_list = {{ name = {this.MakeListVariable(slot, slotPrefix)} target = scope:slot_value }} 
}}";

    public bool IsSmall => Elements.Length < 4;
        
    bool ISavable.HaveSomethingToSave => !IsSmall;

    private int _index;
    public int Index
    {
        get => _index;
        set
        {
            _index = value;
            if (IsSmall)
            {
                var i = 0;
                foreach (var (element, j) in Elements.Select((e, j) => new Tuple<ILogicalElement, int>( e, j)))
                {
                    element.Index = _index + (IsSmall ? 0 : i);
                    i += element.NumberOfFlagsNeeded;
                    element.IndexInGroup = j;
                }
            }
               
        } 
    }
    public int IndexInGroup { get; set; }

    private string ScriptedGuiForLargeGroups => 
        $@"{ScriptedGuiName} = {{
    saved_scopes = {{
        ctrl_value
        position
    }}
    is_shown = {{
        any_in_global_list = {{
            variable = {this.ListVariable()}
            save_temporary_scope_value_as = {{ name = offset value = -1 }}
            this = acs_position_value_offset
        }}
    }}     
    effect = {{
        set_local_variable = {{ name = acs_start value = scope:position }}
        acs_simple_checkbox = {{ LIST = {this.ListVariable()} START = local_var:acs_start }}
        if = {{
            limit = {{
                any_in_global_list = {{
                    variable = {this.ListVariable()}
                    always = yes
                }}
            }}
            add_to_global_variable_list = {{ name = {MainSavable.Instance.ListVariable()} target = {Index} }}
        }}
        else = {{
            remove_list_global_variable = {{ name = {MainSavable.Instance.ListVariable()} target = {Index} }}
        }}
        acs_se_auto_apply_sorting_and_filters = yes
    }}
}}";
        
    private string ScriptedGroupGuiForLargeGroups => 
        $@"{ScriptedGuiName}_group = {{
        saved_scopes = {{
        ctrl_value
    }}
    is_shown = {{
        trigger_if = {{
            limit = {{ 
                scope:ctrl_value = 0
            }}
            NOT = {{
                any_in_global_list = {{
                    variable = {this.ListVariable()}
                    always = yes
                }}
            }}    
        }} 
        trigger_else_if = {{
            limit = {{ 
                scope:ctrl_value = 1
            }}
            any_in_global_list = {{
                variable =  {this.ListVariable()}
                save_temporary_scope_value_as = {{ name = to_get_modulo value = this }}
                save_temporary_scope_value_as = {{ name = to_test value = acs_modulo_2 }}
                scope:to_test = 0
                count = {Elements.Length}
            }}
        }}
        trigger_else = {{
             any_in_global_list = {{
                variable =  {this.ListVariable()}
                save_temporary_scope_value_as = {{ name = to_get_modulo value = this }}
                save_temporary_scope_value_as = {{ name = to_test value = acs_modulo_2 }}
                scope:to_test = 1
                count = {Elements.Length}
            }}
        }}
        
    }}     
    effect = {{
        acs_save_undo_0_filters = yes
        if = {{
            limit = {{
                NOT = {{
                    any_in_global_list = {{
                        variable = {this.ListVariable()}
                        always = yes
                    }}
                }}
            }}
            set_local_variable = {{ name = acs_counter value = 0 }}
            while = {{
                limit = {{
                    local_var:acs_counter < {Elements.Length * 2}
                }}
                add_to_global_variable_list = {{ name =  {this.ListVariable()} target = local_var:acs_counter }}
                change_local_variable = {{ name = acs_counter add = 2 }}
            }}
            add_to_global_variable_list = {{ name = {MainSavable.Instance.ListVariable()} target = {Index} }}
        }}
        else_if = {{
            limit = {{
                any_in_global_list = {{
                    variable =  {this.ListVariable()}
                    save_temporary_scope_value_as = {{ name = to_get_modulo value = this }}
                    save_temporary_scope_value_as = {{ name = to_test value = acs_modulo_2 }}
                    scope:to_test = 0
                    count = {Elements.Length}
                }}
            }}
            set_local_variable = {{ name = acs_counter value = 0 }}
            while = {{
                limit = {{
                    local_var:acs_counter < {Elements.Length * 2}
                }}
                remove_list_global_variable = {{ name = {this.ListVariable()} target = local_var:acs_counter }}
                change_local_variable = {{ name = acs_counter add = 1 }}
                add_to_global_variable_list = {{ name =  {this.ListVariable()} target = local_var:acs_counter }}
                change_local_variable = {{ name = acs_counter add = 1 }}
            }}
        }}
        else = {{
            set_local_variable = {{ name = acs_counter value = 0 }}
            while = {{
                limit = {{
                    local_var:acs_counter < {Elements.Length * 2}
                }}
                remove_list_global_variable = {{ name = {this.ListVariable()} target = local_var:acs_counter }}
                change_local_variable = {{ name = acs_counter add = 1 }}
            }}
            remove_list_global_variable = {{ name = {MainSavable.Instance.ListVariable()} target = {Index} }}
        }}
        acs_se_auto_apply_sorting_and_filters = yes
    }} 
}}";
        
    private string ScriptedGroupGuiForEducationGeneral => 
        $@"{ScriptedGuiName}_group = {{
    saved_scopes = {{
        ctrl_value
        position
    }}
    is_shown = {{
        trigger_if = {{
            limit = {{ 
                scope:ctrl_value = 0
            }}
            NOT = {{
                any_in_global_list = {{
                    variable = {this.ListVariable()}
                    AND = {{
                        this >= scope:position
                        save_temporary_scope_value_as = {{ name = offset value = 7 }}
                        this <= acs_position_offset
                    }}
                }}
            }}    
        }} 
        trigger_else_if = {{
            limit = {{ 
                scope:ctrl_value = 1
            }}
            any_in_global_list = {{
                variable =  {this.ListVariable()}
                OR = {{
                    this = scope:position
                    save_temporary_scope_value_as = {{ name = offset value = 2 }}
                    this = acs_position_offset
                    save_temporary_scope_value_as = {{ name = offset value = 4 }}
                    this = acs_position_offset
                    save_temporary_scope_value_as = {{ name = offset value = 6 }}
                    this = acs_position_offset
                }}
                count = 4
            }}
        }}
        trigger_else = {{
             any_in_global_list = {{
                variable =  {this.ListVariable()}
                OR = {{
                    save_temporary_scope_value_as = {{ name = offset value = 1 }}
                    this = acs_position_offset
                    save_temporary_scope_value_as = {{ name = offset value = 3 }}
                    this = acs_position_offset
                    save_temporary_scope_value_as = {{ name = offset value = 5 }}
                    this = acs_position_offset
                    save_temporary_scope_value_as = {{ name = offset value = 7 }}
                    this = acs_position_offset
                }}
                count = 4
            }}
        }}
    }}     
    effect = {{
        acs_save_undo_0_filters = yes
        if = {{
            limit = {{
                NOT = {{
                    any_in_global_list = {{
                        variable = {this.ListVariable()}
                        AND = {{
                            this >= scope:position
                            save_temporary_scope_value_as = {{ name = offset value = 7 }}
                            this <= acs_position_offset
                        }}
                    }}
                }}
            }}
            set_local_variable = {{ name = acs_counter value = scope:position }}
            set_local_variable = {{ name = acs_total_left value = 4 }}
            while = {{
                limit = {{
                    local_var:acs_total_left > 0
                }}
                add_to_global_variable_list = {{ name =  {this.ListVariable()} target = local_var:acs_counter }}
                change_local_variable = {{ name = acs_counter add = 2 }}
                change_local_variable = {{ name = acs_total_left add = -1 }}
            }}
        }}
        else_if = {{
            limit = {{
                any_in_global_list = {{
                    variable =  {this.ListVariable()}
                    OR = {{
                        this = scope:position
                        save_temporary_scope_value_as = {{ name = offset value = 2 }}
                        this = acs_position_offset
                        save_temporary_scope_value_as = {{ name = offset value = 4 }}
                        this = acs_position_offset
                        save_temporary_scope_value_as = {{ name = offset value = 6 }}
                        this = acs_position_offset
                    }}
                    count = 4
                }}
            }}
            set_local_variable = {{ name = acs_counter value = scope:position }}
            set_local_variable = {{ name = acs_total_left value = 4 }}
            while = {{
                limit = {{
                    local_var:acs_total_left > 0
                }}
                remove_list_global_variable = {{ name = {this.ListVariable()} target = local_var:acs_counter }}
                change_local_variable = {{ name = acs_counter add = 1 }}
                add_to_global_variable_list = {{ name =  {this.ListVariable()} target = local_var:acs_counter }}
                change_local_variable = {{ name = acs_counter add = 1 }}
                change_local_variable = {{ name = acs_total_left add = -1 }}
            }}
        }}
        else = {{
            set_local_variable = {{ name = acs_counter value = scope:position }}
            set_local_variable = {{ name = acs_total_left value = 8 }}
            while = {{
                limit = {{
                    local_var:acs_total_left > 0
                }}
                remove_list_global_variable = {{ name = {this.ListVariable()} target = local_var:acs_counter }}
                change_local_variable = {{ name = acs_counter add = 1 }}
                change_local_variable = {{ name = acs_total_left add = -1 }}
            }}
        }}
        if = {{
            limit = {{
                any_in_global_list = {{
                    variable = {this.ListVariable()}
                    always = yes
                }}
            }}
            add_to_global_variable_list = {{ name = {MainSavable.Instance.ListVariable()} target = {Index} }}
        }}
        else = {{
            remove_list_global_variable = {{ name = {MainSavable.Instance.ListVariable()} target = {Index} }}
        }}
        acs_se_auto_apply_sorting_and_filters = yes
    }}
}}";
    public string ScriptedGui => IsSmall ? "" : ScriptedGuiForLargeGroups + 
                                                (Name.Contains("education_general") ? "\n" + ScriptedGroupGuiForEducationGeneral :
                                                    (Name.Contains("education_martial") || Name.Contains("born_status")  ? "\n" + ScriptedGroupGuiForLargeGroups : ""));
}