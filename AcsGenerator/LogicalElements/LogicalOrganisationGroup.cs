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
        public static void Parse(XmlDocument document)
        {
            foreach (var element in document.GetElementsByTagName("Main").OfType<XmlElement>().First()!.ChildNodes.OfType<XmlElement>())
            {
                switch (element.Name)
                {
                    case "CheckBox":
                        var checkbox = new SimpleCustomCheckBoxVisualElement(element.GetAttribute("Variable"), element["Icon"]!.InnerText, element["Localisation"]!.InnerText, element["PositiveTrigger"]!.InnerText, element["NegativeTrigger"]!.InnerText) {Owner = MainSavable.Instance};
                        MainSavable.Instance.Elements.Add(checkbox);

                        break;
                    
                    case nameof(Trait):
                        var trait = new Trait(element);
                        MainSavable.Instance.Elements.Add(trait);
                        trait.Owner = MainSavable.Instance;
                        break;
                    case nameof(CheckboxLogicalGroup):
                        var group = new CheckboxLogicalGroup(element);
                        MainSavable.Instance.Elements.Add(group);
                        if (element.GetAttribute("IsOwnerMain") == "false")
                        {
                            ISavable.All.Add(group);
                        }
                        break;
                }
            }
        }
        
        public static void PrepareLogicalElements()
        {
            var logicalElementsDocument = new XmlDocument();
            logicalElementsDocument.Load("logicalElements.xml");
            Parse(logicalElementsDocument);
            ISavable.All.Add(MainSavable.Instance);
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
            {MainSavable.Instance.Elements.Select(e => e.SwitchTrigger).Join(3)}
        }}
    }}
}}";


    }
}