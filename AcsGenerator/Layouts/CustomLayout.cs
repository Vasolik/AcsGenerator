using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using Vipl.AcsGenerator.VisualElements;

namespace Vipl.AcsGenerator.Layouts
{
    public class CustomLayout : ILayout
    {
        public CustomLayout(IVisualElement[] leftElements, IVisualElement[] rightElements)
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

        public Trait[] Traits
            => LeftElements.SelectMany(e => e.Traits)
                .Concat(RightElements.SelectMany(e => e.Traits)).ToArray();

        private static IVisualElement GetVisualElement(XmlElement element)
        {
            return element.Name switch
            {
                "LogicalElement" => SimpleCheckBoxVisualElement.All[element.GetAttribute("name")],
                nameof(CustomCheckBoxVisualGroup) => CustomCheckBoxVisualGroup.All[element.GetAttribute("name")],

                _ => throw new Exception("Invalid layout.txt file")
            };
        }

        public IVisualElement[] LeftElements { get; }
        public IVisualElement[] RightElements { get; }

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
        public string[] Localizations
            => LeftElements.SelectMany(e => e.Localizations)
                .Concat(RightElements.SelectMany(e => e.Localizations)).ToArray();
    }
}