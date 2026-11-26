
using System;
using System.Collections.Generic;
using System.Numerics;
using ProjectAlps.Generation.WorldGeneration.RegionNodes;

public class WorldMatrixDiscretization
{
    public int[,] WorldMatrix {get; private set;}
    public Dictionary<int,Vector2> keyValuePairs {get; private set;} = new();
    public int width;
    public int height;

    private const int TARGETCELLNUMBER = 20000;
    private const int PADDING = 10;

    public WorldMatrixDiscretization(Dictionary<RegionNode, Vector2> spaceInstance)
    {
        // CALCULATE GRAPH DIMENSIONS

        double north = double.MinValue;
        double south = double.MaxValue;
        double west = double.MaxValue;
        double east = double.MinValue;
    
        foreach (var point in spaceInstance.Values)
        {
            north = Math.Max(north, point.Y);
            south = Math.Min(south, point.Y);
            west = Math.Min(west, point.X);
            east = Math.Max(east, point.X);
        }

        double graphAR = (east-west)/(north-south);
    
        // CALCULATE NUMBER AND SIZE OF CELLS PER DIMENSION

        double a = 1/graphAR;
        double b = 2*PADDING/graphAR + 2*PADDING;
        double c = 2*PADDING*2*PADDING-TARGETCELLNUMBER;
        
        double graphXCells = (-b + Math.Sqrt(b*b - 4*a*c))/( 2*a ); // Solving a system so that AR is conserved and number of cells is TARGETCELLNUMBER
        double graphYCells = graphXCells/graphAR;

        double CellXSize = (east-west)/graphXCells;
        double CellYSize = (north-south)/graphYCells;

        // CREATE BLANK WORLD MATRIX

        width = (int)Math.Ceiling(graphXCells) + 2*PADDING;
        height = (int)Math.Ceiling(graphYCells) + 2*PADDING;

        WorldMatrix = new int[width,height];
        
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                WorldMatrix[x, y] = -1;
            }
        }

        // INSERT DISCRETIZED NODE POSITIONS

        foreach (var node in spaceInstance)
        {
            Vector2 point = node.Value;

            int x = (int)Math.Floor((point.X - west)/CellXSize) + PADDING;
            int y = (int)Math.Floor((point.Y - south)/CellYSize) + PADDING;

            Vector2 newPointCoordinates = new Vector2(x,y);

            WorldMatrix[x, y] = node.Key.Id;

            keyValuePairs.Add(node.Key.Id, newPointCoordinates);
        }
    }    
    
    public void PrintMatrix()
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Console.Write($"{WorldMatrix[x, y],2}");
            }

            Console.WriteLine();
        }
    }
}