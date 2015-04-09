using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BGE
{
    public class Space
    {
        public Bounds bounds;        
        public List<Cell> cells = new List<Cell>();
        Vector3 numCells = new Vector3();
        Vector3 celSize = new Vector3();
        
        public List<GameObject> boids;

        public Space(Vector3 center, float w, float h, float d, float numCells1, List<GameObject> boids)
        {
            this.boids = boids;
            bounds = new Bounds(center, new Vector3(w, h, d));
            numCells = new Vector3(numCells1, numCells1, numCells1); // The number of cells in each axis
            celSize.x = bounds.size.x / numCells.x;
            celSize.y = bounds.size.y / numCells.y;
            celSize.z = bounds.size.z / numCells.z;

            int num = 0;
            int row = 0; int col = 0;
            float y = 0;
            {
                for (float z = bounds.min.z; z < bounds.max.z; z += celSize.z)
                {
                    col = 0;
                    for (float x = bounds.min.x; x < bounds.max.x; x += celSize.x)
                    {
                        Cell cell = new Cell();
                        cell.bounds.min = new Vector3(x, y, z);
                        cell.bounds.max = new Vector3(x + celSize.x, y, z + celSize.z);
                        cell.number = num++;
                        cell.row = row;
                        cell.col = col;
                        col++;
                        cells.Add(cell);
                    }
                    row++;
                    
                }
            }

            //Now find each of the neighbours for each cell
            float width = celSize.x;
            float height = celSize.z;
            foreach (Cell cell in cells)
            {                
                // Add me
                AddAdjacent(cell, new Vector3(0, 0, 0));

                // Add the side
                AddAdjacent(cell, new Vector3(-width, 0, 0));
                AddAdjacent(cell, new Vector3(width, 0, 0));
                // Add the top row of cells
                AddAdjacent(cell, new Vector3(-width, 0, -height));
                AddAdjacent(cell, new Vector3(0, 0, -height));
                AddAdjacent(cell, new Vector3(width, 0, -height));
                // Add the bottomo row of cells
                AddAdjacent(cell, new Vector3(-width, 0, height));
                AddAdjacent(cell, new Vector3(0, 0, height));
                AddAdjacent(cell, new Vector3(width, 0, height));

                /*
                Vector3 extra = new Vector3(10, 0, 10);
                Bounds expanded = cell.bounds;
                expanded.min = expanded.min - extra;
                expanded.max = expanded.max + extra;
                foreach (Cell neighbour in cells)
                {
                    if (neighbour.Intersects(expanded))
                    {
                        cell.adjacent.Add(neighbour);
                    }
                } 
                */
            }
        }

        private void AddAdjacent(Cell cell, Vector3 cellOffset)
        {
            Vector3 point = cell.bounds.center + cellOffset;
            int neighbour = FindCell(point);
            if (neighbour != -1)
            {
                cell.adjacent.Add(cells[neighbour]);
            }
        }

        public Cell GetCell(int row, int col)
        {
            int cellNumber = col + (row * (int) numCells.x);

            return (cellNumber >= cells.Count) || (cellNumber < 0) ? null : cells[cellNumber];
        }

        public int FindCell(Vector3 pos)
        {
            Vector3 transPos = pos - bounds.min;
            transPos.y = 0;
            int cellNumber = ((int)(transPos.x / celSize.x))
                + ((int)(transPos.z / celSize.z)) * (int)numCells.x;

            if ((cellNumber >= cells.Count) || (cellNumber < 0))
            {
                return -1;
            }
            else
            {
                return cellNumber;
            }
        }

        public void Draw()
        {
            float y = 0;
            for (float x = bounds.min.x; x <= bounds.max.x; x += celSize.x)
            {
                Vector3 start, end;
                start = new Vector3(x, y, bounds.min.z);
                end = new Vector3(x, y, bounds.max.z);
                LineDrawer.DrawLine(start, end, Color.cyan);
            }
            for (float z = bounds.min.z; z <= bounds.max.z; z += celSize.z)
            {
                Vector3 start, end;
                start = new Vector3(bounds.min.x, y, z);
                end = new Vector3(bounds.max.x, y, z);
                LineDrawer.DrawLine(start, end, Color.cyan);
            } 
            /*foreach (Cell cell in cells)
            {
                LineDrawer.DrawSquare(cell.bounds.min, cell.bounds.max, Color.cyan);
            }
             */
        }

        public void Partition()
        { 
            foreach (Cell cell in cells)
            {
                cell.contained.Clear();
            }
            foreach (GameObject boid in boids)
            {
                int cell = FindCell(boid.transform.position);
                if (cell != -1)
                {
                    cells[cell].contained.Add(boid.gameObject);
               }
            }
        }
    }
}
