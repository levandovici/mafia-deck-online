using UnityEngine;

public class Table : MonoBehaviour
{
    [SerializeField]
    private Chair[] _chairs;

    private TableData _table;



    public TableData Data
    {
        get
        {
            return _table;
        }

        private set
        {
            _table = value;
        }
    }



    public void Setup(TableData table)
    {
        Data = table;

        for(int i = 0; i < _chairs.Length; i++)
        {
            _chairs[i].Setup(table.chairs[i]);
        }
    }
}
