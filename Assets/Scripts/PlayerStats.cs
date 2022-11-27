using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private Text _playerText;
    public void SetData(int index, string pairKey, int pairValue)
    {
        _playerText.text = $"{index}. {pairKey}\t\t{pairValue}";
    }
}