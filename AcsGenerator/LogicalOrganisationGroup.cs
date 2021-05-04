using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Vipl.AcsGenerator
{
    public class LogicalOrganisationGroup
    {
        public static readonly IList<LogicalOrganisationGroup> All = new List<LogicalOrganisationGroup>();
        public IList<ILogicalElement> Elements { get; }
        public LogicalOrganisationGroup(string name)
        {
            Name = name;
            All.Add(this);
            Elements = new List<ILogicalElement>();

        }

        public string Name { get; }
        
        public static void Parse(string toParse)
        {
            var tokens = toParse.Tokenized(true);
            
            LogicalOrganisationGroup logicalOrganisationGroup = null;
            LogicalGroup logicalGroup = null; 
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
                        logicalOrganisationGroup = new LogicalOrganisationGroup(name);
                        break;
                    case '/':
                        prefix = token.Substring(1);
                        break;
                    case '=':
                        logicalGroup = new LogicalGroup($"{prefix}_{name}");
                        lgPrefix = null;
                        logicalOrganisationGroup?.Elements.Add(logicalGroup);
                        break;
                    case '-':
                        lgPrefix = name;
                        break;
                    case '*' :
                        logicalGroup = null;
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
                        if (logicalGroup is not null)
                        {
                            trait.LogicalOwnerImplementation = logicalGroup;
                            logicalGroup.Elements.Add(trait);
                        }
                        else
                        {
                            logicalOrganisationGroup?.Elements.Add(trait);
                        }
                        break;
                }
            }
        }

        public static string Switch =>
$@"acs_switch_filter = {{
    $CANDIDATE$ = {{
        switch = {{
            trigger = $FILTER$
            {All.SelectMany(o => o.Elements.Select(e => e.Switch)).Join(3)}
        }}
    }}
}}
{All.SelectMany(o => o.Elements.Select(e => e.SmallSwitchForLargeGroup)).Join()}";


    }
}