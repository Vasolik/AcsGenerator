using Vipl.AcsGenerator.LogicalElements;

namespace Vipl.AcsGenerator.SaveLoad;

public interface ISavable
{
    public string Name { get; }
    public string CountVariable => $"{Name}_count";
    public string ListVariable => $"{Name}_list";
    public string DefaultCheck { get;  }
    string ResetValue { get;  }
    public string GetSlotCheck(int slot, string slotPrefix = "");
    string SaveToSlot(int slot, string slotPrefix = "", bool fromPrev = false);
    string LoadFromSlot(int slot, string slotPrefix = "", bool toPrev = false);
    bool HaveSomethingToSave { get; }
    public string MakeReducedListAndCount { get; }
    bool IsMain => false;
    public List<ILogicalElement> Elements { get; }
        
    public static List<ISavable> All { get; } = new();
    string ScriptedGuiName { get;  }
}
public class MainSavable : ISavable
{
    private MainSavable()
    {
    }
    private static MainSavable _instance;
    public static MainSavable Instance => _instance ??= new MainSavable();
    public bool IsMain => true;
    public string Name => "acs_active_filter";
    public string DefaultCheck => $@"any_in_global_list = {{ variable = {this.ListVariable()} always = yes count = 0 }}";
    public string ResetValue => $@"clear_global_variable_list = {this.ListVariable()}";
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
    public string MakeReducedListAndCount =>  "";
        
    public bool HaveSomethingToSave => true;

    public List<ILogicalElement> Elements { get; } = new ();

    public string ScriptedGuiName => "acs_filter_simple_checkbox";
}

public static class SavableExtensions
{
    public static string MakePrevVariable(this ISavable toSave, int slot, string slotPrefix, bool fromPrev)
        => $"{toSave.Name}{(fromPrev ? $"{slotPrefix}_{slot - 1}" : "")}";
        
    public static string MakeVariable(this ISavable toSave, int slot, string slotPrefix)
        => $"{toSave.Name}{slotPrefix}_{slot}";

    public static string MakePrevListVariable(this ISavable toSave, int slot, string slotPrefix, bool fromPrev)
        => $"{toSave.Name}_list{(fromPrev ? $"{slotPrefix}_{slot - 1}" : "")}";
        
    public static string MakeListVariable(this ISavable toSave, int slot, string slotPrefix)
        => $"{toSave.Name}_list{slotPrefix}_{slot}";
        
    public static string CountVariable(this ISavable savable) => savable.CountVariable;
    public static string ListVariable(this ISavable savable) => savable.ListVariable;

        
}