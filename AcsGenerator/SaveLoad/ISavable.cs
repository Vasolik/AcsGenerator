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
        
    }

    public static class ISavableExtensions
    {
        public static string MakePrevVariable(this ISavable toSave, int slot, string slotPrefix, bool fromPrev)
            => $"{toSave.Variable}{(fromPrev ? $"{slotPrefix}_{slot - 1}" : "")}";
        
        public static string MakeVariable(this ISavable toSave, int slot, string slotPrefix)
            => $"{toSave.Variable}{slotPrefix}_{slot}";
    }
        
}