using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Random = UnityEngine.Random;


public class Sudoku : MonoBehaviour
{
    /* REFERENCES & SETTING */
    public Cell prefabCell;
    public Canvas canvas;
    public Text feedback;                           // Comunicacion con el usuario
    public float stepDuration = 0.05f;
    [Range(1, 82)] public int difficulty = 40;

    /* PRIVATE VARIABLES */
    private Matrix<Cell> _board;                    // Matriz de celdas a usar como tablero
    private Matrix<int> _mtx;                       // Matriz de numeros del board
    private int _boardSide;                         // Guarda la cantidad de casillas por lado del board
    private int _side;                              // Guarda 3 (tamaño del recuadro a controlar)
    private int _quad;                              // Guarda 3 (cantidad de recuadros a controlar)
    private string _memory = "";                    // Guarda el el debugueo de la memoria

    private List<int> _nums = new List<int>();      // Guardar los numeros posibles a usar
    //string canSolve = "";
    //List<int> nums = new List<int>();

    private void Start()
    {
        DebugMemory();

        _side = 3;// VER
        _quad = 3;// VER
        _boardSide = _side * _quad;// VER  

        CreateEmptyBoard();
        ClearBoard();
    }

    private void DebugMemory() // DONE
    {
        long mem = System.GC.GetTotalMemory(true);
        _memory = string.Format("MEM: {0:f2}MB", mem / (1024f * 1024f));

        feedback.text = _memory;
    }
    private void CreateEmptyBoard() // DONE
    {
        float spacing = 68f;
        float startX = -spacing * 4f;
        float startY = spacing * 4f;

        _board = new Matrix<Cell>(_boardSide, _boardSide);

        for (int x = 0; x < _board.Width; x++)
        {
            for (int y = 0; y < _board.Height; y++)
            {
                var cell = _board[x, y] = Instantiate(prefabCell, canvas.transform, false);
                cell.transform.localPosition = new Vector3(startX + x * spacing, startY - y * spacing, 0);
            }
        }
    }
    private void ClearBoard() // DONE
    {
        _mtx = new Matrix<int>(_boardSide, _boardSide);

        foreach (var cell in _board)
        {
            cell.number = 0;
            cell.locked = cell.invalid = false;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) || Input.GetMouseButtonDown(1))
            SolvedSudoku();
        else if (Input.GetKeyDown(KeyCode.C) || Input.GetMouseButtonDown(0))
            LoadFromTest(10);
            //CreateSudoku();
    }
    private void SolvedSudoku() // P: Modificar lo necesario para que funcione.
    {
        StopAllCoroutines();




        //nums = new List<int>();
        //var solution = new List<Matrix<int>>();
        // watchdog = 100000;
        // var result = false;
        //????


        long mem = System.GC.GetTotalMemory(true);
        _memory = string.Format("MEM: {0:f2}MB", mem / (1024f * 1024f));
        //canSolve = result ? " VALID" : " INVALID";
        //???
    }
    private void CreateSudoku()
    {
        StopAllCoroutines();

        GenerateFromEmpty();

        //nums = new List<int>();
        //List<Matrix<int>> l = new List<Matrix<int>>();
        //watchdog = 100000;


        //GenerateValidLine(_boardContent, 0, 0);

        var result = false;

        //_boardContent = l[0].Clone(); // MMM TA RARO ESO

        //LockRandomCells();
        //ClearUnlocked(_mtx);


        DebugMemory(); // VER el feedback

        //canSolve = result ? " VALID" : " INVALID";

        //feedback.text = "Pasos: " + l.Count + "/" + l.Count + " - " + _memory + " - " + canSolve;
    }

    private void GenerateFromEmpty() // TEST
    {
        ClearBoard();

        for (int i = 1; i <= _quad; i++)
        {
            PlaceNumber(i);
        }

        //RefillNums();

        //// Genero primera linea
        //for (int i = 0; i < _boardSide; i++)
        //{
        //    _mtx[i, 0] = _nums[i];
        //    Debug.Log("Cargado en " + i + ";0 = " + _nums[i]);
        //}

        //// Genero el resto de las lineas de abajo hacia arriba
        //for (int j = 1; j < _boardSide; j++)
        //{
        //    RefillNums();
        //    for (int i = 0; i < _boardSide; i++)
        //    {
        //        if (!PlaceFromNums(i, j))
        //        {
        //            Debug.Log("NO PUDE PONER VALOR EN " + i + ";" + j);
        //            break;
        //        }
        //    }
        //}

        LoadBoardFromMtx();
    }
    private bool PlaceNumber(int value)
    {
        List<int> v = FillRamdoms();
        List<int> w = new List<int>();

        for (int i = 0; i < _boardSide; i++)
        {
            for (int j = 0; j < v.Count; j++)
            {
                var q = GetQuad(i, v[j]);
                if (w.Contains(q)) continue;

                if (_mtx[i, v[j]] == 0) _mtx[i, v[j]] = value;
                else continue;

                v.RemoveAt(j);
                w.Add(q);
                break;
            }
        }

        return v.Count == 0;
    }
    private List<int> FillRamdoms() // TEST
    {
        List<int> bag = new List<int>();

        for (int i = 0; i < _boardSide; i++)
            bag.Insert(Random.Range(0, i), i);

        return bag;
    }
    private int GetQuad(int x, int y) //TEST
    {
        int qX = (int)(x / _side);
        int qY = (int)(y / _side);

        return qX + qY * _quad;
    }


    private void RefillNums() // TEST
    {
        _nums.Clear();

        for (int i = 1; i <= _boardSide; i++)
            _nums.Insert(Random.Range(0, i), i);
    }
    private bool PlaceFromNums(int x, int y) // TEST
    {
        foreach (var value in _nums)
        {
            if (CheckDown(value, x, y))
            {
                _mtx[x, y] = value;
                Debug.Log("Cargado en " + x + ";" + y + " = " + value);
                _nums.Remove(value);
                return true;
            }
        }

        return false;
    }
    private bool CheckDown(int value, int x, int y) // TEST
    {
        // REVISO LA COLUMNA
        for (int i = 0; i < y; i++)
        {
            if (_mtx[x, i] == value)
            {
                Debug.Log("CD: " + x + ";" + y + " = " + value + " : Coincidencia en " + x + ";" + i);
                return false;          // Si uno es igual corto la validaciones
            }
        }

        // REVISO LA FILA
        for (int i = 0; i < _boardSide; i++)
        {
            if (i == x) continue;                           // Si soy yo me salto
            if (_mtx[i, y] == value)
            {
                Debug.Log("CD: " + x + ";" + y + " = " + value + " : Coincidencia en " + i + ";" + y);
                return false;          // Si uno es igual corto la validaciones
            }
        }

        // REVISO EL CUADRANTE
        int auxX = (int)(x / _side);                        // Mi X inicio de cuadrante
        int auxY = (int)(y / _side);                        // Mi Y inicio de cuadrante

        for (int j = 0; j < _side; j++)
        {
            var v = j + auxY * _side;
            if (v == y) return true;             // Si es mi fila corto
            for (int i = 0; i < _side; i++)
            {
                var u = i + auxX * _side;
                if (u == x) continue;                    // Si es mi columna me salto
                if (_mtx[u, v] == value)
                {
                    Debug.Log("CD: " + x + ";" + y + " = " + value + " : Coincidencia en " + u + ";" + v);
                    return false;      // Si uno es igual corto la validaciones
                }
            }
        }

        return true;
    }
    private void LoadBoardFromMtx(bool locked = false) // TEST
    {
        for (int y = 0; y < _boardSide; y++)
        {
            for (int x = 0; x < _boardSide; x++)
            {
                _board[x, y].number = _mtx[x, y];
                _board[x, y].locked = (locked && _mtx[x, y] != 0);
            }
        }
    }

    private void LockRandomCells() // DONE
    {
        List<Vector2> posibles = new List<Vector2>();

        // Carga las cordenadas de las celdas debloqueadas
        for (int y = 0; y < _board.Height; y++)
        {
            for (int x = 0; x < _board.Width; x++)
            {
                if (!_board[x, y].locked)
                    posibles.Add(new Vector2(x, y));
            }
        }

        // Bloquea celdas aleatoriamente
        for (int i = 0; i < 82 - difficulty; i++)
        {
            int rdm = Random.Range(0, posibles.Count);
            _board[(int)posibles[rdm].x, (int)posibles[rdm].y].locked = true;
            posibles.RemoveAt(rdm);
        }
    }
    private void ClearUnlocked(Matrix<int> content) // DONE
    {
        for (int y = 0; y < _board.Height; y++)
        {
            for (int x = 0; x < _board.Width; x++)
            {
                if (!_board[x, y].locked)
                    content[x, y] = Cell.EMPTY;
            }
        }
    }

    private bool CanPlaceValue(int value, int x, int y) // TEST
    {
        // VALIDACIONES
        if (value == 0) return true;                        // Si es 0 no chequeo y doy OK

        // REVISO LA FILA
        for (int i = 0; i < _mtx.Width; i++)
        {
            if (i == x) continue;                           // Si soy yo me salto
            if (_mtx[i, y] == value) return false;          // Si uno es igual corto la validaciones
        }

        // REVISO LA COLUMNA
        for (int i = 0; i < _mtx.Height; i++)
        {
            if (i == y) continue;                           // Si soy yo me salto
            if (_mtx[x, i] == value) return false;          // Si uno es igual corto la validaciones
        }

        // REVISO EL CUADRANTE
        int auxX = (int)(x / _side);                        // Mi X inicio de cuadrante
        int auxY = (int)(y / _side);                        // Mi Y inicio de cuadrante
        for (int i = 0; i < _side; i++)
        {
            var u = i + auxX * _side;
            if (u == x) continue;                           // Si es mi columna me salto
            for (int j = 0; j < _side; j++)
            {
                var v = j + auxY * _side;
                if (v == y) continue;                       // Si es mi fila me salto
                if (_mtx[u, v] == value) return false;      // Si uno es igual corto la validaciones
            }
        }

        return true;
    }




    //IMPLEMENTAR
    int watchdog = 0;
    bool RecuSolve(Matrix<int> matrixParent, int x, int y, int protectMaxDepth, List<Matrix<int>> solution)
    {
        return false;
    }

    //IMPLEMENTAR - punto 3
    IEnumerator ShowSequence(List<Matrix<int>> seq)
    {
        yield return new WaitForSeconds(0);
    }

    private void TranslateSpecific(int value, int x, int y)
    {
        _board[x, y].number = value;
    }
    private void TranslateRange(int x0, int y0, int xf, int yf)
    {
        for (int x = x0; x < xf; x++)
        {
            for (int y = y0; y < yf; y++)
            {
                _board[x, y].number = _mtx[x, y];
            }
        }
    }

    void LoadFromTest(int i)
    {
        _mtx = new Matrix<int>(Tests.validBoards[i]);
        LoadBoardFromMtx(true);
    }
}
