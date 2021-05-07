using System;
using System.Linq;
using System.Text.RegularExpressions;
using Vipl.AcsGenerator.VisualElements;

namespace Vipl.AcsGenerator.Layouts
{
    public class ThreePackLayout : ILayout
    {
        public ThreePackLayout(IVisualElement[] elements)
        {
            Elements = elements;
        }
        
        public ThreePackLayout(string row)
        {
            var threePackRegex = new Regex("^(\\w+)(?:\\s+(\\w+)){2}");
            var rowInfo = threePackRegex.Match(row);
            if(!rowInfo.Success)
                throw new Exception("Invalid layout file");
            Elements = row.Tokenized(true)
                .Select(x => SimpleCheckBoxVisualElement.All[x])
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

        public string ScriptedGui =>
            Elements.Select(e => e.ScriptedGui).Join();
        
        public string[] Localizations
            => Elements.SelectMany(e => e.Localizations).ToArray();
        public Trait[] Traits
            => Elements.SelectMany(e => e.Traits).ToArray();
    }
}