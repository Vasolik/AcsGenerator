using Vipl.AcsGenerator.LogicalElements;
using Vipl.AcsGenerator.SaveLoad;
using Vipl.AcsGenerator.VisualElements;

const string modFolder = "mod/";

if (Directory.Exists(modFolder))
{
    Directory.Delete(modFolder, true);
}
Directory.CreateDirectory($"{modFolder}common/scripted_triggers");
Directory.CreateDirectory($"{modFolder}common/scripted_guis");
Directory.CreateDirectory($"{modFolder}common/scripted_effects");
Directory.CreateDirectory($"{modFolder}common/script_values");
Directory.CreateDirectory($"{modFolder}gui/gen");
Directory.CreateDirectory($"{modFolder}localization/english");
Directory.CreateDirectory("report");

LogicalOrganisationGroup.PrepareLogicalElements();
CustomCheckBoxVisualGroup.Parse();
VisualOrganizationGroup.Parse();



File.WriteAllText($"{modFolder}/common/scripted_triggers/gen_acs_st_big_switch.txt",LogicalOrganisationGroup.SwitchTrigger, new System.Text.UTF8Encoding(true));

File.WriteAllText($"{modFolder}/common/scripted_guis/gen_acs_sg_filter_large_groups.txt",LogicalOrganisationGroup.ScriptedGui, new System.Text.UTF8Encoding(true));
File.WriteAllText($"{modFolder}localization/english/acs_filter_trait_l_english.yml",VisualOrganizationGroup.GetCompleteLocalization(LocalizationFiles.TraitFile), new System.Text.UTF8Encoding(true));
File.WriteAllText($"{modFolder}localization/english/acs_filter_dropdown_l_english.yml",VisualOrganizationGroup.GetCompleteLocalization(LocalizationFiles.DropDownFile), new System.Text.UTF8Encoding(true));
VisualOrganizationGroup.GenerateGuiElement(modFolder);
VisualOrganizationGroup.GenerateGuiCall();

SaveSlotGenerator.GenerateSaveSlot();
File.WriteAllText($"{modFolder}common/scripted_effects/gen_acs_se_save_slot_manipulation.txt",SaveSlotGenerator.SlotManipulation, new System.Text.UTF8Encoding(true));
File.WriteAllText($"{modFolder}common/scripted_effects/gen_acs_se_make_reduced_and_count.txt",SaveSlotGenerator.MakeReducedListAndCount, new System.Text.UTF8Encoding(true));
File.WriteAllText($"{modFolder}/common/scripted_triggers/gen_acs_st_slot_manipulation.txt",SaveSlotGenerator.SlotEqualTrigger, new System.Text.UTF8Encoding(true));
var reportFile = new StreamWriter("report/report.txt");

reportFile.Close();