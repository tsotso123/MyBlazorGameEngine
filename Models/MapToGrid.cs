using game.Models.units;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace game.Models
{
    public class MapToGrid
    {
        
        public int cellSizeHeight;
        public int cellSizeWidth;
        public int Width;
        public int Height;
        // 2d array of units, each element is a list of units at that cell
        List<AbstractUnit>[,] units;
        public MapToGrid(int map_w, int map_h,int celSizeHeight,int cellSizeWidth)
        {
            this.cellSizeHeight = celSizeHeight;
            this.cellSizeWidth = cellSizeWidth;

            units = new List<AbstractUnit>[map_w,map_h];
            for (int i = 0; i < map_w; i += cellSizeWidth)
            {
                for (int j = 0; j < map_h; j += celSizeHeight)
                {
                    units[i,j] = new List<AbstractUnit>();
                }
            }

            Width = map_w;
            Height = map_h;
        }

        public List<AbstractUnit> GetUnitsAt(float x, float y)
        {
            int x_ = (int)x;
            int y_ = (int)y;
            x_ = x_ / cellSizeWidth * cellSizeWidth;
            y_ = y_ / cellSizeHeight * cellSizeHeight;
            //return units[x_, y_];
            try
            {
                if (x_ <0)
                {
                    x_ = Math.Abs(x_);
                }
                if (y_ < 0)
                {
                    y_ = Math.Abs(y_);
                }

                if (x_>= Width)
                {
                    x_ = Width - cellSizeWidth;
                    
                }
                if (y_>= Height)
                {
                    y_ = Height - cellSizeHeight;
                    
                }
                

                return units[x_, y_];
            }
            catch
            {
                Console.WriteLine("x:"+ x+ " y:"+y);
                return null;
            }

        }

        public void RemoveAt(float x, float y, AbstractUnit unit)
        {
            int x_ = (int)x;
            int y_ = (int)y;
            x_ = x_ / cellSizeWidth * cellSizeWidth;
            y_ = y_ / cellSizeHeight * cellSizeHeight;
            try
            {
                units[x_,y_].Remove(unit);                
            }
            catch 
            {
                
            }
        }

        public void InsertAt(float x, float y, AbstractUnit unit)
        {
            int x_ = (int)x;
            int y_ = (int)y;
            x_ = x_ / cellSizeWidth * cellSizeWidth;
            y_ = y_ / cellSizeHeight * cellSizeHeight;
            try
            {
                units[x_,y_].Add(unit);
            }
            catch 
            {
                
            }

        }
    }
}
