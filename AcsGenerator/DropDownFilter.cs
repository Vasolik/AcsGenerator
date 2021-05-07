using System;
using System.Collections.Generic;
using Vipl.AcsGenerator.SaveLoad;

namespace Vipl.AcsGenerator
{
    public class DropDownFilter : ISavable
    {
        public DropDownFilter(string variable, int defaultValue)
        {
            Variable = variable;
            DefaultValue = defaultValue;
        }

        public static IList<DropDownFilter> All { get; } = new List<DropDownFilter>();
        public string Variable { get;  }
        public int DefaultValue { get; }

        public string MakeReducedListAndCount => null;
        public static void Parse(string toParse)
        {
            foreach (var row in toParse.Tokenized())
            {
                var tokens = row.Tokenized(true);
                All.Add(new DropDownFilter(tokens[0], Int32.Parse(tokens[1])));
            }
        }

        public string DefaultCheck => $"global_var:{Variable} = {DefaultValue}";

        public string ResetValue
            => $"set_global_variable = {{ name = {Variable} value = {DefaultValue} }}";

        public string GetSlotCheck(int slot, string slotPrefix = "")
            => $"global_var:{Variable} = global_var:{this.MakeVariable(slot, slotPrefix)}";
        public string SaveToSlot(int slot, string slotPrefix = "", bool fromPrev = false)
            => $"set_global_variable = {{ name = {this.MakeVariable(slot, slotPrefix)} value = global_var:{this.MakePrevVariable(slot, slotPrefix, fromPrev)} }}";

        public string LoadFromSlot(int slot, string slotPrefix = "", bool toPrev = false)
            => @$"set_global_variable = {{ name = {this.MakePrevVariable(slot, slotPrefix, toPrev)} value = global_var:{this.MakeVariable(slot, slotPrefix)} }}";

        public bool HaveSomethingToSave => false;
    }
}