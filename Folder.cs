using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt1
{
    public class Folder
    {
        public string Name;
       public List <Folder> priecinky= new List<Folder> ();
       public  List <string> subory=new List<string> ();
    }
}
