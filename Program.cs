using System;
using System.Collections.Generic;
using System.Threading;

namespace Zad1._1
{
    public struct sasiad{
        public string name;
        public double x;
        public double y;
        public double odleglosc;
        public sasiad(string _name, double _x,double _y,double _mx,double _my)
        {
            name = _name;
            x = _x;
            y = _y;
            odleglosc =Math.Round(Math.Sqrt(Math.Pow(Math.Abs(_x-_mx),2)+Math.Pow(Math.Abs(_y-_my),2)),2);
        }   
    }
    class InfoEventArgs : EventArgs
    {
        public readonly string name;
        public readonly double x;
        public readonly double y;
        public readonly int count;
        public InfoEventArgs(string _name, double _x, double _y, int _count)
        {
            name = _name;
            x = _x;
            y = _y;
            count = _count;
        }
    }
    class Obserwator
    {
        private List<sasiad> sasiedzi = new List<sasiad>();
        private double x;
        private double y;
        private string name;
        private int count;
        
        public void Subscribe(Tworca _t)
        {
            _t.OnTworcaChange += new Tworca.TworcaChangeHandler(NewObserwator);

        }
        public Obserwator(string _name, double _x, double _y,int _count)
        {
            name = _name;
            x = _x;
            y = _y;
            count = _count;
        }
        public void  NewObserwator(object _obj,InfoEventArgs _info) //zamiast info dac obserwatora?
        {
            if(count < _info.count)
            {
                var z = new sasiad(_info.name,_info.x,_info.y,this.x,this.y);
                if(!sasiedzi.Contains(z))
                    sasiedzi.Add(z);
            }
            sasiedzi.Sort((s1,s2) => s1.odleglosc.CompareTo(s2.odleglosc));
            sasiedzi.Reverse();
            while(sasiedzi.Count > 2)
            {
                sasiedzi.RemoveAt(sasiedzi.Count-1);
            }
            // Console.WriteLine("cos robie"+count);
            WypiszSasiadow();
        }
        public void WypiszSasiadow()
        {
            Console.Write("Jestem "+name+" a to moi sasiedzi: ");
            foreach (var i in sasiedzi)
            {
                Console.Write(i.name);
                Console.Write(" ");
                Console.Write(i.odleglosc);
                Console.Write(" ");
            }
            Console.WriteLine(" "); //dodac endl!!!!
        }
    }
    
    class Tworca
    {
        public List<Obserwator> listaObs;
        private int count;
        private int second;
        static Random rnd = new Random();
        private Obserwator newObj; 

        public Tworca()
        {
            count = 0;
            listaObs = new List<Obserwator>();
        }
        public delegate void TworcaChangeHandler(object _t,InfoEventArgs _info);
        public event TworcaChangeHandler OnTworcaChange;
        public void Run()
        {
            for(;;)
            {
                Thread.Sleep(10);
                DateTime dt = DateTime.Now;
                if (dt.Second != second)
                {
                    var x = rnd.NextDouble()*(1.0);
                    var y = rnd.NextDouble()*(1.0);
                    var name = "Obs"+count;
                    count++;

                    newObj = new Obserwator(name,x,y,count);
                    
                    listaObs.Add(newObj);
                    InfoEventArgs infoEvent = new InfoEventArgs(name,x,y,count);
                    if(OnTworcaChange != null)
                    {
                        OnTworcaChange(this,infoEvent);
                    }
                    newObj.Subscribe(this);
                }
                second = dt.Second;
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Tworca mojTworca = new Tworca();
            mojTworca.Run();
        }
    }
}
