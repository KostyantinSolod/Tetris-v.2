using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayerRating : MonoBehaviour
{
    [SerializeField] private PlayerStats _playerStats;
    // Start is called before the first frame update
    void Start()
    {
        var rating = UserProfile.LoadData();
        var ordererRating = rating.OrderByDescending(pair => pair.Value);
        var index = 1;
        foreach (var pair in ordererRating)
        {
            var player = Instantiate(_playerStats, transform);
            player.SetData(index, pair.Key, pair.Value);
            index++;
        }
    }
}