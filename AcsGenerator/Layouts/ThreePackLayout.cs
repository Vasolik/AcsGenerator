using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using Vipl.AcsGenerator.VisualElements;

namespace Vipl.AcsGenerator.Layouts
{
    public class ThreePackLayout : ILayout
    {
        public ThreePackLayout(XmlElement element)
        {
            Elements = element.ChildNodes.OfType<XmlElement>()
                .Select(x => SimpleCheckBoxVisualElement.All[x.GetAttribute("name")])
                .Cast<IVisualElement>()
                .ToArray();
        }

        public IVisualElement[] Elements { get; }
   
        public string GuiElement =>
            $@"flowcontainer = {{
    {Elements[0].GetGuiElement("left_third").Intend(1)}
    {Elements[1].GetGuiElement("left_third").Intend(1)}
    {Elements[2].GetGuiElement("right_third").Intend(1)}
}}";
        public string[] Localizations
            => Elements.SelectMany(e => e.Localizations).ToArray();
        public Trait[] Traits
            => Elements.SelectMany(e => e.Traits).ToArray();
    }
}