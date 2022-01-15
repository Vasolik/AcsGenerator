using Vipl.AcsGenerator.LogicalElements;

namespace Vipl.AcsGenerator.SaveLoad;

public interface ISavable
{
    public string Name { get; }
    public string CountVariable(bool isGlobal) => $"acs_{(isGlobal ? "g": "")}v_filter_{Name}_count";
    public string ListVariable(bool isGlobal) => $"acs_{(isGlobal ? "g": "")}vl_filter_{Name}";
    public string? DefaultCheck { get;  }
    string? ResetValue { get;  }
    public string GetSlotCheck { get; }
    bool HaveSomethingToSave { get; }
    public string MakeReducedListAndCount { get; }
    bool IsMain => false;
    public List<ILogicalElement> Elements { get; }
        
    public static List<ISavable> All { get; } = new();
    string ScriptedGuiName { get;  }
    string CopySlots { get; }
    string? ClearSlot { get;  }
}
public class MainSavable : ISavable
{
    private MainSavable()
    {
    }
    private static MainSavable? _instance;
    public static MainSavable Instance => _instance ??= new MainSavable();
    public bool IsMain => true;
    public string Name => "active";
    public string DefaultCheck => $@"any_in_list = {{ variable = {this.ListVariable(false)} always = yes count = 0 }}";
    public string ResetValue => $@"clear_variable_list = {this.ListVariable(false)}";
    public string GetSlotCheck =>
        $@"acs_stt_are_list_equal = {{ SLOT1 = $SLOT1$ SLOT2 = $SLOT2$ LIST_NAME = {this.ListVariable(false)} }}";
    public string MakeReducedListAndCount =>  "";
        
    public bool HaveSomethingToSave => true;

    public List<ILogicalElement> Elements { get; } = new ();

    public string ScriptedGuiName => "acs_filter_simple_checkbox";
    public string CopySlots =>
$@"acs_set_copy_list = {{ TO = $TO$ LIST_NAME = {this.ListVariable(false)} }}";

    public string ClearSlot => ResetValue;
}

public static class SavableExtensions
{
    public static string CountVariable(this ISavable savable, bool isGlobal) => savable.CountVariable(isGlobal);
    public static string ListVariable(this ISavable savable, bool isGlobal) => savable.ListVariable(isGlobal);
}