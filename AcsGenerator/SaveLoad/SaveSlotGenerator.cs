namespace Vipl.AcsGenerator.SaveLoad;

public static class SaveSlotGenerator
{
    public static List<SaveSlot> Slots { get; } = new();
    public static IEnumerable<SaveSlot> SlotsWithDefault => new[] {DefaultSlot}.Concat(Slots);
    public static SaveSlot DefaultSlot { get; private set; }

    public static void GenerateSaveSlot()
    {
        var allSavable = ISavable.All.ToArray();
        DefaultSlot = new SaveSlot(allSavable, -1);
        Slots.AddRange(Enumerable.Range(0, 13)
            .Select(i => new SaveSlot(allSavable, i)));
    }

    public static string CopySlots => Slots.First(s => !s.IsDefault).CopySlots;

    public static string SlotManipulation =>
        $@"{CopySlots}
{Reset}
{ClearSlot}
";

    public static string Reset => DefaultSlot.Reset;
    public static string ClearSlot => DefaultSlot.ClearSlot;
    public static string MakeReducedListAndCount => DefaultSlot.MakeReducedListAndCount;

    public static string? SlotEqualTrigger => @$"{DefaultSlot.SlotEqualTrigger}
{Slots.First(s => !s.IsDefault).SlotEqualTrigger}";
    
}