using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BGE
{
    public class Space
    {
        public Bounds spaceBounds;        
        public List<Cell> cells = new List<Cell>();
        Vector3 spaceCells = new Vector3();
        Vector3 cellUnit = new Vector3();
        Boid[] boids; 

        public Space(float w, float h, float d, float numCells)
        {
            // Default bounds for the space in world space
            //float w = 1000;
            //float s = 50;
            spaceBounds = new Bounds(Vector3.zero, new Vector3(w, h, d));
            spaceCells = new Vector3(numCells, numCells, numCells); // The number of cells in each axis
            cellUnit.x = spaceBounds.size.x / spaceCells.x;
            cellUnit.y = spaceBounds.size.y / spaceCells.y;
            cellUnit.z = spaceBounds.size.z / spaceCells.z;

            int num = 0;

            float y = 0;
            {
                for (float z = spaceBounds.min.z; z < spaceBounds.max.z; z += cellUnit.z)
                {
                    for (float x = spaceBounds.min.x; x < spaceBounds.max.z; x += cellUnit.x)
                    {
                        Cell cell = new Cell();
                        cell.bounds.min = new Vector3(x, y, z);
                        cell.bounds.max = new Vector3(x + cellUnit.x, y, z + cellUnit.z);
                        cell.number = num++;
                        cells.Add(cell);
                    }
                }
            }

            //Now find each of the neighbours for each cell
            float width = cellUnit.x;
            float height = cellUnit.z;
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

        public int FindCell(Vector3 pos)
        {          

            pos.y = 0;            
			pos.x += (spaceBounds.size.x / 2);
            pos.z += (spaceBounds.size.z / 2); ;
			int cellNumber = ((int)(pos.x / cellUnit.x))
                + ((int)(pos.z / cellUnit.x)) * (int) spaceCells.x;

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
            for (float x = spaceBounds.min.x; x <= spaceBounds.max.z; x += cellUnit.x)
            {
                Vector3 start, end;
                start = new Vector3(x, y, spaceBounds.min.z);
                end = new Vector3(x, y, spaceBounds.max.z);
                LineDrawer.DrawLine(start, end, Color.cyan);
            }
            for (float z = spaceBounds.min.z; z <= spaceBounds.max.z; z += cellUnit.z)
            {
                Vector3 start, end;
                start = new Vector3(spaceBounds.min.x, y, z);
                end = new Vector3(spaceBounds.max.x, y, z);
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
            if (boids == null)
            {
                boids = GameObject.FindObjectsOfType(typeof(Boid)) as Boid[];                        
            }
            foreach (Cell cell in cells)
            {
                cell.contained.Clear();
            }
            foreach (Boid boid in boids)
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
