using System;
using System.Linq;
using System.Text.RegularExpressions;
using Vipl.AcsGenerator.VisualElements;

namespace Vipl.AcsGenerator.Layouts
{
    public class NormalLayout : ILayout
    {
        public NormalLayout(IVisualElement left, IVisualElement right)
        {
            Left = left;
            Right = right;
        }

        public NormalLayout(string row)
        {
            var normalLayoutRegex = new Regex("^(?<left>\\w+)\\s+(?<right>\\w+)");
            var rowInfo = normalLayoutRegex.Match(row);
            if (!rowInfo.Success)
                throw new Exception("Invalid layout file");
            Left = SimpleCheckBoxVisualElement.All[rowInfo.Groups["left"].Value];
            Right = SimpleCheckBoxVisualElement.All[rowInfo.Groups["right"].Value];
        }
        public IVisualElement Left { get; }
        public IVisualElement Right { get; }
        public string GuiElement => 
$@"flowcontainer = {{ 
    {Left.GetGuiElement("left").Intend(1)}
    {Right.GetGuiElement("right").Intend(1)} 
}}";
        public string ScriptedGui => Left.ScriptedGui + Environment.NewLine + Right.ScriptedGui;
        public Trait[] Traits
            => Left.Traits.AsEnumerable().Concat(Right.Traits).ToArray();

        public string[] Localizations
            => Left.Localizations
                .Concat(Right.Localizations)
                .ToArray();
    }
}