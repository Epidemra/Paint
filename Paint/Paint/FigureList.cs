using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paint
{
    class FigureList
    {
        public Dictionary<int, Figure> figureList = new Dictionary<int, Figure>();

        public FigureList()
        {
            upDate();
        }

        public void upDate()
        {
            figureList.Clear();
            figureList.Add(0, new Line());
            figureList.Add(1, new Ellipse());
            figureList.Add(2, new Circle());
        }
    }
}
