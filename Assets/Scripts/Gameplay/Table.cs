using UnityEngine;

public class Table : MonoBehaviour
{
    [SerializeField]
    private Chair[] _chairs;

    private TableData _table;



    public Chair[] Chairs => _chairs;

    public Chair GetChair(int index)
    {
        if (_chairs != null && index >= 0 && index < _chairs.Length)
            return _chairs[index];
        return null;
    }

    public CameraPoint[] CameraPoints
    {
        get
        {
            CameraPoint[] points = new CameraPoint[_chairs.Length];

            for(int i = 0; i < _chairs.Length; i++)
            {
                points[i] = _chairs[i].CameraPoint;
            }

            return points;
        }
    }

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



    [ContextMenu("Sit Down All")]
    public void SitDownAll()
    {
        for(int i = 0; i < _chairs.Length; i++)
        {
            _chairs[i].SitDown();
        }
    }

    [ContextMenu("Stand Up All")]
    public void StandUpAll()
    {
        for (int i = 0; i < _chairs.Length; i++)
        {
            _chairs[i].StandUp();
        }
    }
}
