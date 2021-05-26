using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using Vipl.AcsGenerator.VisualElements;

namespace Vipl.AcsGenerator.Layouts
{
    public class StarLayout : ILayout
    {
        public StarLayout(XmlElement element)
        {
            Top = SimpleCheckBoxVisualElement.All[element[nameof(Top)].GetAttribute("name")];
            Left = SimpleCheckBoxVisualElement.All[element[nameof(Left)].GetAttribute("name")];
            Right = SimpleCheckBoxVisualElement.All[element[nameof(Right)].GetAttribute("name")];
        }

        public IVisualElement Top { get; }
        public IVisualElement Left { get; }
        public IVisualElement Right { get; }
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
        public Trait[] Traits
            => Left.Traits.AsEnumerable()
                .Concat(Right.Traits)
                .Concat(Top.Traits)
                .ToArray();
        public string[] Localizations
            => Top.Localizations
                .Concat(Left.Localizations)
                .Concat(Right.Localizations)
                .ToArray();
    }
}