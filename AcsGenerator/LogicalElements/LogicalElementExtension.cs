using Vipl.AcsGenerator.SaveLoad;

namespace Vipl.AcsGenerator.LogicalElements;

public static class LogicalElementExtension 
{
    public static string GetListVariable(this ILogicalElement element, bool isGlobal) =>  (element.Owner?.HaveSomethingToSave ?? false) ? element.Owner.ListVariable(isGlobal) : MainSavable.Instance.ListVariable(isGlobal);
}