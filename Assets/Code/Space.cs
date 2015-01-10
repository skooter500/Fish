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

        public Space()
        {
            // Default bounds for the space in world space
            float w = 1000;
            float s = 50;
            spaceBounds = new Bounds(Vector3.zero, new Vector3(w, w, w));
            spaceCells = new Vector3(s, s, s); // The number of cells in each axis
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
            foreach (Cell cell in cells)
            {
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
            foreach (Cell cell in cells)
            {
                LineDrawer.DrawSquare(cell.bounds.min, cell.bounds.max, Color.cyan);
            }
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
