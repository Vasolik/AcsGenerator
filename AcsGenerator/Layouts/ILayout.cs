using Vipl.AcsGenerator.VisualElements;

namespace Vipl.AcsGenerator.Layouts
{

    public interface ILayout:  IVisualElement
    {
        string GuiElement { get; }
    }
}

