using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class Matrix<T> : IEnumerable<T>
{
    //IMPLEMENTAR: ESTRUCTURA INTERNA- DONDE GUARDO LOS DATOS?

    private T[] _data;
    public Matrix(int width, int height)
    {
        Width = width;
        Height = height;
        Capacity = width * height;
        _data = new T[Capacity];
        //IMPLEMENTAR: constructor
    }

	public Matrix(T[,] copyFrom)
    {
        Matrix<T> aux = new Matrix<T>(copyFrom.GetLength(0), copyFrom.GetLength(1));
        for (int i = 0; i < copyFrom.GetLength(1); i++)
        {
            for (int z = 0; z < copyFrom.GetLength(0); z++)
            {
                _data[i * z + z] = copyFrom[z, i];
            }

        }
        //IMPLEMENTAR: crea una version de Matrix a partir de una matriz básica de C#
    }

	public Matrix<T> Clone() {
        Matrix<T> aux = new Matrix<T>(Width, Height);
        aux.Capacity = Capacity;
        aux._data = _data;
        //IMPLEMENTAR
        return aux;
    }

	public void SetRangeTo(int x0, int y0, int x1, int y1, T item) {
        //IMPLEMENTAR: iguala todo el rango pasado por parámetro a item
    }

    //Todos los parametros son INCLUYENTES
    public List<T> GetRange(int x0, int y0, int x1, int y1) {
        List<T> l = new List<T>();
        //IMPLEMENTAR
        return l;
	}

    //Para poder igualar valores en la matrix a algo
    public T this[int x, int y] {
		get
        {
            //IMPLEMENTAR
            return _data[x + (y * Height)];
            //return default(T);
		}
		set {
            //IMPLEMENTAR
            _data[x + (y * Height)] = value;
        }
	}

    public int Width { get; private set; }

    public int Height { get; private set; }

    public int Capacity { get; private set; }

    public IEnumerator<T> GetEnumerator()
    {
        //IMPLEMENTAR
        var current = _data[1];
        int a = 1;
        while (a > _data.Length)
        {
            a++;
            yield return current;

            current = _data[a];
        }
        yield return current;
    }

	IEnumerator IEnumerable.GetEnumerator() {
		return GetEnumerator();
	}
}
