using System.Xml;

using Vipl.AcsGenerator.SaveLoad;

namespace Vipl.AcsGenerator.LogicalElements;

public class ReligionFamily : ILogicalElement
{
    public ReligionFamily(XmlElement element)
    {
        Family = element.GetAttribute(nameof(Family));
        IsOther = element.GetAttribute(nameof(IsOther)).ToBool();
    }
    private string Variable => $"acs_v_{Family}_selected";
    public string SwitchTrigger => IsOther ?   
$@"{Index} = {{
    NOR = {{
        {(Owner as ReligionFilter)!.NonOtherReligion.Select(f => $"religion = {{ is_in_family = {f.Family} }}").Join(2)}
    }}
}}
{Index + 1} = {{ 
    AND = {{
        {(Owner as ReligionFilter)!.NonOtherReligion.Select(f => $"religion = {{ is_in_family = {f.Family} }}").Join(2)}
    }}    
}}" :
$@"{Index} = {{
    religion = {{ is_in_family = {Family} }}
}}
{Index + 1 } = {{
    NOT = {{ religion = {{ is_in_family = {Family} }} }}
}}";
    
    public ISavable? Owner { get; set; }
    public int NumberOfFlagsNeeded => 2;
    public int Index { get; set; }
    public int IndexInGroup { get; set; }

    private string Family { get; init; }
    public bool IsOther { get; init; }
    public string DefaultCheck => $"NOT = {{ has_variable = {Variable} }}";
    public string ResetValue => $"remove_variable = {Variable}";
    public string SlotCheck
=> @$"trigger_if = {{
    limit = {{ NOT = {{ has_variable = {Variable} }} }}
    $SLOT2$ = {{ NOT = {{ has_variable = {Variable} }} }}
}}
trigger_else_if = {{
    limit = {{ $SLOT2$ = {{ NOT = {{ has_variable = {Variable} }} }} }}
    always = no
}}
trigger_else = {{
    var:{Variable} = $SLOT2$.var:{Variable}
}}";

    public string CopySlots => 
        $@"if = {{
    limit = {{ has_variable = {Variable} }}
    $TO$ = {{ set_variable = {{ name = {Variable} value = $FROM$.var:{Variable} }} }}
}}
else = {{
    $TO$ = {{ remove_variable = {Variable} }}
}}";
    public string ClearSlot => $"remove_variable = {Variable}";
    
    public string AddYesIfNeeded => 
$@"if = {{
    limit = {{ 
        trigger_if = {{
            limit = {{ has_variable = {Variable}  }}
            var:{Variable} = 1
        }}
        trigger_else = {{ always = no }}
    }}
    add_to_variable_list = {{ name = acs_vl_religion_filter target = {Index} }} 
}}";
    
public string AddIfNoNeeded => 
$@"if = {{
    limit = {{ 
        trigger_if = {{
            limit = {{ has_variable = {Variable}  }}
            var:{Variable} = 2
        }}
        trigger_else = {{ always = no }}
        
    }}
    add_to_variable_list = {{ name = acs_vl_religion_filter target = {Index + 1} }} 
    change_variable = {{ name = acs_v_religion_filter_count add = 1 }} 
}}";
}

public class ReligionFilter : ISavable, ILogicalElement
{
    
    public ReligionFilter(XmlElement element)
    {
        All = element.ChildNodes.OfType<XmlElement>().Select(x => new ReligionFamily(x) {Owner = this}).ToArray();
        Other = All.First(f => f.IsOther);
        Owner = MainSavable.Instance;
        MainSavable.Instance.Elements.Add(this);
    }
    
    private ReligionFamily[] All { get;  }
    public  ICollection<ReligionFamily> NonOtherReligion => All.Where(f => !f.IsOther).ToList();
    
    public static ReligionFamily Other { get; private set; } = null!;
    public string Name => "religion";
    public string DefaultCheck => @$"{All.Select(f => f.DefaultCheck).Join()}
any_in_list = {{ variable = acs_vl_has_faith always = yes count = 0 }}
any_in_list = {{ variable = acs_vl_has_religion always = yes count = 0 }}
any_in_list = {{ variable = acs_vl_dont_have_faith always = yes count = 0 }}
any_in_list = {{ variable = acs_vl_dont_have_religion always = yes count = 0 }}
OR = {{
    NOT = {{ has_variable = acs_v_religion_filter_count }}
    var:acs_v_religion_filter_count = 0
}}
any_in_list = {{ variable = acs_vl_religion_filter  always = yes count = 0 }}";

    public string ResetValue => @$"{All.Select(f => f.ResetValue).Join()}
clear_variable_list = acs_vl_has_faith
clear_variable_list = acs_vl_has_religion
clear_variable_list = acs_vl_dont_have_faith
clear_variable_list = acs_vl_dont_have_religion
remove_variable = acs_v_religion_filter_count
clear_variable_list = acs_vl_religion_filter";

    public string GetSlotCheck => @$"{All.Select(f => f.SlotCheck).Join()}
acs_stt_are_list_equal = {{ SLOT1 = $SLOT1$ SLOT2 = $SLOT2$ LIST_NAME = acs_vl_has_faith }}
acs_stt_are_list_equal = {{ SLOT1 = $SLOT1$ SLOT2 = $SLOT2$ LIST_NAME = acs_vl_has_religion }}
acs_stt_are_list_equal = {{ SLOT1 = $SLOT1$ SLOT2 = $SLOT2$ LIST_NAME = acs_vl_dont_have_faith }}
acs_stt_are_list_equal = {{ SLOT1 = $SLOT1$ SLOT2 = $SLOT2$ LIST_NAME = acs_vl_dont_have_religion }}
trigger_if = {{
    limit = {{ NOT = {{ has_variable = acs_v_religion_filter_count }} }}
    $SLOT2$ = {{
        trigger_if = {{
            limit = {{ has_variable = acs_v_religion_filter_count }}
            var:acs_v_religion_filter_count = 0
        }}
        trigger_else = {{ always = yes }}
    }}
}}
trigger_else_if = {{
    limit = {{ $SLOT2$ = {{ NOT = {{ has_variable = acs_v_religion_filter_count }} }} }}
    var:acs_v_religion_filter_count = 0
}}
trigger_else = {{
    var:acs_v_religion_filter_count = $SLOT2$.var:acs_v_religion_filter_count
}}
acs_stt_are_list_equal = {{ SLOT1 = $SLOT1$ SLOT2 = $SLOT2$ LIST_NAME = acs_vl_religion_filter }}";

    public bool HaveSomethingToSave => true;
    
    public List<ILogicalElement> Elements => All.Cast<ILogicalElement>().ToList();

    public string ScriptedGuiName => null!;

    public string CopySlots => 
@$"{All.Select(f => f.CopySlots).Join()}
acs_set_copy_list = {{ TO = $TO$ LIST_NAME = acs_vl_has_faith }}
acs_set_copy_list = {{ TO = $TO$ LIST_NAME = acs_vl_has_religion }}
acs_set_copy_list = {{ TO = $TO$ LIST_NAME = acs_vl_dont_have_faith }}
acs_set_copy_list = {{ TO = $TO$ LIST_NAME = acs_vl_dont_have_religion }}
if = {{
    limit = {{ has_variable = acs_v_religion_filter_count }}
    $TO$ = {{ set_variable = {{ name = acs_v_religion_filter_count value = $FROM$.var:acs_v_religion_filter_count }} }}
}}
else = {{
    $TO$ = {{ set_variable = {{ name = acs_v_religion_filter_count value = 0 }} }}
}}
acs_set_copy_list = {{ TO = $TO$ LIST_NAME = acs_vl_religion_filter }}";

    public string ClearSlot => @$"{All.Select(f => f.ClearSlot).Join()}
clear_variable_list = acs_vl_has_faith
clear_variable_list = acs_vl_has_religion
clear_variable_list = acs_vl_dont_have_faith
clear_variable_list = acs_vl_dont_have_religion
remove_variable = acs_v_religion_filter_count
clear_variable_list = acs_vl_religion_filter";
    
    public bool IsMakeReducedListAndCountSeparate => true;
    public string MakeReducedListAndCount => $@"acs_se_update_religion_filter = {{
    global_var:acs_gv_main_filters = {{
        set_variable = {{ name = acs_v_religion_filter_count value = 0  }}
        clear_variable_list = acs_vl_religion_filter
        {All.Select(f => f.AddYesIfNeeded).Join(2)}
        if = {{
            limit = {{ any_in_list = {{ variable = acs_vl_has_faith always = yes count > 0 }} }}
            add_to_variable_list = {{ name = acs_vl_religion_filter target = {NextFreeIndex} }} 
        }}
        if = {{
            limit = {{ any_in_list = {{ variable = acs_vl_has_religion always = yes count > 0 }} }}
            add_to_variable_list = {{ name = acs_vl_religion_filter target = {NextFreeIndex + 1} }} 
        }}     
        if = {{
            limit = {{ 
                any_in_list = {{ variable = acs_vl_religion_filter always = yes count = 0 }}
            }}
            {All.Select(f => f.AddIfNoNeeded).Join(3)}
            if = {{
                limit = {{ any_in_list = {{ variable = acs_vl_dont_have_faith always = yes count > 0 }} }}
                add_to_variable_list = {{ name = acs_vl_religion_filter target = {NextFreeIndex + 2} }} 
                change_variable = {{ name = acs_v_religion_filter_count add = 1 }} 
            }}
            if = {{
                limit = {{ any_in_list = {{ variable = acs_vl_dont_have_religion always = yes count > 0 }} }}
                add_to_variable_list = {{ name = acs_vl_religion_filter target = {NextFreeIndex + 3} }} 
                change_variable = {{ name = acs_v_religion_filter_count add = 1 }} 
            }}
            
        }}
        else = {{
            change_variable = {{ name = acs_v_religion_filter_count add = 1 }} 
        }}  
        if = {{
            limit = {{ 
                var:acs_v_religion_filter_count > 0
            }}
            add_to_variable_list = {{ name = {MainSavable.Instance.ListVariable(false)} target = {Index} }} 
        }}  
    }}
    
}}
";

    public string SwitchTrigger =>
$@"{Index} = {{
    global_var:acs_gv_main_filters = {{
        any_in_list = {{
            variable = acs_vl_religion_filter
            save_temporary_scope_as = filter2
            $CANDIDATE$ = {{
                switch = {{
                    trigger = scope:filter2
                    {All.Select(e => e.SwitchTrigger).Join(5)}
                    {NextFreeIndex} = {{
                        faith = {{ 
                            save_temporary_scope_as = acs_ts_faith
                            global_var:acs_gv_main_filters = {{
                                any_in_list = {{ variable = acs_vl_has_faith this = scope:acs_ts_faith count > 0 }}
                            }}
                        }}
                    }}
                    {NextFreeIndex + 1} = {{
                        religion = {{ 
                            save_temporary_scope_as = acs_ts_religion
                            global_var:acs_gv_main_filters = {{
                                any_in_list = {{ variable = acs_vl_has_religion this = scope:acs_ts_religion count > 0 }}
                            }}
                        }}
                    }}
                    {NextFreeIndex + 2} = {{
                        faith = {{ 
                            save_temporary_scope_as = acs_ts_faith
                            global_var:acs_gv_main_filters = {{
                                any_in_list = {{ variable = acs_vl_dont_have_faith this = scope:acs_ts_faith count = 0 }}
                            }}
                            
                        }}
                    }}
                    {NextFreeIndex + 3} = {{
                        religion = {{ 
                            save_temporary_scope_as = acs_ts_religion
                            global_var:acs_gv_main_filters = {{
                                 any_in_list = {{ variable = acs_vl_dont_have_religion this = scope:acs_ts_religion count = 0 }}
                            }}          
                        }}
                    }}
                }}
            }}
            count = global_var:acs_gv_main_filters.var:acs_v_religion_filter_count
        }}
    }}
}}";

    private int NextFreeIndex => All.Max(f=> f.Index) + 2;

    public ISavable? Owner { get; set; }

    public int NumberOfFlagsNeeded => 1;

    public int Index { get; set; }

    public int IndexInGroup { get; set; }
}