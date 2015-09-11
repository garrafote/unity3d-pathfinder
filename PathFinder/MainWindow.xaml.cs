using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PathFinder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Search API

        interface INode
        {
            float Weight { get; }
            IEnumerable<INode> Connections { get; }

        }

        interface ISearchSpace
        {
            IEnumerable<INode> Nodes { get; }
        }

        interface IPathFinder
        {

        }

        #endregion

        #region 2D Grid search

        class Grid2D : ISearchSpace
        {
            private Node[,] nodes;

            public IEnumerable<INode> Nodes
            {
                get
                {
                    // cast will convert the 2D array into IEnumerable ;)
                    return nodes.Cast<INode>();
                }
            }

            public Grid2D(int width, int height)
            {
                nodes = new Node[width, height];
            }

            public IEnumerable<INode> GetNodeConnections(int x, int y)
            {
                var notLeftEdge = x >= 0;
                var notRightEdge = x < nodes.GetLength(0);
                var notBottomEdge = y >= 0;
                var notTopEdge = y < nodes.GetLength(1);

                var size = 8;
                if (notBottomEdge || notTopEdge) size -= 3;
                if (notLeftEdge || notRightEdge)
                {
                    if (notBottomEdge || notTopEdge) size -= 2;
                    else size -= 3;
                }

                var connections = new INode[size];
                var index = 0;

                if (notLeftEdge) connections[index++] = nodes[x - 1, y];
                if (notRightEdge) connections[index++] = nodes[x + 1, y];
                if (notBottomEdge) connections[index++] = nodes[x, y - 1];
                if (notTopEdge) connections[index++] = nodes[x, y + 1];

                if (notLeftEdge && notBottomEdge) connections[index++] = nodes[x - 1, y - 1];
                if (notLeftEdge && notTopEdge) connections[index++] = nodes[x - 1, y + 1];
                if (notRightEdge && notBottomEdge) connections[index++] = nodes[x + 1, y - 1];
                if (notRightEdge && notTopEdge) connections[index++] = nodes[x + 1, y + 1];
                
                return connections;
            }
        }

        class Node : INode
        {
            public readonly Grid2D Grid;

            public readonly int X;
            public readonly int Y;

            public float Weight { get; set; }

            public IEnumerable<INode> Connections
            {
                get
                {
                    return Grid.GetNodeConnections(X, Y);
                }
            }

            public Node(Grid2D grid, int x, int y, float weight = 1)
            {
                Grid = grid;
                X = x;
                Y = y;
                Weight = weight;
            }
        }

        #endregion

        #region A*

        class AStar : IPathFinder
        {
            public void FindPath(INode startNode, INode endNode)
            {

            }
        }

        #endregion

        public MainWindow()
        {
            InitializeComponent();
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 5; i++)
            {
                WorldGrid.RowDefinitions.Add(new RowDefinition
                {
                    Height = new GridLength(1, GridUnitType.Star),
                });
            }

            for (int i = 0; i < 5; i++)
            {
                WorldGrid.ColumnDefinitions.Add(new ColumnDefinition
                {
                    Width = new GridLength(1, GridUnitType.Star),
                });
            }

            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    var b = new Grid { Background = new SolidColorBrush(new Color { R = (byte)(i* 30), G = (byte)((5 - j)*30), A = 255 }) };
                    WorldGrid.Children.Add(b);
                    Grid.SetColumn(b, i);
                    Grid.SetRow(b, j);
                }
            }
        }
    }
}
