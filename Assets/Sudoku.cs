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
    public bool rawStep = false;                    // Define si muestra todos los pasos o solo los correctos
    public float stepDuration = 0.05f;              // Velocidad de los pasos que muestra
    [Range(0, 1)] public float difficulty = 0.5f;   // Define la dificultad en base a la cantidad total de celdas
    public int side = 3;                            // Guarda el tamaño del recuadro a controlar

    /* PRIVATE VARIABLES */
    private Matrix<Cell> _board;                    // Matriz de celdas a usar como tablero
    private Matrix<int> _mtx;                       // Matriz de numeros del board
    private int _boardSide;                         // Guarda la cantidad de casillas por lado del board
    private string _memory = "";                    // Guarda el el debugueo de la memoria
    private List<int[]> _path;                      // Guarda el camino de resolucion a reproducir
    private bool _input;                            // Bloquea el input mientras resuelve
    private int _total;                             // cantidad total de numeros probados

    private void Start()
    {
        DebugMemory();

        _boardSide = side * side;

        _path = new List<int[]>();
        _input = true;

        CreateEmptyBoard();
        ClearBoard();
    }

    private void DebugMemory(string msg = "") // DONE: Comunica el uso de memoria y algun mensaje extra
    {
        long mem = System.GC.GetTotalMemory(true);
        _memory = string.Format("MEM: {0:f2}MB - ", mem / (1024f * 1024f));

        feedback.text = _memory + msg;
    }
    private void CreateEmptyBoard() // DONE: Crea el tablero
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
    private void ClearBoard() // FIX: Pone el tablero en cero y desbloquea las celdas
    {
        _mtx = new Matrix<int>(_boardSide, _boardSide);

        for (int y = 0; y < _boardSide; y++)
        {
            for (int x = 0; x < _boardSide; x++)
            {
                _board[x, y].number = Cell.EMPTY;
                _board[x, y].locked = false;
                _board[x, y].focused = false;
            }
        }

        // NO ANDA EL FOREACH
        //foreach (var cell in _board)
        //{
        //    Debug.Log("entre");
        //    cell.number = Cell.EMPTY;
        //    cell.focused = cell.locked = cell.invalid = false;
        //}
    }

    private void Update()
    {
        if (!_input) return;

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
        _path.Clear();
        _input = false;
        _total = 0;
        DebugMemory("Resolving...");

        if (RecuSolve(0, 0)) StartCoroutine("ShowPath");
        else
        {
            DebugMemory("Recusolving Failed");
            _input = true;
        }
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
                _total++;
                if (CanPlaceValue(i, x, y))
                {
                    _mtx[x, y] = i;
                    var step = new int[] { x, y, i };
                    _path.Add(step);

                    if (RecuSolve(nx, ny))
                        return true;
                    else
                    {
                        if (rawStep) _path.Add(new int[] { x, y, 0 });
                        else _path.Remove(step);
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
        int auxX = (int)(x / side);                        // Mi X inicio de cuadrante
        int auxY = (int)(y / side);                        // Mi Y inicio de cuadrante
        for (int i = 0; i < side; i++)
        {
            var u = i + auxX * side;
            if (u == x) continue;                           // Si es mi columna me salto
            for (int j = 0; j < side; j++)
            {
                var v = j + auxY * side;
                if (v == y) continue;                       // Si es mi fila me salto
                if (_mtx[u, v] == value) return false;      // Si uno es igual corto la validaciones
            }
        }

        return true;
    }
    private IEnumerator ShowPath() // DONE: Carga paso a paso el camino de resolucion
    {
        for (int i = 0; i < _path.Count; i++)
        {
            int x = _path[i][0];
            int y = _path[i][1];
            _board[x, y].focused = true;
            _board[x, y].number = _path[i][2];
            yield return new WaitForSeconds(stepDuration);
            _board[x, y].focused = false;
        }

        DebugMemory("Solved in " + _total + " steps");
        _input = true;
    }

    private void CreateSudoku() // DONE: Crea un nuevo sudoku
    {
        StopAllCoroutines();
        ClearBoard();
        _path.Clear();
        _input = false;
        _total = 0;
        DebugMemory("Creating...");

        // Genero primera linea
        List<int> nums = FillRamdoms();
        for (int i = 0; i < _boardSide; i++)
        {
            _total++;
            _mtx[i, 0] = nums[i];
        }

        // Genero primera columna
        nums = FillRamdoms();
        nums.Remove(_mtx[0, 0]);
        for (int j = 1; j < _boardSide; j++)
        {
            for (int i = 0; i < nums.Count; i++)
            {
                _total++;
                if (j < side && !CanPlaceValue(nums[i], 0, j))
                    continue;
                _mtx[0, j] = nums[i];
                nums.RemoveAt(i);
                break;
            }
        }

        // Completo el resto de las celdas
        if (RecuSolve(1, 1)) DebugMemory();
        else
        {
            DebugMemory("Creation Failed");
            _input = true;
            return;
        }

        SetDifficulty();
        LoadBoardFromMtx();
        DebugMemory("Created in " + _total + " steps");
        _input = true;
    }
    private List<int> FillRamdoms() // DONE: Devuelve una lista con los numeros desordenados
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
    private void SetDifficulty() // DONE: Inicia el tablero en la dificultad seleccionada
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

        // Limpia los casilleros y guarda el path por si quiero mostrar la resolucion original
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
        StopAllCoroutines();
        ClearBoard();

        _mtx = new Matrix<int>(Tests.validBoards[i]);
        LoadBoardFromMtx();
    }

    private void CheckValidity(int x, int y) //DONE: Revisa validez de una celda
    {
        _board[x, y].invalid = !CanPlaceValue(_board[x, y].number, x, y);
    }
}
