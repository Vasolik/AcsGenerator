using System.Linq;
using Vipl.AcsGenerator.LogicalElements;
using Vipl.AcsGenerator.SaveLoad;

namespace Vipl.AcsGenerator.VisualElements
{
    public abstract class CheckBoxVisualGroup : ICheckBoxVisualElement
    {
        public virtual string Variable { get; protected init; }
        protected abstract string  GetGroupGuiElement(string style);
        
        protected abstract SimpleCheckBoxVisualElement[] Elements { get; }
        // ReSharper disable once CoVariantArrayConversion
        public  string GetGuiElement(string style) =>
            $@"flowcontainer = {{ 
    direction = vertical
    spacing = 5
    {GetGroupGuiElement(style).Intend(1)}
    { Elements.Select(t => t.GetGuiElement($"{style}_moved")).Join(1)}
}}";

        public virtual string ScriptedGui =>
            Elements.Select(e => e.ScriptedGui).Join() + "\n" +ScriptedGuiForThisElement;

        protected string ScriptedGuiForThisElement => IsSmallGroup ? ScriptedGuiForThisElementForSmallGroup : ScriptedGuiForThisElementForLargeGroup;
        private string ScriptedGuiForThisElementForLargeGroup => IsSameSizeAsLogical ? ScriptedGuiForThisElementForLargeGroupSameAsLogical : ScriptedGuiForThisElementForLargeGroupDifferentAsLogical;
        
        private bool IsSmallGroup => LogicalOwner.IsSmall;
        private bool IsSameSizeAsLogical => LogicalOwner.Elements.Count == Elements.Length;
        private CheckboxLogicalGroup LogicalOwner => Elements[0].CheckBoxLogicalOwner;
        private string ScriptedGuiForThisElementForSmallGroup =>
            $@"{Variable}_negative = {{
    is_shown = {{
        any_in_global_list = {{
            variable = {MainSavable.Instance.ListVariable()}
            {LogicalOwner.FullNegativeFlag} = this
        }} 
    }}
}}
{Variable}_none = {{
    is_shown = {{
        NOT = {{
            any_in_global_list = {{
                variable = {MainSavable.Instance.ListVariable()}
                OR = {{
                    {LogicalOwner.AllFlags.Select(flag => $"{flag} = this").Join(5)}
                }}
            }}           
        }}
    }}
}}
{Variable} = {{
 
    is_shown = {{
        any_in_global_list = {{
            variable = {MainSavable.Instance.ListVariable()}
            {LogicalOwner.FullPositiveFlag} = this
        }}  
    }} 
        
    effect = {{
        acs_save_undo_0_filters = yes
        if = {{
            limit = {{ 
                NOT = {{
                    any_in_global_list = {{
                        variable = {MainSavable.Instance.ListVariable()}
                        OR = {{
                            {LogicalOwner.AllFlags.Select(flag => $"{flag} = this").Join(7)}
                        }}
                    }}           
                }}
            }}
            add_to_global_variable_list = {{ name = {MainSavable.Instance.ListVariable()} target = {LogicalOwner.FullPositiveFlag} }}
        }}
        else = {{
            if = {{
                limit = {{
                    any_in_global_list = {{
                        variable = {MainSavable.Instance.ListVariable()}
                        {LogicalOwner.FullPositiveFlag} = this
                    }}
                }}
                remove_list_global_variable = {{ name = {MainSavable.Instance.ListVariable()} target = {LogicalOwner.FullPositiveFlag} }}
                add_to_global_variable_list = {{ name = {MainSavable.Instance.ListVariable()} target = {LogicalOwner.FullNegativeFlag} }}
            }}
            else = {{
                {LogicalOwner.AllFlags.Select(flag => $"remove_list_global_variable = {{ name = {MainSavable.Instance.ListVariable()} target = {flag} }}").Join(4)}
            }}
        }}
        acs_auto_apply_sorting_and_filters = yes
    }}
}}";
        public string ScriptedGuiForThisElementForLargeGroupSameAsLogical =>
            $@"{Variable}_negative = {{
    is_shown = {{
        any_in_global_list = {{
            variable = {LogicalOwner.ListVariable()}
            OR = {{
                {Elements.Select(e => $"{e.NegativeFlag} = this").Join(3)}
            }}
            count = {Elements.Length}
        }} 
    }}
}}
{Variable}_none = {{
    is_shown = {{
        NOT = {{
            any_in_global_list = {{
                variable = {LogicalOwner.ListVariable()}
                always = yes
            }}           
        }}
    }}
}}
{Variable} = {{
 
    is_shown = {{
        any_in_global_list = {{
            variable = {LogicalOwner.ListVariable()}
            OR = {{
                {Elements.Select(e => $"{e.PositiveFlag} = this").Join(4)}
            }}
            count = {Elements.Length}
        }}  
    }} 
        
    effect = {{
        acs_save_undo_0_filters = yes
        if = {{
            limit = {{ 
                NOT = {{
                    any_in_global_list = {{
                        variable = {LogicalOwner.ListVariable()}
                        always = yes
                    }}           
                }}
            }}
            {Elements.Select(e => $"add_to_global_variable_list = {{ name = {LogicalOwner.ListVariable()} target = {e.PositiveFlag} }}").Join(3)}
            add_to_global_variable_list = {{ name = {MainSavable.Instance.ListVariable()} target = {LogicalOwner.Flag} }}
        }}
        else = {{
            if = {{
                limit = {{
                    any_in_global_list = {{
                        variable = {LogicalOwner.ListVariable()}
                        OR = {{
                            {Elements.Select(e => $"{e.PositiveFlag} = this").Join(7)}
                        }}
                        count = {Elements.Length}
                    }}
                }}
                {Elements.Select(e => $"remove_list_global_variable = {{ name = {LogicalOwner.ListVariable()} target = {e.PositiveFlag} }}").Join(4)}
                {Elements.Select(e => $"add_to_global_variable_list = {{ name = {LogicalOwner.ListVariable()} target = {e.NegativeFlag} }}").Join(4)}
            }}
            else = {{
                {Elements.Select(e => $"remove_list_global_variable = {{ name = {LogicalOwner.ListVariable()} target = {e.PositiveFlag} }}").Join(4)}
                {Elements.Select(e => $"remove_list_global_variable = {{ name = {LogicalOwner.ListVariable()} target = {e.NegativeFlag} }}").Join(4)}
                remove_list_global_variable = {{ name = {MainSavable.Instance.ListVariable()} target = {LogicalOwner.Flag} }}
            }}
        }}
        acs_auto_apply_sorting_and_filters = yes
    }}
}}";
        
                public string ScriptedGuiForThisElementForLargeGroupDifferentAsLogical =>
            $@"{Variable}_negative = {{
    is_shown = {{
        any_in_global_list = {{
            variable = {LogicalOwner.ListVariable()}
            OR = {{
                {Elements.Select(e => $"{e.NegativeFlag} = this").Join(3)}
            }}
            count = {Elements.Length}
        }} 
    }}
}}
{Variable}_none = {{
    is_shown = {{
        NOT = {{
            any_in_global_list = {{
                variable = {LogicalOwner.ListVariable()}
                OR = {{
                    {Elements.Select(e => $"{e.NegativeFlag} = this").Join(4)}
                    {Elements.Select(e => $"{e.PositiveFlag} = this").Join(4)}
                }}
            }}
        }}
    }}
}}
{Variable} = {{
 
    is_shown = {{
        any_in_global_list = {{
            variable = {LogicalOwner.ListVariable()}
            OR = {{
                {Elements.Select(e => $"{e.PositiveFlag} = this").Join(4)}
            }}
            count = {Elements.Length}
        }}  
    }} 
        
    effect = {{
        acs_save_undo_0_filters = yes
        if = {{
            limit = {{ 
                NOT = {{
                    any_in_global_list = {{
                        variable = {LogicalOwner.ListVariable()}
                        OR = {{
                            {Elements.Select(e => $"{e.NegativeFlag} = this").Join(6)}
                            {Elements.Select(e => $"{e.PositiveFlag} = this").Join(6)}
                        }}
                    }}
                }}
            }}
            {Elements.Select(e => $"add_to_global_variable_list = {{ name = {LogicalOwner.ListVariable()} target = {e.PositiveFlag} }}").Join(3)}
            add_to_global_variable_list = {{ name = {MainSavable.Instance.ListVariable()} target = {LogicalOwner.Flag} }}
        }}
        else = {{
            if = {{
                limit = {{
                    any_in_global_list = {{
                        variable = {LogicalOwner.ListVariable()}
                        OR = {{
                            {Elements.Select(e => $"{e.PositiveFlag} = this").Join(7)}
                        }}
                        count = {Elements.Length}
                    }}
                }}
                {Elements.Select(e => $"remove_list_global_variable = {{ name = {LogicalOwner.ListVariable()} target = {e.PositiveFlag} }}").Join(4)}
                {Elements.Select(e => $"add_to_global_variable_list = {{ name = {LogicalOwner.ListVariable()} target = {e.NegativeFlag} }}").Join(4)}
            }}
            else = {{
                {Elements.Select(e => $"remove_list_global_variable = {{ name = {LogicalOwner.ListVariable()} target = {e.PositiveFlag} }}").Join(4)}
                {Elements.Select(e => $"remove_list_global_variable = {{ name = {LogicalOwner.ListVariable()} target = {e.NegativeFlag} }}").Join(4)}
                if = {{
                    limit = {{
                        NOT = {{
                            any_in_global_list = {{
                                variable = {LogicalOwner.ListVariable()}
                                always = yes
                            }}
                        }}
                    }}
                    remove_list_global_variable = {{ name = {MainSavable.Instance.ListVariable()} target = {LogicalOwner.Flag} }}
                }}
            }}
        }}
        acs_auto_apply_sorting_and_filters = yes
    }}
}}";
        public abstract Trait[] Traits { get; }

        public string CheckBoxFrameSelector
            => $@"frame = ""[Select_int32( GetScriptedGui( '{Variable}' ).IsShown( GuiScope.SetRoot( GetPlayer.MakeScope ).End ) , '(int32)2', Select_int32( GetScriptedGui( '{Variable}_negative' ).IsShown( GuiScope.SetRoot( GetPlayer.MakeScope ).End ) , '(int32)3', Select_int32( GetScriptedGui( '{Variable}_none' ).IsShown( GuiScope.SetRoot( GetPlayer.MakeScope ).End ) , '(int32)1', '(int32)4' ) ) )]""";

        public abstract string[] Localizations { get; }
    }
}