using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using static UnityEngine.JsonUtility;
using Unity.VisualScripting;
using System.Data;

public class SaveLoad : MonoBehaviour
{
    #region Debug Stuff
    //TODO set to false before final submission
    public bool debugging = true;

    #endregion

    public static SaveLoad saveLoad;
    public MapGenerator mapGenerator;

    public enum SaveType{
        chooseDefault,
        binary,
        json
        //TODO smart choosing of saveType that looks at file extension
    }

    public SaveType defaultSaveType = SaveType.json;
    public string fileToUse = "current_save.json";   //default save file
    public string saveFolder = "on Awake(), sets to \"Application.persistentDataPath\"";  //where save files go

    public string playerStatsFile = "player_stats.json";

    public void Awake()
    {
        saveFolder = Application.persistentDataPath;
        if(saveLoad == null)
        {
            saveLoad = this;
        }
    }

    public static void RanUnimplementedCode(string descriptor = "<no description>")
    {
        CustomDebug.RanUnimplementedCode(descriptor);
    }

    #region Text File I/O
    public void WriteFile(string filename, string contents)
    {
        using (StreamWriter writer = new StreamWriter(saveFolder + "/" + filename))
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
    public void SaveToDefault(){
        SaveType saveType = SaveType.chooseDefault;
        if (debugging){
            Debug.Log("Saving game to default file \"" + fileToUse + "\"");
        }
        Save(fileToUse, saveType);
    }

    public void LoadFromDefault(){
        SaveType saveType = SaveType.chooseDefault;
        if (debugging){
            Debug.Log("Loading game from default file \"" + fileToUse + "\"");
        }
        Load(fileToUse, saveType);
    }

    public bool Save(string filename, SaveType saveType = SaveType.chooseDefault){
        if(debugging){
            Debug.Log("Saving game to specified file \"" + fileToUse + "\"");
        }


        GameStateData data = new GameStateData();
        #region Copy game state data to GameStateData

        RanUnimplementedCode("Terrains, base effects and map effects, and TileActors are not yet saveable (or loadable).");

        #region Map Info

        data.sectorCount = getLaneCount();
        data.maxVisibleRange = getRingCount() - 1;
        data.maxActualRange = getRingCount();
        data.curTurn = getCurTurn();

        //TODO iterate over the map to gather terrain data
        for (int i = 0; i < data.maxActualRange; i++)
        {
            for (int j = 0; j < data.sectorCount; j++)
            {
                //TODO
            }
        }

        #endregion

        #region Base
        data.baseHP = getBaseHP();
        data.baseWeapon = getBaseWeapon();
        //include active base defenses/effects here; TODO
        #endregion

        #region TileActors


        //TODO not-yet-spawned enemies
        if (CustomDebug.SaveLoadDebugging())
        {
            RanUnimplementedCode("currently no saving of not-yet-spawned enemies");
        }

        //iterate over all TileActors, adding all the data for each TileActor before moving on to the next TileActor

        List<TileActor> actors = MapManager.Instance.GetTileActorList();
        List<int> ta_maxHealth = new List<int>();       //the max possible health for this TileActor; generally the health that the TileActor spawns with
        List<int> ta_curHealth = new List<int>();       //the current health of the TileActor; generally maxHealth - damageTaken
        List<Vector2Int> ta_pos = new List<Vector2Int>();     //position of tileActors stored as (direction, range)
        List<String> ta_name = new List<String>();
        List<TileActor.TileActorType> ta_type = new List<TileActor.TileActorType>();
        List<int> ta_damage = new List<int>();          //the standard attack damage of this TileActor
        List<int> ta_attackRange = new List<int>();     //the standard attack range of this TileActor

        for (int i = 0; i < actors.Count; i++)
        {
            ta_maxHealth.Add(actors[i].GetMaxHealth());
            ta_curHealth.Add(actors[i].GetCurrentHealth());
            MapTile mapTile = actors[i].GetCurrentTile();
            ta_pos.Add(new Vector2Int(mapTile.GetRingNumber(), mapTile.GetLaneNumber()));
            ta_name.Add(actors[i].GetActorName());
            ta_type.Add(actors[i].GetTileActorType());
            ta_damage.Add(actors[i].GetAttackDamage());
            ta_attackRange.Add(actors[i].GetAttackRange());
        }

        //TODO
        //List<TileActor.ObjType> tileActorTypes = new List<TileActor.ObjType>();
        //List<int> ta_spawnTurn = new List<int>();       //the turn on which this TileActor did/will spawn; mainly important for enemies which have not yet spawned
        
        //List<int> ta_faction = new List<int>();         //0 for defender, 1 for attacker
        

        //special abilities: TODO
        #endregion

        data.initialized = true;

        #endregion


        if (saveType == SaveType.chooseDefault){
            saveType = defaultSaveType;
        }
        switch(saveType){
            case SaveType.binary:
                return BinarySave(filename, data);
            //break;
            case SaveType.json:
                return JsonSave(filename, data);
            default:
                if(debugging){
                    Debug.LogError("Unidentified save type!");
                }
                return false;
        }
    }

    //this has been replaced with json file save system
    private bool BinarySave(string filename, GameStateData data){
        try{
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(saveFolder + "/" + filename);

            bf.Serialize(file, data);
            file.Close();

            if(debugging){
                Debug.Log("Current game has been saved to \"" + filename + "\" in folder \"" + saveFolder);
            }
            return true;
        }catch(Exception e){
            if(debugging){
                Debug.LogError("ERROR WHEN SAVING BINARY: " + e.Message);
            }
            return false;
        }
    }

    private bool JsonSave(string filename, GameStateData data)
    {
        try
        {
            //convert game data into a json
            string jsonData = ToJson(data, true);

            //save the json string into a file
            WriteFile(filename, jsonData);

            //success, presumably
            if (debugging)
            {
                Debug.Log("Current game has been saved to \"" + filename + "\" in folder \"" + saveFolder);
            }
            return true;
        }
        catch (Exception e)
        {
            if (debugging)
            {
                Debug.LogError("ERROR WHEN SAVING JSON: " + e.Message);
            }
            return false;
        }
    }

    public bool Load(string filename, SaveType saveType = SaveType.chooseDefault){
        if(debugging){
            Debug.Log("Loading game from specified file \"" + fileToUse + "\"");
        }

        GameStateData data = new GameStateData();
        bool noProblemLoading = true;   //set to false if there are any issues with the loading process

        if(saveType == SaveType.chooseDefault){
            saveType = defaultSaveType;
        }
        switch(saveType){
            case SaveType.binary:
                noProblemLoading = BinaryLoad(filename, out data);
                break;
            case SaveType.json:
                noProblemLoading = JsonLoad(filename, out data);
                break;
            default:
                if(debugging){
                    Debug.LogError("Unidentified save type!");
                }
                noProblemLoading = false;
                break;
        }

        noProblemLoading = noProblemLoading && data != null;
        if (noProblemLoading)
        {
            #region turn the GameStateData object into the actual state of the game

            #region Overall Game/Map Info

            RanUnimplementedCode("Terrains, base effects and map effects, and TileActors are not yet loadable (or saveable).");

            //general map stuff
            setLaneCount(data.sectorCount);
            setRingCount(data.maxActualRange);
            setCurTurn(data.curTurn);

            //TODO iterate over the map to gather terrain data
            for (int i = 0; i < data.maxActualRange; i++)
            {
                for (int j = 0; j < data.sectorCount; j++)
                {
                    //TODO
                }
            }

            #endregion

            #region Base
            setBaseHP(data.baseHP);
            setBaseWeapon(data.baseWeapon);
            //TODO: include active base defenses/effects here
            #endregion

            #region TileActors

            //TODO iterate over all TileActors, adding all the data for each TileActor before moving on to the next TileActor

            //special abilities: TODO
            #endregion

            #endregion
        }
        return noProblemLoading;

    }

    //this has also been replaced with json file save system, like the save system
    private bool BinaryLoad(string filename, out GameStateData data){
        //remove all current stuff from the board in preparation for the new stuff
        Unload();
        if(File.Exists(saveFolder + "/" + filename)){
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(saveFolder + "/" + filename, FileMode.Open);
            data = (GameStateData)bf.Deserialize(file);
            file.Close();


            if(debugging){
                Debug.Log("Current game has been loaded from \"" + filename + "\" in folder \"" + saveFolder);
            }
            return true;
        }else{
            if(debugging){
                Debug.LogError("Error with file; failed to load.");
            }
            data = null;
            return false;
        }
    }

	private bool JsonLoad(string filename, out GameStateData data)
    {
        try
        {
            //read the file into a string
            string jsonData = ReadFile(filename);

            //convert json into GameStateData object
            data = FromJson<GameStateData>(jsonData);

            //presumably the operation was successful
            if (debugging)
            {
                Debug.Log("Current game has been loaded from \"" + filename + "\" in folder \"" + saveFolder);
            }
            return true;
        }
        catch (Exception e)
        {
            if (debugging)
            {
                Debug.LogError("ERROR WHEN LOADING JSON: " + e.Message);
            }
            data = null;
            return false;
        }
    }

	private void Unload()
	{

        //TODO?
        //for (all tile objects) { destroy them safely to make room for a new load }
    }

    //TODO? I'm not sure if we need this
    /*public bool LoadInAddition(string filename)
	{
		//same thing as Load(), but doesn't unload previous things
	}*/

    #region Getting and Setting of Variables when Saving and Loading

    //get the number of directions on the map
    private int getLaneCount()
    {
        return MapManager.Instance.GetLaneCount();

        /*if (mapGenerator == null)
        {
            if (debugging)
            {
                Debug.LogError("No MapGenerator assigned to SaveLoad.");
            }
            return -1;
        }
        else
        {
            return mapGenerator.laneCount;
        }*/
    }

    //set the number of directions on the map
    private void setLaneCount(int amt)
    {
        MapManager.Instance.SetLaneCount(amt);

        /*if (mapGenerator == null)
        {
            if (debugging) {
                Debug.LogError("No MapGenerator assigned to SaveLoad.");
            }
        }
        else
        {
            mapGenerator.laneCount = amt;
        }*/
    }

    //get the number of rings around the base; does not include the base but does include invisible spawning rings (currently assumes 1 spawning ring, see load and save functions)
    private int getRingCount()
    {
        return MapManager.Instance.GetRingCount();

        /*if (mapgenerator == null)
        {
            if (debugging)
            {
                debug.logerror("no mapgenerator assigned to saveload.");
            }
            return -1;
        }
        else
        {
            return mapgenerator.ringcount;
        }*/
    }

    //set the number of rings around the base
    private void setRingCount(int amt)
    {
        MapManager.Instance.SetRingCount(amt);

        /*if (mapGenerator == null)
        {
            if (debugging)
            {
                Debug.LogError("No MapGenerator assigned to SaveLoad.");
            }
        }
        else
        {
            mapGenerator.ringCount = amt;
        }*/
    }

    //get the current turn counter
    private int getCurTurn()
    {
        //TODO
        RanUnimplementedCode("getCurTurn()");
        return -1;
    }

    //set the turn counter
    private void setCurTurn(int curTurn)
    {
        //TODO
        RanUnimplementedCode("setCurTurn");
    }

    //get the health of the base
    private int getBaseHP()
    {
        //TODO
        RanUnimplementedCode("getBaseHP()");
        return -1;
    }

    //set the base HP
    private void setBaseHP(int hp)
    {
        //TODO
        RanUnimplementedCode("setBaseHP()");
    }

    //get the current damage value of the base weapon
    private int getBaseWeapon()
    {
        //TODO
        RanUnimplementedCode("getBaseWeapon()");
        return -1;
    }

    //set the current base weapon damage
    private void setBaseWeapon(int amt)
    {
        //TODO
        RanUnimplementedCode("setBaseWeapon()");
    }


    #endregion



    #endregion

    #region Player Stats Save/Load
    //Reading json file into a string: https://stackoverflow.com/questions/30502222/read-and-parse-a-json-file-in-c-sharp-in-unity
    //To Json: https://docs.unity3d.com/ScriptReference/JsonUtility.ToJson.html

    //text file io: https://support.unity.com/hc/en-us/articles/115000341143-How-do-I-read-and-write-data-from-a-text-file

    public void SaveStats()
    {
        //first ready the player data
        PlayerStats playerStats = new PlayerStats();

        //get the player stats
        playerStats.cardsUnlocked = Deck.Instance.cardsUnlocked;

        //TODO: save next level variable
        if (CustomDebug.SaveLoadDebugging())
        {
            CustomDebug.RanUnimplementedCode("SaveStats() only saves unlocked cards.");
        }

        //now convert player data into a json
        string jsonStats = ToJson(playerStats, true);

        //finally, save the json string into a file
        WriteFile(playerStatsFile, jsonStats);

    }

    public void LoadStats()
    {
        //first read the file into a string
        string jsonStats = ReadFile(playerStatsFile);

        //now convert json into player data object
        PlayerStats playerStats = FromJson<PlayerStats>(jsonStats);

        //finally, use the player statistics as needed
        Deck.Instance.cardsUnlocked = playerStats.cardsUnlocked;

        //TODO: load next level variable
        if (CustomDebug.SaveLoadDebugging())
        {
            CustomDebug.RanUnimplementedCode("LoadStats() is not complete.");
        }
    }

    #endregion

    /// <summary>
    /// A function that writes a basic text file and then tries to read from it
    /// </summary>
    public void ReadWriteTest()
    {
        string testFileName = "text_io_test.txt";
        WriteFile(testFileName, "Test line 1\nTest line 2");
        Debug.Log("Wrote testing text to " + saveFolder + "/" + testFileName);
        Debug.Log("Attempting to read that test file...");
        Debug.Log("Successful read, contents are as follows:\n" + ReadFile(testFileName));
        if (saveLoad != null)
        {
            if (saveLoad == this)
            {
                Debug.Log("NOTE: SaveLoad.Awake() is running mutliple times.");
            }
            else
            {
                Debug.Log("Multiple SaveLoad objects detected; be careful.");
            }
        }
    }
}

[Serializable]
class GameStateData
{
#pragma warning disable 0649

    public bool initialized = false;    //set to true to indicate that all of the following variables were given the correct information that is wanted for saving/loading

    //we will assume that the game is only ever saved on the player's turn
    #region Map Info
    public int sectorCount;        //how many directions there are (currently this will always be 4)
    public int maxVisibleRange;    //the gameboard as it apears to the player
    public int maxActualRange;     //maxVisibleRange plus the hidden range stuff that is used for spawning and such
    public int curTurn;            //the turn counter's current value
    
    //the terrains are stored in a 1D list
    //this will correlate to a spiral starting with the innermost ring and the "northernmost" direction of the map and working its way slowly outward
    public List<int> terrains = new List<int>();
    #endregion

    #region Base
    public int baseHP;
    public int baseWeapon;
    //include active base defenses/effects here; TODO
    #endregion

    #region TileActors
    //List<TileActor.ObjType> tileActorTypes = new List<TileActor.ObjType>();
    List<int> ta_spawnTurn = new List<int>();       //the turn on which this TileActor did/will spawn; mainly important for enemies which have not yet spawned
    List<Vector2Int> ta_pos = new List<Vector2Int>();     //position of tileActors stored as (direction, range)
    List<int> ta_faction = new List<int>();         //0 for defender, 1 for attacker
    List<int> ta_maxHealth = new List<int>();       //the max possible health for this TileActor; generally the health that the TileActor spawns with
    List<int> ta_curHealth = new List<int>();       //the current health of the TileActor; generally maxHealth - damageTaken
    List<int> ta_damage = new List<int>();          //the standard attack damage of this TileActor

    //special abilities: TODO
    #endregion

    #region Cards
    public List<int> drawPile = new List<int>();
    public List<int> discardPile = new List<int>();
    public List<Card> hand = new List<Card>();
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
    public int nextLevel;

    //each card is an index value; the value at that index is the number of copies of that card unlocked
    //{4, 0, 1} means that card 0 has 4 copies, card 1 has not been unlocked, and card 2 has only 1 available copy
    public List<int> cardsUnlocked = new List<int>();

#pragma warning restore 0649
}