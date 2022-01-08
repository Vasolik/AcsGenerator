namespace Vipl.AcsGenerator.LogicalElements;

public interface ICheckboxLogicalElement : ILogicalElement
{
    public string NegativeTrigger { get; }
    public string PositiveTrigger { get; }
    public int PositiveIndex { get; }
    public int NegativeIndex { get; }
}