using System.Xml;
using Vipl.AcsGenerator.VisualElements;

namespace Vipl.AcsGenerator.Layouts;

public class CustomLayout : ILayout
{
    public CustomLayout(ICheckBoxVisualElement[] leftElements, ICheckBoxVisualElement[] rightElements)
    {
        LeftElements = leftElements;
        RightElements = rightElements;
    }

    public CustomLayout(XmlElement element)
    {

        LeftElements = element["Left"].ChildNodes.OfType<XmlElement>()
            .Select(GetVisualElement).ToArray();
        RightElements = element["Right"].ChildNodes.OfType<XmlElement>()
            .Select(GetVisualElement).ToArray();
    }

    private static ICheckBoxVisualElement GetVisualElement(XmlElement element)
    {
        return element.Name switch
        {
            "LogicalElement" => SimpleCheckBoxVisualElement.All[element.GetAttribute("Name")],
            nameof(CustomCheckBoxVisualGroup) => CustomCheckBoxVisualGroup.All[element.GetAttribute("Name")],

            _ => throw new Exception("Invalid layout.txt file")
        };
    }

    public ICheckBoxVisualElement[] LeftElements { get; }
    public ICheckBoxVisualElement[] RightElements { get; }

    public string GuiElement => 
        $@"flowcontainer = {{
    flowcontainer = {{ 
        direction = vertical
        spacing = 5
        {LeftElements.Select(e => e.GetGuiElement("left")).Join(2)}
    }}
    flowcontainer = {{ 
        direction = vertical
        spacing = 5
        {RightElements.Select(e => e.GetGuiElement("right")).Join(2)}
    }}
}}";
    public LocalizationEntry[] Localizations
        => LeftElements.SelectMany(e => e.Localizations)
            .Concat(RightElements.SelectMany(e => e.Localizations)).ToArray();
}