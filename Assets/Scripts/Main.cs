using System;
using System.Linq;
using UnityEngine;

public class Main : MonoBehaviour
{
    public Material[] colours; // Цвета

    int[,] bigMatrix;      // вся матрица из text.txt
    int[,] smallMatrix;    // матрица 3 на 3, которую будут отображать кубы
    Vector2Int coord;      // (x,y) верхний левый элемент маленькой матрицы

    Renderer[,] cubeR = new Renderer[3, 3]; 
    System.Random randint = new System.Random();

    void Start()
    {
        CubeRenderers();           // по позициям (без зависимости от имён)
        bigMatrix = ReadBigMatrix("TextFiles/text"); // читаем файл

        coord = RandomNumbers(bigMatrix);
        smallMatrix = SmallMatrix(bigMatrix, coord);

        ApplySmallMatrixToCubes();
        LogState();
    }

    // Привязываем функции к клавишам
    void Update() 
    {
        if (Input.GetKeyDown(KeyCode.Space))  // Space - Пробел 
        {
            coord = RandomNumbers(bigMatrix);
            smallMatrix = SmallMatrix(bigMatrix, coord);
            ApplySmallMatrixToCubes();
            LogState();
            return;
        }

        if (Input.GetKeyDown(KeyCode.W))  // W Вверх
        {
            (smallMatrix, coord) = ButtonW(smallMatrix, bigMatrix, coord);
            ApplySmallMatrixToCubes();
            LogState();
        }
        else if (Input.GetKeyDown(KeyCode.S)) // S Вниз
        {
            (smallMatrix, coord) = ButtonS(smallMatrix, bigMatrix, coord);
            ApplySmallMatrixToCubes();
            LogState();
        }
        else if (Input.GetKeyDown(KeyCode.D))  // D Впраов
        {
            (smallMatrix, coord) = ButtonD(smallMatrix, bigMatrix, coord);
            ApplySmallMatrixToCubes();
            LogState();
        }
        else if (Input.GetKeyDown(KeyCode.A)) // A Влево
        {
            (smallMatrix, coord) = ButtonA(smallMatrix, bigMatrix, coord);
            ApplySmallMatrixToCubes();
            LogState();
        }
    }


    // Функция которая читает файл и возвращает матрицу сделанную из него
    static int[,] ReadBigMatrix(string resourcePath)
    {
        TextAsset ta = Resources.Load<TextAsset>(resourcePath);
        if (ta == null)
            throw new Exception($"Не найден файл Resources/{resourcePath}.txt");

        var lines = ta.text
            .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(s => s.Trim())
            .Where(s => s.Length > 0)
            .ToArray();

        // размеры матрицы
        int rows = lines.Length; 
        int cols = lines[0].Length;

        int[,] matrix = new int[rows, cols];

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                char ch = lines[r][c];

                matrix[r, c] = ch - '0';
            }
        }
        return matrix;
    }

    static int H(int[,] m) => m.GetLength(0);
    static int W(int[,] m) => m.GetLength(1);
    static int Mod(int a, int m) => ((a % m) + m) % m; // Зацикливает матрицу, дает возможность идти по кругу


    // Рандомайзер для координат
    Vector2Int RandomNumbers(int[,] matrix)
    {
        int x = randint.Next(0, H(matrix));
        int y = randint.Next(0, W(matrix));
        return new Vector2Int(x, y);
    }

    
    // Функция для создания матрицы 3 на 3
    static int[,] SmallMatrix(int[,] matrix, Vector2Int coordinate)
    {
        int[,] rand = new int[3, 3];

        int x = coordinate.x;
        int y = coordinate.y;

        int h = H(matrix);
        int w = W(matrix);

        rand[0, 0] = matrix[x, y];
        rand[0, 1] = matrix[x, (y + 1) % w];
        rand[0, 2] = matrix[x, (y + 2) % w];

        rand[1, 0] = matrix[(x + 1) % h, y];
        rand[2, 0] = matrix[(x + 2) % h, y];

        rand[1, 1] = matrix[(x + 1) % h, (y + 1) % w];
        rand[2, 1] = matrix[(x + 2) % h, (y + 1) % w];

        rand[1, 2] = matrix[(x + 1) % h, (y + 2) % w];
        rand[2, 2] = matrix[(x + 2) % h, (y + 2) % w];

        return rand;
    }


    // Функция для движения матрицы вверх
    static (int[,] small, Vector2Int newCoord) ButtonW(int[,] small, int[,] matrix, Vector2Int coordinate)
    {
        int x = coordinate.x;
        int y = coordinate.y;

        int t0 = small[1, 0];
        int t1 = small[1, 1];
        int t2 = small[1, 2];

        small[1, 0] = small[0, 0];
        small[1, 1] = small[0, 1];
        small[1, 2] = small[0, 2];

        small[2, 0] = t0;
        small[2, 1] = t1;
        small[2, 2] = t2;

        int h = H(matrix);
        int w = W(matrix);

        int nx = Mod(x - 1, h);

        small[0, 0] = matrix[nx, y];
        small[0, 1] = matrix[nx, (y + 1) % w];
        small[0, 2] = matrix[nx, (y + 2) % w];

        return (small, new Vector2Int(nx, y));
    }


    // Функция для движения матрицы вниз
    static (int[,] small, Vector2Int newCoord) ButtonS(int[,] small, int[,] matrix, Vector2Int coordinate)
    {
        int x = coordinate.x;
        int y = coordinate.y;

        int t0 = small[1, 0];
        int t1 = small[1, 1];
        int t2 = small[1, 2];

        small[1, 0] = small[2, 0];
        small[1, 1] = small[2, 1];
        small[1, 2] = small[2, 2];

        small[0, 0] = t0;
        small[0, 1] = t1;
        small[0, 2] = t2;

        int h = H(matrix);
        int w = W(matrix);

        int bottomX = (x + 3) % h;

        small[2, 0] = matrix[bottomX, y];
        small[2, 1] = matrix[bottomX, (y + 1) % w];
        small[2, 2] = matrix[bottomX, (y + 2) % w];

        int nx = (x + 1) % h;
        return (small, new Vector2Int(nx, y));
    }


    // Функция для движения матрицы вправо
    static (int[,] small, Vector2Int newCoord) ButtonD(int[,] small, int[,] matrix, Vector2Int coordinate)
    {
        int x = coordinate.x;
        int y = coordinate.y;

        int t0 = small[0, 1];
        int t1 = small[1, 1];
        int t2 = small[2, 1];

        small[0, 1] = small[0, 2];
        small[1, 1] = small[1, 2];
        small[2, 1] = small[2, 2];

        small[0, 0] = t0;
        small[1, 0] = t1;
        small[2, 0] = t2;

        int h = H(matrix);
        int w = W(matrix);

        int rightY = (y + 3) % w;

        small[0, 2] = matrix[x, rightY];
        small[1, 2] = matrix[(x + 1) % h, rightY];
        small[2, 2] = matrix[(x + 2) % h, rightY];

        int ny = (y + 1) % w;
        return (small, new Vector2Int(x, ny));
    }

    // Функция для движения матрицы влево
    static (int[,] small, Vector2Int newCoord) ButtonA(int[,] small, int[,] matrix, Vector2Int coordinate)
    {
        int x = coordinate.x;
        int y = coordinate.y;

        int t0 = small[0, 1];
        int t1 = small[1, 1];
        int t2 = small[2, 1];

        small[0, 1] = small[0, 0];
        small[1, 1] = small[1, 0];
        small[2, 1] = small[2, 0];

        small[0, 2] = t0;
        small[1, 2] = t1;
        small[2, 2] = t2;

        int h = H(matrix);
        int w = W(matrix);

        int leftY = Mod(y - 1, w);

        small[0, 0] = matrix[x, leftY];
        small[1, 0] = matrix[(x + 1) % h, leftY];
        small[2, 0] = matrix[(x + 2) % h, leftY];

        return (small, new Vector2Int(x, leftY));
    }

    // Красим кубы

    void ApplySmallMatrixToCubes()
    {
        for (int r = 0; r < 3; r++)
        {
            for (int c = 0; c < 3; c++)
            {
                int value = smallMatrix[r, c];
                cubeR[r, c].material = colours[value - 1]; // Отнимаем 1, так как числа обозначающие цвета на 1 больше индексов в матрице
            }
        }
    }


    //Привязывает каждый куб к индексу в матрице
    void CubeRenderers()
    {
        Transform grid = GameObject.Find("Grid")?.transform;
        var cubes = grid.Cast<Transform>()
            .Select(t => new { t, r = t.GetComponent<Renderer>() })
            .Where(x => x.r != null)
            .ToArray();

        float minY = cubes.Min(c => c.t.localPosition.y);
        float maxY = cubes.Max(c => c.t.localPosition.y);
        float minZ = cubes.Min(c => c.t.localPosition.z);
        float maxZ = cubes.Max(c => c.t.localPosition.z);

        bool gridUsesZ = (maxZ - minZ) > (maxY - minY);

        float[] xs = cubes.Select(c => c.t.localPosition.x)
                          .OrderBy(v => v)
                          .DistinctByRounded()
                          .ToArray();

        float[] rs = cubes.Select(c => gridUsesZ ? c.t.localPosition.z : c.t.localPosition.y)
                          .OrderByDescending(v => v) 
                          .DistinctByRounded()
                          .ToArray();

        foreach (var c in cubes) // перебираем каждый куб
        {
            float x = c.t.localPosition.x;
            float rowAxis = gridUsesZ ? c.t.localPosition.z : c.t.localPosition.y;

            int col = IndexOfNearest(xs, x);
            int row = IndexOfNearest(rs, rowAxis);

            cubeR[row, col] = c.r;
        }
    }


   // уточняет позиции кубов
    static int IndexOfNearest(float[] arr, float value)
    {
        int best = -1;
        float bestD = float.MaxValue;
        for (int i = 0; i < arr.Length; i++)
        {
            float d = Mathf.Abs(arr[i] - value);
            if (d < bestD) { bestD = d; best = i; }
        }
        return best;
    }


    void LogState()
    {
        Debug.Log($"Координаты = ({coord.x}, {coord.y})"); // Выводим координаты в консоль
    }

}

// Функция для решения проблем с погрешностями
static class DistinctRoundedExt
{
    public static float[] DistinctByRounded(this IOrderedEnumerable<float> seq, float step = 0.01f)
    {
        return seq.Select(v => Mathf.Round(v / step) * step).Distinct().ToArray();
    }
}
