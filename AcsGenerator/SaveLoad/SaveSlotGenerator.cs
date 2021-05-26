using System.Collections.Generic;
using System.Linq;
using Vipl.AcsGenerator.LogicalElements;

namespace Vipl.AcsGenerator.SaveLoad
{
    public static class SaveSlotGenerator
    {
        public static List<SaveSlot> Slots { get; } = new();
        public static List<UndoSlot> UndoSlots { get; } = new();
        public static IEnumerable<SaveSlot> SlotsWithDefault => new[] {DefaultSlot}.Concat(Slots);
        public static SaveSlot DefaultSlot { get; private set;}
        
        public static void GenerateSaveSlot()
        {
            var allSavableList = DropDownFilter.All
                .Concat(ISavable.All).ToList();
            var allSavable = allSavableList.ToArray();
            DefaultSlot = new SaveSlot(allSavable, -1);
            Slots.AddRange(Enumerable.Range(0, 13)
                .Select(i => new SaveSlot(allSavable, i)));
            UndoSlots.AddRange(Enumerable.Range(0, UndoCount)
                .Select(i => new UndoSlot(allSavable, i, i == UndoCount - 1  )));
        }

        public static string SelectedSlot
            => $@"acs_current_slot_used = {{
    value = -1
    {SlotsWithDefault.Select(s => s.IsSlotUsed).Join(1)}
}}";

        private static int UndoCount => 10;
        public static string SaveToSlots => Slots.Select(s => s.SaveToSlot).Join();
        public static string LoadFromSlots => Slots.Select(s => s.LoadFromSlot).Join();
        public static string SaveToUndo => UndoSlots.Select(s => s.SaveToSlot).Join();
        public static string LoadFromUndo => UndoSlots.Select(s => s.LoadFromSlot).Join();
        public static string Reset => DefaultSlot.Reset;
        public static string MakeReducedListAndCount => DefaultSlot.MakeReducedListAndCount;

    }
}