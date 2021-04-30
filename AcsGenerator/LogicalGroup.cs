using System.Collections.Generic;
using System.Linq;
using Vipl.AcsGenerator.VisualElements;

namespace Vipl.AcsGenerator
{
    public interface ILogicalElement
    {
        public string FlagGenerator { get; }

        public string Switch { get; }
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

        public string FlagGenerator =>
@$"if = {{
    limit = {{
        OR = {{
            {Traits.Select(t => (t as ISimpleVisualElement).VariableYesCondition).Join(3)}
        }}
    }}
    add_to_global_variable_list = {{ name = acs_active_filter_list target = flag:{Name} }}
    change_global_variable = {{ name = acs_active_filters_count add = 1 }}  
}}
else_if = {{
    limit = {{
        OR = {{
            {Traits.Select(t => (t as ISimpleVisualElement).VariableNoCondition).Join(3)}
        }}
    }}
    add_to_global_variable_list = {{ name = acs_active_filter_list target = flag:{Name}_negative }}
    change_global_variable = {{ name = acs_active_filters_count add = 1 }}  
}}";

        public string Switch => 
$@"flag:{Name} = {{
    OR = {{
        {Traits.Select(t => t.PositiveTrigger).Join(2)}
    }}
}}
flag:{Name}_negative = {{
    NOR = {{
        {Traits.Select(t => t.InvertedNegativeTrigger).Join(2)}
    }}
}}";
    }
}