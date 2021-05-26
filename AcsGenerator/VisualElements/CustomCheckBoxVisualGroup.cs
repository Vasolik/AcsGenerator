﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;

namespace Vipl.AcsGenerator.VisualElements
{
    public sealed class CustomCheckBoxVisualGroup : CheckBoxVisualGroup, ICustomCheckBoxVisualElement
    {
        public static IDictionary<string, CustomCheckBoxVisualGroup> All { get; } = new Dictionary<string, CustomCheckBoxVisualGroup>();

        public CustomCheckBoxVisualGroup(XmlElement element)
        {

            Variable = element.GetAttribute(nameof(Variable));
            Elements = element.ChildNodes.OfType<XmlElement>().Select(t => Trait.All[t.GetAttribute("Name")]).Cast<SimpleCheckBoxVisualElement>().ToArray();
            Icon = element.GetAttribute(nameof(Icon));
            Localization = element.GetAttribute(nameof(Localization));
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
            => new[] {$" {Variable}:0 \"{Localization}\""}
                .Concat(Elements.SelectMany(e => e.Localizations)).ToArray();
        public static void Parse()
        {
            
            var layoutsDocument = new XmlDocument();
            layoutsDocument.Load("visual_groups.xml");
            
            foreach (var element in layoutsDocument.GetElementsByTagName("Main").OfType<XmlElement>().First()!.ChildNodes.OfType<XmlElement>())
            {
                new CustomCheckBoxVisualGroup(element);
            }
            
        }
    }
}