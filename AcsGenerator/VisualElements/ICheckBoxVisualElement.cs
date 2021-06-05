namespace Vipl.AcsGenerator.VisualElements
{
    public interface ICheckBoxVisualElement : IVisualElementWithScriptedGui
    {
        string CheckBoxFrameSelector { get; }
        string GetGuiElement(string style);
        
    }
}