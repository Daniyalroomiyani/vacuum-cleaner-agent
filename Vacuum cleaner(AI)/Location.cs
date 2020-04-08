using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vacuum_cleaner_AI_
{
    class Location
    {
        private int i=0;
        private int j=0;
        private int roomnum;
        public int RoomNum
        {
            get { return roomnum; }
            set { roomnum = value; }
        }   

        public Location(int m, int n  )
        {
            this.i = m;
            this.j = n;
           // this.roomnum = x;
        }

        public override string ToString()
        {
            return "I = " + this.I + "   J =  " + this.J + " Num :  "+ this.RoomNum;
        }

        public override bool Equals(object obj)
        {
            Location l = (Location)obj;
            bool b;
            
             
            b =  (obj is Location location && this.i == l.i && this.j == l.j);
            return b;

                   
        }

        public int I
        {
            get { return i; }
            set { i = value; }
        }
        public int J
        {
            get { return j; }
            set { j = value; }
        }


    }
}
