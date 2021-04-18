using System.IO;
using System.Linq;
using Vipl.AcsGenerator.SaveLoad;
using Vipl.AcsGenerator.VisualElements;

namespace Vipl.AcsGenerator
{
    class Program
    {
        private const string ModFolder = "mod/";

        static void Main(string[] args)
        {

            var listOfTraits = File.OpenText("trait.txt").ReadToEnd().Tokenized();
            
            LogicalOrganisationGroup.Parse(File.ReadAllText("groups.txt"));
            CustomCheckBoxVisualGroup.Parse( File.ReadAllText("visual_groups.txt"));
            VisualOrganisationGroup.Parse(File.ReadAllText("layout.txt"));
            DropDownFilter.Parse(File.ReadAllText("dropdown.txt"));
            
            Directory.Delete(ModFolder, true);
            Directory.CreateDirectory($"{ModFolder}common/scripted_triggers");
            Directory.CreateDirectory($"{ModFolder}common/scripted_guis");
            Directory.CreateDirectory($"{ModFolder}common/scripted_effects");
            Directory.CreateDirectory($"{ModFolder}common/script_values");
            Directory.CreateDirectory($"{ModFolder}gui");
            Directory.CreateDirectory($"{ModFolder}localization/english");
            Directory.CreateDirectory("report");
            
            
            foreach (var organisationGroup in LogicalOrganisationGroup.All)
            {
                organisationGroup.GenerateTriggers(ModFolder);
            }
    
            File.WriteAllText($"{ModFolder}/common/scripted_triggers/acs_big_switch.txt",LogicalOrganisationGroup.Switch, new System.Text.UTF8Encoding(true));
            File.WriteAllText($"{ModFolder}/common/scripted_effects/acs_filter_flag_generate.txt",LogicalOrganisationGroup.FlagGenerator, new System.Text.UTF8Encoding(true));
            File.WriteAllText($"{ModFolder}localization/english/acs_filter_trait_l_english.yml",VisualOrganisationGroup.CompleteLocalization, new System.Text.UTF8Encoding(true));
            VisualOrganisationGroup.GenerateGuiElement(ModFolder);
            VisualOrganisationGroup.GenerateScriptedGui(ModFolder);
            VisualOrganisationGroup.GenerateGuiCall();

            SaveSlotGenerator.GenerateSaveSlot();
            File.WriteAllText($"{ModFolder}common/script_values/acs_filter_save.txt",SaveSlotGenerator.SelectedSlot, new System.Text.UTF8Encoding(true));
            File.WriteAllText($"{ModFolder}common/scripted_effects/acs_load.txt",SaveSlotGenerator.LoadFromSlots, new System.Text.UTF8Encoding(true));
            File.WriteAllText($"{ModFolder}common/scripted_effects/acs_save.txt",SaveSlotGenerator.SaveToSlots, new System.Text.UTF8Encoding(true));
            File.WriteAllText($"{ModFolder}common/scripted_effects/acs_load_undo.txt",SaveSlotGenerator.LoadFromUndo, new System.Text.UTF8Encoding(true));
            File.WriteAllText($"{ModFolder}common/scripted_effects/acs_save_undo.txt",SaveSlotGenerator.SaveToUndo, new System.Text.UTF8Encoding(true));
            File.WriteAllText($"{ModFolder}common/scripted_effects/acs_reset_filters.txt",SaveSlotGenerator.Reset, new System.Text.UTF8Encoding(true));
            var reportFile = new StreamWriter("report/report.txt");
            reportFile.WriteLine("Not Used traits at all");
            reportFile.WriteLine( listOfTraits.Except(Trait.All.Keys).Join());
            reportFile.WriteLine("Not Used on screen");
            reportFile.WriteLine(Trait.All.Keys.Except(VisualOrganisationGroup.All.SelectMany(g => g.Traits.Select(t => t.Name))).Join());

            reportFile.Close();
        }
    }

}