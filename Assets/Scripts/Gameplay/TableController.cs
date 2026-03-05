using System;
using UnityEngine;

public class TableController : MonoBehaviour
{
    [SerializeField]
    private Transform _tablePivot;

    [SerializeField]
    private Table _table;



    public Table Table
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
        string path;

        switch(table.type)
        {
            case ETableType.Players_6:
                path = "Tables/Table-6";
                break;

            case ETableType.Players_8:
                path = "Tables/Table-8";
                break;

            case ETableType.Players_10:
                path = "Tables/Table-10";
                break;

            case ETableType.Players_12:
                path = "Tables/Table-12";
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(table.type), table.type, "Unsupported table type.");
        }

        Table prefab = Resources.Load<Table>(path);

        Table = Instantiate(prefab, Vector3.zero, Quaternion.identity, _tablePivot);
    }
}
