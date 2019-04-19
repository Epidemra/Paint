using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paint
{
    class EllipseCreator : Creator
    {

        private static EllipseCreator instance;

        private EllipseCreator()
        { }

        public static EllipseCreator getInstance()
        {
            if (instance == null)
                instance = new EllipseCreator();
            return instance;
        }

        public override Figure Create()
        {
            return new Ellipse();
        }
    }
}
