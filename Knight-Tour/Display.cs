using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Knight_Tour
{
    // GUI-t kezelő osztály
    // Készítette: Varga Péter
    internal class Display
    {
        private Canvas _cGameArea;
        private Button[] _btCells;
        private int _lastCell;
        private int _stepCnt;
        private Brush _lastColor;
        private Image img;

        // Publikus tulajdonságok, egy cella mérete (px), színei stb...
        public bool isRun { private get; set; }
        public int cellWidth { get; set; }
        public int cellHeight { get; set; }
        public Brush cellDarkColor { get; set; }
        public Brush cellLightColor { get; set; }
        public int startPos { get; private set; }
        public DataTable Steps { get; private set; }
      
        // Konstruktor: Alapértelmezettek beállítása, tábla kirajzolás.
        public Display(Canvas canvas)
        {
            _cGameArea = canvas;
            _btCells = new Button[8 * 8];
            Steps = new DataTable();
            img = new Image();
            BitmapImage bimg = new BitmapImage(new Uri("/images/knight.png", UriKind.Relative));

            img.Source = bimg;
            cellWidth = 50;
            cellHeight = 50;
            cellDarkColor = Brushes.Sienna;
            cellLightColor = Brushes.Wheat;

            for (int i = 0; i < 8; i++)
                Steps.Columns.Add(i.ToString(), typeof(string));
            for (int i = 0; i < 8; i++)
                Steps.Rows.Add();

            for (int i = 0; i < _btCells.Length; i++)
            {
                _btCells[i] = new Button();
                _btCells[i].Name = "btCell_" + i.ToString();
                _btCells[i].Click += _Btn_Click;
            }

            Reset(0);
        }

        // Játéktér, tábla felrajzolása a tulajdonságoknak megfelelően.
        public void Draw()
        {
            Button btn;
            byte odd = 0;
            int x = 0, y = 0;

            _cGameArea.Children.Clear();
            _cGameArea.Width = (cellWidth * 8);
            _cGameArea.Height = (cellHeight * 8);

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    btn = _btCells[(i * 8) + j];

                    btn.Width = cellWidth;
                    btn.Height = cellHeight;
                    btn.Content = null;
                    if (odd == 1) btn.Background = cellDarkColor;
                    else btn.Background = cellLightColor;
                    odd ^= 1;

                    Canvas.SetTop(btn, y);
                    Canvas.SetLeft(btn, x);
                    _cGameArea.Children.Add(btn);

                    x += cellWidth;
                }

                x = 0;
                y += cellHeight;

                odd ^= 1;
            }
        }

        // Egy lépés megtétele a lóval.
        // A jelenlegi helyről az új helyre átrendezzük a lovat, és az útját egy vonallal jelöljük.
        // A táblázatos megjelenítéshez kitöltjül a megfelelő oszlop/sort.
        public void Step(int pos)
        {
            Line line = new Line();
            Ellipse ellipse = new Ellipse();
            int from_x, from_y;
            int to_x, to_y;

            from_x = (int)((double)_btCells[_lastCell].GetValue(Canvas.LeftProperty) + (cellWidth / 2));
            from_y = (int)((double)_btCells[_lastCell].GetValue(Canvas.TopProperty) + (cellHeight / 2));
            to_x = (int)((double)_btCells[pos].GetValue(Canvas.LeftProperty) + (cellWidth / 2));
            to_y = (int)((double)_btCells[pos].GetValue(Canvas.TopProperty) + (cellHeight / 2));
            if (_lastColor == Brushes.Red) _lastColor = Brushes.Green;
            else _lastColor = Brushes.Red;
            
            ellipse.Fill = Brushes.Black;
            ellipse.Width = 10;
            ellipse.Height = 10;
            Canvas.SetLeft(ellipse, from_x - 5);
            Canvas.SetTop(ellipse, from_y - 5);

            line.Stroke = _lastColor;
            line.StrokeThickness = 3;
            line.X1 = from_x;
            line.Y1 = from_y;
            line.X2 = to_x;
            line.Y2 = to_y;

            _cGameArea.Children.Add(ellipse);
            _cGameArea.Children.Add(line);

            _btCells[_lastCell].Content = null;
            _btCells[pos].Content = img;
            _lastCell = pos;

            Steps.Rows[pos >> 3][pos & 0x07] = _stepCnt++;
        }

        // A játéktér törlése, újrarajzolása.
        // Táblázat törlése, ló start pozícióba állítása.
        public void Reset(int start)
        {
            for (int i = 0; i < _btCells.Length; i++)
            {
                _btCells[i].Content = null;
                Steps.Rows[i >> 3][i & 0x07] = String.Empty;
            }

            startPos = start;
            _lastCell = start;
            _stepCnt = 1;

            Draw();
            _btCells[start].Content = img;
        }

        // A kattintásnak megfelelő pozícióba állítjuk a lovat, és reseteljük a játékteret, amennyiben nincs éppen folyamatban a vándorlás.
        private void _Btn_Click(object sender, RoutedEventArgs e)
        {
            if (isRun) return;
            
            Button btn = (Button)sender;
            int index = Convert.ToInt32(btn.Name.Substring(btn.Name.IndexOf('_') + 1));

            Reset(index);
        }
    }
}
