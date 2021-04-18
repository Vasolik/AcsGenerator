using System.Collections.Generic;
using System.Linq;
using Vipl.AcsGenerator.VisualElements;

namespace Vipl.AcsGenerator
{
    public interface ILogicalElement
    {
        public string FlagGenerator { get; }

        public string Switch { get; }
        
        public string Trigger { get; }
    } 
    public class LogicalGroup : ILogicalElement
    {
        public string Name { get; }
        public List<Trait> Traits { get;  }

        public LogicalGroup(string name)
        {
            Name = name;
            Traits = new List<Trait>();
        }

        public string Trigger => 
            @$"{Name} = {{
	AND = {{
		OR = {{
			AND = {{
                {Traits.Select(t => t.NotSelectedTrigger).Join(4)}
			}}
            {Traits.Select(t => t.PositiveTrigger).Join(3)}
		}}
		NOR = {{
            {Traits.Select(t => t.InvertedNegativeTrigger).Join(3)}
		}}
	}}
}}";

        public string FlagGenerator =>
            @$"if = {{
    limit = {{
        OR = {{
            {Traits.Select(t => (t as ISimpleVisualElement).HasVariableCondition).Join(3)}
        }}
    }}
    add_to_global_variable_list = {{ name = acs_active_filter_list target = flag:{Name} }}
    change_global_variable = {{ name = acs_active_filters_count add = 1 }}  
}}";

        public string Switch => $"flag:{Name} = {{ {Name} = yes }}";
    }
}