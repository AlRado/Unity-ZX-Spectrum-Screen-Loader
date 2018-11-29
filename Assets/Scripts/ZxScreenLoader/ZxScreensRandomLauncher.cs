using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

  /**
  * The MIT License (MIT)
  * 
  *  Copyright © 2018 Alexey Radyuk
  * 
  * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and 
  * associated documentation files (the «Software»), to deal in the Software without restriction, including 
  * without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies 
  * of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
  * 
  * The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
  * 
  * THE SOFTWARE IS PROVIDED «AS IS», WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
  * INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
  * IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
  * WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR 
  * THE USE OR OTHER DEALINGS IN THE SOFTWARE.
  */

// The class randomly load zx-screens from list: web address or local path
public class ZxScreensRandomLauncher : MonoBehaviour {

  [Header ("Set the volume of sounds before launching")]
  public float Volume = 0.5f;

  [Header ("Set the palette before launching")]
  public ZxPalettes.Type Palette = ZxPalettes.Type.Orthodox;

  private List<KeyValuePair<string, string>> screens = new List<KeyValuePair<string, string>> () {

    new KeyValuePair<string, string> ("Carlitos", "Al-Rado_Carlitos_on_the_road_(2016).scr"),
    new KeyValuePair<string, string> ("Life", "Al-Rado_Life_(2015).scr"),
    new KeyValuePair<string, string> ("Secret", "Al-Rado_Old_School_Sends_Secret_Knowledge_(2015).scr"),
    new KeyValuePair<string, string> ("P-girl", "Al-Rado_Power_girl_(2016).scr"),
    new KeyValuePair<string, string> ("Screen", "Al-Rado_Screen_(2015).scr"),
    new KeyValuePair<string, string> ("Truck", "Al-Rado_Truck_(2015).scr"),
    new KeyValuePair<string, string> ("Warrior", "Al-Rado_WarriorWithMask_(2015).scr"),
    new KeyValuePair<string, string> ("Zx-girl", "Al-Rado_Zx-girl_(2010).scr"),
    new KeyValuePair<string, string> ("Winter", "Al-Rado_Winter_(2015).scr"),
    new KeyValuePair<string, string> ("Duck", "Al-Rado_Duck_(2016).scr"),

    new KeyValuePair<string, string> ("AgentX", "https://zxart.ee/file/id:53787/filename:Mick_Farrow_-_Agent_X_%282014%29.scr"),
    new KeyValuePair<string, string> ("Battle_C", "https://zxart.ee/file/id:193091/filename:Joe_Vondayl_-_Battle_City_%282017%29_%28Chaos_Constructions_2017%2C_3%29.scr"),
    new KeyValuePair<string, string> ("CSchool2", "https://zxart.ee/file/id:5325/filename:Ivan_Horn_-_Combat_School_%281987%29.scr"),
    new KeyValuePair<string, string> ("CraBattl", "https://zxart.ee/file/id:193003/filename:Oleg_Origin_-_Matthew_Cranston_Battles_-_ingame_%282014%29.scr"),
    new KeyValuePair<string, string> ("BlaOrSun", "https://zxart.ee/file/id:89731/filename:Al-Rado_-_Black_or_Sun%2C_what_is_come_%282016%29.scr"),
    new KeyValuePair<string, string> ("Goodness", "https://zxart.ee/file/id:89680/filename:Al-Rado_-_Добро_должно_быть_с_кулаками%21_%29%29_%282016%29.scr"),
    new KeyValuePair<string, string> ("Carlitos", "https://zxart.ee/file/id:87928/filename:Al-Rado_-_Carlitos_on_the_road_%282016%29.scr"),
    new KeyValuePair<string, string> ("Gaika", "https://zxart.ee/file/id:86502/filename:Al-Rado_-_Gaika_%282016%29.scr"),
    new KeyValuePair<string, string> ("ATWremak", "https://zxart.ee/file/id:81307/filename:Al-Rado_-_ATWremake_%282015%29.scr"),
    new KeyValuePair<string, string> ("GameOver", "https://zxart.ee/file/id:5125/filename:Snatcho_-_Game_Over_2_%281987%29.scr"),

    new KeyValuePair<string, string> ("CK_Dizzy", "https://zxart.ee/file/id:192310/filename:MAC_-_Crystal_Kingdom_Dizzy_%282017%29.scr"),
    new KeyValuePair<string, string> ("Castleva", "https://zxart.ee/file/id:58303/filename:diver_-_Castlevania_%282014%29.scr"),
    new KeyValuePair<string, string> ("Dan_Dare", "https://zxart.ee/file/id:7496/filename:Martin_Wheeler_-_Dan_Dare_Pilot_of_the_Future_%281986%29.scr"),
    new KeyValuePair<string, string> ("N_Rally", "https://zxart.ee/file/id:6193/filename:F._David_Thorpe_-_Nightmare_Rally_%281986%29.scr"),
    new KeyValuePair<string, string> ("Hundra", "https://zxart.ee/file/id:5081/filename:Raúl_López_-_Hundra_%281988%29.scr"),
    new KeyValuePair<string, string> ("Batman", "https://zxart.ee/file/id:6541/filename:Charles_Davies_-_Batman_the_Caped_Crusader_%281988%29.scr"),
    new KeyValuePair<string, string> ("FW_Dizzy", "https://zxart.ee/file/id:5498/filename:Neil_Adamson_-_Fantasy_World_Dizzy_%281989%29.scr"),
    new KeyValuePair<string, string> ("DJ_Puff", "https://zxart.ee/file/id:20013/filename:Michael_Sanderson_-_DJ_Puff_%281992%29.scr"),
    new KeyValuePair<string, string> ("Bronx", "https://zxart.ee/file/id:5231/filename:Kantxo_Design_-_Bronx_%281990%29.scr"),

    new KeyValuePair<string, string> ("NavMoves", "https://zxart.ee/file/id:5085/filename:Déborah%2C_Jorge_Azpiri_-_Navy_Moves_%281988%29.scr"),
    new KeyValuePair<string, string> ("C_School", "https://zxart.ee/file/id:5325/filename:Ivan_Horn_-_Combat_School_%281987%29.scr"),
    new KeyValuePair<string, string> ("Lorna", "https://zxart.ee/file/id:5550/filename:-AF-%2C_Jorge_Azpiri_-_Lorna_%281990%29.scr"),
    new KeyValuePair<string, string> ("Kliatba", "https://zxart.ee/file/id:6338/filename:Dušan_Balara_-_Kliatba_Noci_%281993%29.scr"),
    new KeyValuePair<string, string> ("RamboIII", "https://zxart.ee/file/id:13564/filename:Ivan_Horn_-_Rambo_III_%281988%29.scr"),
    new KeyValuePair<string, string> ("MegaCorp", "https://zxart.ee/file/id:5060/filename:Javier_Cubedo_-_Mega-Corp_%281987%29.scr"),
    new KeyValuePair<string, string> ("GameOver", "https://zxart.ee/file/id:5027/filename:Snatcho_-_Game_Over_%281987%29.scr"),
    new KeyValuePair<string, string> ("AForce_II", "https://zxart.ee/file/id:7508/filename:Martin_Wheeler_-_Action_Force_II_%281988%29.scr"),
    new KeyValuePair<string, string> ("Renegade", "https://zxart.ee/file/id:13153/filename:Ronny_Fowles_-_Renegade_%281987%29.scr"),
    new KeyValuePair<string, string> ("Gryzor", "https://zxart.ee/file/id:3163/filename:Mark_R._Jones_-_Gryzor_%281987%29.scr"),

    new KeyValuePair<string, string> ("OGunship", "https://zxart.ee/file/id:5510/filename:Neil_Adamson_-_Operation_Gunship_%281989%29.scr"),
    new KeyValuePair<string, string> ("ScoobyDo", "https://zxart.ee/file/id:51406/filename:Richard_Morton_-_Scooby_and_Scrappy_Doo_%281991%29.scr"),
    new KeyValuePair<string, string> ("Seymour", "https://zxart.ee/file/id:5360/filename:Chris_Graham_-_Seymour_Goes_to_Hollywood_%281991%29.scr"),
    new KeyValuePair<string, string> ("DanDarII", "https://zxart.ee/file/id:5297/filename:Martin_Wheeler_-_Dan_Dare_II_Mekon%27s_Revenge_%281988%29.scr"),
    new KeyValuePair<string, string> ("NavySEAL", "https://zxart.ee/file/id:13573/filename:Martin_McDonald_-_Navy_SEALs_%281991%29.scr"),
    new KeyValuePair<string, string> ("AfterTW", "https://zxart.ee/file/id:2546/filename:Snatcho%2C_Déborah_-_After_the_war_%281989%29.scr"),
    new KeyValuePair<string, string> ("TurboG", "https://zxart.ee/file/id:5092/filename:Javier_Cubedo%2C_Snatcho_-_Turbo_Girl_%281988%29.scr"),
    new KeyValuePair<string, string> ("WestBank", "https://zxart.ee/file/id:61662/filename:Snatcho_-_West_Bank_%281985%29.scr"),
    new KeyValuePair<string, string> ("OperatW", "https://zxart.ee/file/id:13551/filename:Ivan_Horn_-_Operation_Wolf_%281988%29.scr"),
    new KeyValuePair<string, string> ("RockRoll", "https://zxart.ee/file/id:5190/filename:ACE_-_Rock%27n_Roller_%281988%29.scr"),

    new KeyValuePair<string, string> ("Cabal", "https://zxart.ee/file/id:6543/filename:Charles_Davies_-_Cabal_%281989%29.scr"),
    new KeyValuePair<string, string> ("CiscoH", "https://zxart.ee/file/id:5991/filename:Alan_Grier_-_Cisco_Heat_%281991%29.scr"),
    new KeyValuePair<string, string> ("MCCasino", "https://zxart.ee/file/id:5352/filename:Chris_Graham_-_Monte_Carlo_Casino_%281989%29.scr"),
    new KeyValuePair<string, string> ("Nonamed", "https://zxart.ee/file/id:5064/filename:Javier_Cubedo_-_Nonamed_%281987%29.scr"),
    new KeyValuePair<string, string> ("SuperB", "https://zxart.ee/file/id:5516/filename:Neil_Adamson_-_Super_Bike_TransAm_%281989%29.scr"),
    new KeyValuePair<string, string> ("TMNT", "https://zxart.ee/file/id:49700/filename:Doug_Townsley_-_Teenage_Mutant_Hero_Turtles_-_The_Coin-Op_%281991%29.scr"),
    new KeyValuePair<string, string> ("Cabal_D", "https://zxart.ee/file/id:52398/filename:Charles_Davies_-_Cabal_demo_version_%281988%29.scr"),
    new KeyValuePair<string, string> ("R_Moscow", "https://zxart.ee/file/id:6195/filename:F._David_Thorpe_-_Raid_over_Moscow_%281985%29.scr"),
    new KeyValuePair<string, string> ("Tantalus", "https://zxart.ee/file/id:17716/filename:Paul_Hargreaves_-_Tantalus_%281986%29.scr"),
    new KeyValuePair<string, string> ("Savage", "https://zxart.ee/file/id:5254/filename:Nick_Bruty_-_Savage_%281988%29.scr"),

    new KeyValuePair<string, string> ("Renegade", "https://zxart.ee/file/id:13539/filename:Dawn_Drake_-_Target_Renegade_%281988%29.scr"),
    new KeyValuePair<string, string> ("Zigurat", "https://zxart.ee/file/id:5555/filename:Igor_-_Jungle_Warrior_%28Zigurat%29_%281990%29.scr"),
    new KeyValuePair<string, string> ("AngelPol", "https://zxart.ee/file/id:13854/filename:Jason_G._Lihou_-_Angel_Nieto_Pole_500_%281990%29.scr"),
    new KeyValuePair<string, string> ("Cobra_F", "https://zxart.ee/file/id:4751/filename:Martin_Severn_-_Cobra_Force_%281989%29.scr"),
    new KeyValuePair<string, string> ("FutureB", "https://zxart.ee/file/id:6437/filename:Nigel_Speight_-_Future_Bike_Simulator_%281990%29.scr"),
    new KeyValuePair<string, string> ("MissionJ", "https://zxart.ee/file/id:5786/filename:JIM_-_Mission_Jupiter_%281987%29.scr"),
    new KeyValuePair<string, string> ("Munsters", "https://zxart.ee/file/id:6068/filename:Tedd_-_Munsters%2C_The_%281989%29.scr"),
    new KeyValuePair<string, string> ("WhereT", "https://zxart.ee/file/id:90014/filename:Ally_Noble_-_Where_Time_Stood_Still_%281988%29.scr"),
    new KeyValuePair<string, string> ("Bruce_L", "https://zxart.ee/file/id:6122/filename:F._David_Thorpe_-_Bruce_Lee_%281984%29.scr"),
    new KeyValuePair<string, string> ("Conan", "https://zxart.ee/file/id:13977/filename:F._David_Thorpe_-_Conan.scr"),

    new KeyValuePair<string, string> ("Escape_f", "https://zxart.ee/file/id:5506/filename:Neil_Adamson_-_Escape_from_the_Planet_of_the_Robot_Monsters_%281990%29.scr"),
    new KeyValuePair<string, string> ("Marauder", "https://zxart.ee/file/id:5273/filename:Rory_Green_-_Marauder_%281988%29.scr"),
    new KeyValuePair<string, string> ("ArmyMove", "https://zxart.ee/file/id:5044/filename:Javier_Cubedo_-_Army_Moves_%281987%29.scr"),
    new KeyValuePair<string, string> ("WLeMans", "https://zxart.ee/file/id:5925/filename:Bill_Harbison_-_WEC_Le_Mans_%281988%29.scr"),
    new KeyValuePair<string, string> ("Commando", "https://zxart.ee/file/id:6002/filename:KAZ_-_Commando_%281985%29.scr"),
    new KeyValuePair<string, string> ("Cobra", "https://zxart.ee/file/id:4159/filename:Steve_Cain_-_Cobra_%281986%29.scr"),
    new KeyValuePair<string, string> ("CrystDiz", "https://zxart.ee/file/id:81457/filename:Jarrod_Bentley_-_Crystal_Kingdom_Dizzy_%281992%29.scr"),
    new KeyValuePair<string, string> ("DaDarIII", "https://zxart.ee/file/id:5299/filename:Simon_Butler_-_Dan_Dare_III_The_Escape_%281990%29.scr"),
    new KeyValuePair<string, string> ("Supercar", "https://zxart.ee/file/id:5342/filename:Chris_Graham_-_Italian_Supercar_%281990%29.scr"),
    new KeyValuePair<string, string> ("Phantis", "https://zxart.ee/file/id:47111/filename:MAC_-_Phantis_%282014%29_%283BM_OpenAir_2014%2C_1%29.scr"),
  };

  private ZxScreenLoader zxScreenLoader;

  private int ix = 0;

  private void Start () {
    zxScreenLoader = gameObject.GetComponent<ZxScreenLoader> ();
    // shuffle screens
    screens = screens.OrderBy (elem => Guid.NewGuid ()).ToList ();
    LoadNext (0);
  }

  private void LoadNext (float delay) {
    StartCoroutine (LoadNextCoroutine (delay));
  }

  private IEnumerator LoadNextCoroutine (float delay) {
    yield return new WaitForSeconds (delay);

    ix++;
    if (ix > screens.Count - 1) ix = 0;

    zxScreenLoader.Reset ();

    var scr = screens[ix];
    zxScreenLoader.Load (scr.Key, scr.Value, Volume, Palette, () => LoadNext (5));
  }

}