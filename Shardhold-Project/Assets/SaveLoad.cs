using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using JsonUtility;

public class SaveLoad : MonoBehaviour
{
    #region Debug Stuff
    //TODO set to false before final submission
    public bool debugging = true;

    #endregion

    public enum SaveType{
        chooseDefault,
        binary
        //TODO: json
    }

    public SaveType defaultSaveType = SaveType.binary;
    public string fileToUse = "";   //default save file
    public string saveFolder = Application.persistentDataPath;  //where save files go

    public string playerStatsFile;

    #region Text File I/O
    public void WriteFile(string filename, string contents)
    {
        using (StreamWriter writer = new StreamWriter(saveFolder + "/" + filename, ))
        {
            writer.Write(contents);
        }
    }

    public string ReadFile(string filename)
    {
        string contents = "";
        using (StreamReader reader = new StreamReader(saveFolder + "/" + filename)) {
            contents = reader.ReadToEnd();
        }
        return contents;
    }
    #endregion


    #region Save/Load, see https://www.youtube.com/watch?v=J6FfcJpbPXE
    public bool SaveToDefault(SaveType saveType = SaveType.chooseDefault){
        if(debugging){
            Debug.Log("Saving game to default file \"" + fileToUse = "\"");
        }
        return Save(fileToUse, saveType);
    }

    public bool LoadFromDefault(SaveType saveType = SaveType.chooseDefault){
        if(debugging){
            Debug.Log("Loading game from default file \"" + fileToUse = "\"");
        }
        return Load(fileToUse, saveType);
    }

    public bool Save(string filename, SaveType saveType = SaveType.chooseDefault){
        if(debugging){
            Debug.Log("Saving game to specified file \"" + fileToUse = "\"");
        }
        if(saveType = SaveType.chooseDefault){
            saveType = defaultSaveType;
        }
        switch(saveType){
            case SaveType.binary:
                return BinarySave(filename);
                break;
            default:
                if(debugging){
                    Debug.LogError("Unidentified save type!");
                }
        }
    }

    //TODO replace with json save system
    private bool BinarySave(string filename){
        try{
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(saveFolder + "/" + filename);

            GameStateData data = new GameStateData();

            #region Copy game state data to GameStateData
            
            #region Map Info

            data.sectorCount = 4;
            data.maxVisibleRange = 5;
            data.maxActualRange = 6;
            data.curTurn = -1;  //TODO

            //TODO iterate over the map to gather terrain data
            for(int i = 0; i < data.maxActualRange; i++){
                for(int j = 0; j < data.sectorCount; j++){
                    //TODO
                }
            }

            #endregion

            #region Base
            data.baseHP = -1;
            data.baseWeapon = -1;
            //include active base defenses/effects here; TODO
            #endregion

            #region TileActors

            //TODO iterate over all TileActors, adding all the data for each TileActor before moving on to the next TileActor
            List<TileActor.ObjType> tileActorTypes = new List<TileActor.ObjType>();
            List<int> ta_spawnTurn = new List<int>();       //the turn on which this TileActor did/will spawn; mainly important for enemies which have not yet spawned
            List<Vector2Int> ta_pos = new List<Vector2Int>();     //position of tileActors stored as (direction, range)
            List<int> ta_faction = new List<int>();         //0 for defender, 1 for attacker
            List<int> ta_maxHealth = new List<int>();       //the max possible health for this TileActor; generally the health that the TileActor spawns with
            List<int> ta_curHealth = new List<int>();       //the current health of the TileActor; generally maxHealth - damageTaken
            List<int> ta_damage = new List<int>();          //the standard attack damage of this TileActor

            //special abilities: TODO
            #endregion
            #endregion


            bf.Serialize(file, data);
            file.Close();
            if(debugging){
                Debug.Log("Current game has been saved to \"" + filename + "\" in folder \"" + saveFolder);
            }
            return true;
        }catch(Exception e){
            if(debugging){
                Debug.LogError("ERROR WHEN SAVING: " + e.Message);
            }
            return false;
        }
    }

    public bool Load(string filename, SaveType saveType = SaveType.chooseDefault){
        if(debugging){
            Debug.Log("Loading game from specified file \"" + fileToUse = "\"");
        }
        if(saveType = SaveType.chooseDefault){
            saveType = defaultSaveType;
        }
        switch(saveType){
            case SaveType.binary:
                return BinaryLoad(filename);
                break;
            default:
                if(debugging){
                    Debug.LogError("Unidentified save type!");
                }
        }
    }

    //TODO replace with json save system here as well
    private bool BinaryLoad(string filename){
        //remove all current stuff from the board in preparation for the new stuff
        Unload();
        if(File.Exists(saveFolder + "/" + filename)){
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(saveFolder + "/" + filename, FileMode.Open);
            GameStateData data = (GameStateData)bf.Deserialize(file);
            file.Close();

            #region turn the GameStateData object into the actual state of the game

            #region Overall Game Info

            //TODO map stuff other than terrain, see BinarySave

            //TODO iterate over the map to gather terrain data
            for(int i = 0; i < data.maxActualRange; i++){
                for(int j = 0; j < data.sectorCount; j++){
                    //TODO
                }
            }

            #endregion

            #region Base
            //TODO
            //include active base defenses/effects here; TODO
            #endregion

            #region TileActors

            //TODO iterate over all TileActors, adding all the data for each TileActor before moving on to the next TileActor

            //special abilities: TODO
            #endregion

            #endregion

            if(debugging){
                Debug.Log("Successfully loaded file.");
            }

        }else{
            if(debugging){
                Debug.LogError("Error with file; failed to load.");
            }
            return false;
        }
    }

	
	private void Unload()
	{

        //TODO
        //for (all tile objects) { destroy them safely to make room for a new load }
    }

    //TODO? I'm not sure if we need this
    /*public bool LoadInAddition(string filename)
	{
		//same thing as Load(), but doesn't unload previous things
	}*/
    #endregion

    #region Player Stats Save/Load
    //Reading json file into a string: https://stackoverflow.com/questions/30502222/read-and-parse-a-json-file-in-c-sharp-in-unity
    //To Json: https://docs.unity3d.com/ScriptReference/JsonUtility.ToJson.html

    //text file io: https://support.unity.com/hc/en-us/articles/115000341143-How-do-I-read-and-write-data-from-a-text-file

    public void SaveStats()
    {
        //first ready the player data
        PlayerStats playerStats = new PlayerStats();
        //TODO: get the player stats

        //now convert player data into a json
        string jsonStats = ToJson(playerStats, true);

        //finally, save the json string into a file
        //TODO

    }

    public void LoadStats()
    {
        //TODO
    }

    #endregion
}

[Serializable]
class GameStateData
{
#pragma warning disable 0649

    //we will assume that the game is only ever saved on the player's turn
    #region Map Info
    int sectorCount;        //how many directions there are (currently this will always be 4)
    int maxVisibleRange;    //the gameboard as it apears to the player
    int maxActualRange;     //maxVisibleRange plus the hidden range stuff that is used for spawning and such
    int curTurn;            //the turn counter's current value
    
    //the terrains are stored in a 1D list
    //this will correlate to a spiral starting with the innermost ring and the "northernmost" direction of the map and working its way slowly outward
    public List<int> terrains = new List<int>();
    #endregion

    #region Base
    int baseHP;
    int baseWeapon;
    //include active base defenses/effects here; TODO
    #endregion

    #region TileActors
    List<TileActor.ObjType> tileActorTypes = new List<TileActor.ObjType>();
    List<int> ta_spawnTurn = new List<int>();       //the turn on which this TileActor did/will spawn; mainly important for enemies which have not yet spawned
    List<Vector2Int> ta_pos = new List<Vector2Int>();     //position of tileActors stored as (direction, range)
    List<int> ta_faction = new List<int>();         //0 for defender, 1 for attacker
    List<int> ta_maxHealth = new List<int>();       //the max possible health for this TileActor; generally the health that the TileActor spawns with
    List<int> ta_curHealth = new List<int>();       //the current health of the TileActor; generally maxHealth - damageTaken
    List<int> ta_damage = new List<int>();          //the standard attack damage of this TileActor

    //special abilities: TODO
    #endregion


#pragma warning restore 0649
}


[Serializable]
class PlayerStats
{
#pragma warning disable 0649

    //the next level for the user to play
    //0 = new player
    //1 = player has played tutorial, so next they will play level 1
    //2 = player has played level 1, so next level is level 2
    //unlocked levels should generally be <= nextLevel
    int nextLevel;

    //each card is an index value; the value at that index is the number of copies of that card unlocked
    //{4, 0, 1} means that card 0 has 4 copies, card 1 has not been unlocked, and card 2 has only 1 available copy
    List<int> cardsUnlocked = new List<int>();

#pragma warning restore 0649
}