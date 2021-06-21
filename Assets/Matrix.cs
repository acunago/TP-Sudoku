using System.Collections;
using System.Collections.Generic;

public class Matrix<T> : IEnumerable<T>
{
    private T[] _data;

    public Matrix(int width, int height) // DONE: Contructor
    {
        Width = width;
        Height = height;
        Capacity = width * height;
        _data = new T[Capacity];
    }
    public Matrix(T[,] copyFrom) // DONE: Crea una version de Matrix a partir de una matriz básica de C#
    {
        Width = copyFrom.GetLength(0);
        Height = copyFrom.GetLength(1);
        Capacity = Width * Height;
        _data = new T[Capacity];

        for (int j = 0; j < Height; j++)
        {
            for (int i = 0; i < Width; i++)
            {
                this[i, j] = copyFrom[i, j];
            }
        }
    }

    public Matrix<T> Clone() // TEST
    {
        Matrix<T> aux = new Matrix<T>(Width, Height);
        aux.Capacity = Capacity;
        aux._data = _data;

        return aux;
    }

    public void Clear() // DONE: Limpia el contenido de la matriz
    {
        for (int i = 0; i < Capacity; i++)
        {
            _data[i] = default;
        }
    }

    public void SetRangeTo(int x0, int y0, int x1, int y1, T item) // TEST: Iguala todo el rango pasado por parámetro a item
    {
        for (int i = x0; i < x1; i++)
        {
            for (int z = y0; z < y1; z++)
            {

                _data[i * z + z] = item;
            }
        }
    }

    public List<T> GetRange(int x0, int y0, int x1, int y1)  // TEST: Todos los parametros son INCLUYENTES
    {
        List<T> l = new List<T>();

        for (int i = x0; i <= x1; i++)
        {
            for (int z = y0; z <= y1; z++)
            {
                l.Add(_data[i * z + z]);
            }
        }

        return l;
    }

    public T this[int x, int y]  // DONE: Para poder igualar valores en la matrix a algo
    {
        get
        {
            return _data[x + (y * Width)];
        }
        set
        {
            _data[x + (y * Width)] = value;
        }
    }

    public int Width { get; private set; } // DONE
    public int Height { get; private set; } // DONE
    public int Capacity { get; private set; } // DONE

    public IEnumerator<T> GetEnumerator() // TEST
    {
        var current = _data[0];
        int a = 0;
        while (a < _data.Length - 1)
        {
            a++;
            yield return current;

            current = _data[a];
        }
        yield return current;
    }
    IEnumerator IEnumerable.GetEnumerator() // DONE
    {
        return GetEnumerator();
    }
}
