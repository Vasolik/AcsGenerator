using Vipl.AcsGenerator.SaveLoad;

namespace Vipl.AcsGenerator.LogicalElements;

public interface ILogicalElement 
{
    string SwitchTrigger { get; }
    ISavable Owner { get;  }
    int NumberOfFlagsNeeded { get; }
    int Index { get; set; }
    int IndexInGroup { get; set; }
}