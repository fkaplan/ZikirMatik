using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App3.Model
{
    [Table("DuaList")]
    public class Dualar
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string DuaName { get; set; }
        public string Dua { get; set; }
        public int kacKezOkunmali { get; set; }
        public int kacKezOkundu { get; set; }
    }
}
