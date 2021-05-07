using Vipl.AcsGenerator.SaveLoad;

namespace Vipl.AcsGenerator.LogicalElements
{
    public interface ILogicalElement 
    {
        string Switch { get; }
        
        ISavable Owner { get;  }
    }
}