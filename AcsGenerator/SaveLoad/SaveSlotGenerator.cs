namespace Vipl.AcsGenerator.SaveLoad;

public static class SaveSlotGenerator
{

    private static SaveSlot DefaultSlot { get; set; } = null!;

    public static void GenerateSaveSlot()
    {
        DefaultSlot = new SaveSlot(ISavable.All.ToArray());
    }

    public static string CopySlots => DefaultSlot.CopySlots;

    public static string SlotManipulation =>
        $@"{CopySlots}
{Reset}
{ClearSlot}
";

    public static string Reset => DefaultSlot.Reset;
    public static string ClearSlot => DefaultSlot.ClearSlot;
    public static string MakeReducedListAndCount => DefaultSlot.MakeReducedListAndCount;

    public static string SlotEqualTrigger => @$"{DefaultSlot.SlotEqualTriggerDefault}
{DefaultSlot.SlotEqualTriggerNormal}";
    
}