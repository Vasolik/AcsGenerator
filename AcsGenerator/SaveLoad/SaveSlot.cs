﻿namespace Vipl.AcsGenerator.SaveLoad;

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

    public string CopySlots
        => $@"acs_set_copy_save_slots = {{ # FROM : scope, TO : scope
    $FROM$ = {{
        {Items.Select(i => i.CopySlots).Join(2)}
    }} 
}}";
    
    public string ClearSlot
=> $@"acs_set_clear_save_slot = {{ # SLOT : scope
    $SLOT$ = {{
        {Items.Select(i => i.ClearSlot).Join(2)}
    }} 
}}";

    public string Reset => 
$@"acs_set_reset_save_slot_to_default = {{ # SLOT : scope
    $SLOT$ = {{ 
        {Items.Select(i => i.ResetValue).Join(2)}  
    }}
}}";
    public string MakeReducedListAndCount => 
        $@"acs_make_reduced_and_count = {{ 
    {Items.Select(i => i.MakeReducedListAndCount).Join(1)}
}}";

    public string SlotEqualTrigger => IsDefault ? SlotEqualTriggerDefault : SlotEqualTriggerNormal;
    public string SlotEqualTriggerNormal =>
$@"acs_stt_equal_slots = {{ # SLOT1 : scope, SLOT2 : scope
    $SLOT1$ = {{
        {Items.Select(i => i.GetSlotCheck).Join(2)}
    }}
}}";

    public string SlotEqualTriggerDefault =>
$@"acs_stt_is_slot_default = {{ # SLOT : scope
    $SLOT$ = {{
        {Items.Select(i => i.DefaultCheck).Join(2)}
    }}
}}";
    
}