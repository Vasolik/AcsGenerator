﻿using System.Linq;

namespace Vipl.AcsGenerator.SaveLoad
{
    public class UndoSlot
    {
        public UndoSlot(ISavable[] items, int slot, bool isLast)
        {
            Items = items;
            Slot = slot;
            IsLast = isLast;
        }

        public ISavable[] Items { get; }
        public int Slot { get; }
        public bool IsLast { get; }
        public bool IsFirst => Slot == 0;

        public string LoadFromSlot
            => $@"acs_load_undo_{Slot}_filters = {{
    if = {{
        limit = {{
            has_global_variable = asc_save_slot_undo_{Slot}_used
        }}
        {Items.Select(i => i.LoadFromSlot(Slot, "_undo", !IsFirst)).Join(2)}
        {(IsLast ? RemoveCurrentSlot : LoadFromNextSlot).Intend(2)}{(IsFirst? "\n        acs_auto_apply_sorting_and_filters = yes" : "") }
    }}
}}";

        public string LoadFromNextSlot
            => @$"if = {{
    limit = {{
        NOT = {{ has_global_variable = asc_save_slot_undo_{Slot + 1}_used }}
    }}
    {RemoveCurrentSlot}
}}
acs_load_undo_{Slot + 1}_filters = yes";

        public string SaveToNextSlot => !IsLast ? $@"
if = {{
    limit = {{
        has_global_variable = asc_save_slot_undo_{Slot}_used    
    }}
    acs_save_undo_{Slot + 1}_filters = yes
}}": "";
        public string RemoveCurrentSlot
            => $"remove_global_variable = asc_save_slot_undo_{Slot}_used";
        
        public string SaveToSlot
            => $@"acs_save_undo_{Slot}_filters = {{{SaveToNextSlot.Intend(1)}
    {Items.Select(i => i.SaveToSlot(Slot, "_undo", !IsFirst)).Join(1)}
    set_global_variable = asc_save_slot_undo_{Slot}_used
}}";
    }
}