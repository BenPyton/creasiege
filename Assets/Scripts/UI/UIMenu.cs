using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;
using Newtonsoft.Json;

[System.Serializable]
public struct LevelInfo
{
    public string name;
    public int type;
	public string path;
	public string description;
}

public class UIMenu : MonoBehaviour
{
    [SerializeField] GameObject titleScreen;
	[SerializeField] GameObject levelSelect;
	[SerializeField] Button playButton;
	[SerializeField] Button backButton;
	[SerializeField] Button quitButton;
	[SerializeField] Button prefabLevelButton;
	[SerializeField] Transform levelButtonContainer;
	[SerializeField] Text levelNameText;
	[SerializeField] Text levelTypeText;
	[SerializeField] Text levelDescriptionText;
	[SerializeField] Button startLevelButton;

	LevelInfo selectedLevel;

    // Start is called before the first frame update
    void Start()
	{
        List<BundleFile> levels = AssetBundleManager.ListBundles("levels");
		// foreach(BundleFile file in levels)
		// {
		//     Button levelButton = Instantiate(prefabLevelButton, levelButtonContainer).GetComponent<Button>();
		//     levelButton.GetComponentInChildren<Text>().text = file.name;

		//     levelButton.onClick.AddListener(() => 
		//     {
		//         SceneManager.LoadScene("Game");

		//         AssetBundle level = AssetBundleManager.LoadBundle(file.fullPath);
		//         if (level.isStreamedSceneAssetBundle)
		//         {
		//             foreach (string scenePath in level.GetAllScenePaths())
		//             {
		//                 int firstIndex = scenePath.LastIndexOf('/');
		//                 int lastIndex = scenePath.LastIndexOf('.');
		//                 string levelName = scenePath.Substring(firstIndex + 1, lastIndex - firstIndex - 1);
		//                 //Debug.Log("Scene path: " + scenePath + " | Scene name: " + levelName);
		//                 SceneManager.LoadScene(levelName, LoadSceneMode.Additive);
		//             }
		//         }
		//     });
		// }
		string path = Path.Combine(Application.streamingAssetsPath, "levels.json");
        JsonSerializer serializer = new JsonSerializer();
		using (FileStream file = new FileStream(path, FileMode.Open))
		using (StreamReader reader = new StreamReader(file))
		using (JsonReader json = new JsonTextReader(reader))
		{
            JSONLevelList levelInfos = JsonUtility.FromJson<JSONLevelList>(reader.ReadToEnd());
			//List<LevelInfo> levelInfos = serializer.Deserialize<List<LevelInfo>>(json);
			for(int i = 0; i < levelInfos.levels.Count; i++)//LevelInfo info in levelInfos)
			{
				LevelInfo info = levelInfos.levels[i];
				Debug.Log("Level name: " + info.name + " | type: " + (MissionType)info.type);
				Button b = Instantiate(prefabLevelButton, levelButtonContainer).GetComponent<Button>();
				b.GetComponentInChildren<Text>().text = (i+1).ToString("D2") + " - " + info.name;

				b.onClick.AddListener(() => {
					levelNameText.text = info.name;
					levelTypeText.text = ((MissionType)info.type).ToString();
					levelDescriptionText.text = info.description;
					selectedLevel = info;
				});
			}
		}

		startLevelButton.onClick.AddListener(() => {
			if(selectedLevel.path == "")
				return;

			DataManager.instance.missionType = (MissionType)selectedLevel.type;
			DataManager.instance.missionEnded = false;

			SceneManager.LoadScene("Game");
			AssetBundle level = AssetBundleManager.LoadBundle(Path.Combine("levels", selectedLevel.path));
			if (level.isStreamedSceneAssetBundle)
			{
				foreach (string scenePath in level.GetAllScenePaths())
				{
					int firstIndex = scenePath.LastIndexOf('/');
					int lastIndex = scenePath.LastIndexOf('.');
					string levelName = scenePath.Substring(firstIndex + 1, lastIndex - firstIndex - 1);
					SceneManager.LoadScene(levelName, LoadSceneMode.Additive);
				}
			}
		});

		playButton.onClick.AddListener(() => {
			titleScreen.SetActive(false);
			levelSelect.SetActive(true);
        });
		backButton.onClick.AddListener(() =>
		{
			titleScreen.SetActive(true);
			levelSelect.SetActive(false);
		});
        quitButton.onClick.AddListener(() => Application.Quit());

		// levelButton.onClick.AddListener(() => {

		// 	SceneManager.LoadScene("Game");

        //     AssetBundle level = AssetBundleManager.LoadBundle("levels/level01");
        //     if(level.isStreamedSceneAssetBundle)
        //     {
        //         //level.LoadAllAssets();
        //         string[] scenePaths = level.GetAllScenePaths();
        //         foreach(string scenePath in scenePaths)
        //         {
        //             int firstIndex = scenePath.LastIndexOf('/');
		// 			int lastIndex = scenePath.LastIndexOf('.');
        //             string levelName = scenePath.Substring(firstIndex + 1, lastIndex - firstIndex - 1);
        //             Debug.Log("Scene path: " + scenePath + " | Scene name: " + levelName);
        //             SceneManager.LoadScene(levelName, LoadSceneMode.Additive);
        //         }
        //     }
        // });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

[System.Serializable]
public struct JSONLevelList
{
    public List<LevelInfo> levels;
}
