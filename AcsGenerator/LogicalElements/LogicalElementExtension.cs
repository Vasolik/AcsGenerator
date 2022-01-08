using Vipl.AcsGenerator.SaveLoad;

namespace Vipl.AcsGenerator.LogicalElements;

public static class LogicalElementExtension 
{
    public static string GetListVariable(this ILogicalElement element) =>  element.Owner.HaveSomethingToSave ? element.Owner.ListVariable : MainSavable.Instance.ListVariable();
}