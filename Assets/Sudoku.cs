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
    [Range(0, 1)] public float difficulty = 0.5f;

    /* PRIVATE VARIABLES */
    private Matrix<Cell> _board;                    // Matriz de celdas a usar como tablero
    private Matrix<int> _mtx;                       // Matriz de numeros del board
    private int _boardSide;                         // Guarda la cantidad de casillas por lado del board
    private int _side;                              // Guarda 3 (tamaño del recuadro a controlar)
    private int _quad;                              // Guarda 3 (cantidad de recuadros a controlar)
    private string _memory = "";                    // Guarda el el debugueo de la memoria
    private List<int[]> _path;                      // Guarda el camino de resolucion a reproducir


    private void Start()
    {
        DebugMemory();

        _side = 3;// VER
        _quad = 3;// VER
        _boardSide = _side * _quad;

        _path = new List<int[]>();

        CreateEmptyBoard();
        ClearBoard();
    }

    private void DebugMemory(string msg = "") // DONE: Comunica el uso de memoria y algun mensaje extra
    {
        long mem = System.GC.GetTotalMemory(true);
        _memory = string.Format("MEM: {0:f2}MB", mem / (1024f * 1024f));

        feedback.text = _memory + msg;
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
            CreateSudoku();
        else if (Input.GetKeyDown(KeyCode.T))
            LoadFromValidTest(Random.Range(0, 25));
    }
    private void SolvedSudoku() // DONE: Resuelve el sudoku
    {
        StopAllCoroutines();

        if (RecuSolve(0, 0)) StartCoroutine("ShowPath");
        else DebugMemory("Recusolving Failed");
    }
    private bool RecuSolve(int x, int y) // DONE: Resuelve con recursion y backtracking
    {
        if (x == _boardSide) return true;

        int nx = x;
        int ny = 0;
        if (y == _boardSide - 1) nx++;
        else ny = y + 1;

        if (_board[x, y].locked)
            return RecuSolve(nx, ny);
        else
        {
            for (int i = 1; i <= _boardSide; i++)
            {
                if (CanPlaceValue(i, x, y))
                {
                    _mtx[x, y] = i;
                    _path.Add(new int[] { x, y, i });

                    if (RecuSolve(nx, ny))
                        return true;
                    else
                    {
                        _path.RemoveAt(_path.Count - 1);
                        _mtx[x, y] = 0;
                    }
                }
            }
        }

        return false;
    }
    private bool CanPlaceValue(int value, int x, int y) // DONE: Revisa si cumple el valor en el lugar
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
    private IEnumerator ShowPath() // TEST: Carga paso a paso el camino de resolucion
    {
        for (int i = 0; i < _path.Count; i++)
        {
            _board[_path[i][0], _path[i][1]].number = _path[i][2];
            yield return new WaitForSeconds(stepDuration);
        }

        DebugMemory("Sudoku Solved");
    }

    private void CreateSudoku() // TEST: Crea un nuevo sudoku
    {
        StopAllCoroutines();
        ClearBoard();

        // Genero primera linea
        List<int> nums = FillRamdoms();
        for (int i = 0; i < _boardSide; i++)
        {
            _mtx[i, 0] = nums[i];
        }

        // Genero primera columna
        nums = FillRamdoms();
        nums.Remove(_mtx[0, 0]);
        for (int j = 1; j < _boardSide; j++)
        {
            for (int i = 0; i < nums.Count; i++)
            {
                if (j < _quad && !CanPlaceValue(0, j, nums[i]))
                    continue;
                _mtx[0, j] = nums[i];
                nums.RemoveAt(i);
            }
        }

        // Completo el resto de las celdas
        if (RecuSolve(1, 1)) DebugMemory();
        else DebugMemory("Creation Failed");

        SetDifficulty();
        LoadBoardFromMtx();


        //LockRandomCells();
        //ClearUnlocked(_mtx);


        DebugMemory(); // VER el feedback

        //canSolve = result ? " VALID" : " INVALID";

        //feedback.text = "Pasos: " + l.Count + "/" + l.Count + " - " + _memory + " - " + canSolve;
    }
    private List<int> FillRamdoms() // TEST: Devuelve una lista con los numeros desordenados
    {
        List<int> bag = new List<int>();

        for (int i = 1; i <= _boardSide; i++)
            bag.Insert(Random.Range(0, i), i);

        return bag;
    }
    private void LoadBoardFromMtx() // DONE: Carga la matriz numérica en el board
    {
        for (int y = 0; y < _boardSide; y++)
        {
            for (int x = 0; x < _boardSide; x++)
            {
                _board[x, y].number = _mtx[x, y];
                _board[x, y].locked = _mtx[x, y] != Cell.EMPTY;
            }
        }
    }
    private void SetDifficulty() // TEST: Inicia el tablero en la dificultad seleccionada
    {
        List<int[]> posibles = new List<int[]>();

        // Carga las cordenadas de las celdas debloqueadas
        for (int y = 0; y < _board.Height; y++)
            for (int x = 0; x < _board.Width; x++)
                posibles.Add(new int[] { x, y });

        // Salta celdas aleatoriamente segun la dificultad
        int locks = Mathf.RoundToInt(_board.Capacity * difficulty);
        for (int i = 0; i < locks; i++)
        {
            int rdm = Random.Range(0, posibles.Count);
            posibles.RemoveAt(rdm);
        }

        // Limpia los casilleros y guarda el path
        _path.Clear();
        for (int i = 0; i < posibles.Count; i++)
        {
            int x = posibles[i][0];
            int y = posibles[i][1];
            _path.Add(new int[] { x, y, _mtx[x, y] });
            _mtx[x, y] = Cell.EMPTY;
        }
    }
    
    private void LoadFromValidTest(int i) // DONE: Carga el tablero indicado
    {
        _mtx = new Matrix<int>(Tests.validBoards[i]);
        LoadBoardFromMtx();
    }
}
