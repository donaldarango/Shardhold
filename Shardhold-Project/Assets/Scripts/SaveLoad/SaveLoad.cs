using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using static UnityEngine.JsonUtility;
using Unity.VisualScripting;
using System.Data;
using JetBrains.Annotations;
using System.Linq;
using TMPro;
using NUnit.Framework;

using static CustomDebug;

public class SaveLoad : MonoBehaviour
{


    public static SaveLoad saveLoad;
    //public MapGenerator mapGenerator;

    public enum FileType
    {
        chooseDefault,
        binary,
        json
    }

    public enum SaveType
    {
        playerSave,
        levelFile,
        misc,
        debugging
    }

    public FileType defaultFileType = FileType.json;
    public string fileToUse = "current_save.json";   //default save file
    public string saveFolder = "Assets/Resources/PlayerSaves";   //"on Awake(), sets to \"Application.persistentDataPath\"";  //where save files go (when the player saves a game)
    public string levelsFolder = "Assets/Resources/LevelSettings";  //from TileActor
    public string miscFolder = "Assets/Resources/Misc";
    public string debugFolder = "Assets/Resources/Debug";

    public PlayerStats playerStats = null;
    public string playerStatsFile = "player_stats.json";

    public List<SavedLane> lastLoadedLanes = null;

    private string curSaveLoadVersion = "1.00";
    private string saveFileVersionsFile = "file_details.json";

    public GameStateData mostRecentLoad = null;

    public void Awake()
    {
        //saveFolder = Application.persistentDataPath;
        if (saveLoad == null)
        {
            saveLoad = this;
        }
    }

    public static void RanUnimplementedCode(string descriptor = "<no description>", CustomDebug.DebuggingType level = CustomDebug.DebuggingType.ErrorOnly)
    {
        if (CustomDebug.SaveLoadDebugging(level))
        {
            CustomDebug.RanUnimplementedCode(descriptor);
        }
    }

    public static void Print(string output, CustomDebug.DebuggingType level = CustomDebug.DebuggingType.Normal)
    {
        if (CustomDebug.SaveLoadDebugging(level))
        {
            Debug.Log(output);
        }
    }

    #region Text File I/O
    public void WriteFile(string filename, string fileLocation, string contents)
    {
        try
        {
            using (StreamWriter writer = new StreamWriter(fileLocation + "/" + filename))
            {
                writer.Write(contents);
            }
        }
        catch (ArgumentException e)
        {
            Print("An Argument Exception occurred when writing to file. File was: " + fileLocation + "/" + filename + "; the error message is as follows:\n" + e.ToString());
        }
    }

    public bool ReadFile(string filename, string fileLocation, out string contents)
    {
        contents = "";
        string absoluteFile = fileLocation + "/" + filename;
        if (File.Exists(absoluteFile))
        {
            using (StreamReader reader = new StreamReader(absoluteFile))
            {
                contents = reader.ReadToEnd();
            }
            return true;
        }
        else
        {
            return false;
        }
    }
    #endregion


    #region Save/Load, see https://www.youtube.com/watch?v=J6FfcJpbPXE
    public void SaveToDefault(TMP_Text saveText)
    {
        FileType fileType = FileType.chooseDefault;
        if (CustomDebug.SaveLoadDebugging(CustomDebug.DebuggingType.Normal))
        {
            Debug.Log("Saving game to default file \"" + fileToUse + "\"");
        }
        if (Save(fileToUse, SaveType.playerSave, fileType) && saveText != null)
        {
            saveText.text = "GAME SAVED";
        }
    }

    public void LoadFromDefault(TMP_Text loadText)
    {
        FileType saveType = FileType.chooseDefault;
        if (CustomDebug.SaveLoadDebugging(CustomDebug.DebuggingType.Normal))
        {
            Debug.Log("Loading game from default file \"" + fileToUse + "\"");
        }
        if (Load(fileToUse, SaveType.playerSave, saveType) && loadText != null)
        {
            loadText.text = "GAME LOADED";
        }
    }

    public bool Save(string filename, SaveType saveType = SaveType.playerSave, FileType fileType = FileType.chooseDefault)
    {
        if (CustomDebug.SaveLoadDebugging(CustomDebug.DebuggingType.Normal))
        {
            Debug.Log("Saving game to specified file \"" + fileToUse + "\"");
        }

        GameStateData data = CreateData();

        StoreVersion(filename, data.saveVersion);

        #region Writing to File

        if (fileType == FileType.chooseDefault)
        {
            fileType = defaultFileType;
        }
        bool output = false;
        switch (fileType)
        {
            case FileType.binary:
                output = BinarySave(filename, data);
                break;
            case FileType.json:
                output = JsonSave(filename, saveType, data);
                break;
            default:
                if (CustomDebug.SaveLoadDebugging(CustomDebug.DebuggingType.ErrorOnly))
                {
                    Debug.LogError("Unidentified save type!");
                }
                output = false;
                break;
        }

        #endregion

        if (CustomDebug.instance.runAssertionTesting && CustomDebug.instance.saveVerificationTesting)
        {
            //CustomDebug.instance.prevData = data;
            Load(filename, saveType, fileType);
            GameStateData verificationData = CreateData();
            JsonSave("saveVerificationTestData.json", SaveType.misc, verificationData);
            Assert.IsTrue(StrictDataCompare(data, verificationData));
        }

        return output;
    }

    public GameStateData CreateData()
    {
        GameStateData data = new GameStateData();

        #region Copy game state data to GameStateData

        //RanUnimplementedCode("Any effects are not yet saveable (or loadable).");

        #region Map Info

        data.sectorCount = getLaneCount();
        //data.maxVisibleRange = getRingCount() - 1;
        data.maxActualRange = getRingCount();
        data.curTurn = getCurTurn();

        //data.terrainArray = new Terrain[4,MapManager.Instance.GetLaneCount() * MapManager.Instance.GetRingCount()];
        data.lanes = new List<SavedLane>();
        /*for (int i = 0; i < 4; i++)
        {
            //data.terrains.Add(new List<Terrain>());
            SavedLane curLane = new SavedLane();
            data.lanes.Add(curLane);
            List<MapTile> tileList = MapManager.Instance.GetQuadrant(i).GetMapTilesList();
            for (int j = 0; j < tileList.Count; j++)
            {
                //data.terrains[i].Add(tileList[j].GetTerrain());
                //data.terrainArray[i,j] = tileList[j].GetTerrain();
                curLane.terrains.Add(MapManager.Instance.GetTile)
                Print("i, j: " + i + ", " + j, CustomDebug.DebuggingType.Verbose);
            }
        }*/

        for (int i = 0; i < MapManager.Instance.GetLaneCount() * 4; i++)
        {
            SavedLane curLane = new SavedLane();
            curLane.terrains = new List<int>();
            data.lanes.Add(curLane);
            for (int j = 0; j < MapManager.Instance.GetRingCount(); j++)
            {
                curLane.terrains.Add(((int)MapManager.Instance.GetTile(j, i).GetTerrain().terrainType));
            }
        }


        #endregion

        #region Base
        data.baseHP = getBaseHP();
        data.baseWeapon = getBaseWeapon();
        //include active base defenses/effects here; TODO
        #endregion

        #region TileActors

        #region Spawned TileActors

        //iterate over all TileActors, adding all the data for each TileActor before moving on to the next TileActor
        data.spawnedTileActors = new List<SpawnedTileActor>();
        List<TileActor> actors = MapManager.Instance.GetTileActorList(true);

        /*
        data.ta_maxHealth = new List<int>();       //the max possible health for this TileActor; generally the health that the TileActor spawns with
        data.ta_curHealth = new List<int>();       //the current health of the TileActor; generally maxHealth - damageTaken
        data.ta_pos = new List<Vector2Int>();     //position of tileActors stored as (direction, range)
        data.ta_name = new List<String>();
        data.ta_type = new List<TileActor.TileActorType>();
        data.ta_damage = new List<int>();          //the standard attack damage of this TileActor
        data.ta_attackRange = new List<int>();     //the standard attack range of this TileActor
        */

        for (int i = 0; i < actors.Count; i++)
        {
            data.spawnedTileActors.Add(new SpawnedTileActor());
            SpawnedTileActor sta = data.spawnedTileActors[i];
            sta.name = actors[i].GetActorName();
            sta.type = actors[i].GetTileActorType();
            //sta.maxHealth = actors[i].GetMaxHealth();
            sta.curHealth = actors[i].GetCurrentHealth();
            MapTile mapTile = actors[i].GetCurrentTile();
            sta.pos = new Vector2Int(mapTile.GetRingNumber(), mapTile.GetLaneNumber());
            sta.attackRange = actors[i].GetAttackRange();
            sta.damage = actors[i].GetAttackDamage();
            sta.isShielded = actors[i].GetIsShielded();
            sta.isPoisoned = actors[i].GetIsPoisoned();


            /*
            data.ta_maxHealth.Add(actors[i].GetMaxHealth());
            data.ta_curHealth.Add(actors[i].GetCurrentHealth());
            MapTile mapTile = actors[i].GetCurrentTile();
            data.ta_pos.Add(new Vector2Int(mapTile.GetRingNumber(), mapTile.GetLaneNumber()));
            data.ta_name.Add(actors[i].GetActorName());
            data.ta_type.Add(actors[i].GetTileActorType());
            data.ta_damage.Add(actors[i].GetAttackDamage());
            data.ta_attackRange.Add(actors[i].GetAttackRange());
            */
        }
        #endregion

        #region Not-Yet-Spawned Enemies

        data.spawnList = TileActorManager.Instance.GetSpawnList();

        #endregion

        #endregion

        #region Cards

        //data.hand = new List<Card>(Deck.Instance.hand);
        data.hand = new List<SavedCard>();
        for (int i = 0; i < Deck.Instance.hand.Length; i++)
        {
            if (Deck.Instance.hand[i] != null)
            {
                SavedCard savedCard = new SavedCard();
                data.hand.Add(savedCard);
                ScriptableObject intermediate = Deck.Instance.hand[i];
                if (intermediate is Card)
                {
                    savedCard.cardId = ((Card)intermediate).GetId();
                    savedCard.cardHealth = ((Card)intermediate).hp;
                }
                else
                {
                    savedCard.cardId = ((AllyUnitStats)intermediate).GetId();
                    savedCard.cardHealth = ((AllyUnitStats)intermediate).hp;
                }
            }
        }
        data.drawPile = Deck.Instance.drawPile;
        data.discardPile = Deck.Instance.discardPile;

        #endregion

        //data.saveTime = DateTime.Now;
        data.initialized = true;

        #endregion

        return data;
    }

    //this has been replaced with json file save system
    private bool BinarySave(string filename, GameStateData data)
    {
        try
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(saveFolder + "/" + filename);

            bf.Serialize(file, data);
            file.Close();

            if (CustomDebug.SaveLoadDebugging(CustomDebug.DebuggingType.Normal))
            {
                Debug.Log("Current game has been saved to \"" + filename + "\" in folder \"" + saveFolder);
            }
            return true;
        }
        catch (Exception e)
        {
            if (CustomDebug.SaveLoadDebugging(CustomDebug.DebuggingType.ErrorOnly))
            {
                Debug.LogError("ERROR WHEN SAVING BINARY: " + e.Message);
            }
            return false;
        }
    }

    private bool JsonSave(string filename, SaveType saveType, GameStateData data)
    {
        try
        {
            //convert game data into a json
            string jsonData = ToJson(data, true);

            string saveLocation = GetSaveLocation(saveType);


            //save the json string into a file
            WriteFile(filename, saveLocation, jsonData);

            //success, presumably
            if (CustomDebug.SaveLoadDebugging(CustomDebug.DebuggingType.Normal))
            {
                Debug.Log("Current game has been saved to \"" + filename + "\" in folder \"" + saveLocation + "\"");
            }
            return true;
        }
        catch (Exception e)
        {
            if (CustomDebug.SaveLoadDebugging(CustomDebug.DebuggingType.ErrorOnly))
            {
                Debug.LogError("ERROR WHEN SAVING JSON: " + e.Message);
            }
            return false;
        }
    }

    public bool Load(string filename, SaveType saveType = SaveType.playerSave, FileType fileType = FileType.chooseDefault)
    {
        if (CustomDebug.SaveLoadDebugging(CustomDebug.DebuggingType.Normal))
        {
            Debug.Log("Loading game from specified file \"" + fileToUse + "\"");
        }

        #region Reading File

        GameStateData data = new GameStateData();
        bool noProblemLoading = true;   //set to false if there are any issues with the loading process

        if (fileType == FileType.chooseDefault)
        {
            fileType = defaultFileType;
        }
        switch (fileType)
        {
            case FileType.binary:
                noProblemLoading = BinaryLoad(filename, out data);
                break;
            case FileType.json:
                noProblemLoading = JsonLoad(filename, saveType, out data);
                Print("Json no problem loading: " + noProblemLoading, CustomDebug.DebuggingType.Normal);
                break;
            default:
                if (CustomDebug.SaveLoadDebugging(CustomDebug.DebuggingType.ErrorOnly))
                {
                    Debug.LogError("Unidentified save type!");
                }
                noProblemLoading = false;
                break;
        }

        #endregion

        noProblemLoading = noProblemLoading && data != null;
        if (noProblemLoading)
        {
            Unload();
            MapManager.Instance.RemoveAllTiles();
            mostRecentLoad = data;

            #region turn the GameStateData object into the actual state of the game

            #region Overall Game/Map Info

            //RanUnimplementedCode("Any effects are not yet loadable (or savable).");

            //general map stuff
            setLaneCount(data.sectorCount);
            setRingCount(data.maxActualRange);
            setCurTurn(data.curTurn);

            //MapManager.Instance.InitializeQuadrants();

            //clear loaded lanes
            lastLoadedLanes.Clear();

            for (int i = 0; i < data.lanes.Count; i++)
            {
                lastLoadedLanes.Add(data.lanes[i]);
            }

            MapGenerator.Instance.GenerateMap();

            #endregion

            #region Base
            setBaseHP(data.baseHP);
            setBaseWeapon(data.baseWeapon);
            //TODO: include active base defenses/effects here
            #endregion

            #region TileActors

            #region Spawned TileActors

            //iterate over all TileActors, adding all the data for each TileActor before moving on to the next TileActor
            for (int i = 0; i < data.spawnedTileActors.Count; i++)
            {
                SpawnedTileActor sta = data.spawnedTileActors[i];
                TileActor newTA = null;
                switch (sta.type)
                {
                    case TileActor.TileActorType.EnemyUnit:
                        newTA = MapManager.Instance.AddEnemyToMapTile(sta.pos.x, sta.pos.y, sta.name);
                        break;
                    case TileActor.TileActorType.Structure:
                        newTA = MapManager.Instance.AddStructureToMapTile(sta.pos.x, sta.pos.y, sta.name);
                        break;
                    case TileActor.TileActorType.Trap:
                        newTA = MapManager.Instance.AddTrapToMapTile(sta.pos.x, sta.pos.y, sta.name);
                        /*if (CustomDebug.SaveLoadDebugging(CustomDebug.DebuggingType.Warnings))
                        {
                            Debug.Log("Trap detected in the TileActor slot for tile: " + sta.pos.x + ", " + sta.pos.y);
                        }*/
                        break;
                    default:
                        if (CustomDebug.SaveLoadDebugging(CustomDebug.DebuggingType.ErrorOnly))
                        {
                            RanUnimplementedCode("Cannot load this unknown tileactor type: " + sta.type.ToString());
                        }
                        break;

                }

                Print("Created new tile actor: " + newTA.name);

                //put in all the other variables for this tileactor
                newTA.SetCurrentHealth(sta.curHealth);
                newTA.SetIsShielded(sta.isShielded);
                newTA.SetIsPoisoned(sta.isPoisoned);
                //NOTE: we aren't actually setting the max health



            }

            #endregion

            #region Not-Yet-Spawned Enemies

            TileActorManager.Instance.SetSpawnList(data.spawnList);

            #endregion



            //special abilities: TODO
            #endregion

            #region Cards

            Deck.Instance.DeleteAllCardsInHand();
            Deck.Instance.hand = new ScriptableObject[Deck.Instance.hand.Length];
            for (int i = 0; i < data.hand.Count; i++)
            {
                Deck.Instance.CreateCard(data.hand[i].cardId);
                ScriptableObject intermediate = Deck.Instance.hand[i];

                if (intermediate is Card)
                {
                    ((Card)intermediate).hp = data.hand[i].cardHealth;  //is this necessary/correct?
                }
                else
                {

                    /*AllyUnitStats intermediateAllyUnitStats = (AllyUnitStats)intermediate;*/
                    GameObject intermediateAllyUnitStatsGameObject = Deck.Instance.UIHand[i];
                    AllyUnit intermediateAllyUnit = intermediateAllyUnitStatsGameObject.GetComponent<AllyUnit>();
                    intermediateAllyUnit.currentHealth = data.hand[i].cardHealth;
                }
            }

            #endregion

            #endregion
        }
        else
        {
            Print("Problem Loading.", CustomDebug.DebuggingType.Warnings);
        }
        return noProblemLoading;

    }

    //this has also been replaced with json file save system, like the save system
    private bool BinaryLoad(string filename, out GameStateData data)
    {
        //remove all current stuff from the board in preparation for the new stuff
        //Unload();
        if (File.Exists(saveFolder + "/" + filename))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(saveFolder + "/" + filename, FileMode.Open);
            data = (GameStateData)bf.Deserialize(file);
            file.Close();


            if (CustomDebug.SaveLoadDebugging(CustomDebug.DebuggingType.Normal))
            {
                Debug.Log("Current game has been loaded from \"" + filename + "\" in folder \"" + saveFolder + "\"");
            }
            return true;
        }
        else
        {
            if (CustomDebug.SaveLoadDebugging(CustomDebug.DebuggingType.ErrorOnly))
            {
                Debug.LogError("Error with file; failed to load.");
            }
            data = null;
            return false;
        }
    }

    private bool JsonLoad(string filename, SaveType saveType, out GameStateData data)
    {
        try
        {
            //read the file into a string
            string jsonData;
            string saveLocation = GetSaveLocation(saveType);
            if (ReadFile(filename, saveLocation, out jsonData))
            {

                //convert json into GameStateData object
                data = FromJson<GameStateData>(jsonData);

                //presumably the operation was successful
                if (CustomDebug.SaveLoadDebugging(CustomDebug.DebuggingType.Normal))
                {
                    Debug.Log("Current game has been loaded from \"" + filename + "\" in folder \"" + saveLocation);
                }
                return true;
            }
            else
            {
                Print("Problem reading json file.", CustomDebug.DebuggingType.Warnings);
                data = null;
                return false;
            }
        }
        catch (Exception e)
        {
            if (CustomDebug.SaveLoadDebugging(CustomDebug.DebuggingType.ErrorOnly))
            {
                Debug.LogError("ERROR WHEN LOADING JSON: " + e.Message);
            }
            data = null;
            return false;
        }
    }

    private void Unload()
    {
        //TODO remove any map effects or similar; don't worry about adjusting base HP or weapon, as these will be set when loading anyways



        List<TileActor> actors = MapManager.Instance.GetTileActorList(true);

        for (int i = 0; i < actors.Count; i++)
        {
            actors[i].Die();
        }

        TileActorManager.Instance.SetSpawnList(new List<TileActorManager.RoundSpawnInfo>());    //empty list

        //for (all tile objects) { destroy them safely to make room for a new load }

        //destroy map tiles moved to load to avoid issues with Update functions accessing map

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
        //MapManager.Instance.SetLaneCount(amt);
        MapGenerator.Instance.laneCount = amt;
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
        //MapManager.Instance.SetRingCount(amt);
        MapGenerator.Instance.ringCount = amt;
    }

    //get the current turn counter
    private int getCurTurn()
    {
        return TileActorManager.Instance.currentRound;
    }

    //set the turn counter
    private void setCurTurn(int curTurn)
    {
        TileActorManager.Instance.currentRound = curTurn;
    }

    //get the health of the base
    private int getBaseHP()
    {
        return Base.Instance.GetBaseHealth();
    }

    //set the base HP
    private void setBaseHP(int hp)
    {
        Base.Instance.SetBaseHealth(hp);
    }

    private int getBaseHPMax()
    {
        //TODO
        //RanUnimplementedCode("getBaseHPMax()");
        return 100;
    }

    //get the current damage value of the base weapon
    private int getBaseWeapon()
    {
        //TODO
        //RanUnimplementedCode("getBaseWeapon()");
        return -1;
    }

    //set the current base weapon damage
    private void setBaseWeapon(int amt)
    {
        //TODO
        //RanUnimplementedCode("setBaseWeapon()");
    }

    private int getBaseWeaponMax()
    {
        //TODO
        //RanUnimplementedCode("getBaseWeaponMax()");
        return 100;
    }


    #endregion



    #endregion

    #region Player Stats Save/Load
    //Reading json file into a string: https://stackoverflow.com/questions/30502222/read-and-parse-a-json-file-in-c-sharp-in-unity
    //To Json: https://docs.unity3d.com/ScriptReference/JsonUtility.ToJson.html

    //text file io: https://support.unity.com/hc/en-us/articles/115000341143-How-do-I-read-and-write-data-from-a-text-file


    public void SaveStats()
    {
        //get the playerStats cardsUnlocked from the Deck cardsUnlocked
        playerStats.cardsUnlocked = Deck.Instance.cardsUnlocked;

        //now convert player data into a json
        string jsonStats = ToJson(playerStats, true);

        //finally, save the json string into a file
        WriteFile(playerStatsFile, saveFolder, jsonStats);
    }

    public void LoadStats()
    {
        //first read the file into a string
        string jsonStats;
        if (ReadFile(playerStatsFile, saveFolder, out jsonStats))
        {
            //now convert json into player data object
            playerStats = FromJson<PlayerStats>(jsonStats);
        }
        else
        {
            //no stats yet
            playerStats = new PlayerStats();

            //TODO default player stats
            RanUnimplementedCode("Default stats not set up.");
        }

        //finally, use the player statistics as needed
        Deck.Instance.cardsUnlocked = playerStats.cardsUnlocked;
    }

    #endregion

    /// <summary>
    /// A function that writes a basic text file and then tries to read from it
    /// </summary>
    public void ReadWriteTest()
    {
        string testFileName = "text_io_test.txt";
        WriteFile(testFileName, saveFolder, "Test line 1\nTest line 2");
        Debug.Log("Wrote testing text to " + saveFolder + "/" + testFileName);
        Debug.Log("Attempting to read that test file...");
        string readString = "";
        if (ReadFile(testFileName, saveFolder, out readString))
        {
            Debug.Log("Successful read, contents are as follows:\n" + readString);
        }
        else
        {
            Debug.Log("Unsuccessful read; file not found.");
        }
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

    #region Save File Version System

    public string GetVersion(string fileName)
    {
        string jsonVersionFile;
        if (ReadFile(saveFileVersionsFile, GetSaveLocation(SaveType.misc), out jsonVersionFile))
        {
            SaveFileList saveFileList = FromJson<SaveFileList>(jsonVersionFile);

            for (int i = 0; i < saveFileList.files.Count; i++)
            {
                if (saveFileList.files[i].fileName == fileName)
                {
                    return saveFileList.files[i].version;
                }
            }
        }
        return "";

    }

    public void StoreVersion(string fileName, string version)
    {
        string jsonVersionFile;
        SaveFileList saveFileList = null;
        if (ReadFile(saveFileVersionsFile, GetSaveLocation(SaveType.misc), out jsonVersionFile))
        {
            saveFileList = FromJson<SaveFileList>(jsonVersionFile);
        }

        if (saveFileList != null)
        {
            for (int i = 0; i < saveFileList.files.Count; i++)
            {
                if (saveFileList.files[i].fileName == fileName)
                {
                    saveFileList.files[i].version = version;

                    //save the updated list
                    jsonVersionFile = ToJson(saveFileList, true);
                    WriteFile(saveFileVersionsFile, GetSaveLocation(SaveType.misc), jsonVersionFile);
                    return;
                }
            }
        }
        else
        {
            saveFileList = new SaveFileList();
            saveFileList.files = new List<SaveFileMetaData>();
        }
        SaveFileMetaData newData = new SaveFileMetaData();
        newData.fileName = fileName;
        newData.version = version;
        saveFileList.files.Add(newData);

        //save the updated list
        jsonVersionFile = ToJson(saveFileList, true);
        WriteFile(saveFileVersionsFile, GetSaveLocation(SaveType.misc), jsonVersionFile);
        return;

    }

    #endregion

    public string GetSaveLocation(SaveType saveType)
    {
        if (saveType == SaveType.levelFile)
        {
            return levelsFolder;
        }
        else if (saveType == SaveType.playerSave)
        {
            return saveFolder;
        }
        else if (saveType == SaveType.misc)
        {
            return miscFolder;
        }else if (saveType == SaveType.debugging)
        {
            return debugFolder;
        }
        else
        {
            Print("Unhandled saveType: " + saveType.ToString() + "; file will be saved in miscFolder", CustomDebug.DebuggingType.ErrorOnly);
            return miscFolder;
        }
    }

    public bool StrictDataCompare(GameStateData data1, GameStateData data2)
    {
        DateTime compareDate = DateTime.Now;
        bool output = true;
        //Debug.Log(DateTime.Now.ToFileTimeUtc());  //the current time
        string logFile = "StrictDataCompareOutput_" + compareDate.ToFileTimeUtc() + ".txt";

        string log = "Strict GameStateData Comparison Results\n";
        log += "Date/Time:" + Tab() + compareDate.ToString("G") + "\n\n";

        log += "PER-SECTION RESULTS:\n";

        string successStr = "   passed   " + Tab();
        string failStr = "***FAILED***" + Tab();

        //int tabShift = 0;

        //log += "initialized:" + Tab(tabShift + 1);
        if (data1.initialized == data2.initialized)
        {
            log += successStr;
        }
        else
        {
            log += failStr;
            output = false;
        }
        log += "initialization status\n";

        //log += "saveVersion:" + Tab(tabShift + 1);
        if (data1.saveVersion.Equals(data2.saveVersion))
        {
            log += successStr;
        }
        else
        {
            log += failStr;
            output = false;
        }
        log += "save version\n";

        //log += "level:" + Tab(tabShift + 2);
        if (data1.level == data2.level)
        {
            log += successStr;
        }
        else
        {
            log += failStr;
            output = false;
        }
        log += "current level\n";

        //log += "sectorCount:" + Tab(tabShift + 1);
        if (data1.sectorCount == data2.sectorCount)
        {
            log += successStr;
        }
        else
        {
            log += failStr;
            output = false;
        }
        log += "sector count\n";

        //log += "maxActualRange:" + Tab(tabShift + 1);
        if (data1.maxActualRange == data2.maxActualRange)
        {
            log += successStr;
        }
        else
        {
            log += failStr;
            output = false;
        }
        log += "max range/rings\n";

        //log += "curTurn:" + Tab(tabShift + 1);
        if (data1.curTurn == data2.curTurn)
        {
            log += successStr;
        }
        else
        {
            log += failStr;
            output = false;
        }
        log += "current turn\n";

        //log += "lanes:" + Tab(tabShift + 2);
        if (EqualLanes(data1.lanes, data2.lanes, false))
        {
            log += successStr;
        }
        else
        {
            log += failStr;
            output = false;
        }
        log += "terrains\n";

        //log += "baseHP:" + Tab(tabShift + 2);
        if (data1.baseHP == data2.baseHP)
        {
            log += successStr;
        }
        else
        {
            log += failStr;
            output = false;
        }
        log += "base health\n";

        //log += "baseWeapon:" + Tab(tabShift + 1);
        if (data1.baseWeapon == data2.baseWeapon)
        {
            log += successStr;
        }
        else
        {
            log += failStr;
            output = false;
        }
        log += "base weapon\n";

        //log += "tile actors:" + Tab(tabShift + 1);
        if (EqualSpawnedTileActors(data1.spawnedTileActors, data2.spawnedTileActors, false))
        {
            log += successStr;
        }
        else
        {
            log += failStr;
            output = false;
        }
        log += "spawned tile actors\n";

        //log += "spawnList:" + Tab(tabShift + 1);
        if (TileActorManager.RoundSpawnInfo.EqualRoundSpawnInfo(data1.spawnList, data2.spawnList, false))
        {
            log += successStr;
        }
        else
        {
            log += failStr;
            output = false;
        }
        log += "spawn list\n";

        /*log += "trapUnits:\t";
        if (data1.trapUnits.Equals(data2.trapUnits))
        {
            log += successStr;
        }
        else
        {
            log += failStr;
            output = false;
        }*/

        //log += "drawPile:" + Tab(tabShift + 1);
        if (data1.drawPile.Equals(data2.drawPile))
        {
            log += successStr;
        }
        else
        {
            log += failStr;
            output = false;
        }
        log += "draw pile\n";

        //log += "discardPile:" + Tab(tabShift + 1);
        if (data1.discardPile.Equals(data2.discardPile))
        {
            log += successStr;
        }
        else
        {
            log += failStr;
            output = false;
        }
        log += "discard pile\n";

        //log += "hand:" + Tab(tabShift + 2);
        if (EqualSavedCards(data1.hand, data2.hand, false))
        {
            log += successStr;
        }
        else
        {
            log += failStr;
            output = false;
        }
        log += "hand\n";

        log += "\nOVERALL RESULT:\n";
        if (output)
        {
            log += successStr;
        }
        else
        {
            log += failStr;
        }

        if (CustomDebug.Debugging(CustomDebug.DebuggingType.ErrorOnly))
        {
            Debug.Log(log);
        }
        if (CustomDebug.instance.saveVerificationTesting_saveOutputToFile)
        {
            if (CustomDebug.Debugging(CustomDebug.DebuggingType.Normal))
            {
                Debug.Log("Results saved to log file: " + logFile);
            }
            WriteFile(logFile, GetSaveLocation(SaveType.debugging), log);
        }

        return output;
    }

    public static bool EqualLanes(List<SavedLane> lanes1, List<SavedLane> lanes2, bool allowNull)
    {
        if (lanes1 == null)
        {
            return allowNull && lanes2 == null;
        }

        if (lanes2 == null)
        {
            return false;
        }

        if (lanes1.Count != lanes2.Count)
        {
            return false;
        }

        for (int i = 0; i < lanes1.Count; i++)
        {
            if (!EqualLanes(lanes1[i], lanes2[i], allowNull))
            {
                return false;
            }
        }

        return true;
    }

    public static bool EqualLanes(SavedLane lane1, SavedLane lane2, bool allowNull)
    {
        if (lane1 == null)
        {
            return allowNull && lane2 == null;
        }

        if (lane2 == null)
        {
            return false;
        }

        if (lane1.terrains.Count != lane2.terrains.Count)
        {
            return false;
        }

        for (int i = 0; i < lane1.terrains.Count; i++)
        {
            if (lane1.terrains[i] != lane2.terrains[i])
            {
                return false;
            }
        }

        return true;
    }

    public static bool EqualSpawnedTileActors(List<SpawnedTileActor> lhs, List<SpawnedTileActor> rhs, bool allowNull)
    {
        if (lhs == null)
        {
            return allowNull && rhs == null;
        }
        else if (rhs == null)
        {
            return false;
        }

        if (lhs.Count != rhs.Count)
        {
            return false;
        }

        for (int i = 0; i < lhs.Count; i++)
        {
            if (!EqualSpawnedTileActors(lhs[i], rhs[i], allowNull))
            {
                return false;
            }
        }

        return true;
    }

    public static bool EqualSpawnedTileActors(SpawnedTileActor lhs, SpawnedTileActor rhs, bool allowNull)
    {
        if (lhs == null)
        {
            return allowNull && rhs == null;
        }
        else if (rhs == null)
        {
            return false;
        }

        bool output = true;

        if (CustomDebug.SaveLoadDebugging(CustomDebug.DebuggingType.Verbose)) { Debug.Log("State 0: " + output); }
        output = output && lhs.name.Equals(rhs.name);
        if (CustomDebug.SaveLoadDebugging(CustomDebug.DebuggingType.Verbose)) { Debug.Log("State 1: " + output); }
        output = output && lhs.curHealth == rhs.curHealth;
        if (CustomDebug.SaveLoadDebugging(CustomDebug.DebuggingType.Verbose)) { Debug.Log("State 2: " + output); }
        output = output && lhs.pos.Equals(rhs.pos);
        if (CustomDebug.SaveLoadDebugging(CustomDebug.DebuggingType.Verbose)) { Debug.Log("State 3: " + output); }
        output = output && lhs.type.Equals(rhs.type);
        if (CustomDebug.SaveLoadDebugging(CustomDebug.DebuggingType.Verbose)) { Debug.Log("State 4: " + output); }
        output = output && lhs.damage == rhs.damage;
        if (CustomDebug.SaveLoadDebugging(CustomDebug.DebuggingType.Verbose)) { Debug.Log("State 5: " + output); }
        output = output && lhs.attackRange == rhs.attackRange;
        if (CustomDebug.SaveLoadDebugging(CustomDebug.DebuggingType.Verbose)) { Debug.Log("State 6: " + output); }
        output = output && lhs.isShielded == rhs.isShielded;
        if (CustomDebug.SaveLoadDebugging(CustomDebug.DebuggingType.Verbose)) { Debug.Log("State 7: " + output); }
        output = output && lhs.isPoisoned == rhs.isPoisoned;
        if (CustomDebug.SaveLoadDebugging(CustomDebug.DebuggingType.Verbose)) { Debug.Log("State 8: " + output); }

        return output;
    }

    public static bool EqualSavedCards(List<SavedCard> lhs, List<SavedCard> rhs, bool allowNull)
    {
        if (lhs == null)
        {
            return allowNull && rhs == null;
        }
        else if (rhs == null)
        {
            return false;
        }

        if (lhs.Count != rhs.Count)
        {
            return false;
        }

        for (int i = 0; i < lhs.Count; i++)
        {
            if (!EqualSavedCards(lhs[i], rhs[i], allowNull))
            {
                return false;
            }
        }

        return true;
    }

    public static bool EqualSavedCards(SavedCard lhs, SavedCard rhs, bool allowNull)
    {
        if (lhs == null)
        {
            return allowNull && rhs == null;
        }
        else if (rhs == null)
        {
            return false;
        }

        bool output = true;

        if (CustomDebug.SaveLoadDebugging(CustomDebug.DebuggingType.Verbose)) { Debug.Log("State 0: " + output); }
        output = output && lhs.cardId == rhs.cardId;
        if (CustomDebug.SaveLoadDebugging(CustomDebug.DebuggingType.Verbose)) { Debug.Log("State 1: " + output); }
        output = output && lhs.cardHealth == rhs.cardHealth;
        if (CustomDebug.SaveLoadDebugging(CustomDebug.DebuggingType.Verbose)) { Debug.Log("State 2: " + output); }

        return output;
    }
}

[Serializable]
public class GameStateData
{
#pragma warning disable 0649

    public bool initialized = false;    //set to true to indicate that all of the following variables were given the correct information that is wanted for saving/loading
    public String saveVersion = "1.00"; //an indicator of what iteration of save system is in use; probably won't ever be used, but it is better to have it and never need it than need it and not have it
    public int level = -1;

    //we will assume that the game is only ever saved on the player's turn
    #region Map Info
    public int sectorCount;        //how many directions there are (currently this will always be 4)
    //public int maxVisibleRange;    //the gameboard as it apears to the player
    public int maxActualRange;     //maxVisibleRange plus the hidden range stuff that is used for spawning and such
    public int curTurn;            //the turn counter's current value
    
    //the terrains are stored in a 1D list
    //this will correlate to a spiral starting with the innermost ring and the "northernmost" direction of the map and working its way slowly outward
    //public List<int> terrains = new List<int>();

    //same index order as quadrant map tile list
    /*public List<Terrain> NETerrains = new List<Terrain>();
    public List<Terrain> NWTerrains = new List<Terrain>();
    public List<Terrain> SETerrains = new List<Terrain>();
    public List<Terrain> SWTerrains = new List<Terrain>();*/
    //public List<List<Terrain>> terrains = new List<List<Terrain>>();
    //public Terrain[,] terrainArray;
    public List<SavedLane> lanes = new List<SavedLane>();
    #endregion

    #region Base
    public int baseHP;
    public int baseWeapon;
    //include active base defenses/effects here; TODO
    #endregion

    #region TileActors
    //List<TileActor.ObjType> tileActorTypes = new List<TileActor.ObjType>();
    /*List<int> ta_spawnTurn = new List<int>();       //the turn on which this TileActor did/will spawn; mainly important for enemies which have not yet spawned
    List<Vector2Int> ta_pos = new List<Vector2Int>();     //position of tileActors stored as (direction, range)
    List<int> ta_faction = new List<int>();         //0 for defender, 1 for attacker
    List<int> ta_maxHealth = new List<int>();       //the max possible health for this TileActor; generally the health that the TileActor spawns with
    List<int> ta_curHealth = new List<int>();       //the current health of the TileActor; generally maxHealth - damageTaken
    List<int> ta_damage = new List<int>();          //the standard attack damage of this TileActor
    */

    public List<SpawnedTileActor> spawnedTileActors;
    public List<TileActorManager.RoundSpawnInfo> spawnList;

    //public List<TrapUnit> trapUnits;

    //public List<TileActor> actors;
    /*
    public List<String> ta_name;
    public List<int> ta_maxHealth;       //the max possible health for this TileActor; generally the health that the TileActor spawns with
    public List<int> ta_curHealth;       //the current health of the TileActor; generally maxHealth - damageTaken
    public List<Vector2Int> ta_pos;     //position of tileActors stored as (direction, range)
    public List<TileActor.TileActorType> ta_type;
    public List<int> ta_damage;          //the standard attack damage of this TileActor
    public List<int> ta_attackRange;     //the standard attack range of this TileActor
    */


    //special abilities: TODO
    #endregion

    #region Cards
    public List<int> drawPile = new List<int>();
    public List<int> discardPile = new List<int>();
    public List<SavedCard> hand = new List<SavedCard>();
    #endregion


#pragma warning restore 0649
}

[Serializable]
public class SpawnedTileActor
{
#pragma warning disable 0649

    public string name;
    //public int maxHealth;
    public int curHealth;
    public Vector2Int pos;
    public TileActor.TileActorType type;
    public int damage;
    public int attackRange;
    public bool isShielded;
    public bool isPoisoned;

#pragma warning restore 0649
}

[Serializable]
public class SpawnedTrap
{
#pragma warning disable 0649
    public string name;
    public int curHealth;
    public Vector2Int pos;
    public bool isShielded;
    public bool isPoisoned;
#pragma warning restore 0649
}


[Serializable]
public class SavedLane
{
#pragma warning disable 0649
    public List<int> terrains;
#pragma warning restore 0649
}

[Serializable]
public class SavedCard
{
#pragma warning disable 0649
    public int cardId;
    public int cardHealth;
#pragma warning restore 0649
}

[Serializable]
public class PlayerStats
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


[Serializable]
public class SaveFileList
{
#pragma warning disable 0649

    public List<SaveFileMetaData> files;

#pragma warning restore 0649
}

[Serializable]
public class SaveFileMetaData
{
#pragma warning disable 0649

    public string fileName;
    public string version;

#pragma warning restore 0649
}