using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Vipl.AcsGenerator.VisualElements
{
    public sealed class CustomCheckBoxVisualGroup : CheckBoxVisualGroup, ICustomCheckBoxVisualElement
    {
        public static IDictionary<string, CustomCheckBoxVisualGroup> All { get; } = new Dictionary<string, CustomCheckBoxVisualGroup>();
        public CustomCheckBoxVisualGroup(string variable, SimpleCheckBoxVisualElement[] elements, string icon, string localization)
        {
            Variable = variable;
            Elements = elements;
            Icon = icon;
            Localization = localization;
            All[Variable] = this;
        }
        
        public CustomCheckBoxVisualGroup(string row)
        {
            Regex regex =
                new Regex("(?<variable>\\w+)\\s(?<traits>(?:\\w+\\s+)+)(?<localization>\"[^\"]+\")\\s+(?<icon>\"[^\"]+\")");
            var rowInfo = regex.Match(row);
            if (!rowInfo.Success)
                throw new Exception("Invalid visual_groups.txt file");
            Variable = rowInfo.Groups["variable"].Value;
            Elements = rowInfo.Groups["traits"].Value.Tokenized(true).Select(t => Trait.All[t]).Cast<SimpleCheckBoxVisualElement>().ToArray();
            Icon = rowInfo.Groups["icon"].Value;
            Localization = rowInfo.Groups["localization"].Value;
            All[Variable] = this;
        }

        public string Localization { get;  }
        public string Icon { get;  }
        protected override string GetGroupGuiElement(string style) 
            => ((ICustomCheckBoxVisualElement)this).GetCustomCheckBox(style);
        protected override SimpleCheckBoxVisualElement[] Elements { get; }
        
        public string MajorDigit => this.GetDigit(LogicalOwner.Index / 40);
        public string MinorDigit => this.GetDigit(LogicalOwner.Index % 40);
        public override string GetSetScopes(int value) =>
            LogicalOwner.IsSmall ? GetSetScopeForSmallGroups(value) : GetSetScopeForLargeGroups(value);
        
        public string GetSetScopeForLargeGroups(int value ) => $"GuiScope.SetRoot( {this.GetDigit(value)} ).End";
        public string GetSetScopeForSmallGroups(int value ) => 
            $"GuiScope.SetRoot( {this.GetDigit(value)} )" +
            $".AddScope( 'major_digit' , {MajorDigit} )" +
            $".AddScope( 'minor_digit' , {MinorDigit} )" +
            ".End";

        public override Trait[] Traits => Elements.SelectMany(e => e.Traits).ToArray();
        public override string[] Localizations
            => new[] {$" {Variable}:0 {Localization}"}
                .Concat(Elements.SelectMany(e => e.Localizations)).ToArray();
        public static void Parse(string toParse)
        {

            foreach (var row in toParse.Tokenized())
            {
                // ReSharper disable once HeapView.ObjectAllocation.Evident
                // ReSharper disable once ObjectCreationAsStatement
                new CustomCheckBoxVisualGroup(row);
            }
        }
    }
}