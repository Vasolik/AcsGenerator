using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using Vipl.AcsGenerator.VisualElements;

namespace Vipl.AcsGenerator.Layouts
{
    public class NormalLayout : ILayout
    {

        public NormalLayout(XmlElement element)
        {
 
            Left = SimpleCheckBoxVisualElement.All[element[nameof(Left)].GetAttribute("name")];
            Right = SimpleCheckBoxVisualElement.All[element[nameof(Right)].GetAttribute("name")];
        }
        public IVisualElement Left { get; }
        public IVisualElement Right { get; }
        public string GuiElement => 
$@"flowcontainer = {{ 
    {Left.GetGuiElement("left").Intend(1)}
    {Right.GetGuiElement("right").Intend(1)} 
}}";
        public Trait[] Traits
            => Left.Traits.AsEnumerable().Concat(Right.Traits).ToArray();

        public string[] Localizations
            => Left.Localizations
                .Concat(Right.Localizations)
                .ToArray();
    }
}