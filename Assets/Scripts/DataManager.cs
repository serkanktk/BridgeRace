using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;

    public int buttonValue;

    public int totalWinnings = 0;


    
    // [Header("Text")]
    public TextMeshProUGUI totalWinningsText;

    private void Start()
    {
        totalWinningsText = GameObject.Find("Canvas/TotalWinnings")?.GetComponent<TextMeshProUGUI>();
        totalWinnings = PlayerPrefs.GetInt("totalWinnings");
        if (totalWinningsText != null)
        {
            totalWinningsText.text = "Total Winnings: " + totalWinnings.ToString();
        }
    }


    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "SampleScene" && totalWinningsText == null)
        {
            totalWinningsText = GameObject.Find("Canvas/TotalWinnings")?.GetComponent<TextMeshProUGUI>();
            if (totalWinningsText != null)
            {
                totalWinningsText.text = "Total Winnings: " + totalWinnings.ToString();
            }
        }
    }



    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.SetInt("totalWinnings", totalWinnings);
        PlayerPrefs.Save();
    }

}
