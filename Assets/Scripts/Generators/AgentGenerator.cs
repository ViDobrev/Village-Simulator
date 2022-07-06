using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;


public static class AgentGenerator
{
    #region Methods
    public static Agent GenerateAgent(List<Item> items)
    {
        System.Random rng = new System.Random(Data.agentRng.Next());

        string name;
        Gender gender;
        Species species;

        Equipment equipment = null;
        SkillsManager skills = null;

        int genders = System.Enum.GetNames(typeof(Gender)).Length;
        gender = (Gender)rng.Next(0, genders);

        bool intelligent = rng.Next() % 2 == 0;
        Species[] speciesList = (from Species _species in Data.Species.Values
                                 where _species.Intelligent == intelligent
                                 select _species).ToArray();
        species = speciesList[rng.Next(0, speciesList.Length)];


        if (intelligent)
        {
            name = GenerateName(gender, rng);
            skills = new SkillsManager(rng);
            if (items != null) equipment = new Equipment(Data.VillagerInventorySlots, items);
            else equipment = new Equipment(Data.VillagerInventorySlots);
        }
        else
        {
            name = species.Name;
        }

        return new Agent(name, gender, species, equipment, skills);
    }
    public static Agent GenerateAgent(Species species, List<Item> items)
    {
        System.Random rng = new System.Random(Data.agentRng.Next());

        string name;
        Gender gender;

        Equipment equipment = null;
        SkillsManager skills = null;

        int genders = System.Enum.GetNames(typeof(Gender)).Length;
        gender = (Gender)rng.Next(0, genders);

        bool intelligent = species.Intelligent;

        if (intelligent)
        {
            name = GenerateName(gender, rng);
            skills = new SkillsManager(rng);
            if (items != null) equipment = new Equipment(Data.VillagerInventorySlots, items);
            else equipment = new Equipment(Data.VillagerInventorySlots);
        }
        else
        {
            name = species.Name;
        }

        return new Agent(name, gender, species, equipment, skills);
    }



    private static string GenerateName(Gender gender, System.Random rng)
    {
        string name;

        if (gender == Gender.Male)
        {
            string path = Application.dataPath + @"\Database\Names_Male.txt";
            List<string> names = new List<string>(File.ReadAllLines(path));
            name = names[rng.Next(0, names.Count)];
        }
        else
        {
            string path = Application.dataPath + @"\Database\Names_Male.txt";
            List<string> names = new List<string>(File.ReadAllLines(path));
            name = names[rng.Next(0, names.Count)];
        }

        return name;
    }
    #endregion Methods
}