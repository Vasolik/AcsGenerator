using Vipl.AcsGenerator.VisualElements;

namespace Vipl.AcsGenerator.Layouts
{
    public interface ILayout: ILocalizable
    {
        string GuiElement { get; }
        
        string ScriptedGui { get; }
        public Trait[] Traits { get; }
    }
}

