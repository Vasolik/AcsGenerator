using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using Vipl.AcsGenerator.SaveLoad;
using Vipl.AcsGenerator.VisualElements;

namespace Vipl.AcsGenerator.LogicalElements
{
    public class LogicalOrganisationGroup
    {
        public static readonly IList<LogicalOrganisationGroup> All = new List<LogicalOrganisationGroup>();
        public IList<ILogicalElement> Elements { get; }
        private LogicalOrganisationGroup()
        {
            All.Add(this);
            Elements = new List<ILogicalElement>();

        }

        public static void Parse(string toParse)
        {
            var tokens = toParse.Tokenized(true);
            
            LogicalOrganisationGroup logicalOrganisationGroup = null;
            CheckboxLogicalGroup checkboxLogicalGroup = null; 
            var prefix = "";
            string lgPrefix = null;
            Trait trait = null;
            var regex = new Regex("^(?<control>(?:[+/=!\\-*]|))(?<name>\\w+)$");
            foreach (var token in tokens)
            {
                var match = regex.Match(token);
                if (!match.Success)
                    throw new Exception("Invalid group.txt file");
                var name = match.Groups["name"].Value;
                switch ((match.Groups["control"].Value+ "x")[0])
                {
                    case '+':
                        prefix = name;
                        logicalOrganisationGroup = new LogicalOrganisationGroup();
                        break;
                    case '/':
                        prefix = token.Substring(1);
                        break;
                    case '=':
                        AddToAllSavable(checkboxLogicalGroup);
                        checkboxLogicalGroup = new CheckboxLogicalGroup($"{prefix}_{name}");
                        lgPrefix = null;
                        logicalOrganisationGroup?.Elements.Add(checkboxLogicalGroup);
                        break;
                    case '-':
                        lgPrefix = name;
                        break;
                    case '*' :
                        AddToAllSavable(checkboxLogicalGroup);
                        checkboxLogicalGroup = null;
                        lgPrefix = null;
                        break;
                    case '!':
                        trait?.HiddenTraits.Add(name);
                        break;
                    default:
                        var variable = prefix;
                        if (lgPrefix is not null)
                        {
                            variable += "_" + lgPrefix;
                        }

                        variable += "_" + name;
                        trait = new Trait(variable, name);
                        if (checkboxLogicalGroup is not null)
                        {
                            trait.Owner = checkboxLogicalGroup;
                            checkboxLogicalGroup.Elements.Add(trait);
                        }
                        else
                        {
                            trait.Owner = MainSavable.Instance;
                            logicalOrganisationGroup?.Elements.Add(trait);
                            MainSavable.Instance.Elements.Add(trait);
                        }
                        break;
                }
            }
            AddToAllSavable(checkboxLogicalGroup);
        }
        private static void AddToAllSavable(CheckboxLogicalGroup checkboxLogicalGroup)
        {
            if (checkboxLogicalGroup == null)
                return;
            if (((ISavable)checkboxLogicalGroup).HaveSomethingToSave)
            {
                ISavable.All.Add(checkboxLogicalGroup);
            }
            MainSavable.Instance.Elements.Add(checkboxLogicalGroup);
        }

        public static void Parse(XmlDocument document)
        { 
            var group = new LogicalOrganisationGroup();
            foreach (var element in document.GetElementsByTagName("Main").OfType<XmlElement>().First()!.ChildNodes.OfType<XmlElement>())
            {
                switch (element.Name)
                {
                    case "CheckBox":
                        var checkbox = new SimpleCustomCheckBoxVisualElement(element.GetAttribute("variable"), element["Icon"]!.InnerText, element["Localisation"]!.InnerText, element["PositiveTrigger"]!.InnerText, element["NegativeTrigger"]!.InnerText) {Owner = MainSavable.Instance};
                        MainSavable.Instance.Elements.Add(checkbox);
                        group.Elements.Add(checkbox);
                        
                        break;
                }
            }
        }
        
        public static void PrepareLogicalElements()
        {
            ISavable.All.Add(MainSavable.Instance);
            Parse(File.ReadAllText("groups.txt"));
            var logicalElementsDocument = new XmlDocument();
            logicalElementsDocument.Load("logicalElements.xml");
            Parse(logicalElementsDocument);

            foreach (var savable in ISavable.All)
            {
                var i = 0;
                foreach (var (element, j) in savable.Elements.Select((x, j)=> new Tuple<ILogicalElement, int>(x, j) ))
                {
                    element.IndexInGroup = j;
                    element.Index = i;
                    i += element.NumberOfFlagsNeeded;
                }
            }
        }
        public static string ScriptedGui => ISavable.All.OfType<CheckboxLogicalGroup>().Select(g => g.ScriptedGui).Join();
     
        public static string SwitchTrigger =>
$@"acs_switch_filter = {{
    $CANDIDATE$ = {{
        switch = {{
            trigger = $FILTER$
            {All.SelectMany(o => o.Elements.Select(e => e.SwitchTrigger)).Join(3)}
        }}
    }}
}}";


    }
}