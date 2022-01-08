using System.Xml;
using Vipl.AcsGenerator.VisualElements;

namespace Vipl.AcsGenerator.Layouts;

public class NormalLayout : ILayout
{

    public NormalLayout(XmlElement element)
    {
 
        Left = SimpleCheckBoxVisualElement.All[element[nameof(Left)].GetAttribute("Name")];
        Right = SimpleCheckBoxVisualElement.All[element[nameof(Right)].GetAttribute("Name")];
    }
    public ICheckBoxVisualElement Left { get; }
    public ICheckBoxVisualElement Right { get; }
    public string GuiElement => 
        $@"flowcontainer = {{ 
    {Left.GetGuiElement("left").Intend(1)}
    {Right.GetGuiElement("right").Intend(1)} 
}}";

    public LocalizationEntry[] Localizations
        => Left.Localizations
            .Concat(Right.Localizations)
            .ToArray();
}