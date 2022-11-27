using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenu : MonoBehaviour
{
   [SerializeField] private Button _playButton; 
   [SerializeField] private Button _ratingButton;
   [SerializeField] private TMP_Text _loginLabel; 
   
   private void Start()
   {
      _playButton.onClick.AddListener(StartGame);
      _ratingButton.onClick.AddListener(() => SceneManager.LoadScene("Rating"));
      
      Debug.Log("Load data: " + UserProfile.LoadData());
   }

   private void StartGame()
   {
      if (_loginLabel.text.Length < 3 || _loginLabel.text.Contains(" "))
         return;

      UserProfile.Login = _loginLabel.text;
      SceneManager.LoadScene(sceneBuildIndex: 1);
   }
}
