using System.Linq;

namespace Vipl.AcsGenerator.SaveLoad
{
    public class SaveSlot
    {
        public SaveSlot(ISavable[] items, int slot)
        {
            Items = items;
            Slot = slot;
        }

        public ISavable[] Items { get; }
        public int Slot { get; }
        public bool IsDefault => Slot < 0;

        public string IsSlotUsed
            =>
                $@"{(IsDefault? "": "else_")}if = {{
    limit = {{{(!IsDefault ? $"\n        has_global_variable = asc_save_slot_{Slot}_used": "")}
        {(IsDefault ? Items.Select(i => i.DefaultCheck) : Items.Select(i => i.GetSlotCheck(Slot))).Join(2)}
    }}
    add = { ( IsDefault ? Slot : Slot + 1)}
}}";

        public string SaveToSlot
            => $@"acs_save_{Slot}_filters = {{
    if = {{
        limit = {{
            NOT = {{ acs_current_slot_used = -2 }}
        }}
        {Items.Select(i => i.SaveToSlot(Slot)).Join(2)} 
        set_global_variable = asc_save_slot_{Slot}_used
    }}
    else = {{
        remove_global_variable = asc_save_slot_{Slot}_used
    }}
}}";
        
        public string LoadFromSlot
            => $@"acs_load_{Slot}_filters = {{
    if = {{
        limit = {{
            has_global_variable = asc_save_slot_{Slot}_used
        }}
        if = {{
            limit = {{  NOT = {{ acs_current_slot_used = {Slot} }} }}
            acs_save_undo_0_filters = yes
        }}
        {Items.Select(i => i.LoadFromSlot(Slot)).Join(2)}
        acs_se_auto_apply_sorting_and_filters = yes
    }}  
}}";

        public string Reset => 
            $@"acs_reset_filters_and_sorting = {{
    if = {{
        limit = {{ 
            NOR = {{
                NOT = {{ has_global_variable = acs_first_time_setup_v8_1 }}
                acs_current_slot_used = -2 
            }}
        }}
        acs_save_undo_0_filters = yes
    }}
    set_global_variable = {{ name = acs_sort_by value = 0  }}
    set_global_variable = acs_sort_by_ascending
    set_global_variable = {{ name = acs_select_count value = 100 }}
    {Items.Select(i => i.ResetValue).Join(1)}  
    if = {{
        limit = {{
            NOT = {{ has_global_variable = acs_first_time_setup_v8_1 }}
        }}
        set_global_variable = acs_gv_auto_apply_sorting_and_filters
        set_global_variable = acs_first_time_setup_v8_1
    }}
}}";
        public string MakeReducedListAndCount => 
$@"acs_make_reduced_and_count = {{
    {Items.Select(i => i.MakeReducedListAndCount).Join(1)}
}}";    
    }
}