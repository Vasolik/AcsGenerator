using System.Xml;
using Vipl.AcsGenerator.VisualElements;

namespace Vipl.AcsGenerator.Layouts;

public class StarLayout : ILayout
{
    public StarLayout(XmlElement element)
    {
        Top = SimpleCheckBoxVisualElement.All[element[nameof(Top)].GetAttribute("Name")];
        Left = SimpleCheckBoxVisualElement.All[element[nameof(Left)].GetAttribute("Name")];
        Right = SimpleCheckBoxVisualElement.All[element[nameof(Right)].GetAttribute("Name")];
    }

    public ICheckBoxVisualElement Top { get; }
    public ICheckBoxVisualElement Left { get; }
    public ICheckBoxVisualElement Right { get; }
    public string GuiElement =>
        $@"widget = {{
    allow_outside = yes
    size = {{ 555 35 }}
    flowcontainer = {{ 
        position = {{ 0 -100 }}
        direction = vertical
        {Top.GetGuiElement("top").Intend(2)}
        flowcontainer = {{ 
            {Left.GetGuiElement("right").Intend(3)}
            {Right.GetGuiElement("left").Intend(3)}
        }}
    }}
}}";
    public LocalizationEntry[] Localizations
        => Top.Localizations
            .Concat(Left.Localizations)
            .Concat(Right.Localizations)
            .ToArray();
}