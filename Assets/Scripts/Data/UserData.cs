using System;
using UnityEngine;
using michitai;

[Serializable]
public class UserData
{
    [SerializeField]
    private string _playerToken = null;



    public string PlayerToken
    {
        get
        {
            return _playerToken;
        }

        private set
        {
            _playerToken = value;
        }
    }

    public bool IsValid
    {
        get
        {
            return !string.IsNullOrEmpty(PlayerToken);
        }
    }



    public UserData() : this(null)
    {

    }

    public UserData(string playerToken)
    {
        PlayerToken = playerToken;
    }
}
