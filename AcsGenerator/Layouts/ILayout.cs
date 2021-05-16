using Vipl.AcsGenerator.VisualElements;

namespace Vipl.AcsGenerator.Layouts
{
    public interface ILayout: ILocalizable
    {
        string GuiElement { get; }
        public Trait[] Traits { get; }
    }
}

