namespace Vipl.AcsGenerator.SaveLoad
{
    public interface ISavable
    {
        public string Variable { get; }
        public string DefaultCheck { get;  }
        string ResetValue { get;  }
        public string GetSlotCheck(int slot, string slotPrefix = "");
        string SaveToSlot(int slot, string slotPrefix = "", bool fromPrev = false);
        string LoadFromSlot(int slot, string slotPrefix = "", bool toPrev = false);
        bool HaveSomethingToSave => true;
        public string MakeReducedListAndCount { get; }
    }
    public class MainSavable : ISavable
    {
        private MainSavable()
        {
        }
        private static MainSavable _instance;
        public static MainSavable Instance => _instance ??= new MainSavable();

        public string CountVariable => $"{Variable}_count";
        public string ListVariable => $"{Variable}_list";

        public string Variable => "acs_active_filter";
        public string DefaultCheck => 
$@"NOT = {{
    any_in_global_list = {{
        variable = {ListVariable}
        always = yes
    }} 
}}";
        public string ResetValue => $@"clear_global_variable_list = {ListVariable}";
        public string GetSlotCheck(int slot, string slotPrefix = "") => 
$@"any_in_global_list = {{
    variable = {this.MakeListVariable(slot, slotPrefix)}
    any_in_global_list = {{
        variable = {ListVariable}
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
        public string MakeReducedListAndCount =>
$@"set_global_variable = {{ name = {CountVariable} value = 0  }} 
every_in_global_list = {{
    variable = {ListVariable}
    change_global_variable = {{ name = {CountVariable} add = 1 }} 
}}
set_variable = {{ name = {CountVariable} value = global_var:{CountVariable}  }}";
    }

    public static class SavableExtensions
    {
        public static string MakePrevVariable(this ISavable toSave, int slot, string slotPrefix, bool fromPrev)
            => $"{toSave.Variable}{(fromPrev ? $"{slotPrefix}_{slot - 1}" : "")}";
        
        public static string MakeVariable(this ISavable toSave, int slot, string slotPrefix)
            => $"{toSave.Variable}{slotPrefix}_{slot}";

        public static string MakePrevListVariable(this ISavable toSave, int slot, string slotPrefix, bool fromPrev)
            => $"{toSave.Variable}_list{(fromPrev ? $"{slotPrefix}_{slot - 1}" : "")}";
        
        public static string MakeListVariable(this ISavable toSave, int slot, string slotPrefix)
            => $"{toSave.Variable}_list{slotPrefix}_{slot}";
    }
        
}