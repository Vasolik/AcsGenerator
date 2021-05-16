using System.IO;
using System.Linq;
using System.Xml;
using Vipl.AcsGenerator.LogicalElements;
using Vipl.AcsGenerator.SaveLoad;
using Vipl.AcsGenerator.VisualElements;

namespace Vipl.AcsGenerator
{
    class Program
    {
        private const string ModFolder = "mod/";

        static void Main(string[] args)
        {
            if (Directory.Exists(ModFolder))
            {
                Directory.Delete(ModFolder, true);
            }
            Directory.CreateDirectory($"{ModFolder}common/scripted_triggers");
            Directory.CreateDirectory($"{ModFolder}common/scripted_guis");
            Directory.CreateDirectory($"{ModFolder}common/scripted_effects");
            Directory.CreateDirectory($"{ModFolder}common/script_values");
            Directory.CreateDirectory($"{ModFolder}gui");
            Directory.CreateDirectory($"{ModFolder}localization/english");
            Directory.CreateDirectory("report");

            var listOfTraits = File.OpenText("trait.txt").ReadToEnd().Tokenized();
            
            LogicalOrganisationGroup.PrepareLogicalElements();
            CustomCheckBoxVisualGroup.Parse( File.ReadAllText("visual_groups.txt"));
            VisualOrganisationGroup.Parse(File.ReadAllText("layout.txt"));
            DropDownFilter.Parse(File.ReadAllText("dropdown.txt"));
            
            
            File.WriteAllText($"{ModFolder}/common/scripted_triggers/acs_big_switch.txt",LogicalOrganisationGroup.SwitchTrigger, new System.Text.UTF8Encoding(true));
            File.WriteAllText($"{ModFolder}/common/scripted_guis/acs_filter_large_groups.txt",LogicalOrganisationGroup.ScriptedGui, new System.Text.UTF8Encoding(true));
            File.WriteAllText($"{ModFolder}localization/english/acs_filter_trait_l_english.yml",VisualOrganisationGroup.CompleteLocalization, new System.Text.UTF8Encoding(true));
            VisualOrganisationGroup.GenerateGuiElement(ModFolder);
            VisualOrganisationGroup.GenerateGuiCall();

            SaveSlotGenerator.GenerateSaveSlot();
            File.WriteAllText($"{ModFolder}common/script_values/acs_filter_save.txt",SaveSlotGenerator.SelectedSlot, new System.Text.UTF8Encoding(true));
            File.WriteAllText($"{ModFolder}common/scripted_effects/acs_load.txt",SaveSlotGenerator.LoadFromSlots, new System.Text.UTF8Encoding(true));
            File.WriteAllText($"{ModFolder}common/scripted_effects/acs_save.txt",SaveSlotGenerator.SaveToSlots, new System.Text.UTF8Encoding(true));
            File.WriteAllText($"{ModFolder}common/scripted_effects/acs_load_undo.txt",SaveSlotGenerator.LoadFromUndo, new System.Text.UTF8Encoding(true));
            File.WriteAllText($"{ModFolder}common/scripted_effects/acs_save_undo.txt",SaveSlotGenerator.SaveToUndo, new System.Text.UTF8Encoding(true));
            File.WriteAllText($"{ModFolder}common/scripted_effects/acs_reset_filters.txt",SaveSlotGenerator.Reset, new System.Text.UTF8Encoding(true));
            File.WriteAllText($"{ModFolder}common/scripted_effects/acs_make_reduced_and_count.txt",SaveSlotGenerator.MakeReducedListAndCount, new System.Text.UTF8Encoding(true));
            var reportFile = new StreamWriter("report/report.txt");
            reportFile.WriteLine("Not Used traits at all");
            reportFile.WriteLine( listOfTraits.Except(Trait.All.Keys).Join());
            reportFile.WriteLine("Not Used on screen");
            reportFile.WriteLine(Trait.All.Keys.Except(VisualOrganisationGroup.All.SelectMany(g => g.Traits.Select(t => t.Name))).Join());

            reportFile.Close();
        }
        
    }

}