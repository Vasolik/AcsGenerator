using System;
using System.Collections.Generic;
using System.Linq;
using Vipl.AcsGenerator.SaveLoad;

namespace Vipl.AcsGenerator.LogicalElements
{

    public class CheckboxLogicalGroup : ILogicalElement, ISavable
    {

        public List<ICheckboxLogicalElement> Elements { get;  }

        public CheckboxLogicalGroup(string variable)
        {
            Variable = variable;
            Elements = new List<ICheckboxLogicalElement>();
        }
        
        public string GetSwitchForCombo(int[] combo)
        {
            if (combo.Any(x => x == 1))
            {
                var positiveElements = Elements.Where((_, i) => combo[i] == 1);
                return 
$@"{GetFlagForCombo(combo)} = {{
    if = {{
        limit = {{
            OR = {{
                {positiveElements.Select(e => e.PositiveTrigger).Join(4)}
            }}
        }}
        change_global_variable = {{ name = acs_filter_passed add = 1 }} 
    }}
}}";
            }
            var negativeElements = Elements.Where((_, i) => combo[i] == 2);
            return 
$@"{GetFlagForCombo(combo)} = {{
    if = {{
        limit = {{
            OR = {{
                {negativeElements.Select(e => e.NegativeTrigger).Join(4)}
            }}
        }}
        change_global_variable = {{ name = acs_filter_passed add = 1 }} 
    }}
}}";
        }
        
        public string MakeReducedListAndCount =>
$@"clear_global_variable_list = {ListReducedVariable}
set_global_variable = {{ name = {this.CountVariable()} value = 0  }} 
every_in_global_list = {{
    variable = {this.ListVariable()}
    save_temporary_scope_as = temp
    if = {{
        limit = {{
            OR = {{
                {Elements.Select(e => $"{e.PositiveFlag} = scope:temp").Join(4)}
            }}
        }}
        add_to_global_variable_list = {{ name = {ListReducedVariable} target = scope:temp }}
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
        save_temporary_scope_as = temp
        if = {{
            limit = {{
                OR = {{
                    {Elements.Select(e => $"{e.NegativeFlag} = scope:temp").Join(5)}
                }}
            }}
            add_to_global_variable_list = {{ name = {ListReducedVariable} target = scope:temp }}
            change_global_variable = {{ name = {this.CountVariable()} add = 1 }} 
        }}
    }}
}}
set_variable = {{ name = {this.CountVariable()} value = global_var:{this.CountVariable()}  }}";
        public string SwitchForSmallGroup =>
            AllFlagIndexes.Select(GetSwitchForCombo).Join();
        public string Switch => IsSmall ? SwitchForSmallGroup : SwitchForLargeGroup;
        public string SwitchForLargeGroup => 
$@"{Flag} = {{
    set_global_variable = {{ name = acs_filter_local_passed value = 0  }} 
    every_in_global_list = {{
        variable = {ListReducedVariable}
        save_scope_as = filter_local_flag
        {Variable} = {{ FILTER2 = scope:filter_local_flag CANDIDATE2 = prev }}
    }}
    if = {{ 
        limit = {{  
            AND = {{
                global_var:acs_filter_local_passed = global_var:{this.CountVariable()}
            }}
        }}
        change_global_variable = {{ name = acs_filter_passed add = 1 }}  
    }}
}}";
        
        public string SmallSwitchForLargeGroup => IsSmall ? null :
$@"{Variable} = {{
    $CANDIDATE2$ = {{
        switch = {{
            trigger = $FILTER2$
            {Elements.Select(e => e.Switch).Join(3)}
        }}
    }}
}}";
        private int GetIndexOfElement(ICheckboxLogicalElement trait) => Elements.IndexOf(trait);
        private int[][] AllFlagIndexes =>
            Enumerable.Range(1, (int)Math.Pow(3, Elements.Count) - 1)
                .Select(x => Enumerable.Range(0, Elements.Count).Select(y => x /(int)Math.Pow(3, y) % 3 ).ToArray())
                .ToArray();
        private string GetFlagForCombo(int[] combo)
        {
            return $"{Flag}_" + combo.Select(x => x switch
            {
                0 => "0",
                1 => "p",
                _ => "n"
            }).Join(separator: "" );
        }
        public string[] GetFlags(ICheckboxLogicalElement trait, int value)
            => AllFlagIndexes.Where(x => x[GetIndexOfElement(trait)] == value)
                .Select(GetFlagForCombo)
                .ToArray();
        public string GetZeroFlag(ICheckboxLogicalElement element, int value)
        {
            var combo = new int[Elements.Count];
            combo[GetIndexOfElement(element)] = value;
            return GetFlagForCombo(combo);
        }
        public string[][] GetAllTransformation(ICheckboxLogicalElement element, int oldValue, int newValue)
        {
            var allOldCombos = AllFlagIndexes.Where(x => x[GetIndexOfElement(element)] == oldValue);
            return allOldCombos.Select(c =>
                {
                    var newCombo = (int[])c.Clone();
                    newCombo[GetIndexOfElement(element)] = newValue;
                    return new[] {GetFlagForCombo(c), GetFlagForCombo(newCombo)};
                }).Where(tp => tp[1] != GetZeroFlag(element, 0))
                .ToArray();
        }
        public string[] AllFlags => AllFlagIndexes.Select(GetFlagForCombo).ToArray();
        public string Flag => $"flag:{Variable}";
        
        public string ListReducedVariable => $"{this.ListVariable()}_reduced";
        public ISavable Owner => null;
        public string Variable { get; }
        public string DefaultCheck => IsSmall ? null :
$@"NOT = {{
    any_in_global_list = {{
        variable = {this.ListVariable()}
        always = yes
    }} 
}}";
        public string ResetValue => IsSmall ? null : $@"clear_global_variable_list = {this.ListVariable()}";
        public string GetSlotCheck(int slot, string slotPrefix = "") => 
            $@"any_in_global_list = {{
    variable = {this.MakeListVariable(slot, slotPrefix)}
    any_in_global_list = {{
        variable = {this.ListVariable()}
        prev = this
    }}
    count = all
}}";
        public string LoadFromSlot(int slot, string slotPrefix = "", bool fromPrev = false)
            => $@"clear_global_variable_list = {this.MakePrevListVariable(slot, slotPrefix, fromPrev)}
every_in_global_list = {{
    variable = {this.MakeListVariable(slot, slotPrefix)}
    add_to_global_variable_list = {{ name = {this.MakePrevListVariable(slot, slotPrefix, fromPrev)} target = this }}
}}";
        public string SaveToSlot(int slot, string slotPrefix = "", bool toPrev = false)
            => $@"clear_global_variable_list = {this.MakeListVariable(slot, slotPrefix)}
every_in_global_list = {{
    variable = {this.MakePrevListVariable(slot, slotPrefix, toPrev)}
    add_to_global_variable_list = {{ name = {this.MakeListVariable(slot, slotPrefix)} target = this }}
}}";

        public bool IsSmall => Elements.Count < 4;
        
        bool ISavable.HaveSomethingToSave => !IsSmall;
        public string FullPositiveFlag => GetFlagForCombo(Enumerable.Repeat(1, Elements.Count).ToArray());
        public string FullNegativeFlag => GetFlagForCombo(Enumerable.Repeat(2, Elements.Count).ToArray());
    }
    
}